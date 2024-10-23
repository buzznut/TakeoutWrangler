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

using Newtonsoft.Json;

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
            return output != originalJsonText;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public bool Save()
    {
        try
        {
            string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            if (output != originalJsonText)
            {
                File.WriteAllText(filePath, output);
                originalJsonText = output;
            }
            return true;
        }
        catch (Exception ex)
        {
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
        catch (Exception ex)
        {
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
