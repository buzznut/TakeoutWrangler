
//  <@$&< copyright begin >&$@> 24FE144C2255E2F7CCB65514965434A807AE8998C9C4D01902A628F980431C98:20241017.A:2025:7:24:7:12
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024-2025 Stewart A. Nutter - All Rights Reserved.
// No warranty is implied or given.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// <@$&< copyright end >&$@>

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
    public bool KeepInbox { get; set; } = true;
    public bool KeepOther { get; set; } = true;
    public bool DoMail { get; set; } = true;
    public bool DoMedia { get; set; } = true;
    public bool DoOther { get; set; } = true;
}
