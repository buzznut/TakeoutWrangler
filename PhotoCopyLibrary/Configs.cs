//  <@$&< copyright begin >&$@> D50225522CB19A3A2E3CA10257DC538D19677A6406D028F0BBE01DE33387A4EA:20241017.A:2024:12:23:9:15
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024 Stewart A. Nutter - All Rights Reserved.
// No warranty is implied or given.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// <@$&< copyright end >&$@>

namespace PhotoCopyLibrary;

public enum ConfigResult
{
    InvalidKey,
    Found,
    UsedDefault
}

/// <summary>
/// Read various types of configuration values. Supports
/// appsettings.json, and commandline arguments.
/// </summary>
public class Configs
{
    private readonly Dictionary<string, ConfigParam> pTypes = new Dictionary<string, ConfigParam>(StringComparer.OrdinalIgnoreCase);
    private ConfigParam pHelp;
    private readonly Dictionary<string, object> values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
    private readonly bool throwIfNotDefined;
    private bool isDirty;

    public Configs(bool throwIfNotDefined = true)
    {
        this.throwIfNotDefined = throwIfNotDefined;
    }

    public Configs(ICollection<ConfigParam> configParams, bool throwIfNotDefined = true) : this(throwIfNotDefined)
    {
        foreach (ConfigParam configParam in configParams)
        {
            Add(configParam);
        }

        if (pHelp == null)
        {
            pTypes["help"] = new ConfigParam { Name = "help", Synonyms = ["?"], Default = false, PType = ParamType.Bool };
            pHelp = pTypes["help"];
        }
    }

    public void Add(ConfigParam configParam)
    {
        switch (configParam.PType)
        {
            case ParamType.String:
                if (configParam.Default != null && configParam.Default is not string) throw new ArgumentException($"Config param type mismatch:{configParam.Name}");
                break;
            case ParamType.Bool:
                if (configParam.Default is not bool) throw new ArgumentException($"Config param type mismatch:{configParam.Name}");
                break;
            case ParamType.Int:
                if (configParam.Default is not int) throw new ArgumentException($"Config param type mismatch:{configParam.Name}");
                break;
        }

        if (configParam.Name.Equals("help", StringComparison.OrdinalIgnoreCase)) pHelp = configParam;
        foreach (string syn in configParam.Synonyms)
        {
            pTypes[syn] = configParam;
        }
    }

    public AppSettingsJson LoadAppSettings(string keyRoot = "Data:Settings")
    {
        string settingsFile;

        string appPath = AppHelpers.GetApplicationDir();
        if (string.IsNullOrEmpty(appPath)) throw new ApplicationException("Could not get application directory");

        settingsFile = Path.Combine(appPath, "appsettings.json");
        if (!File.Exists(settingsFile))
        {
            try
            {
                string fileName = Path.GetFileName(settingsFile);
                string defaultSettings = Path.Combine(appPath, $"default.{fileName}");
                if (!File.Exists(defaultSettings))
                {
                    // app settings file does not exist and neither does the default file
                    throw new ApplicationException($"Could not find {defaultSettings} file");
                }

                File.Copy(defaultSettings, fileName);
            }
            catch
            {
                // could not get access to the file
                throw;
            }
        }

        AppSettingsJson settings = new AppSettingsJson(settingsFile, keyRoot);
        ParseSettings(settings);
        return settings;
    }

    private void ParseSettings(AppSettingsJson settings)
    {
        // settings do not use the synonyms
        if (settings == null) return;

        const string notFoundText = "$(@)";
        foreach (ConfigParam pType in pTypes.Values)
        {
            string value = settings.GetSetting(pType.Name, notFoundText);
            if (string.Compare(value, notFoundText, StringComparison.Ordinal) != 0)
            {
                switch (pType.PType)
                {
                    case ParamType.String:
                    {
                        values[pType.Name] = value;
                        break;
                    }

                    case ParamType.Bool:
                    {
                        ParseBoolObject(value, out bool v);
                        values[pType.Name] = v;
                        break;
                    }

                    case ParamType.Int:
                    {
                        ParseIntObject(value, out int v);
                        values[pType.Name] = v;
                        break;
                    }
                }

                continue;
            }
            if (!values.ContainsKey(pType.Name)) values[pType.Name] = pType.Default;
        }
    }

    public void SaveSettings(AppSettingsJson settings)
    {
        if (!isDirty) return;
        if (settings == null) return;

        foreach (string key in values.Keys)
        {
            if (pTypes.TryGetValue(key, out ConfigParam pType))
            {
                switch (pType.PType)
                {
                    case ParamType.String:
                    {
                        settings.AddOrUpdateSetting(key, values[key] as string);
                        break;
                    }

                    case ParamType.Bool:
                    {
                        ParseBoolObject(values[key], out bool v);
                        settings.AddOrUpdateSetting(key, v);
                        break;
                    }

                    case ParamType.Int:
                    {
                        ParseIntObject(values[key], out int v);
                        settings.AddOrUpdateSetting(key, v);
                        break;
                    }
                }
            }
        }

        settings.Save();
        isDirty = false;
    }

