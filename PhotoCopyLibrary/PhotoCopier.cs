//  <@$&< copyright begin >&$@> 24FE144C2255E2F7CCB65514965434A807AE8998C9C4D01902A628F980431C98:20241017.A:2025:2:25:8:47
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024-2025 Stewart A. Nutter - All Rights Reserved.
// No warranty is implied or given.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// <@$&< copyright end >&$@>

// using Library "CompactExifLib" https://www.codeproject.com/Articles/5251929/CompactExifLib-Access-to-EXIF-Tags-in-JPEG-TIFF-an
// for reading and writing EXIF data in JPEG, TIFF and PNG image files.
// © Copyright 2021 Hans-Peter Kalb

using MetadataExtractor;
using MetadataExtractor.Formats.Avi;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.FileType;
using MetadataExtractor.Formats.QuickTime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;

namespace PhotoCopyLibrary;

// Path name definitions:
// ---------------------------
// path   : full path to object
// dir    : full directory of object
// folder : relative directory of object (minus root)
// file   : name and extension of object
// fname  : name only of object
// ext    : extension (with leading period) of object

public delegate void HandleOutput(string outputStatusCode = null, MessageCode messageCode = MessageCode.Success);
public delegate void StatusUpdate(StatusCode status = StatusCode.Progress, int value = 0, string progressType = "");

