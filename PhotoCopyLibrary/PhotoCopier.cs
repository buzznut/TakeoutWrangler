//  <@$&< copyright begin >&$@> 24FE144C2255E2F7CCB65514965434A807AE8998C9C4D01902A628F980431C98:20241017.A:2025:7:1:14:38
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024-2025 Stewart A. Nutter - All Rights Reserved.
// No warranty is implied or given.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// <@$&< copyright end >&$@>

// using Library "CompactExifLib" https://www.codeproject.com/Articles/5251929/CompactExifLib-Access-to-EXIF-Tags-in-JPEG-TIFF-an
// for reading and writing EXIF data in JPEG, TIFF and PNG image files.
// © Copyright 2021 Hans-Peter Kalb

using CommonLibrary;
using MetadataExtractor;
using MetadataExtractor.Formats.Avi;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.FileType;
using MetadataExtractor.Formats.QuickTime;
using MimeKit;
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
    private CancellationTokenSource cancel;
    private const string retryText = "(Retry) ";
    private  int canceled;
    private Settings settings;
    private string password = null;
    private static bool isAdmin;
    private static readonly DateTime notBeforeDate = new DateTime(1976, 1, 1);
    private HashSet<string> junkExtensions;
    private bool IsCanceled { get { return Interlocked.Add(ref canceled, 0) > 0; } }
    private readonly char[] fileNameSeparator = new char[] { '.' };
    private readonly char[] zipDirSeparator = new char[] { '\\' };
    private readonly HandleOutput outputHandler = DummyOutput;
    private readonly StatusUpdate statusUpdate = DummyStatus;
    private readonly ConcurrentBag<ZipArchive> archives = new ConcurrentBag<ZipArchive>();
    private readonly ConcurrentDictionary<string, DateTime> originalFileNameToDate = new ConcurrentDictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
    private int archivedMailCount = 0;

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
        new ConfigParam { Name = "junk", Default = null, PType = ParamType.String },
        new ConfigParam { Name = "keeplocked", Default = false, PType = ParamType.Bool },
        new ConfigParam { Name = "password", Default = string.Empty, PType = ParamType.String },
        new ConfigParam { Name = "keeptrash", Default = false, PType = ParamType.Bool },
        new ConfigParam { Name = "keepspam", Default = false, PType = ParamType.Bool },
        new ConfigParam { Name = "keepsent", Default = false, PType = ParamType.Bool },
        new ConfigParam { Name = "keeparchived", Default = false, PType = ParamType.Bool }
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
        isAdmin = IsRunningAsAdministrator();
        outputHandler = output;
        statusUpdate = status;
    }

    public ReturnCode Initialize(
        string appName,
        bool help,
        Settings settings,
        string password)
    {
        try
        {
            this.settings = settings;
            string[] allActions = Enum.GetNames<PhotoCopierActions>();

            List<string> reasons = new List<string>();

            // trim trailing backslashes
            if ((settings.Source?.EndsWith('\\')).GetValueOrDefault()) this.settings.Source = settings.Source?.Substring(0, settings.Source.Length - 1);
            if ((settings.Destination?.EndsWith('\\')).GetValueOrDefault()) this.settings.Destination = settings.Destination?.Substring(0, settings.Destination.Length - 1);

            // validate paths
            help |= !Configs.ValidatePath(this.settings.Source, "Source", reasons);
            help |= !Configs.ValidatePath(this.settings.Destination, "Destination", reasons);
            help |= !ValidateLocked(this.settings.KeepLocked, password, reasons);

            if (help)
            {
                outputHandler($"usage: {appName} -source=\"path\" -destination=\"path\" -action={string.Join('|', allActions)} -pattern=$y_$m -filter=takeout-*.zip");
                outputHandler();
                outputHandler($"  Copy all photos, movies, etc. senders a set of archive zip files (Google takeout) senders the source folder");
                outputHandler($"  to destination folder. Subfolders will be created to hold photos with settings.Pattern naming. Should run in admin mode");
                outputHandler($"  for file and folder access.");
                outputHandler();
                outputHandler($"where:");
                outputHandler();
                outputHandler($" source=path        folder that contains the takeout zip file(s).");
                outputHandler($"                    (default=null");
                outputHandler();
                outputHandler($" destination=path   folder where to put photo contents (files and subfolders)");
                outputHandler($"                    (default=null");
                outputHandler();
                outputHandler($" backup=path        folder where to put backup of original contents before reorder action (files and subfolders)");
                outputHandler($"                    (default=empty/null - no backup before reorder");
                outputHandler();
                outputHandler($" action=option      option is one of {PhotoCopierActions.Copy} or {PhotoCopierActions.Reorder}.");
                outputHandler($"                    (default={PhotoCopierActions.Copy}");
                outputHandler();
                outputHandler($"                    {PhotoCopierActions.Copy} media files to destination.");
                outputHandler($"                    {PhotoCopierActions.Reorder} media files only if settings.Pattern is different (create backup zip files).");
                outputHandler();
                outputHandler($" list=value         value is true to list only, false to perform actions.");
                outputHandler($"                    example: list=true would only show potential errors or actions without changing destination.");
                outputHandler();
                outputHandler($" parallel=value     value is true to run in parallel for faster performance, false to run sequentially. Parallem mode uses more memory.");
                outputHandler($"                    example: parallel=true Uses more memory and runs faster. (default=true)");
                outputHandler();
                outputHandler($" keeplocked=value   value is true to keep locked file content.");
                outputHandler($"                    example: keepLocked=true will keep locked file content. Requires password. (default=false)");
                outputHandler();
                outputHandler($" password=text      required password to encrypt locked files if keeplocked is true (not saved to config file).");
                outputHandler();
                outputHandler($" junk=text          junk files with these comma separated extensions to remove. (default=empty)");
                outputHandler();
                outputHandler($" loggin=verbosity   {LoggingVerbosity.Quiet}, {LoggingVerbosity.Change}, or {LoggingVerbosity.Verbose}. {LoggingVerbosity.Quiet}=minimal output, {LoggingVerbosity.Change}=minimal+changed content, {LoggingVerbosity.Verbose}=all");
                outputHandler($"                    (default={LoggingVerbosity.Change})");
                outputHandler();
                outputHandler($" settings.Pattern=text       Target folder name settings.Pattern. (default=$y_$m");
                outputHandler($"                       example:");
                outputHandler($"                         if date-time, is March 27, 2024 at 3:33PM");
                outputHandler($"                         $y = 4 digit year.  ex: 2024");
                outputHandler($"                         $m = 2 digit month. ex: 03");
                outputHandler($"                         $M = month name.    ex: March");
                outputHandler($"                         $d = 2 digit day.   ex: 27");
                outputHandler($"                         $h = 2 digit hour.  ex: 15 (24 hours per day)");
                outputHandler($"                         '$y-$M($d)' \"root\\2024-March(3)\"");
                outputHandler();
                outputHandler($" filter=text        photo archive file filter. (default='takeout-*.zip')");
                outputHandler();
                outputHandler($" keepTrash=value    value is true to keep mail trash folder content.");
                outputHandler($"                    example: keepTrash=true will keep mail trash folder content.");
                outputHandler();
                outputHandler($" settings.KeepSpam) value is true to keep mail spam folder content.");
                outputHandler($"                    example: settings.KeepSpam)=true will keep mail spam folder content.");
                outputHandler();
                outputHandler($" keepSent=value     value is true to keep mail sent folder content.");
                outputHandler($"                    example: keepent=true will keep mail sent folder content.");
                outputHandler();
                outputHandler($" keepArchived=value value is true to keep mail archive folder content.");
                outputHandler($"                    example: keepent=true will keep mail archive folder content.");
                outputHandler();

                if (reasons.Count > 0)
                {
                    outputHandler("Issues: ", MessageCode.Warning);

                    int index = 1;
                    foreach (string reason in reasons)
                    {
                        outputHandler($"  {index++}. {reason}", MessageCode.Warning);
                    }

                    outputHandler();
                }

                return ReturnCode.HadIssues;
            }

            if (settings.Behavior == PhotoCopierActions.Copy && !System.IO.Directory.GetFiles(settings.Source ?? ".\\", settings.Filter).Any())
            {
                outputHandler($"No source files found. sourceDir:\"{settings.Source}\", file filter:\"{settings.Filter}\"");
                return ReturnCode.DirectoryError;
            }

            this.password = password;

            // keep the parameters

            outputHandler("Current configuration:");
            outputHandler($" Running as Admin: {isAdmin}", isAdmin ? MessageCode.Success : MessageCode.Warning);
            outputHandler($" Action: {this.settings.Behavior}");
            outputHandler($" Source: {this.settings.Source}");
            outputHandler($" Destination: {this.settings.Destination}");
            outputHandler($" Filter: {this.settings.Filter}");
            outputHandler($" Pattern: {this.settings.Pattern}");
            outputHandler($" Junk extensions: {this.settings.JunkExtensions ?? string.Empty}");
            outputHandler($" Logging: {this.settings.Logging}");
            outputHandler($" ListOnly: {this.settings.ListOnly}");
            outputHandler($" Parallel: {this.settings.Parallel}");
            outputHandler($" KeepLocked: {this.settings.KeepLocked}");
            outputHandler($" Password: {(string.IsNullOrEmpty(password) ? "Not set" : "Set (hidden)")}");
            outputHandler($" KeepTrash: {this.settings.KeepTrash}");
            outputHandler($" KeepSpam: {this.settings.KeepSpam}");
            outputHandler($" KeepSent: {this.settings.KeepSent}");
            outputHandler($" KeepArchived: {this.settings.KeepArchived}");
            outputHandler();

            if (this.settings.Behavior == PhotoCopierActions.Reorder)
            {
                bool inPlace = this.settings.Source != null && this.settings.Source.Equals(this.settings.Destination, StringComparison.OrdinalIgnoreCase);
                this.settings.Backup = string.IsNullOrEmpty(this.settings.Backup) ? "null" : this.settings.Backup;
                outputHandler($" Backup: {this.settings.Backup ?? "(not set)"}", this.settings.Behavior == PhotoCopierActions.Reorder && inPlace && string.IsNullOrEmpty(this.settings.Backup) ? MessageCode.Warning : MessageCode.Success);
            }

            return (int)ReturnCode.Success;
        }
        catch (Exception ex)
        {
            HandleExceptions(ex, null, "Unhandled Reorder exception");
            return ReturnCode.Error;
        }
    }

    private bool ValidateLocked(bool keepLocked, string password, List<string> reasons)
    {
        if (keepLocked && string.IsNullOrEmpty(password))
        {
            reasons.Add("If keepLocked is true, then a password must be provided.");
            return false;
        }

        return true;
    }

    public bool IsRunning { get; private set; }

    public void Stop()
    {
        if (!IsCanceled)
        {
            outputHandler();
            outputHandler("Operation canceled", MessageCode.Warning);
            outputHandler();

            Interlocked.Increment(ref canceled);
            cancel?.Cancel();
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

            junkExtensions = GetJunkExtensions(settings.JunkExtensions).ToHashSet(StringComparer.OrdinalIgnoreCase);
            canceled = 0;

            IsRunning = true;
            returnCode = ReturnCode.Success;

            if (settings.Behavior == PhotoCopierActions.Copy)
            {
                returnCode = ProcessTakeoutZips(results);
            }
            else if (settings.Behavior == PhotoCopierActions.Reorder)
            {
                returnCode = ProcessReorder(results);
            }
            else
            {
                results.Increment(ResultCounts.CountKeys.Error);
                outputHandler("Invalid behavior", MessageCode.Error);
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
            foreach (ZipArchive archive in archives)
            {
                archive.Dispose();
            }

            archives.Clear();
            IsRunning = false;

            stopwatch.Stop();

            outputHandler();
            outputHandler($"Elapsed={stopwatch.Elapsed}, Filter=\"{settings.Filter}\"");

            outputHandler();
            EmitResults(results);
            outputHandler();
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
        outputHandler(text);

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
            bool inPlace = settings.Source.Equals(settings.Destination, StringComparison.OrdinalIgnoreCase);

            if (inPlace)
            {
                filesToDelete = new ConcurrentBag<string>();
            }

            HashSet<string> allPaths = System.IO.Directory.EnumerateFiles(settings.Source, "*.*", SearchOption.AllDirectories).ToHashSet(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrEmpty(settings.Backup) && inPlace)
            {
                if (!System.IO.Directory.Exists(settings.Backup))
                {
                    results.Increment(ResultCounts.CountKeys.Error);
                    outputHandler($"Backup root directory does not exist: {settings.Backup}", MessageCode.Error);
                    return ReturnCode.Error;
                }

                string verb = settings.ListOnly ? "Would backup" : "Backing up";
                if (settings.Logging != LoggingVerbosity.Quiet)
                {
                    outputHandler($"{verb} source directory:\"{settings.Source}\" to \"{settings.Backup}\"");
                }

                if (!settings.ListOnly)
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

            if (settings.Logging != LoggingVerbosity.Quiet)
            {
                outputHandler();
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

                if (settings.Logging != LoggingVerbosity.Quiet)
                {
                    outputHandler($"Retry({retryCount}): {allPaths.Count} entries.");
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

        if (isAdmin)
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

        if (settings.Logging != LoggingVerbosity.Quiet)
        {
            outputHandler();
        }

        do
        {
            emptyDirs.Clear();
            FindEmptyDirs(settings.Destination, emptyDirs);
            if (emptyDirs.Count == 0 || IsCanceled)
            {
                break;
            }

            total += emptyDirs.Count;
            results.Set(ResultCounts.CountKeys.Total, total);
            statusUpdate(statusCode, total, "(directory cleanup)");
            statusCode = StatusCode.More;
            string dverb = settings.ListOnly ? "Would remove" : "Removed";

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

                    if (!settings.ListOnly)
                    {
                        diOld.Delete();
                        results.Increment(ResultCounts.CountKeys.Change);
                    }

                    if (settings.Logging != LoggingVerbosity.Quiet)
                    {
                        outputHandler($"{dverb} empty directory:\"{dir}\"");
                    }
                }
                catch (Exception ex)
                {
                    HandleExceptions(ex, results, $"Could not delete empty directory:\"{dir}\"");
                }
                finally
                {
                    results.Increment(ResultCounts.CountKeys.Progress);
                    statusUpdate();
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
        statusUpdate(StatusCode.Total, total, "(duplicate/junk file cleanup)");

        foreach (string path in filesToDelete)
        {
            if (IsCanceled) break;

            try
            {
                string verb = settings.ListOnly ? "Would delete" : "Deleting";
                outputHandler($"{verb} duplicate or junk file:\"{path}\"");

                if (!settings.ListOnly)
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
                statusUpdate();
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
        statusUpdate(StatusCode.Total, total, $"({context.ToLower()})");

        try
        {
            if (settings.Parallel)
            {
                cancel = new CancellationTokenSource();
                ParallelOptions parallelOptions = new ParallelOptions();
                parallelOptions.CancellationToken = cancel.Token;

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
            outputHandler($"Error: {reason}. Reason={ex.Message}", MessageCode.Error);
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

            // 1. Get date senders media content
            if (fileType == null)
            {
                parent.Increment(ResultCounts.CountKeys.Skip);
                if (settings.Logging == LoggingVerbosity.Verbose)
                {
                    outputHandler($"{(isRetry ? retryText : "")}Skipped - not a media file:\"{sourceMediaPath}\".", MessageCode.Warning);
                }
                return;
            }

            DateTime mediaDate = GetBestDate(dates, out bool isBest);
            originalFileNameToDate[sourceMediaFileName] = mediaDate;

            string targetMediaDir = DirectoryNameFromDate(mediaDate, false, parent, out string targetMediaDirName);
            if (targetMediaDir == null)
            {
                throw new NullReferenceException($"New media file directory cannot not be null");
            }

            string targetMediaPath = Path.Combine(targetMediaDir, sourceMediaFileName);

            string prefix = settings.ListOnly ? "Would " : string.Empty;
            string verb = inPlace ? $"{prefix}Move" : $"{prefix}Copy";

            if (settings.ListOnly)
            {
                if (settings.Logging != LoggingVerbosity.Quiet)
                {
                    outputHandler($"{(isRetry ? retryText : "")}{verb} file:\"{sourceMediaFileName}\" to \"{Path.Combine(".", targetMediaDirName)}\"");
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
                    string text = settings.ListOnly ? "List(duplicate)" : "Duplicate";
                    if (settings.Logging != LoggingVerbosity.Quiet) outputHandler($"{(isRetry ? retryText : "")}{text}:\"{sourceMediaFileName}\" in \"{targetMediaDirName}\"");
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

                    if (mediaDate > notBeforeDate) File.SetCreationTime(targetMediaPath, mediaDate);
                    parent.Increment(ResultCounts.CountKeys.Change);

                    if (settings.Logging != LoggingVerbosity.Quiet)
                    {
                        outputHandler($"{(isRetry ? retryText : "")}{verb} file:\"{sourceMediaFileName}\" to \"{Path.Combine(".", targetMediaDirName)}\"");
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
            statusUpdate();
        }
    }

    private DateTime GetBestDate(Dictionary<string, DateTime> dates, out bool isBest)
    {
        DateTime mediaDate = DateTime.MinValue;
        if (dates.TryGetValue("best", out mediaDate) && mediaDate > notBeforeDate)
        {
            isBest = true;
            return mediaDate;
        }

        isBest = false;

        if (dates.TryGetValue("Date/Time Original", out mediaDate) && mediaDate > notBeforeDate) return mediaDate;
        if (dates.TryGetValue("8:6", out mediaDate) && mediaDate > notBeforeDate) return mediaDate;
        if (dates.TryGetValue("L8", out mediaDate) && mediaDate > notBeforeDate) return mediaDate;
        if (dates.TryGetValue("Date/Time", out mediaDate) && mediaDate > notBeforeDate) return mediaDate;
        if (dates.TryGetValue("byString", out mediaDate) && mediaDate > notBeforeDate) return mediaDate;
        if (dates.TryGetValue("File Modified Date", out mediaDate) && mediaDate > notBeforeDate) return mediaDate;
        if (dates.TryGetValue("unix", out mediaDate) && mediaDate > notBeforeDate) return mediaDate;

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
                if (date > notBeforeDate)
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
                if (date > notBeforeDate)
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

        StringBuilder sb = new StringBuilder();
        List<string> parts = new List<string>();
        foreach (char c in filename)
        {
            if (char.IsDigit(c))
            {
                sb.Append(c);
            }
            else
            {
                if (sb.Length > 0)
                {
                    parts.Add(sb.ToString());
                    sb.Clear();
                }
            }
        }

        if (sb.Length > 0)
        {
            parts.Add(sb.ToString());
        }

        int year = 0;
        int month = 0;
        int day = 0;
        int hour = -1;
        int minute = -1;
        int second = -1;
        int milliseconds = -1;

        Queue<string[]> queue = new Queue<string[]>();
        queue.Enqueue(parts.ToArray());

        // try to find a date in the filename
        while (queue.Count > 0 && !IsCanceled)
        {
            string[] queueParts = queue.Dequeue();

            foreach (string part in queueParts)
            {
                if (IsCanceled) return;

                if (part.Length == 9)
                {
                    string[] subparts = SplitByLength(part, 2, 2, 2, 3);
                    queue.Enqueue(subparts);
                    continue;
                }

                if (part.Length == 8)
                {
                    string[] subparts = SplitByLength(part, 4, 2, 2);
                    queue.Enqueue(subparts);
                    continue;
                }

                if (part.Length == 6)
                {
                    string[] subparts = SplitByLength(part, 2, 2, 2);
                    queue.Enqueue(subparts);
                    continue;
                }

                if (year == 0 && part.Length == 4 && int.TryParse(part, out year) && year >= 1900 && year <= DateTime.Now.Year)
                {
                    continue;
                }
                if (year == 0 && part.Length == 2 && int.TryParse(part, out year) && year >= 1900 && year <= DateTime.Now.Year)
                {
                    year += 2000;
                    continue;
                }
                if (year > 0 && month == 0 && part.Length == 2 && int.TryParse(part, out month) && month >= 1 && month <= 12)
                {
                    continue;
                }
                if (month > 0 && day == 0 && part.Length == 2 && int.TryParse(part, out day) && day >= 1 && day <= 31)
                {
                    continue;
                }
                if (day > 0 && hour == -1 && part.Length == 2 && int.TryParse(part, out hour) && hour >= 0 && hour <= 23)
                {
                    continue;
                }
                if (hour >= 0 && minute == -1 && part.Length == 2 && int.TryParse(part, out minute) && minute >= 0 && minute <= 59)
                {
                    continue;
                }
                if (minute >= 0 && second == -1 && part.Length == 2 && int.TryParse(part, out second) && second >= 0 && second <= 59)
                {
                    continue;
                }
                if (second >= 0 && milliseconds == -1 && part.Length == 3 && int.TryParse(part, out milliseconds) && milliseconds >= 0 && milliseconds < 1000)
                {
                    continue;
                }
            }
        }

        try
        {
            if (year > 0 && month > 0 && day > 0)
            {
                DateTime date;
                string fromText = null;

                if (hour >= 0 && minute >= 0 && second >= 0)
                {
                    if (milliseconds >= 0)
                    {
                        date = new DateTime(year, month, day, hour, minute, second, milliseconds);
                    }
                    else
                    {
                        date = new DateTime(year, month, day, hour, minute, second);
                    }

                    fromText = "8:6";
                }
                else
                {
                    date = new DateTime(year, month, day);
                    fromText = "L8";
                }

                dates[fromText] = date;
            }
        }
        catch
        {
            // just eat date parsing exceptions
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
                    if (date > notBeforeDate)
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
            ConcurrentDictionary<string, TakeoutInfo> mediaKeys = new ConcurrentDictionary<string, TakeoutInfo>(StringComparer.OrdinalIgnoreCase);
            if (settings.Logging != LoggingVerbosity.Quiet) outputHandler($"Processing dir:{settings.Source}");

            foreach (string entity in System.IO.Directory.EnumerateFiles(settings.Source, settings.Filter, SearchOption.TopDirectoryOnly))
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

            ConcurrentQueue<TakeoutInfo> retryMediaKeys = new ConcurrentQueue<TakeoutInfo>();
            DoMedia(false, results, mediaKeys, retryMediaKeys, "Process media");

            int retryCount = 0;
            while (!IsCanceled)
            {
                retryCount++;
                mediaKeys.Clear();
                if (retryMediaKeys.IsEmpty) break;

                while (retryMediaKeys.TryDequeue(out TakeoutInfo result))
                {
                    mediaKeys[result.EntryKey] = result;
                }

                outputHandler($"Retry({retryCount}): {mediaKeys.Count} entries.");

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

    private void DoMedia(bool inRetry, ResultCounts parent, ConcurrentDictionary<string, TakeoutInfo> mediaKeys, ConcurrentQueue<TakeoutInfo> retryMediaKeys, string context)
    {
        ResultCounts results = new ResultCounts(parent, context);

        try
        {
            int total = mediaKeys.Count;
            results.Set(ResultCounts.CountKeys.Total, total);
            statusUpdate(StatusCode.Total, total, $"({context.ToLower()})");

            if (settings.Parallel && !inRetry)
            {
                cancel = new CancellationTokenSource();
                ParallelOptions parallelOptions = new ParallelOptions
                {
                    CancellationToken = cancel.Token
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

    private void ProcessZip(string zip, ResultCounts parent, ConcurrentDictionary<string, TakeoutInfo> infoKeys)
    {
        try
        {
            ZipArchive archive = ZipFile.OpenRead(zip);
            archives.Add(archive);

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
                    outputHandler($"Error: root, {root} is not a directory. Reason=InvalidPath", MessageCode.Error);
                    continue;
                }

                string[] fileParts = file.Split(fileNameSeparator, 2);

                string ext = null;
                if (fileParts.Length > 1) ext = fileParts[1];
                string key = Path.Combine(dir, fileParts[0]);

                if (!entries.TryGetValue(key, out Dictionary<string, ZipArchiveEntry> values))
                {
                    values = new Dictionary<string, ZipArchiveEntry>(StringComparer.OrdinalIgnoreCase);
                    entries[key] = values;
                }

                if (string.IsNullOrEmpty(ext) || (values.ContainsKey(ext)))
                {
                    if (Debugger.IsAttached)
                    {
                        Debug.WriteLine($"Extension is null or in the skip list: {ext}");
                    }
                    continue;
                }

                values[ext] = entry;
            }

            if (settings.Logging != LoggingVerbosity.Quiet) outputHandler($"Processing zip:{zip}, entries count:{entries.Count}");

            // group the media and meta data with the entries by media name
            foreach (string key in entries.Keys)
            {
                if (IsCanceled) break;

                Dictionary<string, ZipArchiveEntry> values = entries[key];
                ReadInfo(key, infoKeys, values, zip);
            }
        }
        catch (Exception ex)
        {
            HandleExceptions(ex, parent, $"Could not process zipfile {zip}");
        }
    }

    private DataType GetDataType(string key)
    {
        string[] dirParts = key.Split(zipDirSeparator);
        if (dirParts.Length < 2 || !dirParts[0].Equals("takeout", StringComparison.OrdinalIgnoreCase))
        {
            return DataType.Unknown;
        }

        if (key.Contains("locked", StringComparison.OrdinalIgnoreCase))
        {
            // special case for locked takeout
            return DataType.Locked;
        }

        string archiveType = dirParts[1].ToLower();
        switch (archiveType)
        {
            default:
            {
                if (Debugger.IsAttached) { Debugger.Break(); }
                return DataType.Unknown;
            }

            case "access log activity":
            {
                return DataType.AccessLogActivity;
            }

            case "my activity":
            {
                return DataType.MyActivity;
            }

            case "google pay":
            {
                return DataType.GooglePay;
            }

            case "maps (your places)":
            {
                return DataType.MapsYourPlaces;
            }

            case "google play store":
            {
                return DataType.PlayStore;
            }

            case "saved":
            {
                return DataType.Saved;
            }

            case "fitbit":
            {
                return DataType.FitBit;
            }

            case "timeline":
            {
                return DataType.TimeLine;
            }

            case "google play books":
            {
                return DataType.PlayBooks;
            }

            case "google finance":
            {
                return DataType.Finance;
            }

            case "news":
            {
                return DataType.News;
            }

            case "gemini":
            {
                return DataType.Gemini;
            }

            case "calendar":
            {
                return DataType.Calendar;
            }

            case "alerts":
            {
                return DataType.Alerts;
            }

            case "tasks":
            {
                return DataType.Tasks;
            }

            case "recorder":
            {
                return DataType.Recorder;
            }

            case "keep":
            {
                return DataType.Keep;
            }

            case "google play movies & tv":
            {
                return DataType.PlayMoviesTV;
            }

            case "google feedback":
            {
                return DataType.Feedback;
            }

            case "google chat":
            {
                return DataType.Chat;
            }

            case "google workspace marketplace":
            {
                return DataType.WorkspaceMarketplace;
            }

            case "android device configuration service":
            {
                return DataType.DeviceConfigurationService;
            }

            case "chrome":
            {
                return DataType.Chrome;
            }

            case "groups":
            {
                return DataType.Groups;
            }

            case "google business profile":
            {
                return DataType.BusinessProfile;
            }

            case "google help communities":
            {
                return DataType.HelpCommunities;
            }

            case "maps":
            {
                return DataType.Maps;
            }

            case "google meet":
            {
                return DataType.Meet;
            }

            case "voice":
            {
                return DataType.Voice;
            }

            case "google account":
            {
                return DataType.Account;
            }

            case "fit":
            {
                return DataType.Fit;
            }

            case "search notifications":
            {
                return DataType.SearchNotifications;
            }

            case "google shopping":
            {
                return DataType.Shopping;
            }

            case "google store":
            {
                return DataType.Store;
            }

            case "blogger":
            {
                return DataType.Blogger;
            }

            case "profile":
            {
                return DataType.Profile;
            }

            case "home app":
            {
                return DataType.HomeApp;
            }

            case "contacts":
            {
                return DataType.Contacts;
            }

            case "google photos":
            {
                return DataType.Photos;
            }

            case "archive_browser":
            {
                return DataType.ArchiveBrowser;
            }

            case "drive":
            {
                return DataType.Drive;
            }

            case "mail":
            {
                return DataType.Mail;
            }

            case "youtube and youtube music":
            {
                return DataType.YouTube;
            }

            case "discover":
            {
                return DataType.Discover;
            }

            case "assignments":
            {
                return DataType.Assignments;
            }
        }
    }

    private void ReadInfo(string key, ConcurrentDictionary<string, TakeoutInfo> mediaKeys, Dictionary<string, ZipArchiveEntry> values, string zip)
    {
        try
        {
            if (!mediaKeys.TryGetValue(key, out TakeoutInfo mediaInfo))
            {
                mediaInfo = new TakeoutInfo { EntryKey = key };
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
                }
            }

            mediaKeys[key] = mediaInfo;
            mediaInfo = null;
        }
        catch (Exception ex)
        {
            outputHandler(ex.Message, MessageCode.Error);
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }
    }

    private string DirectoryNameFromDataType(DateTime date, DataType dataType, string fullName, bool isRetry, ResultCounts parent, out string newTargetDirName)
    {
        newTargetDirName = dataType.ToString();
        string dir = Path.GetDirectoryName(fullName);
        string subDir = Path.GetFileName(dir);
        string dateString = null;

        if (dataType == DataType.Photos || dataType == DataType.Locked || dataType == DataType.Mail) subDir = null;

        if (date > notBeforeDate)
        {
            string year = $"{date:yyyy}";
            string month = $"{date:MM}";
            string day = $"{date:dd}";
            string hour = $"{date:HH}";

            dateString = settings.Pattern.Replace("$y", year, StringComparison.OrdinalIgnoreCase)
                .Replace("$m", month)
                .Replace("$d", day, StringComparison.OrdinalIgnoreCase)
                .Replace("$h", hour, StringComparison.OrdinalIgnoreCase)
                .Replace("$M", MonthName(date.Month));
        }

        string newFilePath = string.IsNullOrEmpty(dateString) ?
            Path.GetFullPath(Path.Combine(settings.Destination, newTargetDirName)) :
            Path.GetFullPath(Path.Combine(settings.Destination, newTargetDirName, dateString));

        if (!string.IsNullOrEmpty(subDir) && !newTargetDirName.Equals(subDir, StringComparison.OrdinalIgnoreCase)) newFilePath = Path.Combine(newFilePath, subDir);

        if (newFilePath == null || !newFilePath.StartsWith(settings.Destination, StringComparison.OrdinalIgnoreCase))
        {
            if (Debugger.IsAttached)
            {
                Debug.WriteLine($"newFilePath null or does not start with the destination.");
                Debugger.Break();
            }

            parent.Increment(ResultCounts.CountKeys.Error);
            outputHandler($"{(isRetry ? retryText : "")}Error: Invalid target file path:\"{newFilePath ?? "null"}\"", MessageCode.Error);
            return null;
        }

        return newFilePath;
    }

    private string DirectoryNameFromDate(DateTime date, bool isRetry, ResultCounts parent, out string newTargetDirName)
    {
        newTargetDirName = string.Empty;

        if (date > notBeforeDate)
        {
            string year = $"{date:yyyy}";
            string month = $"{date:MM}";
            string day = $"{date:dd}";
            string hour = $"{date:HH}";

            newTargetDirName = settings.Pattern.Replace("$y", year, StringComparison.OrdinalIgnoreCase)
                .Replace("$m", month)
                .Replace("$d", day, StringComparison.OrdinalIgnoreCase)
                .Replace("$h", hour, StringComparison.OrdinalIgnoreCase)
                .Replace("$M", MonthName(date.Month));
        }

        string newFilePath = string.IsNullOrEmpty(newTargetDirName) ? 
            Path.GetFullPath(Path.Combine(settings.Destination)) :
            Path.GetFullPath(Path.Combine(settings.Destination, newTargetDirName));

        if (newFilePath == null || !newFilePath.StartsWith(settings.Destination, StringComparison.OrdinalIgnoreCase))
        {
            if (Debugger.IsAttached)
            {
                Debug.WriteLine($"newFilePath null or does not start with the destination.");
                Debugger.Break();
            }

            parent.Increment(ResultCounts.CountKeys.Error);
            outputHandler($"{(isRetry ? retryText : "")}Error: Invalid target file path:\"{newFilePath ?? "null"}\"", MessageCode.Error);
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
                    outputHandler($"Error: Could not deserialize object {jarray}. Reason=InvalidObject", MessageCode.Error);
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
            HandleExceptions(ex, parent, $"Could not get date senders media: {text}");
        }
    }

    private void GetDateFromUnixTimestamp(double timestamp, IDictionary<string, DateTime> dates, ResultCounts parent)
    {
        try
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            DateTime date = origin.AddSeconds(timestamp);
            if (date > notBeforeDate)
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

    private void ProcessEntry(string entryKey, bool isRetry, ResultCounts parent, ConcurrentDictionary<string, TakeoutInfo> mediaKeys, ConcurrentQueue<TakeoutInfo> retryMediaKeys)
    {
        if (IsCanceled) return;

        DataType dataType = GetDataType(entryKey);

        bool foundEntry = mediaKeys.TryGetValue(entryKey, out TakeoutInfo mediaInfo);

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
                outputHandler($"{(isRetry ? retryText : "")}Error: Invalid zip entry key: {entryKey}", MessageCode.Error);
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

                    if (dataType == DataType.Locked && !settings.KeepLocked)
                    {
                        // skip locked files
                        if (settings.Logging != LoggingVerbosity.Quiet)
                        {
                            outputHandler($"{(isRetry ? retryText : "")}Skipping locked file: {entry.Name}", MessageCode.Warning);
                        }
                        parent.Increment(ResultCounts.CountKeys.Skip);
                        return;
                    }

                    bool isMediaType = dataType == DataType.Photos || dataType == DataType.Locked;

                    if (isMediaType)
                    {
                        // 1. get date senders media metadata
                        string fileType = GetDateFromMediaStream(zipStream, dates, parent);

                        if (fileType == null)
                        {
                            if (Debugger.IsAttached)
                            {
                                Debug.WriteLine($"Invalid media file type: {fileType ?? "null"}");
                                Debugger.Break();
                            }

                            parent.Increment(ResultCounts.CountKeys.Error);
                            outputHandler($"{(isRetry ? retryText : "")}Error: Invalid media file type", MessageCode.Error);
                            return;
                        }
                    }

                    if (mediaInfo.MetaJson != null)
                    {
                        // 2. get date senders archive meta data
                        GetDateFromTakeoutMetadata(mediaInfo.MetaJson, dates, parent);
                    }

                    // 3. get date senders entry name
                    TryGetDateFromMediaFileName(entry.FullName, dates);

                    DateTime mediaDate = GetBestDate(dates, out bool isBest);

                    string targetMediaDirName = null;
                    string targetMediaDir = DirectoryNameFromDataType(mediaDate, dataType, entry.FullName, isRetry, parent, out targetMediaDirName);

                    string targetMediaFile = Path.GetFileName(entry.FullName);
                    string targetMediaPath = Path.Combine(targetMediaDir, targetMediaFile);

                    originalFileNameToDate[targetMediaFile] = mediaDate;

                    string prefix = settings.ListOnly ? "Would " : string.Empty;
                    string verb = $"{prefix}Copy";

                    if (settings.ListOnly)
                    {
                        if (settings.Logging != LoggingVerbosity.Quiet)
                        {
                            outputHandler($"{(isRetry ? retryText : "")}{verb} file:\"{entry.Name}\" to \"{Path.Combine(".", targetMediaDirName)}\"");
                        }

                        return;
                    }

                    System.IO.Directory.CreateDirectory(targetMediaDir);

                    string originalExtension = Path.GetExtension(entry.Name);
                    string outputTargetPath = targetMediaPath;
                    if (dataType == DataType.Locked)
                    {
                        string targetFile = Path.GetFileName(outputTargetPath);
                        string targetDir = Path.GetDirectoryName(outputTargetPath) ?? string.Empty;
                        string newTargetFile = Path.GetFileNameWithoutExtension(targetFile) + ".twlock";
                        outputTargetPath = Path.Combine(targetDir, newTargetFile);
                    }

                    bool alreadyExists = false;
                    if (dataType != DataType.Locked)
                    {
                        alreadyExists = ResolveFileConflict(isRetry, parent, entry.Length, ref outputTargetPath);
                    }

                    try
                    {
                        if (alreadyExists)
                        {
                            // already exists
                            // the files are the same - the entry is a duplicate
                            string text = settings.ListOnly ? "List(duplicate)" : "Duplicate";
                            if (settings.Logging == LoggingVerbosity.Verbose) outputHandler($"{(isRetry ? retryText : "")}{text}:\"{entry.Name}\" in \"{targetMediaDirName}\"");
                            parent.Increment(ResultCounts.CountKeys.Duplicate);
                        }
                        else
                        {
                            // the file does not exist - copy the entry to the file
                            const int mb = 1024 * 1024;
                            int bufferSize = entry.Length >= mb ? mb : Convert.ToInt32(entry.Length);

                            if (bufferSize > 0)
                            {
                                bool setFileDate = true;
                                switch (dataType)
                                {
                                    case DataType.Mail:
                                    {
                                        setFileDate = false; // do not set the file date for mail files
                                        zipStream.Seek(0, SeekOrigin.Begin);
                                        ReadMBoxFile(zipStream, isRetry, parent, targetMediaPath);
                                        break;
                                    }

                                    default:
                                    {
                                        using (FileStream fileStream = new FileStream(outputTargetPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, true))
                                        {
                                            zipStream.Seek(0, SeekOrigin.Begin);
                                            switch (dataType)
                                            {
                                                case DataType.Locked:
                                                {
                                                    FileEncryption.EncryptFile(zipStream, fileStream, password, originalExtension, mediaDate);
                                                    break;
                                                }

                                                default:
                                                {
                                                    // no special handling for other data types
                                                    zipStream.CopyTo(fileStream, bufferSize);
                                                    break;
                                                }
                                            }
                                        }

                                        break;
                                    }
                                }

                                if (setFileDate && mediaDate > notBeforeDate) File.SetCreationTime(outputTargetPath, mediaDate);

                                if (settings.Logging != LoggingVerbosity.Quiet)
                                {
                                    outputHandler($"{(isRetry ? retryText : "")}{verb} file:\"{entry.Name}\" to \"{Path.Combine(".", targetMediaDirName)}\"");
                                }

                                parent.Increment(ResultCounts.CountKeys.Change);
                            }
                            else
                            {
                                // zero byte content - create empty file
                                using (FileStream fileStream = new FileStream(outputTargetPath, FileMode.Create, FileAccess.Write, FileShare.None, 128, true))
                                {
                                }

                                if (mediaDate > notBeforeDate) File.SetCreationTime(outputTargetPath, mediaDate);
                                if (settings.Logging != LoggingVerbosity.Quiet)
                                {
                                    outputHandler($"{(isRetry ? retryText : "")}{verb} empty file:\"{entry.Name}\" to \"{Path.Combine(".", targetMediaDirName)}\"");
                                }

                                parent.Increment(ResultCounts.CountKeys.Change);
                            }
                        }
                    }
                    catch (IOException ioex)
                    {
                        if (ioex.HResult == -2147024864)
                        {
                            retryMediaKeys.Enqueue(mediaInfo);
                            return;
                        }

                        HandleExceptions(ioex, parent, $"Could not create {outputTargetPath}");
                    }
                    catch (Exception ex)
                    {
                        HandleExceptions(ex, parent, $"Could not create {outputTargetPath}");
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
            statusUpdate();
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
            HandleExceptions(ex, parent, $"Could not validate settings.Pattern {text}");
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

        string subDir = Path.Combine(settings.Backup, $"TOWBackup-{DateString(utcNow)}");
        string rtext = settings.Source + "\\";

        outputHandler($"Creating backup of media directory: {settings.Source} to {subDir}");

        ResultCounts results = new ResultCounts(parent, "Backup");
        int total = filesToBackup.Count;
        results.Set(ResultCounts.CountKeys.Total, total);
        statusUpdate(StatusCode.Total, total, "(backup)");

        if (settings.Parallel)
        {
            cancel = new CancellationTokenSource();
            ParallelOptions parallelOptions = new ParallelOptions();
            parallelOptions.CancellationToken = cancel.Token;

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
                    if (isAdmin)
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

            outputHandler($"Backing up \"{newFnameInSubDir}\".");
            File.Copy(oldFile, newFile);
        }
        catch (Exception ex)
        {
            HandleExceptions(ex, parent, ex.Message);
        }
        finally
        {
            parent.Increment(ResultCounts.CountKeys.Progress);
            statusUpdate();
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

    public void ReadMBoxFile(Stream stream, bool isRetry, ResultCounts parent, string outputTargetPath)
    {
        HashSet<string> labelsToIgnore = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "unread",
            "important",
            "opened"
        };

        string mailRoot = Path.GetDirectoryName(outputTargetPath) ?? string.Empty;
        int count = 0;

        try
        {
            MimeParser parser = new MimeParser(stream, MimeFormat.Mbox);

            foreach (MimeMessage entity in parser)
            {
                HashSet<string> mailFolders = null;
                Header[] headers = entity.Headers.Where(h => h.Field.Equals("x-gmail-labels", StringComparison.OrdinalIgnoreCase)).ToArray();
                if (headers.Length > 0)
                {
                    // skip category labels
                    string[] labels = headers[0].Value.Trim().Split(',', StringSplitOptions.TrimEntries).Where(l => !l.StartsWith("category", StringComparison.OrdinalIgnoreCase)).ToArray();
                    mailFolders = labels.Where(l => !labelsToIgnore.Contains(l)).ToHashSet(StringComparer.OrdinalIgnoreCase);
                }

                string dateString = $"{entity.Date:yyyy}{entity.Date:MM}{entity.Date:dd}T{entity.Date:HH}{entity.Date:mm}{entity.Date:ss}";

                if (mailFolders == null || mailFolders.Count == 0) mailFolders = ["Inbox"];

                ICollection<string> addresses = GetSenders(entity);

                if (mailFolders.Contains("archived"))
                {
                    mailFolders.Remove("archived");

                    List<string> folders = mailFolders.ToList();
                    folders.Insert(0, "Archived");

                    if (folders.Count == 1)
                    {
                        folders.Add("Inbox");
                    }

                    mailFolders.Add(string.Join("/", folders));
                }

                foreach (string mailFolder in mailFolders)
                {
                    if (IsCanceled) return;
                    string folder = mailFolder.Replace('/', '\\');

                    if (folder.Equals("trash", StringComparison.OrdinalIgnoreCase))
                    {
                        // skip trash folder
                        if (!settings.KeepTrash) continue;
                    }
                    else if (folder.Equals("spam", StringComparison.OrdinalIgnoreCase))
                    {
                        // skip spam folder
                        if (!settings.KeepSpam) continue;
                    }
                    else if (folder.Equals("sent", StringComparison.OrdinalIgnoreCase))
                    {
                        // skip sent folder
                        if (!settings.KeepSent) continue;

                        addresses = GetRecipients(entity);
                    }
                    else if (folder.StartsWith("archived", StringComparison.OrdinalIgnoreCase))
                    {
                        // skip archived folder
                        if (!settings.KeepArchived) continue;

                        archivedMailCount++;
                    }

                    foreach (string address in addresses)
                    {
                        // folder/values-date.eml (e.g. "inbox/johndoe@deere.com-20240101T120000.eml")
                        string fileName = $"{address}-{dateString}.eml";

                        string targetDir = Path.Combine(mailRoot, folder);
                        System.IO.Directory.CreateDirectory(targetDir);

                        if (settings.Logging != LoggingVerbosity.Quiet) outputHandler($"{(isRetry ? retryText : "")}mail:\"{folder}\\{fileName}\"");
                        statusUpdate(StatusCode.Progress, 1, $"(mail:{++count})");

                        string fullName = Path.Combine(targetDir, fileName);
                        entity.WriteTo(fullName);
                    }
                }

                if (IsCanceled) return;
            }
        }
        catch (Exception ex)
        {
            HandleExceptions(ex, parent, $"Error reading mbox file");
            if (Debugger.IsAttached) { Debugger.Break(); }
        }
        finally
        {
            statusUpdate(StatusCode.Progress);
        }
    }

    private ICollection<string> GetRecipients(MimeMessage entity)
    {
        HashSet<string> addresses = new HashSet<string>(StringComparer.Ordinal);

        if (replaceChars == null)
        {
            replaceChars = new HashSet<char>(Path.GetInvalidFileNameChars());
        }

        if (entity.Sender != null)
        {
            string[] values = entity.GetRecipients(true).Select(r => r.Address).ToArray();
            foreach (string recipient in values)
            {
                if (recipient.Contains('@'))
                {
                    addresses.Add(recipient.Trim());
                }
            }
        }

        Header[] headers = entity.Headers.Where(h => h.Field.Equals("to", StringComparison.OrdinalIgnoreCase)).ToArray();
        if (headers.Length > 0)
        {
            string[] values = headers[0].Value.Trim().Split(',', StringSplitOptions.TrimEntries);
            foreach (string recipient in values)
            {
                if (recipient.Contains('@'))
                {
                    addresses.Add(recipient.Trim());
                }
            }
        }

        string[] recipients = addresses.ToArray();

        for (int ii = 0; ii < recipients.Length; ii++)
        {
            recipients[ii] = NormalizeAddress(recipients[ii]);
        }

        return recipients;
    }

    private string NormalizeAddress(string address)
    {
        string[] addressParts = address.Split(['>', '<'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        string addressPart = string.Empty;
        string namePart = string.Empty;

        foreach (string part in addressParts)
        {
            if (part.Contains('@'))
            {
                addressPart = part.Trim();
            }
            else
            {
                namePart = part.Trim();
            }
        }

        string domain = string.Empty;
        int atIndex = addressPart.IndexOf('@');
        if (atIndex > 0)
        {
            string name = addressPart.Substring(0, atIndex).Trim();
            if (namePart.Length == 0 && name.Length > 0)
            {
                namePart = name;
            }

            string fullDomain = addressPart.Substring(atIndex + 1).Trim();
            if (fullDomain != null && fullDomain.Length > 0)
            {
                string[] parts = fullDomain.Split('.', StringSplitOptions.TrimEntries);

                if (parts.Length > 1)
                {
                    // remove the domain part
                    domain = parts[parts.Length - 2];
                }
            }
        }
        else
        {
            if (Debugger.IsAttached) { Debugger.Break(); }
        }

        if (namePart.Length > 0)
        {
            StringBuilder sbName = new StringBuilder();
            foreach (char ch in namePart)
            {
                if (!char.IsLetterOrDigit(ch)) continue;
                sbName.Append(ch);
            }

            namePart = sbName.ToString();
        }

        return $"{namePart}@{domain}";
    }

    private HashSet<char> replaceChars = null;

    private ICollection<string> GetSenders(MimeMessage entity)
    {
        HashSet<string> addresses = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (replaceChars == null)
        {
            replaceChars = new HashSet<char>(Path.GetInvalidFileNameChars());
        }

        if (entity.Sender != null)
        {
            string sender = entity.Sender.ToString().Trim();
            if (sender.Contains('@')) addresses.Add(sender);
        }
        
        if (entity.From.Count > 0)
        {
            foreach (InternetAddress address in entity.From)
            {
                if (address is MailboxAddress mailboxAddress)
                {
                    // use the mailbox address
                    string sender = mailboxAddress.Address.Trim();
                    if (!sender.Contains('@')) continue;

                    addresses.Add(mailboxAddress.Address);
                }
            }
        }

        Header[] headers = entity.Headers.Where(h => h.Field.Equals("senders", StringComparison.OrdinalIgnoreCase)).ToArray();
        foreach (Header header in headers)
        {
            string sender = header.Value.Trim();
            if (!sender.Contains('@')) continue;

            addresses.Add(header.Value.Trim());
        }

        string[] all = addresses.ToArray();
        foreach (string sender in all)
        {
            if (addresses.Count > 1 && !sender.Contains('<'))
            {
                addresses.Remove(sender);
            }
        }

        List<string> senders = new List<string>();
        for (int ii = 0; ii < addresses.Count; ii++)
        {
            senders.Add(NormalizeAddress(all[ii]));
        }

        return senders;
    }

    public Settings GetSettings(Configs configs)
    {
        Settings newSettings = new Settings();

        configs.TryGetString("source", out string sourceDir);
        configs.TryGetString("destination", out string destinationDir);
        configs.TryGetString("backup", out string backup);
        configs.TryGetString("pattern", out string pattern);
        configs.TryGetString("filter", out string fileFilter);
        configs.TryGetString("logging", out string loggingString);
        configs.TryGetString("action", out string actionString);
        configs.TryGetBool("listonly", out bool listOnly);
        configs.TryGetBool("parallel", out bool parallel);
        configs.TryGetString("junk", out string junk);
        configs.TryGetBool("keeplocked", out bool keepLocked);
        configs.TryGetBool("keepTrash", out bool keepTrash);
        configs.TryGetBool("keepSpam", out bool keepSpam);
        configs.TryGetBool("keepSent", out bool keepSent);
        configs.TryGetBool("keepArchived", out bool keepArchived);
        configs.TryGetBool("purgeGmailArchive", out bool purgeArchived);

        if (!Enum.TryParse(actionString, true, out PhotoCopierActions behavior)) behavior = PhotoCopierActions.Copy;
        if (!Enum.TryParse(loggingString, true, out LoggingVerbosity logging)) logging = LoggingVerbosity.Verbose;

        newSettings.Source = sourceDir;
        newSettings.Destination = destinationDir;
        newSettings.Backup = backup;
        newSettings.Pattern = pattern;
        newSettings.Filter = fileFilter;
        newSettings.Logging = logging;
        newSettings.Behavior = behavior;
        newSettings.ListOnly = listOnly;
        newSettings.Parallel = parallel;
        newSettings.KeepLocked = keepLocked;
        newSettings.JunkExtensions = junk;
        newSettings.KeepTrash = keepTrash;
        newSettings.KeepSpam = keepSpam;
        newSettings.KeepSent = keepSent;
        newSettings.KeepArchived = keepArchived;

        return newSettings;
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

public enum DataType
{
    Unknown,
    AccessLogActivity,
    Account,
    Alerts,
    ArchiveBrowser,
    Assignments,
    Blogger,
    BusinessProfile,
    Calendar,
    Chat,
    Chrome,
    Contacts,
    DeviceConfigurationService,
    Discover,
    Drive,
    Feedback,
    Finance,
    Fit,
    FitBit,
    Gemini,
    GooglePay,
    Groups,
    HelpCommunities,
    HomeApp,
    Keep,
    Locked,
    Mail,
    Maps,
    MapsYourPlaces,
    Meet,
    MyActivity,
    News,
    Photos,
    PlayBooks,
    PlayMoviesTV,
    PlayStore,
    Profile,
    Recorder,
    Saved,
    SearchNotifications,
    Shopping,
    Store,
    Tasks,
    TimeLine,
    Voice,
    WorkspaceMarketplace,
    YouTube,
}
