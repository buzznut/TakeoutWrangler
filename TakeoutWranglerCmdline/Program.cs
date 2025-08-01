//  <@$&< copyright begin >&$@> 24FE144C2255E2F7CCB65514965434A807AE8998C9C4D01902A628F980431C98:20241017.A:2025:7:24:7:12
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024-2025 Stewart A. Nutter - All Rights Reserved.
// No warranty is implied or given.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// <@$&< copyright end >&$@>

using CommonLibrary;
using PhotoCopyLibrary;
using System;
using System.Security.Cryptography;

namespace TakeoutWranglerCmdline;

internal static class Program
{
    static int Main(string[] args)
    {
        PhotoCopier photoCopier = new PhotoCopier(OutputHandler, StatusHandler);
        Configs configs = photoCopier.GetConfiguration();

        configs.LoadAppSettings();
        configs.ParseArgs(args);

        Settings settings = photoCopier.GetSettings(configs);
        configs.TryGetString("password", out string password);
        if (password != null && password.Length < 6)
        {
            password = null;
        }

        ReturnCode result = photoCopier.Initialize(
            nameof(TakeoutWranglerCmdline),
            false,
            settings,
            password);

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