    public void ParseArgs(string[] args)
    {
        // arg synonyms are okay
        if (args == null || args.Length == 0) return;

        foreach (string arg in args)
        {
            string a = arg.Trim();
            if (!a.StartsWith('-') && !a.StartsWith('/'))
            {
                throw new InvalidDataException($"Could not parse: '{arg}'");
            }

            int index = a.IndexOf('-');
            if (index < 0) index = arg.IndexOf('/');
            if (index < 0)
            {
                values["help"] = true;
                continue;
            }

            string[] parts = arg.Substring(index + 1).Split('=', 2);
            if (parts.Length == 0) continue;
            if (!pTypes.TryGetValue(parts[0], out ConfigParam pType)) continue;

            values[pType.Name] = parts.Length > 1 ? parts[1] : pType.Default;
        }
    }

    public bool SetBool(string key, bool value)
    {
        isDirty = true;
        if (!pTypes.ContainsKey(key) || pTypes[key].PType != ParamType.Bool) return false;
        values[key] = value;
        return true;
    }

    public bool SetInt(string key, int value)
    {
        isDirty = true;
        if (!pTypes.ContainsKey(key) || pTypes[key].PType != ParamType.Int) return false;
        values[key] = value;
        return true;
    }

    public bool SetString(string key, string value)
    {
        isDirty = true;
        if (!pTypes.ContainsKey(key) || pTypes[key].PType != ParamType.String) return false;
        values[key] = value;
        return true;
    }

    public ConfigResult GetBool(string key, out bool value)
    {
        values.TryGetValue(key, out object valueObject);
        pTypes.TryGetValue(key, out ConfigParam pType);

        if (pType == null)
        {
            if (throwIfNotDefined) throw new KeyNotFoundException($"Invalid key:{key}");
            value = false;
            return ConfigResult.InvalidKey;
        }

        if (ParseBoolObject(valueObject, out value))
        {
            return ConfigResult.Found;
        }

        ParseBoolObject(pType.Default, out value);
        return ConfigResult.UsedDefault;
    }

    private bool ParseBoolObject(object valueObject, out bool value)
    {
        if (valueObject is bool vb)
        {
            value = vb;
            return true;
        }

        if (valueObject is int vi)
        {
            value = vi != 0;
            return true;
        }

        if (valueObject is string vt)
        {
            char ch = vt.ToLower()[0];
            if (ch == 't' || ch == 'y')
            {
                value = true;
                return true;
            }

            if (ch == 'f' || ch == 'n')
            {
                value = false;
                return true;
            }
        }

        value = false;
        return false;
    }

    public ConfigResult GetInt(string key, out int value)
    {
        values.TryGetValue(key, out object valueObject);
        pTypes.TryGetValue(key, out ConfigParam pType);

        if (pType == null)
        {
            if (throwIfNotDefined) throw new KeyNotFoundException($"Invalid key:{key}");
            value = 0;
            return ConfigResult.InvalidKey;
        }

        if (ParseIntObject(valueObject, out value))
        {
            return ConfigResult.Found;
        }

        ParseIntObject(pType.Default, out value);
        return ConfigResult.UsedDefault;
    }

    private bool ParseIntObject(object valueObject, out int value)
    {
        if (valueObject is int)
        {
            value = (int)valueObject;
            return true;
        }

        if (valueObject is double)
        {
            value = Convert.ToInt32(Math.Truncate((double)valueObject));
            return true;
        }

        if (valueObject is string && int.TryParse((string)valueObject, out value))
        {
            return true;
        }

        value = 0;
        return false;
    }

    public ConfigResult GetString(string key, out string value)
    {
        bool gotValue = values.TryGetValue(key, out object valueObject);
        pTypes.TryGetValue(key, out ConfigParam pType);

        if (pType == null)
        {
            if (throwIfNotDefined) throw new KeyNotFoundException($"Invalid key:{key}");
            value = null;
            return ConfigResult.InvalidKey;
        }

        string valueText = valueObject?.ToString();
        string defaultText = null;
        if (pType.Default != null && pType.Default is string v)
        {
            defaultText = v;
        }

        value = valueText;

        if (!gotValue)
        {
            value = defaultText;
            return ConfigResult.UsedDefault;
        }

        return ConfigResult.Found;
    }

    public static bool ValidatePath(string dir, string context, List<string> reasons)
    {
        if (string.IsNullOrEmpty(dir))
        {
            reasons.Add($"{context} cannot be null or empty.");
            return false;
        }

        if (dir.Contains("..") || dir.Contains(".\\") || !dir.Equals(Path.GetFullPath(dir), StringComparison.OrdinalIgnoreCase))
        {
            reasons.Add($"{context} must be fully qualified and without relative pathing: (\"{dir}\".");
            return false;
        }

        if (!Directory.Exists(dir))
        {
            reasons.Add($"{context} must exist: \"{dir}\".");
            return false;
        }

        return true;
    }
}
