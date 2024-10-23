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

using System.Diagnostics;
using System.Reflection;

namespace PhotoCopyLibrary;

public class AppHelpers
{
    private static readonly object lockObject = new object();

    private static volatile string dataPath;
    public static string GetAppDataPath()
    {
        lock (lockObject)
        {
            if (dataPath != null) return dataPath;
            dataPath ??= Path.Combine(LocalAppDataFolder, GetCompanyName(), GetApplicationName());
        }

        return dataPath;
    }

    private static volatile string appDataFolder;
    public static string LocalAppDataFolder
    {
        get
        {
            lock (lockObject)
            {
                if (appDataFolder != null) return appDataFolder;
                appDataFolder ??= Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            }

            return appDataFolder;
        }
    }

    private static volatile string applicationName;
    public static string GetApplicationName()
    {
        lock (lockObject)
        {
            if (applicationName != null) return applicationName;
            string name = null;

            // System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName
            using (Process process = Process.GetCurrentProcess())
            {
                if (process.MainModule != null)
                {
                    name = process.MainModule.FileName;
                }
            }

            if (name == null)
            {
                Assembly assembly = Assembly.GetEntryAssembly();
                if (assembly != null)
                {
                    name = Path.GetFileName(assembly.Location);
                }
            }

            applicationName = Path.GetFileNameWithoutExtension(name) ?? throw new NullReferenceException($"{nameof(GetApplicationName)}: Application name cannot be null");
        }

        return applicationName;
    }

    private static volatile string applicationFile;
    public static string GetApplicationFile()
    {
        lock (lockObject)
        {
            if (applicationFile != null) return applicationFile;
            string name = null;

            // System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName
            using (Process process = Process.GetCurrentProcess())
            {
                if (process.MainModule != null)
                {
                    name = process.MainModule.FileName;
                }
            }

            if (name == null)
            {
                Assembly assembly = Assembly.GetEntryAssembly();
                if (assembly != null)
                {
                    name = Path.GetFileName(assembly.Location);
                }
            }

            applicationFile = Path.GetFileName(name) ?? throw new NullReferenceException($"{nameof(GetApplicationFile)}: Application file cannot be null");
        }

        return applicationFile;
    }

    private static volatile string applicationDir;
    public static string GetApplicationDir()
    {
        lock (lockObject)
        {
            if (applicationDir != null) return applicationDir;
            string path = null;

            using (Process process = Process.GetCurrentProcess())
            {
                if (process.MainModule != null)
                {
                    path = process.MainModule.FileName;
                }
            }

            if (path == null)
            {
                Assembly assembly = Assembly.GetEntryAssembly();
                if (assembly != null)
                {
                    path = Path.GetFileName(assembly.Location);
                }
            }

            applicationDir = Path.GetDirectoryName(path) ?? throw new NullReferenceException($"{nameof(GetApplicationDir)}: Application dir cannot be null");
        }

        return applicationDir;
    }

    private static volatile string applicationPath;
    public static string GetApplicationPath()
    {
        lock (lockObject)
        {
            if (applicationPath != null) return applicationPath;
            string path = null;

            using (Process process = Process.GetCurrentProcess())
            {
                if (process.MainModule != null)
                {
                    path = process.MainModule.FileName;
                }
            }

            if (path == null)
            {
                Assembly assembly = Assembly.GetEntryAssembly();
                if (assembly != null)
                {
                    path = Path.GetFileName(assembly.Location);
                }
            }

            applicationPath = path ?? throw new NullReferenceException($"{nameof(GetApplicationPath)}: Application name cannot be null");
        }

        return applicationPath;
    }

    public static string GetCompanyName() => Constants.CompanyName;
}
