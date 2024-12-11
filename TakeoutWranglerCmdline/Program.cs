//  <@$&< copyright begin >&$@> D50225522CB19A3A2E3CA10257DC538D19677A6406D028F0BBE01DE33387A4EA:20241017.A:2024:11:16:13:40
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024 Stewart A. Nutter - All Rights Reserved.
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

        configs.GetBool("help", out bool showHelp);
        configs.GetString("source", out string sourceDir);
        configs.GetString("destination", out string destinationDir);
        configs.GetString("pattern", out string pattern);
        configs.GetString("filter", out string fileFilter);
        configs.GetString("logging", out string loggingString);
        configs.GetString("action", out string actionString);
        configs.GetBool("listonly", out bool listOnly);

        if (!Enum.TryParse(actionString, true, out PhotoCopierActions behavior)) behavior = PhotoCopierActions.Copy;
        if (!Enum.TryParse(loggingString, true, out LoggingVerbosity logging)) logging = LoggingVerbosity.Verbose;

        int result = photoCopier.Initialize(nameof(TakeoutWranglerCmdline), showHelp, sourceDir, destinationDir, behavior, pattern, fileFilter, logging, listOnly);
        if (result != (int)ReturnCode.Success) return result;

        return (int)photoCopier.RunAsync().GetAwaiter().GetResult();
    }

    private static void OutputHandler(string output)
    {
        if (output != null)
            Console.WriteLine(output);
        else
            Console.WriteLine();
    }

    private static void StatusHandler(string output)
    {
    }
}
