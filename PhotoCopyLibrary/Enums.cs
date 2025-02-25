//  <@$&< copyright begin >&$@> 24FE144C2255E2F7CCB65514965434A807AE8998C9C4D01902A628F980431C98:20241017.A:2025:2:25:8:47
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024-2025 Stewart A. Nutter - All Rights Reserved.
// No warranty is implied or given.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// <@$&< copyright end >&$@>

namespace PhotoCopyLibrary;

public enum PhotoCopierActions
{
    Copy,
    Reorder,
}

public enum LoggingVerbosity
{
    Quiet,
    Change,
    Verbose
}

public enum ReturnCode
{
    Success,
    HadIssues,
    DirectoryError,
    Error,
    Canceled
}

public enum MessageCode
{
    Success,
    Error,
    Warning
}

public enum StatusCode
{
    Progress,
    Total,
    More
}