public class PhotoCopier
{
    private CancellationTokenSource _cancel;
    private const string _retryText = "(Retry) ";
    private  int _canceled;
    private string _source;
    private string _destination;
    private string _backup;
    private string _pattern = "$y_$m";
    private string _fileFilter = "takeout-*.zip";
    private PhotoCopierActions _behavior = PhotoCopierActions.Copy;
    private LoggingVerbosity _logging = LoggingVerbosity.Verbose;
    private bool _listOnly = true;
    private bool _parallel = true;
    private string _junk;
    private static bool _isAdmin;
    private static readonly DateTime _notBeforeDate = new DateTime(1976, 1, 1);
    private HashSet<string> junkExtensions;
    private bool IsCanceled { get { return Interlocked.Add(ref _canceled, 0) > 0; } }
    private readonly char[] _separator = new char[] { '.' };
    private readonly HandleOutput _outputHandler = DummyOutput;
    private readonly StatusUpdate _statusUpdate = DummyStatus;
    private readonly ConcurrentBag<ZipArchive> _archives = new ConcurrentBag<ZipArchive>();
    private readonly ConcurrentDictionary<string, DateTime> _originalFileNameToDate = new ConcurrentDictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);

    readonly List<ConfigParam> _paramTypes = new List<ConfigParam>
    {
        new ConfigParam { Name = "filter", Default="takeout-*.zip", PType = ParamType.String },
        new ConfigParam { Name = "source", Synonyms = ["src", "sourceDir"], PType = ParamType.String },
        new ConfigParam { Name = "destination", Synonyms = ["dst", "dest", "tgt", "target", "destinationDir"], PType = ParamType.String },
        new ConfigParam { Name = "backup", PType = ParamType.String },
        new ConfigParam { Name = "logging", PType = ParamType.String, Default = nameof(LoggingVerbosity.Verbose) },
        new ConfigParam { Name = "help", Synonyms = ["?"], Default = false, PType = ParamType.Bool },
        new ConfigParam { Name = "action", Default = nameof(PhotoCopierActions.Copy), PType = ParamType.String },
        new ConfigParam { Name = "pattern", Default = "$y_$m", PType = ParamType.String },
        new ConfigParam { Name = "listonly", Synonyms = ["list"], Default = true, PType = ParamType.Bool },
        new ConfigParam { Name = "parallel", Synonyms = ["fast"], Default = false, PType = ParamType.Bool },
        new ConfigParam { Name = "junk", Default = null, PType = ParamType.String }
    };

    private static void DummyOutput(string output, MessageCode messageCode = MessageCode.Success)
    {
    }

    private static void DummyStatus(StatusCode status, int value, string progressType)
    {
    }

    private PhotoCopier() { }

    public PhotoCopier(HandleOutput output, StatusUpdate status)
    {
        _isAdmin = IsRunningAsAdministrator();
        _outputHandler = output;
        _statusUpdate = status;
    }

    public ReturnCode Initialize(
        string appName,
        bool help,
        string sourceDir,
        string destDir,
        string backup,
        PhotoCopierActions behavior,
        string pattern,
        string fileFilter,
        LoggingVerbosity logging,
        bool listOnly,
        bool parallel,
        string junk)
    {
        try
        {
            string[] allActions = Enum.GetNames<PhotoCopierActions>();

            List<string> reasons = new List<string>();

            // trim trailing backslashes
            if ((sourceDir?.EndsWith('\\')).GetValueOrDefault()) sourceDir = sourceDir?.Substring(0, sourceDir.Length - 1);
            if ((destDir?.EndsWith('\\')).GetValueOrDefault()) destDir = destDir?.Substring(0, destDir.Length - 1);

            // validate paths
            help |= !Configs.ValidatePath(sourceDir, "Source", reasons);
            help |= !Configs.ValidatePath(destDir, "Destination", reasons);

            if (help)
            {
                _outputHandler($"usage: {appName} -source=\"path\" -destination=\"path\" -action={string.Join('|', allActions)} -pattern=$y_$m -filter=takeout-*.zip");
                _outputHandler();
                _outputHandler($"  Copy all photos, movies, etc. from a set of archive zip files (Google takeout) from the source folder");
                _outputHandler($"  to destination folder. Subfolders will be created to hold photos with pattern naming. Should run in admin mode");
                _outputHandler($"  for file and folder access.");
                _outputHandler();
                _outputHandler($"where:");
                _outputHandler();
                _outputHandler($" source=path        folder that contains the takeout zip file(s).");
                _outputHandler($"                    (default=null");
                _outputHandler();
                _outputHandler($" destination=path   folder where to put photo contents (files and subfolders)");
                _outputHandler($"                    (default=null");
                _outputHandler();
                _outputHandler($" backup=path        folder where to put backup of original contents before reorder action (files and subfolders)");
                _outputHandler($"                    (default=empty/null - no backup before reorder");
                _outputHandler();
                _outputHandler($" action=option      option is one of {PhotoCopierActions.Copy} or {PhotoCopierActions.Reorder}.");
                _outputHandler($"                    (default={PhotoCopierActions.Copy}");
                _outputHandler();
                _outputHandler($"                    {PhotoCopierActions.Copy} media files to destination.");
                _outputHandler($"                    {PhotoCopierActions.Reorder} media files only if pattern is different (create backup zip files).");
                _outputHandler();
                _outputHandler($" list=value         value is true to list only, false to perform actions.");
                _outputHandler($"                    example: value=true would only show potential errors or actions without changing destination.");
                _outputHandler();
                _outputHandler($" parallel=value     value is true to run in parallel for faster performance, false to run sequentially. Parallem mode uses more memory.");
                _outputHandler($"                    example: value=true Uses more memory and runs faster. (default=true)");
                _outputHandler();
                _outputHandler($" junk=text          Junk files with these comma separated extensions to remove. (default=empty)");
                _outputHandler();
                _outputHandler($" loggin=verbosity   {LoggingVerbosity.Quiet}, {LoggingVerbosity.Change}, or {LoggingVerbosity.Verbose}. {LoggingVerbosity.Quiet}=minimal output, {LoggingVerbosity.Change}=minimal+changed content, {LoggingVerbosity.Verbose}=all");
                _outputHandler($"                    (default={LoggingVerbosity.Change})");
                _outputHandler();
                _outputHandler($" pattern=text       Target folder name pattern. (default=$y_$m");
                _outputHandler($"                       example:");
                _outputHandler($"                         if date-time, is March 27, 2024 at 3:33PM");
                _outputHandler($"                         $y = 4 digit year.  ex: 2024");
                _outputHandler($"                         $m = 2 digit month. ex: 03");
                _outputHandler($"                         $M = month name.    ex: March");
                _outputHandler($"                         $d = 2 digit day.   ex: 27");
                _outputHandler($"                         $h = 2 digit hour.  ex: 15 (24 hours per day)");
                _outputHandler($"                         '$y-$M($d)' \"root\\2024-March(3)\"");
                _outputHandler();
                _outputHandler($" filter=text        photo archive file filter. (default='takeout-*.zip')");
                _outputHandler();

                if (reasons.Count > 0)
                {
                    _outputHandler("Issues: ", MessageCode.Warning);

                    int index = 1;
                    foreach (string reason in reasons)
                    {
                        _outputHandler($"  {index++}. {reason}", MessageCode.Warning);
                    }

                    _outputHandler();
                }

                return ReturnCode.HadIssues;
            }

            if (behavior == PhotoCopierActions.Copy && !System.IO.Directory.GetFiles(sourceDir ?? ".\\", fileFilter).Any())
            {
                _outputHandler($"No source files found. sourceDir:\"{sourceDir}\", file filter:\"{fileFilter}\"");
                return ReturnCode.DirectoryError;
            }

            // keep the parameters

            _outputHandler("Current configuration:");

            _outputHandler($" Running as Admin: {_isAdmin}", _isAdmin ? MessageCode.Success : MessageCode.Warning);

            _behavior = behavior;
            _outputHandler($" Action: {behavior}");

            _source = sourceDir;
            _outputHandler($" Source: {sourceDir}");

            _destination = destDir;
            _outputHandler($" Destination: {destDir}");

            _fileFilter = fileFilter;
            _outputHandler($" Filter: {fileFilter}");

            _pattern = pattern;
            _outputHandler($" Pattern: {pattern}");

            _logging = logging;
            _outputHandler($" Logging: {logging}");

            _listOnly = listOnly;
            _outputHandler($" ListOnly: {listOnly}");

            _parallel = parallel;
            _outputHandler($" Parallel: {parallel}");

            if (behavior == PhotoCopierActions.Reorder)
            {
                bool inPlace = _source != null && _source.Equals(_destination, StringComparison.OrdinalIgnoreCase);
                _backup = string.IsNullOrEmpty(backup) ? "null" : backup;
                _outputHandler($" Backup: {backup ?? "(not set)"}", behavior == PhotoCopierActions.Reorder && inPlace && string.IsNullOrEmpty(backup) ? MessageCode.Warning : MessageCode.Success);
            }

            _junk = junk;
            _outputHandler($" Junk extensions: {junk ?? string.Empty}");

            _outputHandler();

            return (int)ReturnCode.Success;
        }
        catch (Exception ex)
        {
            HandleExceptions(ex, null, "Unhandled Reorder exception");
            return ReturnCode.Error;
        }
    }

    public bool IsRunning { get; private set; }

    public void Stop()
    {
        if (!IsCanceled)
        {
            _outputHandler();
            _outputHandler("Operation canceled", MessageCode.Warning);
            _outputHandler();

            Interlocked.Increment(ref _canceled);
            _cancel?.Cancel();
        }
    }

    public ReturnCode Run()
    {
        ResultCounts results = new ResultCounts(null, "TakeoutWrangler");

        ReturnCode returnCode = ReturnCode.HadIssues;
        Stopwatch stopwatch = new Stopwatch();

        try
        {
            stopwatch.Start();

            junkExtensions = GetJunkExtensions(_junk).ToHashSet(StringComparer.OrdinalIgnoreCase);
            _canceled = 0;

            IsRunning = true;
            returnCode = ReturnCode.Success;

            if (_behavior == PhotoCopierActions.Copy)
            {
                returnCode = ProcessTakeoutZips(results);
            }
            else if (_behavior == PhotoCopierActions.Reorder)
            {
                returnCode = ProcessReorder(results);
            }
            else
            {
                results.Increment(ResultCounts.CountKeys.Error);
                _outputHandler("Invalid behavior", MessageCode.Error);
                returnCode = ReturnCode.Error;
            }
        }
        catch (Exception ex)
        {
            results.Increment(ResultCounts.CountKeys.Error);
            HandleExceptions(ex, results, "Unhandled Reorder exception");
            returnCode = ReturnCode.Error;
        }
        finally
        {
            foreach (ZipArchive archive in _archives)
            {
                archive.Dispose();
            }

            _archives.Clear();
            IsRunning = false;

            stopwatch.Stop();

            _outputHandler();
            _outputHandler($"Elapsed={stopwatch.Elapsed}, Filter=\"{_fileFilter}\"");

            _outputHandler();
            EmitResults(results);
            _outputHandler();
        }

        return IsCanceled ? ReturnCode.Canceled : returnCode;
    }

    private void EmitResults(ResultCounts results, int pad = 0)
    {
        if (results == null) return;
        int? errors = results.Get(ResultCounts.CountKeys.Error);
        int? total = results.Get(ResultCounts.CountKeys.Total);
        int? skips = results.Get(ResultCounts.CountKeys.Skip);
        int? duplicates = results.Get(ResultCounts.CountKeys.Duplicate);
        int? changes = results.Get(ResultCounts.CountKeys.Change);
        int? progress = results.Get(ResultCounts.CountKeys.Progress);

        List<string> parts = new List<string>();
        if (errors.HasValue) parts.Add($"Errors={errors.Value}");
        if (progress.HasValue) parts.Add($"Progress={progress.Value}");
        if (total.HasValue) parts.Add($"Items={total.Value}");
        if (skips.HasValue) parts.Add($"Skips={skips.Value}");
        if (duplicates.HasValue) parts.Add($"Duplicates={duplicates.Value}");
        if (changes.HasValue) parts.Add($"Changes={changes.Value}");

        string text = $"{"".PadLeft(pad)}{results.Text ?? "unknown"}: {string.Join(", ", parts)}";
        _outputHandler(text);

        foreach (ResultCounts child in results.Children)
        {
            EmitResults(child, pad + 2);
        }
    }

    private ReturnCode ProcessReorder(ResultCounts parent)
    {
        ResultCounts results = new ResultCounts(parent, "Process Reorder");

        try
        {
            ConcurrentBag<string> filesToDelete = null;
            bool inPlace = _source.Equals(_destination, StringComparison.OrdinalIgnoreCase);

            if (inPlace)
            {
                filesToDelete = new ConcurrentBag<string>();
            }

            HashSet<string> allPaths = System.IO.Directory.EnumerateFiles(_source, "*.*", SearchOption.AllDirectories).ToHashSet(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrEmpty(_backup) && inPlace)
            {
                if (!System.IO.Directory.Exists(_backup))
                {
                    results.Increment(ResultCounts.CountKeys.Error);
                    _outputHandler($"Backup root directory does not exist: {_backup}", MessageCode.Error);
                    return ReturnCode.Error;
                }

                string verb = _listOnly ? "Would backup" : "Backing up";
                if (_logging != LoggingVerbosity.Quiet)
                {
                    _outputHandler($"{verb} source directory:\"{_source}\" to \"{_backup}\"");
                }

                if (!_listOnly)
                {
                    // 2GB file
                    GenerateBackup(allPaths, results);
                }
            }

            List<string> toRemoveFromAll = new List<string>();
            foreach (string path in allPaths)
            {
                string ext = Path.GetExtension(path);
                if (junkExtensions.Contains(ext))
                {
                    filesToDelete?.Add(path.ToLower());
                    toRemoveFromAll.Add(path);
                }
            }

            foreach (string removeName in toRemoveFromAll)
            {
                allPaths.Remove(removeName);
            }

            ConcurrentBag<string> retryMediaPaths = new ConcurrentBag<string>();
            ReorderDir(allPaths, inPlace, false, filesToDelete, results, retryMediaPaths, "Reorder");

            if (_logging != LoggingVerbosity.Quiet)
            {
                _outputHandler();
            }

            int retryCount = 0;
            while (!IsCanceled)
            {
                retryCount++;

                allPaths.Clear();

                if (retryMediaPaths.IsEmpty) break;

                while (retryMediaPaths.TryTake(out string result))
                {
                    allPaths.Add(result);
                }

                retryMediaPaths.Clear();

                if (_logging != LoggingVerbosity.Quiet)
                {
                    _outputHandler($"Retry({retryCount}): {allPaths.Count} entries.");
                }

                ReorderDir(allPaths, inPlace, true, filesToDelete, results, retryMediaPaths, $"Reorder retry({retryCount})");

                if (!IsCanceled && !retryMediaPaths.IsEmpty)
                {
                    // we did something so wait a bit and try to see if there are more retries
                    Stopwatch timer = Stopwatch.StartNew();
                    while (!IsCanceled && timer.Elapsed < TimeSpan.FromSeconds(1))
                    {
                        Task.Delay(50).Wait();
                    }
                }
            }

            if (inPlace)
            {
                if (!IsCanceled)
                {
                    RemoveDuplicateAndJunkFiles(filesToDelete, results);
                }

                if (!IsCanceled)
                {
                    RemoveEmptyDirs(results);
                }
            }

            return results.Get(ResultCounts.CountKeys.Error) > 0 ? ReturnCode.Error : ReturnCode.Success;
        }
        catch (Exception ex)
        {
            HandleExceptions(ex, results, "Unhandled Reorder exception");
        }

        return ReturnCode.Error;
    }

    private void RemoveEmptyDirs(ResultCounts parent)
    {
        ResultCounts results = new ResultCounts(parent, "Directory cleanup");
        FileSystemAccessRule accessRule = null;

        if (_isAdmin)
        {
            // Create a new rule that grants full control to the "Everyone" group
            accessRule = new FileSystemAccessRule(
                new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                FileSystemRights.FullControl,
                InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                PropagationFlags.None,
                AccessControlType.Allow);
        }

        List<string> emptyDirs = new List<string>();
        int total = 0;
        StatusCode statusCode = StatusCode.Total;

        if (_logging != LoggingVerbosity.Quiet)
        {
            _outputHandler();
        }

        do
        {
            emptyDirs.Clear();
            FindEmptyDirs(_destination, emptyDirs);
            if (emptyDirs.Count == 0 || IsCanceled)
            {
                break;
            }

            total += emptyDirs.Count;
            results.Set(ResultCounts.CountKeys.Total, total);
            _statusUpdate(statusCode, total, "(directory cleanup)");
            statusCode = StatusCode.More;
            string dverb = _listOnly ? "Would remove" : "Removed";

            foreach (string dir in emptyDirs)
            {
                if (IsCanceled) break;

                try
                {
                    DirectoryInfo diOld = new DirectoryInfo(dir);
                    DirectorySecurity directorySecurity = diOld.GetAccessControl();

                    if (accessRule != null)
                    {
                        // Add the new rule to the directory's security settings
                        directorySecurity.AddAccessRule(accessRule);

                        // Apply the updated access control settings to the directory
                        diOld.SetAccessControl(directorySecurity);
                        diOld.SetAccessControl(directorySecurity);
                    }

                    diOld.Attributes = FileAttributes.Normal;

                    if (!_listOnly)
                    {
                        diOld.Delete();
                        results.Increment(ResultCounts.CountKeys.Change);
                    }

                    if (_logging != LoggingVerbosity.Quiet)
                    {
                        _outputHandler($"{dverb} empty directory:\"{dir}\"");
                    }
                }
                catch (Exception ex)
                {
                    HandleExceptions(ex, results, $"Could not delete empty directory:\"{dir}\"");
                }
                finally
                {
                    results.Increment(ResultCounts.CountKeys.Progress);
                    _statusUpdate();
                }
            }
        }
        while (!IsCanceled);
    }

    private void RemoveDuplicateAndJunkFiles(ConcurrentBag<string> filesToDelete, ResultCounts parent)
    {
        if (filesToDelete == null) return;
        ResultCounts results = new ResultCounts(parent, "Duplicate/Junk file cleanup");
        int total = filesToDelete.Count;
        results.Set(ResultCounts.CountKeys.Total, total);
        _statusUpdate(StatusCode.Total, total, "(duplicate/junk file cleanup)");

        foreach (string path in filesToDelete)
        {
            if (IsCanceled) break;

            try
            {
                string verb = _listOnly ? "Would delete" : "Deleting";
                _outputHandler($"{verb} duplicate or junk file:\"{path}\"");

                if (!_listOnly)
                {
                    File.Delete(path);
                    results.Increment(ResultCounts.CountKeys.Change);
                }
            }
            catch (Exception ex)
            {
                HandleExceptions(ex, results, $"Could not remove duplicate or junk file:\"{path}\"");
            }
            finally
            {
                results.Increment(ResultCounts.CountKeys.Progress);
                _statusUpdate();
            }
        }
    }

    private void ReorderDir(
        ICollection<string> sourcePaths,
        bool inRetry,
        bool inPlace,
        ConcurrentBag<string> filesToRemove,
        ResultCounts parent,
        ConcurrentBag<string> retryMediaPaths,
        string context)
    {
        ResultCounts results = new ResultCounts(parent, context);
        int total = sourcePaths.Count;
        results.Set(ResultCounts.CountKeys.Total, total);
        _statusUpdate(StatusCode.Total, total, $"({context.ToLower()})");

        try
        {
            if (_parallel)
            {
                _cancel = new CancellationTokenSource();
                ParallelOptions parallelOptions = new ParallelOptions();
                parallelOptions.CancellationToken = _cancel.Token;

                Parallel.ForEach(sourcePaths, parallelOptions, sourceMediaPath => ReorderWork(inRetry, inPlace, filesToRemove, sourceMediaPath, results, retryMediaPaths));
            }
            else
            {
                foreach (string sourceMediaPath in sourcePaths)
                {
                    ReorderWork(inRetry, inPlace, filesToRemove, sourceMediaPath, results, retryMediaPaths);
                }
            }
        }
        catch (Exception ex)
        {
            HandleExceptions(ex, results, "Unhandled Reorder exception");
        }
    }

    private void HandleExceptions(Exception ex, ResultCounts parent, string reason)
    {
        if (ex is OperationCanceledException)
        {
            return;
        }

        if (!IsCanceled)
        {
            if (Debugger.IsAttached)
            {
                Debug.WriteLine(ex.Message);
                Debugger.Break();
            }
            parent?.Increment(ResultCounts.CountKeys.Error);
            _outputHandler($"Error: {reason}. Reason={ex.Message}", MessageCode.Error);
        }
    }

    private void ReorderWork(bool isRetry, bool inPlace, ConcurrentBag<string> filesToRemove, string sourceMediaPath, ResultCounts parent, ConcurrentBag<string> retryMediaFiles)
    {
        try
        {
            if (IsCanceled) return;

            string sourceMediaFileName = Path.GetFileName(sourceMediaPath);

            Dictionary<string, DateTime> dates = new Dictionary<string, DateTime>();

            TryGetDateFromMediaFileName(sourceMediaFileName, dates);
            string fileType = GetDateFromMediaFileContent(sourceMediaPath, dates, parent);

            // 1. Get date from media content
            if (fileType == null)
            {
                parent.Increment(ResultCounts.CountKeys.Skip);
                if (_logging == LoggingVerbosity.Verbose)
                {
                    _outputHandler($"{(isRetry ? _retryText : "")}Skipped - not a media file:\"{sourceMediaPath}\".", MessageCode.Warning);
                }
                return;
            }

            DateTime mediaDate = GetBestDate(dates, out bool isBest);
            _originalFileNameToDate[sourceMediaFileName] = mediaDate;

            string targetMediaDir = DirectoryNameFromDate(mediaDate, false, parent, out string targetMediaDirName);
            if (targetMediaDir == null)
            {
                throw new NullReferenceException($"New media file directory cannot not be null");
            }

            string targetMediaPath = Path.Combine(targetMediaDir, sourceMediaFileName);

            string prefix = _listOnly ? "Would " : string.Empty;
            string verb = inPlace ? $"{prefix}Move" : $"{prefix}Copy";

            if (_listOnly)
            {
                if (_logging != LoggingVerbosity.Quiet)
                {
                    _outputHandler($"{(isRetry ? _retryText : "")}{verb} file:\"{sourceMediaFileName}\" to \"{Path.Combine(".", targetMediaDirName)}\"");
                }

                return;
            }

            System.IO.Directory.CreateDirectory(targetMediaDir);
            bool alreadyExits = ResolveFileConflict(isRetry, parent, sourceMediaPath, ref targetMediaPath);

            try
            {
                if (alreadyExits)
                {
                    // already exists
                    // the files are the same - the file is a duplicate
                    string text = _listOnly ? "List(duplicate)" : "Duplicate";
                    if (_logging != LoggingVerbosity.Quiet) _outputHandler($"{(isRetry ? _retryText : "")}{text}:\"{sourceMediaFileName}\" in \"{targetMediaDirName}\"");
                    parent.Increment(ResultCounts.CountKeys.Duplicate);

                    filesToRemove?.Add(sourceMediaPath.ToLower());
                }
                else
                {
                    if (inPlace)
                    {
                        File.Move(sourceMediaPath, targetMediaPath);
                    }
                    else
                    {
                        File.Copy(sourceMediaPath, targetMediaPath, false);
                    }

                    if (mediaDate > _notBeforeDate) File.SetCreationTime(targetMediaPath, mediaDate);
                    parent.Increment(ResultCounts.CountKeys.Change);

                    if (_logging != LoggingVerbosity.Quiet)
                    {
                        _outputHandler($"{(isRetry ? _retryText : "")}{verb} file:\"{sourceMediaFileName}\" to \"{Path.Combine(".", targetMediaDirName)}\"");
                    }
                }
            }
            catch (IOException ioex)
            {
                // the file might exist now - will redo the conflict resolution
                if (ioex.HResult == -2147024713)
                {
                    // this one exists
                    alreadyExits = true;
                }
                else
                {
                    HandleExceptions(ioex, parent, $"Error with existing file: {sourceMediaPath} and {targetMediaPath}");
                }
            }
            catch (Exception ex)
            {
                HandleExceptions(ex, parent, $"Error with existing file: {sourceMediaPath} and {targetMediaPath}");
            }
        }
        catch (IOException ioex)
        {
            if (ioex.HResult == -2147024864)
            {
                retryMediaFiles.Add(sourceMediaPath);
                return;
            }

            HandleExceptions(ioex, parent, $"Could not reorder {sourceMediaPath}");
        }
        catch (Exception ex)
        {
            HandleExceptions(ex, parent, $"Could not reorder {sourceMediaPath}");
        }
        finally
        {
            parent.Increment(ResultCounts.CountKeys.Progress);
            _statusUpdate();
        }
    }

    private DateTime GetBestDate(Dictionary<string, DateTime> dates, out bool isBest)
    {
        DateTime mediaDate = DateTime.MinValue;
        if (dates.TryGetValue("best", out mediaDate) && mediaDate > _notBeforeDate)
        {
            isBest = true;
            return mediaDate;
        }

        isBest = false;

        if (dates.TryGetValue("Date/Time Original", out mediaDate) && mediaDate > _notBeforeDate) return mediaDate;
        if (dates.TryGetValue("8:6", out mediaDate) && mediaDate > _notBeforeDate) return mediaDate;
        if (dates.TryGetValue("L8", out mediaDate) && mediaDate > _notBeforeDate) return mediaDate;
        if (dates.TryGetValue("Date/Time", out mediaDate) && mediaDate > _notBeforeDate) return mediaDate;
        if (dates.TryGetValue("byString", out mediaDate) && mediaDate > _notBeforeDate) return mediaDate;
        if (dates.TryGetValue("File Modified Date", out mediaDate) && mediaDate > _notBeforeDate) return mediaDate;
        if (dates.TryGetValue("unix", out mediaDate) && mediaDate > _notBeforeDate) return mediaDate;

        return DateTime.MinValue;
    }

    private void FindEmptyDirs(string path, List<string> emptyDirs)
    {
        foreach (string subDir in System.IO.Directory.EnumerateDirectories(path, "*.*", SearchOption.TopDirectoryOnly))
        {
            if (IsCanceled) return;

            FindEmptyDirs(subDir, emptyDirs);

            if (!System.IO.Directory.EnumerateFileSystemEntries(subDir).Any())
            {
                emptyDirs.Add(subDir);
            }
        }
    }

    private string GetDateFromMediaStream(Stream mediaStream, IDictionary<string, DateTime> dates, ResultCounts parent)
    {
        ArgumentNullException.ThrowIfNull(mediaStream);

        mediaStream.Seek(0, SeekOrigin.Begin);
        IReadOnlyList<MetadataExtractor.Directory> metaDir = ImageMetadataReader.ReadMetadata(mediaStream);
        return GetDateFromMetaDir(metaDir, dates, parent);
    }

    private string GetDateFromMediaFileContent(string mediaFile, IDictionary<string, DateTime> dates, ResultCounts parent)
    {
        if (!File.Exists(mediaFile)) return null;

        try
        {
            IReadOnlyList<MetadataExtractor.Directory> metaDir = ImageMetadataReader.ReadMetadata(mediaFile);
            return GetDateFromMetaDir(metaDir, dates, parent);
        }
        catch (Exception ex)
        {
            // ignore this exception
            _ = ex;
        }

        // is it a pdf file?
        string ext = (Path.GetExtension(mediaFile) ?? string.Empty).ToLower();
        switch (ext)
        {
            case ".pdf":
            {
                PdfSharp.Pdf.PdfDocument pdfDocument = PdfSharp.Pdf.IO.PdfReader.Open(mediaFile);
                DateTime date = pdfDocument.Info.CreationDate;
                if (date > _notBeforeDate)
                {
                    dates.Add("best", date);
                    return "pdf";
                }
                break;
            }

            case ".mts":
            {
                FileInfo fi = new FileInfo(mediaFile);
                DateTime date = fi.LastWriteTime;
                if (date > _notBeforeDate)
                {
                    dates.Add("best", date);
                    return "mts";
                }
                break;
            }
        }

        return null;
    }

    private void TryGetDateFromMediaFileName(string mediaFile, IDictionary<string, DateTime> dates)
    {
        string filename = Path.GetFileNameWithoutExtension(mediaFile);

        string[] parts = null;
        if (filename.Contains('-'))
        {
            parts = filename.Split('-');
        }
        else if (filename.Contains("_"))
        {
            parts = filename.Split("_");
        }
        else
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in filename)
            {
                if (char.IsDigit(c))
                {
                    sb.Append(c);
                }
            }

            parts = [sb.ToString()];
        }

        List<int> index8 = new List<int>();
        List<int> index10 = new List<int>();
        List<int> index6 = new List<int>();

        for (int ii = 0; ii < parts.Length; ii++)
        {
            string part = parts[ii];
            
            if (part.Length == 8)
            {
                index8.Add(ii);
                continue;
            }

            if (part.Length == 10)
            {
                index10.Add(ii);
                continue;
            }

            if (part.Length == 6)
            {
                index6.Add(ii);
                continue;
            }
        }

        if (index6.Count > 0 && index8.Count > 0)
        {
            bool done = false;
            foreach (int i6 in index6)
            {
                if (done) break;

                foreach (int i8 in index8)
                {
                    if (done) break;

                    // "yyyyMMdd" + "hhmmss"
                    string t = $"{parts[i8]}{parts[i6]}";
                    string[] pieces = SplitByLength(t, 4, 2, 2, 2, 2, 2);
                    if (pieces.Length > 5)
                    {
                        if (DateTime.TryParse($"{pieces[0]}-{pieces[1]}-{pieces[2]} {pieces[3]}:{pieces[4]}:{pieces[5]}", out DateTime date))
                        {
                            dates["8:6"] = date;
                        }
                    }
                }
            }
        }

        for (int ii = 0; ii < parts.Length; ii++)
        {
            string part = parts[ii];
            if (part.Length == 10)
            {
                // might be MMddyyhhmm
                string[] pieces = SplitByLength(part, 2, 2, 2, 2, 2);
                if (pieces.Length > 4)
                {
                    if (DateTime.TryParse($"{pieces[0]}-{pieces[1]}-{pieces[2]} {pieces[3]}:{pieces[4]}", out DateTime date))
                    {
                        dates[$"L10/4"] = date;
                    }
                }
                else if (pieces.Length > 2)
                {
                    if (DateTime.TryParse($"{pieces[0]}-{pieces[1]}-{pieces[2]}", out DateTime date))
                    {
                        dates[$"L10/2"] = date;
                    }
                }
            }
            else if (part.Length == 8)
            {
                // might be yyyyMMdd
                string[] pieces = SplitByLength(filename, 4, 2, 2);
                if (DateTime.TryParse($"{pieces[0]}-{pieces[1]}-{pieces[2]}", out DateTime date))
                {
                    dates[$"L8"] = date;
                }
            }
        }
    }

    private string GetDateFromMetaDir(IReadOnlyList<MetadataExtractor.Directory> metaDir, IDictionary<string, DateTime> dates, ResultCounts parent)
    {
        FileTypeDirectory fileType = metaDir.OfType<FileTypeDirectory>().FirstOrDefault();
        string fileTypeName = fileType?.GetDescription(FileTypeDirectory.TagDetectedFileTypeName)?.ToLower();
        if (fileTypeName == null) return null;

        DatesFromMetaDir(metaDir, dates);

        switch (fileTypeName)
        {
            case "heic":
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
                        dates.Add("best", date);
                    }
                    else
                    {
                        DateFromStringTryParse(originalDate, dates, parent);
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
                    DateTime date = ParseDate(creationDate);
                    if (date != DateTime.MinValue)
                    {
                        dates.Add("best", date);
                    }
                }

                return fileTypeName;
            }

            case "gif":
            case "riff":
            {
                return fileTypeName;
            }

            case "avi":
            {
                AviDirectory aviDir = metaDir.OfType<AviDirectory>().FirstOrDefault();
                string creationDate = aviDir?.GetDescription(AviDirectory.TagDateTimeOriginal);

                if (creationDate != null)
                {
                    DateTime date = ParseDate(creationDate);
                    if (date != DateTime.MinValue)
                    {
                        dates.Add("best", date);
                    }
                }

                return fileTypeName;
            }

            case "bmp":
            {
                return fileTypeName;
            }

            default:
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine($"Invalid fileTypeName: {fileTypeName}");
                    Debugger.Break();
                }
                break;
            }
        }

        return fileTypeName;
    }

    private void DatesFromMetaDir(IReadOnlyList<MetadataExtractor.Directory> metaDir, IDictionary<string, DateTime> dates)
    {
        DateTime oldest = DateTime.MaxValue;
        foreach (MetadataExtractor.Directory md in metaDir)
        {
            foreach (Tag tag in md.Tags)
            {
                if (tag.Name.Contains("date", StringComparison.OrdinalIgnoreCase))
                {
                    DateTime date = ParseDate(tag.Description);
                    if (date > _notBeforeDate)
                    {
                        dates[tag.Name] = date;
                    }
                }
            }
        }
    }

    private  DateTime ParseDate(string creationDate)
    {
        DateTime date = DateTime.MinValue;

        string last = creationDate;
        do
        {
            last = creationDate;
            creationDate = creationDate.Replace("  ", " ");
        }
        while (last != creationDate);

        if (DateTime.TryParse(creationDate, out date))
        {
            return date;
        }

        //"2016:03:12 18:51:45"
        CultureInfo enUS = new CultureInfo("en-US");
        if (DateTime.TryParseExact(creationDate, "yyyy:MM:dd HH:mm:ss", enUS, DateTimeStyles.AssumeLocal, out date))
        {
            if (date != DateTime.MinValue)
            {
                return date;
            }
        }

        //"2017:12:14"
        if (DateTime.TryParseExact(creationDate, "yyyy:MM:dd", enUS, DateTimeStyles.AssumeLocal, out date))
        {
            if (date != DateTime.MinValue)
            {
                return date;
            }
        }

        if (DateTime.TryParseExact(creationDate, "ddd MMM dd HH:mm:ss yyyy", enUS, DateTimeStyles.AssumeLocal, out date))
        {
            if (date != DateTime.MinValue)
            {
                return date;
            }
        }

        // creationDate = "Fri Mar 6 13:33:45 2009"
        string[] parts = creationDate.Split(' ');

        if (parts.Length >= 5)
        {
            if (parts[2].Length == 1)
            {
                parts[2] = '0' + parts[2];
            }
        }

        string testDate = string.Join(' ', parts);
        if (DateTime.TryParseExact(testDate, "ddd MMM dd HH:mm:ss yyyy", enUS, DateTimeStyles.AssumeLocal, out date))
        {
            return date;
        }

        if (parts.Length == 6)
        {
            List<string> chunks = new List<string>(parts);
            chunks.RemoveAt(4);
            testDate = string.Join(' ', chunks);
            if (DateTime.TryParseExact(testDate, "ddd MMM dd HH:mm:ss yyyy", enUS, DateTimeStyles.AssumeLocal, out date))
            {
                return date;
            }
        }

        return DateTime.MinValue;
    }

    private void DateFromStringTryParse(string dateString, IDictionary<string, DateTime> dates, ResultCounts parent)
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
            dates["byString"] = new DateTime(year, month, day, hour, minute, second);
        }
        catch (Exception ex)
        {
            HandleExceptions(ex, parent, $"Error parsing date: {dateString}");
        }
    }

    private ReturnCode ProcessTakeoutZips(ResultCounts parent)
    {
        ResultCounts results = new ResultCounts(parent, "Process zip files");

        try
        {
            ConcurrentDictionary<string, MediaInfo> mediaKeys = new ConcurrentDictionary<string, MediaInfo>(StringComparer.OrdinalIgnoreCase);
            if (_logging != LoggingVerbosity.Quiet) _outputHandler($"Processing dir:{_source}");

            foreach (string entity in System.IO.Directory.EnumerateFiles(_source, _fileFilter, SearchOption.AllDirectories))
            {
                if (IsCanceled) break;

                FileInfo fileInfo = new FileInfo(entity);
                if (!fileInfo.Attributes.HasFlag(FileAttributes.Directory))
                {
                    ProcessZip(entity, results, mediaKeys);
                }
            }

            string[] keysToRemove = mediaKeys.Where(x => x.Value.EntryName == null).Select(x => x.Key).ToArray();
            foreach (string key in keysToRemove)
            {
                if (IsCanceled) break;

                if (!mediaKeys.TryRemove(key, out _))
                {
                    if (Debugger.IsAttached)
                    {
                        Debug.WriteLine($"Could not remove mediaKey: {key}");
                        Debugger.Break();
                    }
                }
            }

            if (mediaKeys.IsEmpty) return ReturnCode.HadIssues;

            ConcurrentQueue<MediaInfo> retryMediaKeys = new ConcurrentQueue<MediaInfo>();
            DoMedia(false, results, mediaKeys, retryMediaKeys, "Process media");

            int retryCount = 0;
            while (!IsCanceled)
            {
                retryCount++;
                mediaKeys.Clear();
                if (retryMediaKeys.IsEmpty) break;

                while (retryMediaKeys.TryDequeue(out MediaInfo result))
                {
                    mediaKeys[result.EntryKey] = result;
                }

                _outputHandler($"Retry({retryCount}): {mediaKeys.Count} entries.");

                DoMedia(true, results, mediaKeys, retryMediaKeys, $"Process media retry({retryCount})");

                if (!IsCanceled && !retryMediaKeys.IsEmpty)
                {
                    // we did something so wait a bit and try to see if there are more retries
                    Stopwatch timer = Stopwatch.StartNew();
                    while (!IsCanceled && timer.Elapsed < TimeSpan.FromSeconds(1))
                    {
                        Task.Delay(50).Wait();
                    }
                }
            }

            return results.Get(ResultCounts.CountKeys.Error) > 0 ? ReturnCode.Error : ReturnCode.Success;
        }
        catch (Exception ex)
        {
            HandleExceptions(ex, results, $"Error expanding zip entries");
        }

        return ReturnCode.Error;
    }

    private void DoMedia(bool inRetry, ResultCounts parent, ConcurrentDictionary<string, MediaInfo> mediaKeys, ConcurrentQueue<MediaInfo> retryMediaKeys, string context)
    {
        ResultCounts results = new ResultCounts(parent, context);

        try
        {
            int total = mediaKeys.Count;
            results.Set(ResultCounts.CountKeys.Total, total);
            _statusUpdate(StatusCode.Total, total, $"({context.ToLower()})");

            if (_parallel && !inRetry)
            {
                _cancel = new CancellationTokenSource();
                ParallelOptions parallelOptions = new ParallelOptions
                {
                    CancellationToken = _cancel.Token
                };

                Parallel.ForEach(mediaKeys.Keys, parallelOptions, entryKey => ProcessEntry(entryKey, false, results, mediaKeys, retryMediaKeys));
            }
            else
            {
                foreach (string entryKey in mediaKeys.Keys)
                {
                    if (IsCanceled) break;
                    ProcessEntry(entryKey, false, results, mediaKeys, retryMediaKeys);
                }
            }
        }
        catch (Exception ex)
        {
            HandleExceptions(ex, results, $"Unhandled media exception");
        }
    }

    private void ProcessZip(string zip, ResultCounts parent, ConcurrentDictionary<string, MediaInfo> mediaKeys)
    {
        try
        {
            ZipArchive archive = ZipFile.OpenRead(zip);
            _archives.Add(archive);

            parent.Increment(ResultCounts.CountKeys.Total);

            ZipArchiveEntry[] allEntries = archive.Entries.ToArray();

            HashSet<string> extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (ZipArchiveEntry entry in allEntries)
            {
                if (IsCanceled) break;
                extensions.Add(Path.GetExtension(entry.Name));
            }

            Dictionary<string, Dictionary<string, ZipArchiveEntry>> entries = new Dictionary<string, Dictionary<string, ZipArchiveEntry>>(StringComparer.OrdinalIgnoreCase);

            foreach (ZipArchiveEntry entry in allEntries)
            {
                if (IsCanceled) break;
                string root = entry.FullName;

                string file = Path.GetFileName(root);
                string dir = Path.GetDirectoryName(root);
                if (dir == null)
                {
                    parent.Increment(ResultCounts.CountKeys.Error);
                    _outputHandler($"Error: root, {root} is not a directory. Reason=InvalidPath", MessageCode.Error);
                    continue;
                }

                string[] parts = file.Split(_separator, 2);

                string ext = null;
                if (parts.Length > 1) ext = parts[1];
                string key = Path.Combine(dir, parts[0]);

                if (!entries.TryGetValue(key, out Dictionary<string, ZipArchiveEntry> value))
                {
                    value = new Dictionary<string, ZipArchiveEntry>(StringComparer.OrdinalIgnoreCase);
                    entries[key] = value;
                }

                Dictionary<string, ZipArchiveEntry> values = value;
                if (string.IsNullOrEmpty(ext) || (values.ContainsKey(ext)))
                {
                    if (Debugger.IsAttached)
                    {
                        Debug.WriteLine($"Extension is null or in the skip list: {ext}");
                        Debugger.Break();
                    }
                    continue;
                }

                value[ext] = entry;
            }

            if (_logging != LoggingVerbosity.Quiet) _outputHandler($"Processing zip:{zip}, entries count:{entries.Count}");

            // group the media and meta data with the entries by media name
            foreach (string key in entries.Keys)
            {
                if (IsCanceled) break;
                Dictionary<string, ZipArchiveEntry> values = entries[key];

                if (!mediaKeys.TryGetValue(key, out MediaInfo mediaInfo))
                {
                    mediaInfo = new MediaInfo { EntryKey = key };
                }

                foreach (string valueKey in values.Keys)
                {
                    if (IsCanceled) break;

                    ZipArchiveEntry archiveEntry = values[valueKey];

                    if (valueKey.EndsWith("json", StringComparison.OrdinalIgnoreCase))
                    {
                        using (Stream zipStream = archiveEntry.Open())
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
                    else// if (!valueKey.EndsWith("htm", StringComparison.OrdinalIgnoreCase) && !valueKey.EndsWith("html", StringComparison.OrdinalIgnoreCase))
                    {
                        string ext = $".{valueKey}";
                        if (!junkExtensions.Contains(ext))
                        {
                            mediaInfo.EntryName = archiveEntry.FullName;
                            mediaInfo.ArchivePath = zip;
                        }
                        else
                        {
                        }
                    }
                }

                mediaKeys[key] = mediaInfo;
                mediaInfo = null;
            }
        }
        catch (Exception ex)
        {
            HandleExceptions(ex, parent, $"Could not process zipfile {zip}");
        }
    }

    private string DirectoryNameFromDate(DateTime date, bool isRetry, ResultCounts parent, out string newTargetDirName)
    {
        newTargetDirName = "no-date";

        if (date > _notBeforeDate)
        {
            string year = $"{date:yyyy}";
            string month = $"{date:MM}";
            string day = $"{date:dd}";
            string hour = $"{date:HH}";

            newTargetDirName = _pattern.Replace("$y", year, StringComparison.OrdinalIgnoreCase)
                .Replace("$m", month)
                .Replace("$d", day, StringComparison.OrdinalIgnoreCase)
                .Replace("$h", hour, StringComparison.OrdinalIgnoreCase)
                .Replace("$M", MonthName(date.Month));
        }

        string newFilePath = Path.GetFullPath(Path.Combine(_destination, newTargetDirName));

        if (newFilePath == null || !newFilePath.StartsWith(_destination, StringComparison.OrdinalIgnoreCase))
        {
            if (Debugger.IsAttached)
            {
                Debug.WriteLine($"newFilePath null or does not start with the destination.");
                Debugger.Break();
            }

            parent.Increment(ResultCounts.CountKeys.Error);
            _outputHandler($"{(isRetry ? _retryText : "")}Error: Invalid target file path:\"{newFilePath ?? "null"}\"", MessageCode.Error);
            return null;
        }

        return newFilePath;
    }

    private  string MonthName(int monthNumber)
    {
        CultureInfo culture = CultureInfo.CurrentCulture;
        DateTimeFormatInfo dtfi = culture.DateTimeFormat;
        return dtfi.GetMonthName(monthNumber);
    }

    private void RecurseDeserialize(Dictionary<string, object> values, ResultCounts parent)
    {
        foreach (KeyValuePair<string, object> keyValuePair in values.ToArray())
        {
            JArray jarray = keyValuePair.Value as JArray;

            if (jarray != null)
            {
                List<Dictionary<string, object>> dictionaries = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jarray.ToString());
                if (dictionaries == null)
                {
                    parent.Increment(ResultCounts.CountKeys.Error);
                    _outputHandler($"Error: Could not deserialize object {jarray}. Reason=InvalidObject", MessageCode.Error);
                    continue;
                }

                values[keyValuePair.Key] = dictionaries;

                foreach (Dictionary<string, object> dictionary in dictionaries)
                {
                    RecurseDeserialize(dictionary, parent);
                }
            }
            else
            {
                JObject jobject = keyValuePair.Value as JObject;
                if (jobject != null)
                {
                    Dictionary<string, object> dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(jobject.ToString());
                    values[keyValuePair.Key] = dictionary;
                    RecurseDeserialize(dictionary, parent);
                }
            }
        }
    }

    private  string[] SplitByLength(string chunk, params int[] lengths)
    {
        if (string.IsNullOrEmpty(chunk)) return null;
        if (lengths == null || lengths.Length == 0) return new[] { chunk };

        int totalCharacters = 0;
        foreach (int length in lengths)
        {
            totalCharacters += length;
        }

        if (totalCharacters > chunk.Length) return null;

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

    private  string[] FindNumberStrings(string fileName)
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

    private void GetDateFromTakeoutMetadata(string text, IDictionary<string, DateTime> dates, ResultCounts parent)
    {
        try
        {
            Dictionary<string, object> values = JsonConvert.DeserializeObject<Dictionary<string, object>>(text);
            if (values == null)
            {
                return;
            }

            RecurseDeserialize(values, parent);

            if (values.TryGetValue("photoTakenTime", out object photoTaken))
            {
                Dictionary<string, object> data = photoTaken as Dictionary<string, object>;
                if (data != null)
                {
                    if (data.TryGetValue("timestamp", out object value))
                    {
                        if (value is string v)
                        {
                            long l = long.Parse(v);
                            GetDateFromUnixTimestamp(l, dates, parent);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            HandleExceptions(ex, parent, $"Could not get date from media: {text}");
        }
    }

    private void GetDateFromUnixTimestamp(double timestamp, IDictionary<string, DateTime> dates, ResultCounts parent)
    {
        try
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            DateTime date = origin.AddSeconds(timestamp);
            if (date > _notBeforeDate)
            {
                dates["unix"] = date;
            }
        }
        catch (Exception ex)
        {
            HandleExceptions(ex, parent, $"Could not get Unix date");
        }
    }

    public Configs GetConfiguration()
    {
        return new Configs(_paramTypes, true);
    }

    private void ProcessEntry(string entryKey, bool isRetry, ResultCounts parent, ConcurrentDictionary<string, MediaInfo> mediaKeys, ConcurrentQueue<MediaInfo> retryMediaKeys)
    {
        if (IsCanceled) return;

        bool foundEntry = mediaKeys.TryGetValue(entryKey, out MediaInfo mediaInfo);

        ZipArchive archive = null;
        ZipArchiveEntry entry = null;
        Stream zipStream = null;

        try
        {
            if (!foundEntry || mediaInfo == null || mediaInfo.EntryName == null)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine($"Invalid zip entry key: {entryKey}");
                    Debugger.Break();
                }

                parent.Increment(ResultCounts.CountKeys.Error);
                _outputHandler($"{(isRetry ? _retryText : "")}Error: Invalid zip entry key: {entryKey}", MessageCode.Error);
                return;
            }

            using (archive = ZipFile.OpenRead(mediaInfo.ArchivePath))
            {
                entry = archive.GetEntry(mediaInfo.EntryName);
                if (entry == null) return;

                using (zipStream = GetTemporaryStream(entry.Length))
                {
                    using (Stream entryStream = entry.Open())
                    {
                        entryStream?.CopyTo(zipStream);
                    }

                    Dictionary<string, DateTime> dates = new Dictionary<string, DateTime>();

                    // 1. get date from media metadata
                    string fileType = GetDateFromMediaStream(zipStream, dates, parent);

                    if (fileType == null)
                    {
                        if (Debugger.IsAttached)
                        {
                            Debug.WriteLine($"Invalid media file type: {fileType ?? "null"}");
                            Debugger.Break();
                        }

                        parent.Increment(ResultCounts.CountKeys.Error);
                        _outputHandler($"{(isRetry ? _retryText : "")}Error: Invalid media file type", MessageCode.Error);
                        return;
                    }

                    if (mediaInfo.MetaJson != null)
                    {
                        // 2. get date from archive meta data
                        GetDateFromTakeoutMetadata(mediaInfo.MetaJson, dates, parent);
                    }

                    // 3. get date from entry name
                    TryGetDateFromMediaFileName(entry.FullName, dates);

                    DateTime mediaDate = GetBestDate(dates, out bool isBest);

                    string targetMediaDir = DirectoryNameFromDate(mediaDate, isRetry, parent, out string targetMediaDirName);
                    string targetMediaFile = Path.GetFileName(entry.FullName);
                    string targetMediaPath = Path.Combine(targetMediaDir, targetMediaFile);

                    _originalFileNameToDate[targetMediaFile] = mediaDate;

                    string prefix = _listOnly ? "Would " : string.Empty;
                    string verb = $"{prefix}Copy";

                    if (_listOnly)
                    {
                        if (_logging != LoggingVerbosity.Quiet)
                        {
                            _outputHandler($"{(isRetry ? _retryText : "")}{verb} file:\"{entry.Name}\" to \"{Path.Combine(".", targetMediaDirName)}\"");
                        }

                        return;
                    }

                    System.IO.Directory.CreateDirectory(targetMediaDir);
                    bool alreadyExits = ResolveFileConflict(isRetry, parent, entry.Length, ref targetMediaPath);

                    try
                    {
                        if (alreadyExits)
                        {
                            // already exists
                            // the files are the same - the entry is a duplicate
                            string text = _listOnly ? "List(duplicate)" : "Duplicate";
                            if (_logging == LoggingVerbosity.Verbose) _outputHandler($"{(isRetry ? _retryText : "")}{text}:\"{entry.Name}\" in \"{targetMediaDirName}\"");
                            parent.Increment(ResultCounts.CountKeys.Duplicate);
                        }
                        else
                        {
                            // the file does not exist - copy the entry to the file
                            const int mb = 1024 * 1024;
                            int bufferSize = entry.Length >= mb ? mb : Convert.ToInt32(entry.Length);
                            using (FileStream fileStream = new FileStream(targetMediaPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, true))
                            {
                                zipStream.Seek(0, SeekOrigin.Begin);
                                zipStream.CopyTo(fileStream, bufferSize);
                                parent.Increment(ResultCounts.CountKeys.Change);
                            }

                            if (mediaDate > _notBeforeDate) File.SetCreationTime(targetMediaPath, mediaDate);
                            if (_logging != LoggingVerbosity.Quiet)
                            {
                                _outputHandler($"{(isRetry ? _retryText : "")}{verb} file:\"{entry.Name}\" to \"{Path.Combine(".", targetMediaDirName)}\"");
                            }

                            parent.Increment(ResultCounts.CountKeys.Change);
                        }
                    }
                    catch (IOException ioex)
                    {
                        if (ioex.HResult == -2147024864)
                        {
                            retryMediaKeys.Enqueue(mediaInfo);
                            return;
                        }

                        HandleExceptions(ioex, parent, $"Could not create {targetMediaPath}");
                    }
                    catch (Exception ex)
                    {
                        HandleExceptions(ex, parent, $"Could not create {targetMediaPath}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            HandleExceptions(ex, parent, $"Could not extract {entryKey}");
        }
        finally
        {
            parent.Increment(ResultCounts.CountKeys.Progress);
            _statusUpdate();
        }

        return;
    }

    private Stream GetTemporaryStream(long length)
    {
        if (length >= 1 * 1024 * 1024)
        {
            return new FileStream(Path.GetTempFileName(), FileMode.Open, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose);
        }

        return new MemoryStream(Convert.ToInt32(length));
    }

    public  bool ValidateSource(string dir, string filter, PhotoCopierActions behavior, ResultCounts counts, out string reason)
    {
        reason = string.Empty;

        try
        {
            if (!System.IO.Directory.Exists(dir))
            {
                reason = $"Could not find: {dir}";
                return false;
            }

            // test file existence
            if (behavior == PhotoCopierActions.Copy && !System.IO.Directory.EnumerateFiles(dir, filter, SearchOption.TopDirectoryOnly).Any())
            {
                reason = $"Could not locate any media archive directories matching {filter ?? ""}";
                return false;
            }
        }
        catch (Exception ex)
        {
            HandleExceptions(ex, counts, $"Could not validate directory: {dir}");
            return false;
        }

        return true;
    }

    public bool ValidateDestination(string dir, ResultCounts parent, out string reason)
    {
        reason = string.Empty;

        try
        {
            if (!System.IO.Directory.Exists(dir))
            {
                reason = $"Could not find: {dir}";
                return false;
            }

            // test permissions
            Random rnd = new Random();
            StringBuilder sb = new StringBuilder();
            for (int ii = 0; ii < 8; ii++)
            {
                sb.Append(Convert.ToChar('A' + rnd.Next() % 26));
            }
            string testFile = Path.Combine(dir, $"{sb}.tmp");
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
            HandleExceptions(ex, parent, $"Could not validate directory path:  {dir}");
            return false;
        }

        return true;
    }

    public bool ValidateReorderBackupDirectory(string dir, ResultCounts counts, out string reason)
    {
        reason = string.Empty;
        string path = dir?.Trim();

        try
        {
            if (string.IsNullOrEmpty(path?.Trim()))
            {
                reason = "Backup directory path cannot be null or empty";
                return false;
            }

            if (!System.IO.Directory.Exists(path))
            {
                reason = $"Could not find: {path}";
                return false;
            }

            // test permissions
            Random rnd = new Random();
            StringBuilder sb = new StringBuilder();
            for (int ii = 0; ii < 8; ii++)
            {
                sb.Append(Convert.ToChar('A' + rnd.Next() % 26));
            }
            string testFile = Path.Combine(path, $"{sb}.tmp");
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
            HandleExceptions(ex, counts, $"Could not write to backup directory {dir}");
            return false;
        }

        return true;
    }

    public bool ValidatePattern(string text, ResultCounts parent, out string reason)
    {
        reason = string.Empty;

        try
        {
            if (text.Contains(':') || text.Contains(".."))
            {
                reason = "Contains illegal character ':' or '..'";
                return false;
            }
        }
        catch (Exception ex)
        {
            HandleExceptions(ex, parent, $"Could not validate pattern {text}");
            return false;
        }

        return true;
    }

    public bool ValidateJunk(string text, ResultCounts parent, out string reason)
    {
        reason = string.Empty;

        try
        {
            string[] extensions = GetJunkExtensions(text);

            foreach (string extension in extensions)
            {
                if (extension.Contains(':') || extension.Contains("..") || extension.Contains('\\') || extension.Contains('/'))
                {
                    reason = "Junk extensions contains one or more illegal characters or text ':', '\\', '/', or '..'";
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            HandleExceptions(ex, parent, $"Could not validate Junk text {text}");
            return false;
        }

        return true;
    }

    private  string[] GetJunkExtensions(string text)
    {
        if (string.IsNullOrEmpty(text)) return Array.Empty<string>();
        string[] parts = text.Split(',');
        List<string> extensions = new List<string>();
        foreach (string part in parts)
        {
            string t = part.Trim();
            if (!t.StartsWith('.'))
            {
                t = $".{t}";
            }

            extensions.Add(t);
        }

        return extensions.ToArray();
    }

    // 20250129T160914Z.zip
    private string DateString(DateTime date)
    {
        return date.ToString("yyyyMMddhhmmssZ");
    }

    private void GenerateBackup(ICollection<string> filesToBackup, ResultCounts parent)
    {
        DateTime utcNow = DateTime.UtcNow;
        object lockObj = new object();

        string subDir = Path.Combine(_backup, $"TOWBackup-{DateString(utcNow)}");
        string rtext = _source + "\\";

        _outputHandler($"Creating backup of media directory: {_source} to {subDir}");

        ResultCounts results = new ResultCounts(parent, "Backup");
        int total = filesToBackup.Count;
        results.Set(ResultCounts.CountKeys.Total, total);
        _statusUpdate(StatusCode.Total, total, "(backup)");

        if (_parallel)
        {
            _cancel = new CancellationTokenSource();
            ParallelOptions parallelOptions = new ParallelOptions();
            parallelOptions.CancellationToken = _cancel.Token;

            Parallel.ForEach(filesToBackup, parallelOptions, oldFile => BackupFile(subDir, rtext, oldFile, results, lockObj));
        }
        else
        {
            foreach (string oldFile in filesToBackup)
            {
                BackupFile(subDir, rtext, oldFile, results, lockObj);
            }
        }
    }

    private void BackupFile(string subDir, string rtext, string oldFile, ResultCounts parent, object lockObj)
    {
        try
        {
            // remove the source root directory
            string newFnameInSubDir = oldFile.Replace(rtext, string.Empty);
            string newFile = Path.Combine(subDir, newFnameInSubDir);
            string newDir = Path.GetDirectoryName(newFile);

            string oldDir = Path.GetDirectoryName(oldFile);
            if (newDir == null || oldDir == null) return;

            lock (lockObj)
            {
                DirectoryInfo diNew = new DirectoryInfo(newDir);
                if (!diNew.Exists)
                {
                    if (_isAdmin)
                    {
                        DirectoryInfo diOld = new DirectoryInfo(oldDir);
                        DirectorySecurity srcPermissions = diOld.GetAccessControl();
                        byte[] securityDescriptor = srcPermissions.GetSecurityDescriptorBinaryForm();

                        DirectorySecurity tgtPermissions = new DirectorySecurity();
                        tgtPermissions.SetSecurityDescriptorBinaryForm(securityDescriptor);
                        diNew.Create(tgtPermissions);

                        diNew.Attributes = diOld.Attributes;
                    }
                    else
                    {
                        diNew.Create();
                    }
                }
            }

            _outputHandler($"Backing up \"{newFnameInSubDir}\".");
            File.Copy(oldFile, newFile);
        }
        catch (Exception ex)
        {
            HandleExceptions(ex, parent, ex.Message);
        }
        finally
        {
            parent.Increment(ResultCounts.CountKeys.Progress);
            _statusUpdate();
        }
    }

    public static bool IsRunningAsAdministrator()
    {
        using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
        {
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }

    // return true if the file exists
    private bool ResolveFileConflict(bool isRetry, ResultCounts parent, long sourceLength, ref string targetPath)
    {
        int count = 1;
        string targetDir = Path.GetDirectoryName(targetPath);
        if (targetDir == null)
        {
            throw new NullReferenceException("Target folder cannot be null");
        }

        string targetExt = Path.GetExtension(targetPath);
        string targetFName = Path.GetFileNameWithoutExtension(targetPath);

        do
        {
            if (!File.Exists(targetPath)) return false;

            FileInfo fiT = new FileInfo(targetPath);

            // same name, both exist, lengths are the same
            if (sourceLength == fiT.Length) return true;

            targetPath = Path.Combine(targetDir, $"{targetFName}-{count++}{targetExt}");
        }
        while (!IsCanceled);

        return true;
    }

    // return true if the file exists
    private bool ResolveFileConflict(bool isRetry, ResultCounts parent, string sourcePath, ref string targetPath)
    {
        if (!File.Exists(sourcePath)) return false;
        FileInfo fiS = new FileInfo(sourcePath);

        return ResolveFileConflict(isRetry, parent, fiS.Length, ref targetPath);
    }
}

public class ResultCounts
{
    private object locker = new object();
    public enum CountKeys
    {
        Total,
        Error,
        Skip,
        Duplicate,
        Change,
        Progress
    }

    private readonly ConcurrentDictionary<CountKeys, int?> Values = new ConcurrentDictionary<CountKeys, int?>();

    public readonly string Text;
    public readonly ResultCounts Parent;
    public readonly List<ResultCounts> Children = new List<ResultCounts>();

    public ResultCounts(ResultCounts parent, string text)
    {
        lock (locker)
        {
            Children.Clear();
            foreach (CountKeys key in Enum.GetValues<CountKeys>())
            {
                Values[key] = null;
            }

            parent?.Children.Add(this);
            Parent = parent;
            Text = text;
        }
    }

    public void Increment(CountKeys key)
    {
        lock (locker)
        {
            Values[key] = Values[key].GetValueOrDefault() + 1;
        }
    }

    public void Set(CountKeys key, int value)
    {
        lock (locker)
        {
            Values[key] = value;
        }
    }

    public void Add(CountKeys key, int value)
    {
        lock (locker)
        {
            Values[key] += value;
        }
    }

    public int? Get(CountKeys key)
    {
        lock (locker)
        {
            return Values[key];
        }
    }
}
