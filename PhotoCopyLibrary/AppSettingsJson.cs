//  <@$&< copyright begin >&$@> 24FE144C2255E2F7CCB65514965434A807AE8998C9C4D01902A628F980431C98:20241017.A:2025:7:1:14:38
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024-2025 Stewart A. Nutter - All Rights Reserved.
// No warranty is implied or given.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// <@$&< copyright end >&$@>

using Newtonsoft.Json;
using System.Diagnostics;

namespace PhotoCopyLibrary;

public class AppSettingsJson
{
    private readonly string filePath;
    private readonly dynamic jsonObj;
    private string originalJsonText;
    private readonly string root;

    private AppSettingsJson() { }

    public AppSettingsJson(string settingsFilePath, string keyRoot)
    {
        if (!File.Exists(settingsFilePath))
        {
            return;
        }

        root = keyRoot;
        filePath = settingsFilePath;
        originalJsonText = File.ReadAllText(filePath);
        JsonSerializerSettings serializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, EqualityComparer = StringComparer.OrdinalIgnoreCase };
        jsonObj = JsonConvert.DeserializeObject(originalJsonText, serializerSettings);
    }

    public string FilePath { get { return filePath; } }

    public bool IsDirty()
    {
        try
        {
            string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            return string.Compare(output, originalJsonText, StringComparison.Ordinal) != 0;
        }
        catch (Exception)
        {
            if (Debugger.IsAttached) { Debugger.Break(); }
            throw;
        }
    }

    public bool Save()
    {
        try
        {
            string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            if (string.Compare(output, originalJsonText, StringComparison.OrdinalIgnoreCase) != 0)
            {
                File.WriteAllText(filePath, output);
                originalJsonText = output;
            }
            return true;
        }
        catch (Exception)
        {
            if (Debugger.IsAttached) { Debugger.Break(); }
            return false;
        }
    }

    public void AddOrUpdateSetting<T>(string sectionPathKey, T value)
    {
        if (jsonObj == null) throw new NullReferenceException(nameof(jsonObj));
        if (string.IsNullOrWhiteSpace(sectionPathKey)) throw new ArgumentNullException(nameof(sectionPathKey));

        try
        {
            SetValueRecursively($"{root}:{sectionPathKey}", jsonObj, value);
        }
        catch (Exception)
        {
            if (Debugger.IsAttached) { Debugger.Break(); }
            throw;
        }
    }

    public T GetSetting<T>(string sectionPathKey, T defaultValue = default)
    {
        if (jsonObj == null) throw new NullReferenceException(nameof(jsonObj));
        if (string.IsNullOrWhiteSpace(sectionPathKey)) throw new ArgumentNullException(nameof(sectionPathKey));

        object result = GetValueRecursively<T>($"{root}:{sectionPathKey}", jsonObj, defaultValue);
        if (result == null || result.Equals(default))
        {
            result = defaultValue;
        }

        return (T)result;
    }

    private void SetValueRecursively<T>(string sectionPathKey, dynamic currentObj, T value)
    {
        // split the string at the first ':' character
        var remainingSections = sectionPathKey.Split(":", 2);

        var currentSection = remainingSections[0];
        if (remainingSections.Length > 1)
        {
            // continue with the procress, moving down the tree
            var nextSection = remainingSections[1];

            var nextObj = currentObj[currentSection];
            SetValueRecursively(nextSection, nextObj, value);
        }
        else
        {
            // we've got to the end of the tree, set the value
            currentObj[currentSection] = value;
        }
    }

    private T GetValueRecursively<T>(string sectionPathKey, dynamic currentObj, T defaultValue)
    {
        // split the string at the first ':' character
        string[] remainingSections = sectionPathKey.Split(":", 2);

        string currentSection = remainingSections[0];
        if (remainingSections.Length > 1)
        {
            // continue with the procress, moving down the tree
            string nextSection = remainingSections[1];
            var nextObj = currentObj[currentSection];
            return GetValueRecursively(nextSection, nextObj, defaultValue);
        }

        try
        {
            // we've got to the end of the tree, return the value
            return (T)currentObj[currentSection];
        }
        catch
        {
        }

        return defaultValue;
    }
}
