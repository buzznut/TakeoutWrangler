
namespace PhotoCopyLibrary;

public class Settings
{
    public PhotoCopierActions Behavior { get; set; } = PhotoCopierActions.Copy;
    public string Source { get; set; }
    public string Destination { get; set; }
    public string Backup { get; set; }
    public string Pattern { get; set; } = "$y_$m";
    public string Filter { get; set; } = "takeout-*.zip";
    public LoggingVerbosity Logging { get; set; } = LoggingVerbosity.Verbose;
    public bool ListOnly { get; set; }
    public bool Parallel { get; set; }
    public bool KeepLocked { get; set; }
    public string JunkExtensions { get; set; }
    public bool KeepSpam { get; set; }
    public bool KeepTrash { get; set; }
    public bool KeepSent { get; set; } = true;
    public bool KeepArchived { get; set; }
}
