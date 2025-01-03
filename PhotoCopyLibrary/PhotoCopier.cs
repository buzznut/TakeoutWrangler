//  <@$&< copyright begin >&$@> D50225522CB19A3A2E3CA10257DC538D19677A6406D028F0BBE01DE33387A4EA:20241017.A:2024:12:23:9:15
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024 Stewart A. Nutter - All Rights Reserved.
// No warranty is implied or given.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// <@$&< copyright end >&$@>

// using Library "CompactExifLib" https://www.codeproject.com/Articles/5251929/CompactExifLib-Access-to-EXIF-Tags-in-JPEG-TIFF-an
// for reading and writing EXIF data in JPEG, TIFF and PNG image files.
// © Copyright 2021 Hans-Peter Kalb

using ExifDataLibrary;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.FileType;
using MetadataExtractor.Formats.QuickTime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace PhotoCopyLibrary;

public delegate void HandleOutput(string output = null, ErrorCode errorCode = ErrorCode.Success);
public delegate void StatusUpdate(string status);

public class PhotoCopier
{
    private const string RetryText = "(Retry)";
    private int canceled;
    private int zipFileCount;
    private int errorCount;
    private int skipCount;
    private int duplicateCount;
    private int totalCount;
    private int progressCount;

    private string sourceDir;
    private string destDir;
    private string pattern = "$y_$m";
    private string fileFilter = "takeout-*.zip";
    private PhotoCopierActions behavior = PhotoCopierActions.Copy;
    private LoggingVerbosity logging = LoggingVerbosity.Verbose;
    private bool listOnly = true;
    private bool parallel;

