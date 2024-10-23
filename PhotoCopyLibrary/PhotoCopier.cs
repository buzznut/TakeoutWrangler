//  <@$&< copyright begin >&$@> 8EF3F3608034F1A9CC6F945BA1A2053665BCA4FFC65BF31743F47CE665FDB0FB:20241017.A:2024:10:17:18:28
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024 Stewart A. Nutter - All Rights Reserved.
// 
// This software application and source code is copyrighted and is licensed
// for use by you only. Only this product's installation files may be shared.
// 
// This license does not allow the removal or code changes that cause the
// ignoring, or modifying the copyright in any form.
// 
// This software is licensed "as is" and no warranty is implied or given.
// 
// Stewart A. Nutter
// 711 Indigo Ln
// Waunakee, WI  53597
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// <@$&< copyright end >&$@>

using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PhotoCopyLibrary;

public delegate void HandleOutput(string output = null);

public class PhotoCopier
{
    private int zipFileCount;
    private int newCount;
    private int errorCount;
    private int skipCount;
    private int fileCount;
    private int tooBig;

    private string sourceDir;
    private string destDir;
    private string pattern = "$y_$m";
    private string fileFilter = "takeout-*.zip";
    private bool quiet;
    private PhotoCopierActions action = PhotoCopierActions.List;

    private readonly char[] separator = new char[] { '.' };
    private readonly HandleOutput outputHandler = DummyHandler;
    private readonly ConcurrentBag<ZipArchive> archives = new ConcurrentBag<ZipArchive>();
    private readonly ConcurrentDictionary<string, MediaInfo> mediaKeys = new ConcurrentDictionary<string, MediaInfo>(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentQueue<MediaInfo> retryMediaKeys = new ConcurrentQueue<MediaInfo>();

    readonly List<ConfigParam> paramTypes = new List<ConfigParam>
    {
        new ConfigParam { Name = "filter", Default="takeout-*.zip", PType = ParamType.String },
        new ConfigParam { Name = "source", Synonyms = ["src", "sourceDir"], PType = ParamType.String },
        new ConfigParam { Name = "destination", Synonyms = ["dst", "dest", "tgt", "target", "destinationDir"], PType = ParamType.String },
        new ConfigParam { Name = "quiet", PType = ParamType.Bool, Default = false },
        new ConfigParam { Name = "help", Synonyms = ["?"], Default = false, PType = ParamType.Bool },
        new ConfigParam { Name = "action", Default = "list", PType = ParamType.String },
        new ConfigParam { Name = "pattern", Default = "$y_$m", PType = ParamType.String },
    };

    private static void DummyHandler(string output = null)
    {
        // do nothing with the string
    }

    public PhotoCopier() { }

    public PhotoCopier(HandleOutput output)
    {
        outputHandler = output;
    }

    public int Initialize(string appName, bool help, string sourceDir, string destDir, string actionText, string pattern, string fileFilter, bool quiet)
    {
        try
        {
            string[] allActions = Enum.GetNames(typeof(PhotoCopierActions));

            List<string> reasons = new List<string>();

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
                outputHandler($"  source=path        folder that contains the takeout zip file(s).");
                outputHandler($"                     (default=null");
                outputHandler();
                outputHandler($"  destination=path   folder where to put photo contents (files and subfolders)");
                outputHandler($"                     (default=null");
                outputHandler();
                outputHandler($"  action=option      option is one of {PhotoCopierActions.List} or {PhotoCopierActions.Copy}.");
                outputHandler($"                     (default={PhotoCopierActions.List}");
                outputHandler();
                outputHandler($"                     example: action={PhotoCopierActions.List} would only show potential errors without changing destination");
                outputHandler();
                outputHandler($"                     {PhotoCopierActions.List} media files only. Does not change the destination.");
                outputHandler($"                     {PhotoCopierActions.Copy} media files to destination.");
                outputHandler();
                outputHandler($" quiet=boolean       true or false. If true actions do not display log. Valid on copy only.");
                outputHandler($"                     (default=false)");
                outputHandler();
                outputHandler($" pattern=text        Destination folder name pattern. (default=$y_$m");
                outputHandler();
                outputHandler($"                     example: if date-time, is March 27, 2024 at 3:33PM");
                outputHandler($"                              $y = 4 digit year. ex: 2024");
                outputHandler($"                              $m = 2 digit month. ex: 03");
                outputHandler($"                              $d = 2 digit day. ex: 27");
                outputHandler($"                              $h = 2 digit hour. ex: 15 (24 hours per day)");
                outputHandler();
                outputHandler($" filter=text         photo archive file filter. (default='takeout-*.zip')");
                outputHandler();

                if (reasons.Count > 0)
                {
                    outputHandler("Issues: ");

                    int index = 1;
                    foreach (string reason in reasons)
                    {
                        outputHandler($"  {index++}. {reason}");
                    }

                    outputHandler();
                }

                return 1;
            }

            string[] sourceFiles = Directory.GetFiles(sourceDir, fileFilter);
            if (sourceFiles == null || sourceFiles.Length == 0)
            {
                outputHandler($"No source files found. sourceDir:\"{sourceDir}\", file filter:\"{fileFilter}\"");
                return 2;
            }

            // keep the parameters

            Enum.TryParse<PhotoCopierActions>(actionText, out PhotoCopierActions action);

            outputHandler("Parameters:");

            this.action = action;
            outputHandler($" Action:{action}");

            this.sourceDir = sourceDir;
            outputHandler($" Source:{sourceDir}");

            this.destDir = destDir;
            outputHandler($" Destination:{destDir}");

            this.fileFilter = fileFilter;
            outputHandler($" Filter:{fileFilter}");

            this.pattern = pattern;
            outputHandler($" Pattern:{pattern}");

            this.quiet = quiet;
            outputHandler($" Quiet:{quiet}");

            outputHandler();

            return 0;
        }
        catch (Exception ex)
        {
            if (Debugger.IsAttached) Debugger.Break();
            outputHandler(ex.ToString());
            return 1;
        }
    }

