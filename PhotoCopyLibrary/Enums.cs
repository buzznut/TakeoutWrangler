//  <@$&< copyright begin >&$@> D50225522CB19A3A2E3CA10257DC538D19677A6406D028F0BBE01DE33387A4EA:20241017.A:2024:12:23:9:15
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024 Stewart A. Nutter - All Rights Reserved.
// No warranty is implied or given.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// <@$&< copyright end >&$@>

namespace PhotoCopyLibrary;

public enum PhotoCopierActions
{
    Copy,
    Overwrite,
    Reorder
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
    Finish
}