    private readonly char[] separator = new char[] { '.' };
    private readonly HandleOutput outputHandler = DummyOutput;
    private readonly StatusUpdate statusUpdate = DummyStatus;
    private readonly ConcurrentBag<ZipArchive> archives = new ConcurrentBag<ZipArchive>();
    private readonly ConcurrentDictionary<string, MediaInfo> mediaKeys = new ConcurrentDictionary<string, MediaInfo>(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentQueue<MediaInfo> retryMediaKeys = new ConcurrentQueue<MediaInfo>();

    readonly List<ConfigParam> paramTypes = new List<ConfigParam>
    {
        new ConfigParam { Name = "filter", Default="takeout-*.zip", PType = ParamType.String },
        new ConfigParam { Name = "source", Synonyms = ["src", "sourceDir"], PType = ParamType.String },
        new ConfigParam { Name = "destination", Synonyms = ["dst", "dest", "tgt", "target", "destinationDir"], PType = ParamType.String },
        new ConfigParam { Name = "logging", PType = ParamType.String, Default = nameof(LoggingVerbosity.Verbose) },
        new ConfigParam { Name = "help", Synonyms = ["?"], Default = false, PType = ParamType.Bool },
        new ConfigParam { Name = "action", Default = nameof(PhotoCopierActions.Copy), PType = ParamType.String },
        new ConfigParam { Name = "pattern", Default = "$y_$m", PType = ParamType.String },
        new ConfigParam { Name = "listonly", Synonyms = ["list"], Default = true, PType = ParamType.Bool },
        new ConfigParam { Name = "parallel", Synonyms = ["fast"], Default = false, PType = ParamType.Bool }
    };

    private static void DummyOutput(string output = null, ErrorCode errorCode = ErrorCode.Success)
    {
    }

    private static void DummyStatus(string status)
    {
    }

    private PhotoCopier() { }

    public PhotoCopier(HandleOutput output, StatusUpdate status)
    {
        outputHandler = output;
        statusUpdate = status;
    }

    public ReturnCode Initialize(
        string appName,
        bool help,
        string sourceDir,
        string destDir,
        PhotoCopierActions behavior,
        string pattern,
        string fileFilter,
        LoggingVerbosity logging,
        bool listOnly,
        bool parallel)
    {
        try
        {
            string[] allActions = Enum.GetNames(typeof(PhotoCopierActions));

            List<string> reasons = new List<string>();

            // trim trailing backslashes
            if ((sourceDir?.EndsWith('\\')).GetValueOrDefault()) sourceDir = sourceDir?.Substring(0, sourceDir.Length - 1);
            if ((destDir?.EndsWith('\\')).GetValueOrDefault()) destDir = destDir?.Substring(0, destDir.Length - 1);

            // validate paths
            help |= !Configs.ValidatePath(sourceDir, "Source", reasons);
            help |= !Configs.ValidatePath(destDir, "Destination", reasons);

            if (help)
            {
                outputHandler($"usage: {appName} -source=\"path\" -destination=\"path\" -action={string.Join('|', allActions)} -pattern=$y_$m -filter=takeout-*.zip");
                outputHandler();
                outputHandler($"  Copy all photos, movies, etc. from a set of archive zip files (Google takeout) from the source folder");
                outputHandler($"  to destination folder. Subfolders will be created to hold photos with pattern naming.");
                outputHandler();
                outputHandler($"where:");
                outputHandler();
                outputHandler($" source=path        folder that contains the takeout zip file(s).");
                outputHandler($"                    (default=null");
                outputHandler();
                outputHandler($" destination=path   folder where to put photo contents (files and subfolders)");
                outputHandler($"                    (default=null");
                outputHandler();
                outputHandler($" action=option      option is one of {PhotoCopierActions.Copy}, {PhotoCopierActions.Overwrite} or {PhotoCopierActions.Reorder}.");
                outputHandler($"                    (default={PhotoCopierActions.Copy}");
                outputHandler();
                outputHandler($"                    {PhotoCopierActions.Copy} media files to destination.");
                outputHandler($"                    {PhotoCopierActions.Reorder} media files only if pattern is different.");
                outputHandler();
                outputHandler($" list=value         value is true to list only, false to perform actions.");
                outputHandler($"                    example: action=true would only show potential errors or actions without changing destination.");
                outputHandler();
                outputHandler($" parallel=value     value is true to run in parallel for faster performance, false to run sequentially. Parallem mode uses more memory.");
                outputHandler($"                    example: action=true would only show potential errors or actions without changing destination.");
                outputHandler();
                outputHandler($" loggin=verbosity   {LoggingVerbosity.Quiet}, {LoggingVerbosity.Change}, or {LoggingVerbosity.Verbose}. {LoggingVerbosity.Quiet}=minimal output, {LoggingVerbosity.Change}=minimal+changed content, {LoggingVerbosity.Verbose}=all");
                outputHandler($"                    (default={LoggingVerbosity.Change})");
                outputHandler();
                outputHandler($" pattern=text       Destination folder name pattern. (default=$y_$m");
                outputHandler();
                outputHandler($"                    example: if date-time, is March 27, 2024 at 3:33PM");
                outputHandler();
                outputHandler($"                       $y = 4 digit year. ex: 2024");
                outputHandler($"                       $m = 2 digit month. ex: 03");
                outputHandler($"                       $M = month name. ex: March");
                outputHandler($"                       $d = 2 digit day. ex: 27");
                outputHandler($"                       $h = 2 digit hour. ex: 15 (24 hours per day)");
                outputHandler();
                outputHandler($"                       '$y\\Sam-$M($d)' \"root\\2024\\Sam-March(3)\"");
                outputHandler();
                outputHandler($" filter=text        photo archive file filter. (default='takeout-*.zip')");
                outputHandler();

                if (reasons.Count > 0)
                {
                    outputHandler("Issues: ", ErrorCode.Warning);

                    int index = 1;
                    foreach (string reason in reasons)
                    {
                        outputHandler($"  {index++}. {reason}", ErrorCode.Warning);
                    }

                    outputHandler();
                }

                return ReturnCode.HadIssues;
            }

            string[] sourceFiles = System.IO.Directory.GetFiles(sourceDir ?? ".\\", fileFilter);
            if (sourceFiles == null || sourceFiles.Length == 0)
            {
                outputHandler($"No source files found. sourceDir:\"{sourceDir}\", file filter:\"{fileFilter}\"");
                return ReturnCode.DirectoryError;
            }

            // keep the parameters

            outputHandler("Current configuration:");

            this.behavior = behavior;
            outputHandler($" Action: {behavior}");

            this.sourceDir = sourceDir;
            outputHandler($" Source: {sourceDir}");

            this.destDir = destDir;
            outputHandler($" Destination: {destDir}");

            this.fileFilter = fileFilter;
            outputHandler($" Filter: {fileFilter}");

            this.pattern = pattern;
            outputHandler($" Pattern: {pattern}");

            this.logging = logging;
            outputHandler($" Logging: {logging}");

            this.listOnly = listOnly;
            outputHandler($" ListOnly: {listOnly}");

            this.parallel = parallel;
            outputHandler($" Parallel: {parallel}");

            outputHandler();

            return (int)ReturnCode.Success;
        }
        catch (Exception ex)
        {
            if (Debugger.IsAttached) Debugger.Break();
            outputHandler(ex.ToString());
            return ReturnCode.Error;
        }
    }

    public bool IsRunning { get; private set; }

    public void Stop()
    {
        Interlocked.Increment(ref canceled);
    }

    public async Task<ReturnCode> RunAsync()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        try
        {
            canceled = 0;
            skipCount = 0;
            errorCount = 0;
            zipFileCount = 0;
            duplicateCount = 0;
            totalCount = 0;
            progressCount = 0;

            IsRunning = true;
 
            if (behavior == PhotoCopierActions.Copy || behavior == PhotoCopierActions.Overwrite)
            {
                await ProcessDirAsync(sourceDir, destDir, fileFilter).ConfigureAwait(false);
            }
            else if (behavior == PhotoCopierActions.Reorder)
            {
                Interlocked.Exchange(ref totalCount, System.IO.Directory.EnumerateFiles(destDir, "*.*", SearchOption.AllDirectories).Count());
                await ReorderDirAsync(destDir, pattern, destDir, false).ConfigureAwait(false);
            }
            else
            {
                outputHandler("Invalid behavior", ErrorCode.Error);
                return ReturnCode.Error;
            }

            return Interlocked.Add(ref canceled, 0) > 0 ? ReturnCode.Canceled : ReturnCode.Success;
        }
        catch (OperationCanceledException)
        {
            outputHandler("Canceled", ErrorCode.Warning);
            return ReturnCode.Canceled;
        }
        catch (Exception ex)
        {
            if (Interlocked.Add(ref canceled, 0) == 0)
            {
                if (Debugger.IsAttached) Debugger.Break();
                Interlocked.Increment(ref errorCount);
                outputHandler(ex.ToString(), ErrorCode.Error);
            }
            return ReturnCode.Error;
        }
        finally
        {
            foreach (ZipArchive archive in archives)
            {
                archive.Dispose();
            }

            archives.Clear();
            stopwatch.Stop();

            outputHandler();
            outputHandler($"elapsed={stopwatch.Elapsed}, filter=\"{fileFilter}\"");
            outputHandler($"{behavior}: zipFiles={zipFileCount}, newItems={totalCount}, duplicateFiles={duplicateCount}, skippedFolders={skipCount}, errors={errorCount}");

            IsRunning = false;
        }
    }