    public async Task<int> RunAsync()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        try
        {
            return await ProcessDirAsync(sourceDir, destDir, fileFilter, action, pattern);
        }
        catch (Exception ex)
        {
            if (Debugger.IsAttached) Debugger.Break();
            Interlocked.Increment(ref errorCount);
            outputHandler(ex.ToString());
            return 2;
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
            outputHandler($"{action}: counts: zip={zipFileCount}, new={newCount}, skipped={skipCount}, errors={errorCount}");
        }
    }

    private bool IsQuiet(PhotoCopierActions action)
    {
        return quiet;
    }

    private async Task<int> ProcessDirAsync(string src, string dst, string fileFilter, PhotoCopierActions action, string pattern)
    {
        if (!IsQuiet(action)) outputHandler($"Processing dir:{src}");

        foreach (string entity in Directory.EnumerateFiles(src, fileFilter, SearchOption.AllDirectories))
        {
            FileInfo fileInfo = new FileInfo(entity);
            if (!fileInfo.Attributes.HasFlag(FileAttributes.Directory))
            {
                await ProcessZipAsync(entity, action);
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(entity);
            if (directoryInfo.Attributes.HasFlag(FileAttributes.Directory))
            {
                await ProcessDirAsync(Path.Combine(src, entity), Path.Combine(dst, entity), fileFilter, action, pattern);
            }
        }

        string[] entryKeys = mediaKeys.Where(x => x.Value.EntryName == null).Select(x => x.Key).ToArray();
        foreach (string key in entryKeys)
        {
            mediaKeys.TryRemove(key, out _);
        }

        int count = mediaKeys.Count;
        await ProcessMediaAsync(dst, pattern, action, count);

        mediaKeys.Clear();

        while (true)
        {
            int retryCount = retryMediaKeys.Count;
            if (retryCount == 0) break;

            while (retryCount > 0)
            {
                retryCount--;
                retryMediaKeys.TryDequeue(out MediaInfo result);
                if (result == null)
                    continue;

                mediaKeys[result.EntryKey] = result;
            }
        }

        return 0;
    }

    private async Task ProcessZipAsync(string zip, PhotoCopierActions action)
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
                extensions.Add(Path.GetExtension(entry.Name));
            }

            Dictionary<string, Dictionary<string, ZipArchiveEntry>> entries = new Dictionary<string, Dictionary<string, ZipArchiveEntry>>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> skipped = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (ZipArchiveEntry entry in allEntries)
            {
                string root = entry.FullName;

                string fname = Path.GetFileName(root);
                string path = Path.GetDirectoryName(root);
                if (path == null)
                {
                    errorCount++;
                    outputHandler($"Error: root, {root} is not a directory. Reason=InvalidPath");
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

            if (!IsQuiet(action)) outputHandler($"Processing zip:{zip}, entries count:{entries.Count}");

            // group the media and meta data with the entries by media name
            foreach (string key in entries.Keys)
            {
                Dictionary<string, ZipArchiveEntry> values = entries[key];

                if (!mediaKeys.TryGetValue(key, out MediaInfo mediaInfo))
                {
                    mediaInfo = new MediaInfo { EntryKey = key };
                }

                foreach (string valueKey in values.Keys)
                {
                    ZipArchiveEntry archiveEntry = values[valueKey];
                    string name = archiveEntry.FullName;

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
            errorCount++;
            outputHandler($"Error: Could not process zipfile {zip}. Reason={ex.Message}");
        }
    }

    private static string NameFromDate(DateTime? date, string pattern)
    {
        if (date != null)
        {
            string year = $"{date.Value:yyyy}";
            string month = $"{date.Value:MM}";
            string day = $"{date.Value:dd}";
            string hour = $"{date.Value:HH}";

            return pattern.Replace("$y", year, StringComparison.OrdinalIgnoreCase)
                .Replace("$m", month, StringComparison.OrdinalIgnoreCase)
                .Replace("$d", day, StringComparison.OrdinalIgnoreCase)
                .Replace("$h", hour, StringComparison.OrdinalIgnoreCase);
        }

        return pattern.Replace("$y", "unknown", StringComparison.OrdinalIgnoreCase)
            .Replace("$m", "m", StringComparison.OrdinalIgnoreCase)
            .Replace("$d", "d", StringComparison.OrdinalIgnoreCase)
            .Replace("$h", "h", StringComparison.OrdinalIgnoreCase);
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
                    errorCount++;
                    outputHandler($"Error: Could not deserialize object {jarray}. Reason=InvalidObject");
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

    private static bool ByteCompare(byte[] zipHash, byte[] fileHash)
    {
        // both null - same
        if (zipHash == null || fileHash == null) return true;

        // if lengths different then not the same
        if (zipHash.Length != fileHash.Length) return false;

        for (int ii = 0; ii < zipHash.Length; ii++)
        {
            if (zipHash[ii] != fileHash[ii]) return false;
        }

        return true;
    }

    private static string[] FindNumberStrings(string fileName)
    {
        HashSet<string> strings = new HashSet<string>();

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

    private DateTime? GetDateFromMetadata(string text)
    {
        Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(text);
        if (result == null)
        {
            errorCount++;
            outputHandler($"Error: Could not parse {text}. Reason=InvalidJsonText");
            return null;
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
                        return ConvertFromUnixTimestamp(l);
                    }
                }
            }
        }

        return null;
    }

    private static DateTime ConvertFromUnixTimestamp(double timestamp)
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return origin.AddSeconds(timestamp);
    }

    public Configs GetConfiguration()
    {
        return new Configs(paramTypes, true);
    }

    private async Task ProcessMediaAsync(string dst, string pattern, PhotoCopierActions action, int count)
    {
        if (mediaKeys.IsEmpty) return;
        await Task.Run(() => Parallel.ForEach(mediaKeys.Keys, entryKey => ProcessEntry(null, entryKey, dst, pattern, action, count)));
    }

    private void ProcessEntry(ZipArchive archive, string entryKey, string dst, string pattern, PhotoCopierActions action, int count)
    {
        if (!mediaKeys.TryGetValue(entryKey, out MediaInfo mediaInfo) || mediaInfo.EntryName == null) return;

        bool mustDispose = false;
        ZipArchiveEntry entry = null;

        try
        {
            if (archive == null)
            {
                archive = ZipFile.OpenRead(mediaInfo.ArchivePath);
                mustDispose = true;
            }

            DateTime? date = null;
            Interlocked.Increment(ref fileCount);

            if (mediaInfo.MetaJson != null)
            {
                date = GetDateFromMetadata(mediaInfo.MetaJson);
            }

            entry = archive.GetEntry(mediaInfo.EntryName);
            if (entry == null) return;

            // first attempt at a date
            if (date == null && !string.IsNullOrEmpty(mediaInfo.EntryName))
            {
                // try parsing the date from the file name
                // 0210160837.jpg, 20160309_165804-0.jpg, pxl_20221203_171604561.mp4, resized_20160327_085441.jpg
                // vid_20180530_080957826.mp4

                foreach (string chunk in FindNumberStrings(entry.FullName))
                {
                    try
                    {
                        if (string.IsNullOrEmpty(chunk)) continue;
                        if (chunk.Length == 10)
                        {
                            // might be MMddyyhhmm
                            string[] pieces = SplitByLength(chunk, 2, 2, 2, 2, 2);
                            date = DateTime.Parse($"{pieces[0]}-{pieces[1]}-{pieces[2]}");
                        }
                        else if (chunk.Length == 8)
                        {
                            // might be yyyyMMdd
                            string[] pieces = SplitByLength(chunk, 4, 2, 2);
                            date = DateTime.Parse($"{pieces[0]}-{pieces[1]}-{pieces[2]}");
                        }
                    }
                    catch
                    {
                    }

                    if (date != null) break;
                }
            }

            // second attempt at a date - whole file path
            if (date == null)
            {
                // try parsing the date from the file name
                // 0210160837.jpg, 20160309_165804-0.jpg, pxl_20221203_171604561.mp4, resized_20160327_085441.jpg
                // vid_20180530_080957826.mp4

                foreach (string chunk in FindNumberStrings(entryKey))
                {
                    try
                    {
                        if (string.IsNullOrEmpty(chunk)) continue;
                        if (chunk.Length == 10)
                        {
                            // might be MMddyyhhmm
                            string[] pieces = SplitByLength(chunk, 2, 2, 2, 2, 2);
                            date = DateTime.Parse($"{pieces[0]}-{pieces[1]}-{pieces[2]}");
                        }
                        else if (chunk.Length == 8)
                        {
                            // might be yyyyMMdd
                            string[] pieces = SplitByLength(chunk, 4, 2, 2);
                            date = DateTime.Parse($"{pieces[0]}-{pieces[1]}-{pieces[2]}");
                        }
                        else if (chunk.Length == 4)
                        {
                            // might be yyyy
                            string[] pieces = SplitByLength(chunk, 4);
                            if (int.TryParse(pieces[0], out int year))
                            {
                                date = new DateTime(year, 1, 1);
                            }
                        }
                    }
                    catch
                    {
                    }

                    if (date != null) break;
                }
            }
            string filePath = null;

            string folder = NameFromDate(date, pattern);
            string folderPath = Path.Combine(dst, folder);

            if (action != PhotoCopierActions.List) Directory.CreateDirectory(folderPath);
            filePath = Path.Combine(folderPath, entry.Name);

            byte[] zipData = null;
            if (entry.Length < 1 * 1024 * 1024)
            {
                using (Stream zipStream = entry.Open())
                {
                    zipData = ReadAllBytesFromStream(zipStream);
                }
            }
            else
            {
                Interlocked.Increment(ref tooBig);
            }

            if (File.Exists(filePath))
            {
                // check conflict resolution handling
                // get the hash of the contents of the source and destination and 
                // compare. if the names are the same but the content is different then change
                // the name

                byte[] zipHash = null;
                byte[] fileHash = null;

                List<Task> tasks = new List<Task>();
                List<Stream> streams = new List<Stream>();

                try
                {
                    if (zipData != null)
                    {
                        Task t = Task.Run(() => zipHash = SHA256.HashData(zipData.AsSpan(0, zipData.Length)));
                        tasks.Add(t);
                    }
                    else
                    {
                        Task t = Task.Run(() =>
                        {
                            Stream zipStream = entry.Open();
                            if (zipStream == null)
                            {
                                return;
                            }

                            streams.Add(zipStream);

                            zipHash = SHA256.Create().ComputeHash(zipStream);
                        });

                        tasks.Add(t);
                    }

                    FileStream fileStream = File.OpenRead(filePath);
                    if (fileStream == null)
                    {
                        return;
                    }

                    streams.Add(fileStream);

                    Task f = Task.Run(() => fileHash = SHA256.Create().ComputeHash(fileStream));
                    tasks.Add(f);
                }
                finally
                {
                    Task.WaitAll(tasks.ToArray());

                    foreach (Stream stream in streams)
                    {
                        stream.Dispose();
                    }
                }

                // same name - different content?
                if (ByteCompare(zipHash, fileHash))
                {
                    // don't copy existing media
                    if (!IsQuiet(action)) outputHandler($"{Interlocked.Add(ref fileCount, 0)}/{count} Skipping existing file:\"{entryKey}\"->\"{filePath}\"");
                    Interlocked.Increment(ref skipCount);
                    return;
                }

                string path = Path.GetDirectoryName(filePath) ?? "";
                string fname = Path.GetFileNameWithoutExtension(filePath);
                string ext = Path.GetExtension(filePath);
                int ii = 0;

                // change the name
                while (true)
                {
                    filePath = Path.Combine(path, $"{fname}_{ii++}{ext}");
                    if (!File.Exists(filePath)) break;

                    using (FileStream fileStream = File.OpenRead(filePath))
                    {
                        fileHash = SHA256.Create().ComputeHash(fileStream);

                        // same name - different content?
                        if (ByteCompare(zipHash, fileHash))
                        {
                            // don't copy existing media
                            if (!IsQuiet(action)) outputHandler($"{Interlocked.Add(ref fileCount, 0)}/{count} Skipping existing file:\"{entryKey}\"->\"{filePath}\"");
                            Interlocked.Increment(ref skipCount);
                            return;
                        }
                    }
                }
            }

            string verb = action == PhotoCopierActions.List ? "Would copy" : "Copy";
            if (!IsQuiet(action)) outputHandler($"{Interlocked.Add(ref fileCount, 0)}/{count} {verb} file:\"{entryKey}\"->\"{filePath}\"");

            if (action != PhotoCopierActions.List)
            {
                if (zipData != null)
                {
                    using (FileStream mediaStream = File.Create(filePath))
                    {
                        mediaStream.Write(zipData);
                    }

                    zipData = null;
                }
                else
                {
                    entry.ExtractToFile(filePath);
                }
            }

            Interlocked.Increment(ref newCount);
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
            outputHandler($"Error:{Interlocked.Add(ref fileCount, 0)}/{count} Could not extract {entryKey}. Reason={ioex.Message}");
        }
        catch (Exception ex)
        {
            if (Debugger.IsAttached) Debugger.Break();
            Interlocked.Increment(ref errorCount);
            outputHandler($"Error:{Interlocked.Add(ref fileCount, 0)}/{count} Could not extract {entryKey}. Reason={ex.Message}");
        }
        finally
        {
            if (mustDispose)
            {
                archive.Dispose();
            }
        }

        return;
    }

    private static byte[] ReadAllBytesFromStream(Stream stream)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }
}
