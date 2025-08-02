//  <@$&< copyright begin >&$@> 24FE144C2255E2F7CCB65514965434A807AE8998C9C4D01902A628F980431C98:20241017.A:2025:7:24:7:12
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