    private async Task ReorderDirAsync(string dir, string pattern, string root, bool isRetry)
    {
        int progress = 0;
        
        if (logging != LoggingVerbosity.Quiet) outputHandler($"Reordering dir:{Path.Combine(".", dir)}");
        int total = Interlocked.Add(ref totalCount, 0);

        foreach (string mediaFullPath in System.IO.Directory.EnumerateFiles(dir, "*.*", SearchOption.TopDirectoryOnly))
        {
            if (Interlocked.Add(ref canceled, 0) > 0) break;
            progress++;

            string mediaFileName = string.Empty;
            string sourceDir = dir.Replace(root, string.Empty, StringComparison.OrdinalIgnoreCase);
            string targetDir = string.Empty;

            try
            {
                // 1. Get date from media content
                string fileType = GetDateFromMediaFileContent(mediaFullPath, out DateTime? mediaDate);

                if (fileType == null)
                {
                    if (Debugger.IsAttached) Debugger.Break();
                    Interlocked.Increment(ref errorCount);
                    outputHandler($"{(isRetry ? RetryText : "")}Error:{Progress(progress, total)} Invalid file type", ErrorCode.Error);
                    statusUpdate($"{Progress(progress, total)}");
                    return;
                }

                mediaFileName = Path.GetFileName(mediaFullPath);

                bool hasMediaMetaDate = mediaDate.HasValue;
                if (mediaDate == null)
                {
                    // 2. Get date from media file name
                    TryGetDateFromMediaFileName(mediaFullPath, out mediaDate);
                }

                if (mediaDate != null && !hasMediaMetaDate)
                {
                    if (string.Compare(fileType, "jpeg", StringComparison.OrdinalIgnoreCase) == 0 ||
                        string.Compare(fileType, "tiff", StringComparison.OrdinalIgnoreCase) == 0 ||
                        string.Compare(fileType, "png", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        if (!listOnly)
                        {
                            ExifData exif = new ExifData(mediaFullPath);
                            _ = exif.SetDateTaken(mediaDate.Value);
                            _ = exif.SetTagValue(ExifTag.DateTimeOriginal, mediaDate.Value);
                            exif.Save();
                        }
                    }
                    else
                    {
                        if (Debugger.IsAttached) Debugger.Break();
                    }
                }

                string newFileDir = DirectoryNameFromDate(root, mediaDate, progress, total, hasMediaMetaDate, out targetDir);
                if (newFileDir == null) continue;

                string newFilePath = Path.Combine(newFileDir, mediaFileName);

                if (newFilePath.Equals(mediaFullPath, StringComparison.OrdinalIgnoreCase))
                {
                    Interlocked.Increment(ref skipCount);
                    if (logging == LoggingVerbosity.Verbose)
                    {
                        outputHandler($"{(isRetry ? RetryText : "")}{Progress(progress, total)} Skipping duplicate folder:\"{Path.Combine(".", sourceDir)}\"");
                    }
                    statusUpdate($"{Progress(progress, total)}");
                    continue;
                }

                if (newFileDir == null)
                {
                    if (Debugger.IsAttached) Debugger.Break();
                    Interlocked.Increment(ref errorCount);
                    outputHandler($"{(isRetry ? RetryText : "")}Error:{Progress(progress, total)} New file dir should not be null", ErrorCode.Error);
                    statusUpdate($"{Progress(progress, total)}");
                    return;
                }

                if (!listOnly)
                {
                    System.IO.Directory.CreateDirectory(newFileDir);
                    File.Move(mediaFullPath, newFilePath);
                }

                string verb = listOnly ? "Would move" : "Moved";
                if (logging != LoggingVerbosity.Quiet) outputHandler($"{(isRetry ? RetryText : "")}{Progress(progress, total)} {verb} file:\"{mediaFileName}\" to \"{Path.Combine(".", targetDir)}\"");
                statusUpdate($"{Progress(progress, total)}");
            }
            catch (Exception ex)
            {
                if (Interlocked.Add(ref canceled, 0) == 0)
                {
                    if (Debugger.IsAttached) Debugger.Break();
                    Interlocked.Increment(ref errorCount);
                    outputHandler($"Error:{Progress(progress, total)} Could not move file:\"{mediaFileName}\" from \"{Path.Combine(".", sourceDir)}\" to \"{Path.Combine(".", targetDir)}\". Reason={ex.Message}", ErrorCode.Error);
                }
            }
        }

        foreach (string mediaFile in System.IO.Directory.EnumerateDirectories(dir, "*.*", SearchOption.TopDirectoryOnly))
        {
            if (Interlocked.Add(ref canceled, 0) > 0) break;
            await ReorderDirAsync(Path.Combine(dir, mediaFile), pattern, root, isRetry).ConfigureAwait(false);
        }

        if (!System.IO.Directory.EnumerateFileSystemEntries(dir).Any())
        {
            System.IO.Directory.Delete(dir, true);
        }
    }

    private string Progress(int progress, int total)
    {
        return $"{progress}/{total}";
    }

    private string GetDateFromMediaStream(Stream mediaStream, out DateTime? mediaDate)
    {
        ArgumentNullException.ThrowIfNull(mediaStream);

        mediaStream.Seek(0, SeekOrigin.Begin);
        IReadOnlyList<MetadataExtractor.Directory> metaDir = ImageMetadataReader.ReadMetadata(mediaStream);
        return TryGetDateFromMetaDir(metaDir, out mediaDate);
    }

    private string GetDateFromMediaFileContent(string mediaFile, out DateTime? mediaDate)
    {
        mediaDate = null;
        if (!File.Exists(mediaFile)) return null;

        IReadOnlyList<MetadataExtractor.Directory> metaDir = ImageMetadataReader.ReadMetadata(mediaFile);
        return TryGetDateFromMetaDir(metaDir, out mediaDate);
    }

    private static bool TryGetDateFromMediaFileName(string mediaFile, out DateTime? mediaDate)
    {
        mediaDate = null;

        string filename = Path.GetFileNameWithoutExtension(mediaFile);
        if (filename.Length == 10)
        {
            // might be MMddyyhhmm
            string[] pieces = SplitByLength(filename, 2, 2, 2, 2, 2);
            if (pieces.Length > 4)
            {
                if (DateTime.TryParse($"{pieces[0]}-{pieces[1]}-{pieces[2]} {pieces[3]}:{pieces[4]}", out DateTime date))
                {
                    mediaDate = date;
                    return true;
                }
            }
            else if (pieces.Length > 2)
            {
                if (DateTime.TryParse($"{pieces[0]}-{pieces[1]}-{pieces[2]}", out DateTime date))
                {
                    mediaDate = date;
                    return true;
                }
            }
        }
        else if (filename.Length == 8)
        {
            // might be yyyyMMdd
            string[] pieces = SplitByLength(filename, 4, 2, 2);
            if (DateTime.TryParse($"{pieces[0]}-{pieces[1]}-{pieces[2]}", out DateTime date))
            {
                mediaDate = date;
                return true;
            }
        }

        return false;
    }

    private static string TryGetDateFromMetaDir(IReadOnlyList<MetadataExtractor.Directory> metaDir, out DateTime? mediaDate)
    {
        mediaDate = null;

        FileTypeDirectory fileType = metaDir.OfType<FileTypeDirectory>().FirstOrDefault();
        string fileTypeName = fileType?.GetDescription(FileTypeDirectory.TagDetectedFileTypeName)?.ToLower();
        if (fileTypeName == null) return null;

        switch (fileTypeName)
        {
            case "tiff":
            case "png":
            case "jpeg":
            {
                ExifDirectoryBase exif1 = metaDir.OfType<ExifSubIfdDirectory>().FirstOrDefault();
                ExifDirectoryBase exif2 = metaDir.OfType<ExifIfd0Directory>().FirstOrDefault();

                string originalDate = exif1?.GetDescription(ExifDirectoryBase.TagDateTimeOriginal) ??
                    exif2?.GetDescription(ExifDirectoryBase.TagDateTimeOriginal) ??
                    exif1?.GetDescription(ExifDirectoryBase.TagDateTime) ??
                    exif2?.GetDescription(ExifDirectoryBase.TagDateTime);

                if (originalDate != null)
                {
                    if (DateTime.TryParse(originalDate, out DateTime date))
                    {
                        mediaDate = date;
                    }

                    if (mediaDate == null)
                    {
                        DateFromStringTryParse(originalDate, out mediaDate);
                    }
                }

                return fileTypeName;
            }

            case "quicktime":
            case "mp4":
            {
                QuickTimeMovieHeaderDirectory mp4Dir = metaDir.OfType<QuickTimeMovieHeaderDirectory>().FirstOrDefault();
                string creationDate = mp4Dir?.GetDescription(QuickTimeMovieHeaderDirectory.TagCreated);

                if (creationDate != null)
                {
                    CultureInfo enUS = new CultureInfo("en-US");
                    DateTime.TryParseExact(creationDate, "ddd MMM dd HH:mm:ss yyyy", enUS, DateTimeStyles.AssumeLocal, out DateTime date);
                    if (date != DateTime.MinValue)
                    {
                        mediaDate = date;
                    }
                }

                return fileTypeName;
            }

            default:
            {
                if (Debugger.IsAttached) Debugger.Break();
                break;
            }
        }

        return fileTypeName;
    }

    private static bool DateFromStringTryParse(string dateString, out DateTime? mediaDate)
    {
        int year = 1;
        int month = 6;
        int day = 15;
        int hour = 12;
        int minute = 0;
        int second = 0;

        string[] chunks = FindNumberStrings(dateString);

        if (chunks.Length > 0 && int.TryParse(chunks[0], out int y))
        {
            year = y;
            if (year < 2000)
            {
                year += 2000;
            }
        }

        if (chunks.Length > 1 && int.TryParse(chunks[1], out int mo))
        {
            month = mo;
        }

        if (chunks.Length > 2 && int.TryParse(chunks[2], out int d))
        {
            day = d;
        }

        if (chunks.Length > 3 && int.TryParse(chunks[3], out int h))
        {
            hour = h;
        }

        if (chunks.Length > 4 && int.TryParse(chunks[4], out int min))
        {
            minute = min;
        }

        if (chunks.Length > 5 && int.TryParse(chunks[5], out int s))
        {
            second = s;
        }

        try
        {
            mediaDate = new DateTime(year, month, day, hour, minute, second);
            return true;
        }
        catch (Exception ex)
        {
            if (Debugger.IsAttached)
            {
                Debug.WriteLine(ex.Message);
                Debugger.Break();
            }

            mediaDate = null;
        }

        return false;
    }

    private async Task ProcessDirAsync(string src, string dst, string fileFilter)
    {
        if (logging != LoggingVerbosity.Quiet) outputHandler($"Processing dir:{src}");

        foreach (string entity in System.IO.Directory.EnumerateFiles(src, fileFilter, SearchOption.AllDirectories))
        {
            if (Interlocked.Add(ref canceled, 0) > 0) break;

            FileInfo fileInfo = new FileInfo(entity);
            if (!fileInfo.Attributes.HasFlag(FileAttributes.Directory))
            {
                await ProcessZipAsync(entity).ConfigureAwait(false);
            }
        }

        string[] keysToRemove = mediaKeys.Where(x => x.Value.EntryName == null).Select(x => x.Key).ToArray();
        foreach (string key in keysToRemove)
        {
            if (!mediaKeys.TryRemove(key, out _))
            {
                if (Debugger.IsAttached) Debugger.Break();
            }
        }

        if (mediaKeys.IsEmpty) return;

        Interlocked.Exchange(ref totalCount, mediaKeys.Count);

        if (parallel)
        {
            outputHandler("Running in parallel - entry counts may be out of order");
            await Task.Run(() => Parallel.ForEach(mediaKeys.Keys, entryKey => ProcessEntry(entryKey, dst, false))).ConfigureAwait(false);
        }
        else
        {
            outputHandler("Running sequentially");
            foreach (string entryKey in mediaKeys.Keys)
            {
                if (Interlocked.Add(ref canceled, 0) > 0) break;
                await Task.Run(() => ProcessEntry(entryKey, dst, false)).ConfigureAwait(false);
            }
        }

        if (Interlocked.Add(ref canceled, 0) > 0)
        {
            return;
        }

        mediaKeys.Clear();

        if (retryMediaKeys.Count > 0)
        {
            progressCount = 0;
            totalCount = retryMediaKeys.Count;
            outputHandler($"Retrying {retryMediaKeys.Count} entries.");

            bool processed = false;
            while (Interlocked.Add(ref canceled, 0) == 0)
            {
                if (retryMediaKeys.Count == 0) break;

                while (retryMediaKeys.TryDequeue(out MediaInfo result))
                {
                    mediaKeys[result.EntryKey] = result;
                }

                foreach (string key in mediaKeys.Keys)
                {
                    processed = true;
                    await Task.Run(() => ProcessEntry(key, dst, true)).ConfigureAwait(false);
                }

                if (processed)
                {
                    // we did something so wait a bit and try to see if there are more retries
                    Stopwatch timer = Stopwatch.StartNew();
                    while (Interlocked.Add(ref canceled, 0) == 0 && timer.Elapsed < TimeSpan.FromSeconds(1))
                    {
                        Task.Delay(50).Wait();
                    }

                    processed = false;
                }
            }
        }
    }

    private async Task ProcessZipAsync(string zip)
    {
        try
        {
            ZipArchive archive = ZipFile.OpenRead(zip);
            archives.Add(archive);

            zipFileCount++;

            ZipArchiveEntry[] allEntries = archive.Entries.ToArray();

            HashSet<string> extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (ZipArchiveEntry entry in allEntries)
            {
                if (Interlocked.Add(ref canceled, 0) > 0) break;
                extensions.Add(Path.GetExtension(entry.Name));
            }

            Dictionary<string, Dictionary<string, ZipArchiveEntry>> entries = new Dictionary<string, Dictionary<string, ZipArchiveEntry>>(StringComparer.OrdinalIgnoreCase);

            foreach (ZipArchiveEntry entry in allEntries)
            {
                if (Interlocked.Add(ref canceled, 0) > 0) break;
                string root = entry.FullName;

                string fname = Path.GetFileName(root);
                string path = Path.GetDirectoryName(root);
                if (path == null)
                {
                    Interlocked.Increment(ref errorCount);
                    outputHandler($"Error: root, {root} is not a directory. Reason=InvalidPath", ErrorCode.Error);
                    continue;
                }

                string[] parts = fname.Split(separator, 2);

                string ext = null;
                if (parts.Length > 1) ext = parts[1];
                string key = Path.Combine(path, parts[0]);

                if (!entries.TryGetValue(key, out Dictionary<string, ZipArchiveEntry> value))
                {
                    value = new Dictionary<string, ZipArchiveEntry>(StringComparer.OrdinalIgnoreCase);
                    entries[key] = value;
                }

                Dictionary<string, ZipArchiveEntry> values = value;
                if (string.IsNullOrEmpty(ext) || (values.ContainsKey(ext) && Debugger.IsAttached)) Debugger.Break();
                if (string.IsNullOrEmpty(ext) || values.ContainsKey(ext)) continue;
                value[ext] = entry;
            }

            if (logging != LoggingVerbosity.Quiet) outputHandler($"Processing zip:{zip}, entries count:{entries.Count}");

            // group the media and meta data with the entries by media name
            foreach (string key in entries.Keys)
            {
                if (Interlocked.Add(ref canceled, 0) > 0) break;
                Dictionary<string, ZipArchiveEntry> values = entries[key];

                if (!mediaKeys.TryGetValue(key, out MediaInfo mediaInfo))
                {
                    mediaInfo = new MediaInfo { EntryKey = key };
                }

                foreach (string valueKey in values.Keys)
                {
                    if (Interlocked.Add(ref canceled, 0) > 0) break;

                    ZipArchiveEntry archiveEntry = values[valueKey];

                    if (valueKey.EndsWith("json", StringComparison.OrdinalIgnoreCase))
                    {
                        await using (Stream zipStream = archiveEntry.Open())
                        {
                            if (zipStream.CanRead)
                            {
                                using (StreamReader reader = new StreamReader(zipStream, Encoding.UTF8))
                                {
                                    mediaInfo.MetaJson = reader.ReadToEnd();
                                }
                            }
                        }
                    }
                    else if (!valueKey.EndsWith("htm", StringComparison.OrdinalIgnoreCase) && !valueKey.EndsWith("html", StringComparison.OrdinalIgnoreCase))
                    {
                        mediaInfo.EntryName = archiveEntry.FullName;
                        mediaInfo.ArchivePath = zip;
                    }
                }

                mediaKeys[key] = mediaInfo;
                mediaInfo = null;
            }
        }
        catch (Exception ex)
        {
            if (Debugger.IsAttached) Debugger.Break();
            Interlocked.Increment(ref errorCount);
            outputHandler($"Error: Could not process zipfile {zip}. Reason={ex.Message}", ErrorCode.Error);
        }
    }

    private string DirectoryNameFromDate(string root, DateTime? date, int progress, int total, bool isRetry, out string targetDir)
    {
        targetDir = "no-date";

        if (date != null)
        {   
            string year = $"{date.Value:yyyy}";
            string month = $"{date.Value:MM}";
            string day = $"{date.Value:dd}";
            string hour = $"{date.Value:HH}";

            targetDir = pattern.Replace("$y", year, StringComparison.OrdinalIgnoreCase)
                .Replace("$m", month)
                .Replace("$d", day, StringComparison.OrdinalIgnoreCase)
                .Replace("$h", hour, StringComparison.OrdinalIgnoreCase)
                .Replace("$M", MonthName(date.Value.Month));
        }

        string newFilePath = Path.GetFullPath(Path.Combine(root, targetDir));

        if (newFilePath == null || !newFilePath.StartsWith(root, StringComparison.OrdinalIgnoreCase))
        {
            if (Debugger.IsAttached) Debugger.Break();
            Interlocked.Increment(ref errorCount);
            outputHandler($"{(isRetry ? RetryText : "")}Error:{Progress(progress, total)} Invalid target file path:\"{newFilePath ?? "null"}\"", ErrorCode.Error);
            return null;
        }

        return newFilePath;
    }

    private static string MonthName(int monthNumber)
    {
        CultureInfo culture = CultureInfo.CurrentCulture;
        DateTimeFormatInfo dtfi = culture.DateTimeFormat;
        return dtfi.GetMonthName(monthNumber);
    }

    private void RecurseDeserialize(Dictionary<string, object> result)
    {
        foreach (KeyValuePair<string, object> keyValuePair in result.ToArray())
        {
            JArray jarray = keyValuePair.Value as JArray;

            if (jarray != null)
            {
                List<Dictionary<string, object>> dictionaries = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jarray.ToString());
                if (dictionaries == null)
                {
                    Interlocked.Increment(ref errorCount);
                    outputHandler($"Error: Could not deserialize object {jarray}. Reason=InvalidObject", ErrorCode.Error);
                    continue;
                }

                result[keyValuePair.Key] = dictionaries;

                foreach (Dictionary<string, object> dictionary in dictionaries)
                {
                    RecurseDeserialize(dictionary);
                }
            }
            else
            {
                JObject jobject = keyValuePair.Value as JObject;
                if (jobject != null)
                {
                    Dictionary<string, object> dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(jobject.ToString());
                    result[keyValuePair.Key] = dictionary;
                    RecurseDeserialize(dictionary);
                }
            }
        }
    }

