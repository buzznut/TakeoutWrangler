//  <@$&< copyright begin >&$@> 8EF3F3608034F1A9CC6F945BA1A2053665BCA4FFC65BF31743F47CE665FDB0FB:20241017.A:2024:10:17:18:28
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024 Stewart A. Nutter - All Rights Reserved.
// 
// This software application and source code is copyrighted and is licensed
// for use by you only. Only this product's installation files may be shared.
// 
// This license does not allow the removal or code changes that cause the
// ignoring, or modifying the copyright in any form.
// 
// This software is licensed "as is" and no warranty is implied or given.
// 
// Stewart A. Nutter
// 711 Indigo Ln
// Waunakee, WI  53597
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// <@$&< copyright end >&$@>

using System;
using System.Collections.Generic;
using PhotoCopyLibrary;

// PixExpress
// PixGuard
// PixPreserve
// PicturePreserve
// MyMemories
// AllMyPhotos
// MyPhotoGuardian
// PhotoWrangler

namespace PhotoExtractApp;

internal static class Program
{
    static int Main(string[] args)
    {
        PhotoCopier photoCopier = new PhotoCopier(OutputHandler);
        Configs configs = photoCopier.GetConfiguration();

        configs.LoadAppSettings();
        configs.ParseArgs(args);

        configs.GetBool("help", out bool showHelp);
        configs.GetString("source", out string sourceDir);
        configs.GetString("destination", out string destinationDir);
        configs.GetString("action", out string actionText);
        configs.GetString("pattern", out string pattern);
        configs.GetString("speed", out string speedText);
        configs.GetString("filter", out string fileFilter);
        configs.GetBool("quiet", out bool quiet);

        int result = photoCopier.Initialize(nameof(PhotoExtractApp), showHelp, sourceDir, destinationDir, actionText, pattern, fileFilter, quiet);
        if (result != 0) return result;

        return photoCopier.RunAsync().GetAwaiter().GetResult();
    }

    private static void OutputHandler(string output)
    {
        if (output != null)
            Console.WriteLine(output);
        else
            Console.WriteLine();
    }
}
