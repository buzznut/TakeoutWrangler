//  <@$&< copyright begin >&$@> 24FE144C2255E2F7CCB65514965434A807AE8998C9C4D01902A628F980431C98:20241017.A:2025:2:25:8:47
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024-2025 Stewart A. Nutter - All Rights Reserved.
// No warranty is implied or given.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// <@$&< copyright end >&$@>

using PhotoCopyLibrary;
using System;

namespace TakeoutWranglerCmdline;

internal static class Program
{
    static int Main(string[] args)
    {
        PhotoCopier photoCopier = new PhotoCopier(OutputHandler, StatusHandler);
        Configs configs = photoCopier.GetConfiguration();

        configs.LoadAppSettings();
        configs.ParseArgs(args);

        configs.TryGetBool("help", out bool showHelp);
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

        if (!Enum.TryParse(actionString, true, out PhotoCopierActions behavior)) behavior = PhotoCopierActions.Copy;
        if (!Enum.TryParse(loggingString, true, out LoggingVerbosity logging)) logging = LoggingVerbosity.Verbose;

        ReturnCode result = photoCopier.Initialize(nameof(TakeoutWranglerCmdline), showHelp, sourceDir, destinationDir, backup, behavior, pattern, fileFilter, logging, listOnly, parallel, junk);
        if (result != ReturnCode.Success) return (int)result;

        return (int)photoCopier.Run();
    }

    private static void OutputHandler(string output = null, MessageCode errorCode = MessageCode.Success)
    {
        if (output != null)
        {
            if (errorCode != MessageCode.Success)
                Console.Error.WriteLine(output);
            else
                Console.WriteLine(output);
        }
        else
            Console.WriteLine();
    }

    private static void StatusHandler(StatusCode statusCode, int value, string progressType)
    {
    }
}