    private static string[] SplitByLength(string chunk, params int[] lengths)
    {
        if (string.IsNullOrEmpty(chunk)) return null;
        if (lengths == null || lengths.Length == 0) return new[] { chunk };

        int total = 0;
        foreach (int length in lengths)
        {
            total += length;
        }

        if (total > chunk.Length) return null;

        List<string> result = new List<string>();

        int index = 0;
        StringBuilder sb = new StringBuilder();
        foreach (int length in lengths)
        {
            for (int ii = 0; ii < length; ii++)
            {
                sb.Append(chunk[index++]);
            }

            result.Add(sb.ToString());
            sb.Clear();
        }

        if (index < chunk.Length && sb.Length > 0)
        {
            while (index < chunk.Length)
            {
                sb.Append(chunk[index++]);
            }

            result.Add(sb.ToString());
        }

        return result.ToArray();
    }

    private static string[] FindNumberStrings(string fileName)
    {
        List<string> strings = new List<string>();

        StringBuilder sb = new StringBuilder();
        foreach (char ch in fileName)
        {
            if (char.IsDigit(ch))
            {
                sb.Append(ch);
            }
            else if (sb.Length > 0)
            {
                strings.Add(sb.ToString().ToLower());
                sb.Clear();
            }
        }

        if (sb.Length > 0)
        {
            strings.Add(sb.ToString().ToLower());
        }

        return strings.ToArray();
    }

    private bool TryGetDateFromTakeoutMetadata(string text, out DateTime? mediaDate)
    {
        mediaDate = null;

        try
        {
            Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(text);
            if (result == null)
            {
                return false;
            }

            RecurseDeserialize(result);

            if (result.TryGetValue("photoTakenTime", out object photoTaken))
            {
                Dictionary<string, object> data = photoTaken as Dictionary<string, object>;
                if (data != null)
                {
                    if (data.TryGetValue("timestamp", out object value))
                    {
                        if (value is string v)
                        {
                            long l = long.Parse(v);
                            if (TryConvertFromUnixTimestampParse(l, out mediaDate))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            if (Debugger.IsAttached)
            {
                Debug.WriteLine(ex.Message);
                Debugger.Break();
            }
        }

        return false;
    }

    private static bool TryConvertFromUnixTimestampParse(double timestamp, out DateTime? mediaDate)
    {
        mediaDate = null;

        try
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            mediaDate = origin.AddSeconds(timestamp);
            return true;
        }
        catch (Exception ex)
        {
            if (Debugger.IsAttached)
            {
                Debug.WriteLine(ex.Message);
                Debugger.Break();
            }
        }

        return false;
    }

    public Configs GetConfiguration()
    {
        return new Configs(paramTypes, true);
    }

    private void ProcessEntry(string entryKey, string dst, bool isRetry)
    {
        if (Interlocked.Add(ref canceled, 0) > 0) return;
        int total = Interlocked.Add(ref totalCount, 0);

        bool foundEntry = mediaKeys.TryGetValue(entryKey, out MediaInfo mediaInfo);
        int progress = Interlocked.Increment(ref progressCount);

        if (!foundEntry || mediaInfo == null || mediaInfo.EntryName == null)
        {
            if (Debugger.IsAttached) Debugger.Break();
            Interlocked.Increment(ref errorCount);
            outputHandler($"{(isRetry ? RetryText : "")}Error:{Progress(progress, total)} Invalid zip entry key: {entryKey}", ErrorCode.Error);
            statusUpdate($"{Progress(progress, total)}");
            return;
        }

        mediaInfo.ProgressId = progress;

        ZipArchive archive = null;
        ZipArchiveEntry entry = null;
        List<IDisposable> disposables = new List<IDisposable>();
        FileStream fileStream = null;
        Stream zipStream = null;
        DateTime? mediaDate = null;
        ExifData exif = null;
        string targetDir;

        try
        {
            archive = ZipFile.OpenRead(mediaInfo.ArchivePath);
            disposables.Add(archive);

            entry = archive.GetEntry(mediaInfo.EntryName);
            if (entry == null) return;

            zipStream = GetStream(entry.Length);
            disposables.Add(zipStream);

            using (Stream entryStream = entry.Open())
            {
                entryStream?.CopyTo(zipStream);
            }

            // 1. get date from media metadata
            string fileType = GetDateFromMediaStream(zipStream, out mediaDate);

            if (fileType == null)
            {
                if (Debugger.IsAttached) Debugger.Break();
                Interlocked.Increment(ref errorCount);
                outputHandler($"{(isRetry ? RetryText : "")}Error:{Progress(progress, total)} Invalid file type", ErrorCode.Error);
                statusUpdate($"{Progress(progress, total)}");
                return;
            }

            bool hasMediaMetaDate = mediaDate.HasValue;
            if (mediaDate == null && mediaInfo.MetaJson != null)
            {
                // 2. get date from archive meta data
                TryGetDateFromTakeoutMetadata(mediaInfo.MetaJson, out mediaDate);
            }

            if (mediaDate == null)
            {
                // 3. get date from entry name
                TryGetDateFromMediaFileName(entry.FullName, out mediaDate);
            }

            if (mediaDate != null && !hasMediaMetaDate)
            {
                if (string.Compare(fileType, "jpeg", StringComparison.OrdinalIgnoreCase) == 0 ||
                    string.Compare(fileType, "tiff", StringComparison.OrdinalIgnoreCase) == 0 ||
                    string.Compare(fileType, "png", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    zipStream.Seek(0, SeekOrigin.Begin);
                    exif = new ExifData(zipStream);
                    exif.SetDateTaken(mediaDate.Value);
                    exif.SetTagValue(ExifTag.DateTimeOriginal, mediaDate.Value);

                    Stream newZipStream = GetStream(entry.Length + 1024);
                    disposables.Add(newZipStream);

                    zipStream.Seek(0, SeekOrigin.Begin);
                    exif.Save(zipStream, newZipStream);

                    zipStream.Dispose();
                    zipStream = newZipStream;
                }
            }

            zipStream.Seek(0, SeekOrigin.Begin);

            string filePath = null;

            string folderPath = DirectoryNameFromDate(dst, mediaDate, progress, total, hasMediaMetaDate, out targetDir);

            if (!listOnly) System.IO.Directory.CreateDirectory(folderPath);
            filePath = Path.Combine(folderPath, entry.Name);

            string fname = entry.Name;
            if (File.Exists(filePath) && behavior == PhotoCopierActions.Copy)
            {
                // check conflict resolution handling
                // get the hash of the contents of the source and destination and 
                // compare. if the names are the same but the content is different then change
                // the name

                byte[] zipHash = null;
                byte[] fileHash = null;

                zipStream.Seek(0, SeekOrigin.Begin);
                zipHash = SHA512.Create().ComputeHash(zipStream);

                fileStream = new FileStream (filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 4096, true);
                if (fileStream == null)
                {
                    if (Debugger.IsAttached) Debugger.Break();
                    Interlocked.Increment(ref errorCount);
                    outputHandler($"{(isRetry ? RetryText : "")}Error:{Progress(progress, total)} Could not open existing file: {filePath}", ErrorCode.Error);
                    statusUpdate($"{Progress(progress, total)}");
                    return;
                }

                disposables.Add(fileStream);

                fileStream.Seek(0, SeekOrigin.Begin);
                fileHash = SHA512.Create().ComputeHash(fileStream);

                bool sameHash = zipHash.SequenceEqual(fileHash); // BytesEqual(zipHash, fileHash);

                // check content
                if (sameHash)
                {
                    // don't copy since everything is the same - name & content
                    string text = listOnly ? "List(duplicate)" : "Duplicate";
                    if (logging == LoggingVerbosity.Verbose) outputHandler($"{(isRetry ? RetryText : "")}{Progress(progress, total)} {text}:\"{entry.Name}\" in \"{Path.Combine(".", targetDir)}\"");
                    statusUpdate($"{Progress(progress, total)}");
                    Interlocked.Increment(ref duplicateCount);
                    return;
                }

                string path = Path.GetDirectoryName(filePath) ?? "";
                fname = Path.GetFileNameWithoutExtension(filePath);
                string ext = Path.GetExtension(filePath);
                int ii = 0;

                // change the name
                while (true)
                {
                    filePath = Path.Combine(path, $"{fname}_{ii++}{ext}");
                    if (!File.Exists(filePath))
                    {
                        fileStream = null;
                        break;
                    }

                    FileStream testStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 4096, true);
                    disposables.Add(testStream);

                    fileHash = SHA256.Create().ComputeHash(testStream);

                    // same name - different content?
                    if (zipHash.SequenceEqual(fileHash))
                    {
                        // don't copy existing media
                        Interlocked.Increment(ref duplicateCount);
                        if (logging == LoggingVerbosity.Verbose) outputHandler($"{(isRetry ? RetryText : "")}{Progress(progress, total)} Skipping duplicate file:\"{entry.Name}\" in \"{Path.Combine(".", targetDir, fname)}\"");
                        statusUpdate($"{Progress(progress, total)}");
                        return;
                    }

                    fileStream = testStream;
                }
            }

            if (!listOnly && (behavior == PhotoCopierActions.Copy || behavior == PhotoCopierActions.Overwrite))
            {
                if (fileStream == null)
                {
                    string dir = Path.GetDirectoryName(filePath);
                    if (dir != null) System.IO.Directory.CreateDirectory(dir);

                    fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, 4096, true);
                    if (fileStream == null)
                    {
                        if (Debugger.IsAttached) Debugger.Break();
                        Interlocked.Increment(ref errorCount);
                        outputHandler($"{(isRetry ? RetryText : "")}Error:{Progress(progress, total)} Could not open or create file: {filePath}", ErrorCode.Error);
                        statusUpdate($"{Progress(progress, total)}");
                        return;
                    }

                    disposables.Add(fileStream);
                }

                zipStream.Seek(0, SeekOrigin.Begin);
                fileStream.Seek(0, SeekOrigin.Begin);

                string verb = listOnly ? "Would copy" : "Copied";
                if (logging != LoggingVerbosity.Quiet) outputHandler($"{(isRetry ? RetryText : "")}{Progress(progress, total)} {verb} file:\"{entry.Name}\" to \"{Path.Combine(".", targetDir, fname)}\"");
                statusUpdate($"{Progress(progress, total)}");

                if (exif == null)
                {
                    zipStream.CopyTo(fileStream);
                }
                else
                {
                    exif.Save(zipStream, fileStream);
                }

                if (mediaDate != null) File.SetCreationTime(filePath, mediaDate.Value);
            }
        }
        catch (IOException ioex)
        {
            if (ioex.HResult == -2147024864)
            {
                retryMediaKeys.Enqueue(mediaInfo);
                return;
            }

            if (Debugger.IsAttached) Debugger.Break();
            Interlocked.Increment(ref errorCount);
            outputHandler($"{(isRetry ? RetryText : "")}Error:{Progress(progress, total)} Could not extract {entryKey}. Reason={ioex.Message}", ErrorCode.Error);
            statusUpdate($"{Progress(progress, total)}");
        }
        catch (Exception ex)
        {
            if (Debugger.IsAttached) Debugger.Break();
            Interlocked.Increment(ref errorCount);
            outputHandler($"{(isRetry ? RetryText : "")}Error:{Progress(progress, total)} Could not extract {entryKey}. Reason={ex.Message}", ErrorCode.Error);
            statusUpdate($"{Progress(progress, total)}");
        }
        finally
        {
            foreach (IDisposable item in disposables)
            {
                item?.Dispose();
            }
        }

        return;
    }

    private static Stream GetStream(long length)
    {
        if (length >= 1 * 1024 * 1024)
        {
            return new FileStream(Path.GetTempFileName(), FileMode.Open, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose);
        }

        return new MemoryStream(Convert.ToInt32(length));
    }

    public static bool ValidateSource(string folderPath, string filter, out string reason)
    {
        reason = string.Empty;

        try
        {
            if (!System.IO.Directory.Exists(folderPath))
            {
                reason = $"Could not find: {folderPath}";
                return false;
            }

            // test file existence
            if (!System.IO.Directory.EnumerateFiles(folderPath, filter, SearchOption.TopDirectoryOnly).Any())
            {
                reason = $"Could not locate any media archive folders matching {filter ?? ""}";
                return false;
            }
        }
        catch (Exception ex)
        {
            reason= ex.Message;
            return false;
        }

        return true;
    }

    public static bool ValidateDestination(string folderPath, out string reason)
    {
        reason = string.Empty;

        try
        {
            if (!System.IO.Directory.Exists(folderPath))
            {
                reason = $"Could not find: {folderPath}";
                return false;
            }

            // test permissions
            Random rnd = new Random();
            StringBuilder sb = new StringBuilder();
            for (int ii = 0; ii < 8; ii++)
            {
                sb.Append(Convert.ToChar('A' + rnd.Next() % 26));
            }
            string testFile = Path.Combine(folderPath, $"{sb}.tmp");
            using (FileStream stream = new FileStream(testFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose))
            {
                byte test = 200;

                stream.WriteByte(test);
                stream.Flush();

                stream.Position = 0;
                int b = stream.ReadByte();

                if (b != test)
                {
                    reason = $"Could not write to test file";
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            reason = ex.Message;
            return false;
        }

        return true;
    }
}
