//  <@$&< copyright begin >&$@> D50225522CB19A3A2E3CA10257DC538D19677A6406D028F0BBE01DE33387A4EA:20241017.A:2024:12:23:9:15
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024 Stewart A. Nutter - All Rights Reserved.
// No warranty is implied or given.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// <@$&< copyright end >&$@>

using PhotoCopyLibrary;
using System.Text;

namespace TestLibrary;

[TestClass]
public class HelperTests
{

    [TestMethod]
    public void TestConfig()
    {
        List<ConfigParam> paramTypes = new List<ConfigParam>
        {
            new ConfigParam { Name = "foundString", Default="foundString", PType = ParamType.String },
            new ConfigParam { Name = "foundBool", Default=true, PType = ParamType.Bool },
            new ConfigParam { Name = "foundInt", Default=3976, PType = ParamType.Int }
        };

        Configs configs = new Configs(paramTypes, true);

        try
        {
            configs.Add(new ConfigParam { Name = "bad", Default = 1, PType = ParamType.String });
            Assert.Fail();
        }
        catch (ArgumentException ae)
        {
            var foo = ae;
        }

        Assert.IsTrue(configs.TryGetString("foundString", out string foundString));
        Assert.AreEqual("foundString", foundString);

        Assert.IsTrue(configs.TryGetBool("foundBool", out bool foundBool));
        Assert.AreEqual(true, foundBool);

        Assert.IsTrue(configs.TryGetInt("foundInt", out int foundInt));
        Assert.AreEqual(3976, foundInt);

        try
        {
            configs.TryGetString("notfound", out string notFoundText);
        }
        catch (KeyNotFoundException knfe)
        {
            Assert.AreEqual("Invalid key:notfound", knfe.Message);
        }
    }

    [TestMethod]
    public void TestSettings()
    {
        List<ConfigParam> paramTypes = new List<ConfigParam>
        {
            new ConfigParam { Name = "filter", Default="takeout-*.zip", PType = ParamType.String },
            new ConfigParam { Name = "source", Synonyms = ["src"], PType = ParamType.String },
            new ConfigParam { Name = "destination", Synonyms = ["dst", "dest", "tgt", "target"], PType = ParamType.String },
            new ConfigParam { Name = "help", Synonyms = ["?"], Default = false, PType = ParamType.Bool },
            new ConfigParam { Name = "action", Default = "list", PType = ParamType.String },
            new ConfigParam { Name = "pattern", Default = "$y_$m", PType = ParamType.String },
            new ConfigParam { Name = "speed", Default = "faster|1", PType = ParamType.String },
        };

        Configs configs = new Configs(paramTypes, false);

        string appDir = AppHelpers.GetApplicationDir();
        Assert.IsNotNull(appDir);

        string appSettings = Path.Combine(appDir, "appsettings.json");

        if (File.Exists(appSettings))
        {
            File.Delete(appSettings);
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("{");
        sb.AppendLine("    \"Logging\": {");
        sb.AppendLine("      \"LogLevel\": {");
        sb.AppendLine("        \"Default\": \"Information\",");
        sb.AppendLine("        \"System\": \"Error\",");
        sb.AppendLine("        \"Microsoft.AspNetCore\": \"Warning\"");
        sb.AppendLine("      }");
        sb.AppendLine("    },");
        sb.AppendLine("    \"AllowedHosts\": \"*\",");
        sb.AppendLine("    \"Data\": {");
        sb.AppendLine("      \"Settings\": {");
        sb.AppendLine("        \"Test\": \"This is a test\",");
        sb.AppendLine("        \"sourceDir\":  null,");
        sb.AppendLine("        \"destinationDir\": null,");
        sb.AppendLine("        \"action\": \"list\",");
        sb.AppendLine("        \"pattern\": \"$y_$m\",");
        sb.AppendLine("        \"filter\": \"takeout-*.zip\",");
        sb.AppendLine("        \"Deeper\": {");
        sb.AppendLine("        \"One\": \"1\",");
        sb.AppendLine("        \"Text\": \"text\"");
        sb.AppendLine("      }");
        sb.AppendLine("    }");
        sb.AppendLine("  }");
        sb.AppendLine("}");

        File.WriteAllText(appSettings, sb.ToString());
        configs.LoadAppSettings();

        Assert.IsTrue(configs.TryGetString("source", out string sourceDir));
        Assert.IsTrue(configs.TryGetString("destination", out string destinationDir));
        Assert.IsTrue(configs.TryGetString("action", out string actionText));
        Assert.AreEqual("list", actionText);
        Assert.IsTrue(configs.TryGetString("pattern", out string pattern));
        Assert.AreEqual("$y_$m", pattern);
        Assert.IsTrue(configs.TryGetString("filter", out string filter));
        Assert.AreEqual("takeout-*.zip", filter);
        Assert.IsTrue(configs.TryGetBool("help", out bool help));
        Assert.AreEqual(help, false);
        Assert.IsTrue(configs.TryGetString("doesNotExist", out string test));
    }

    [TestMethod]
    public void TestArgs()
    {
        List<ConfigParam> paramTypes = new List<ConfigParam>
        {
            new ConfigParam { Name = "filter", Default="takeout-*.default", PType = ParamType.String },
            new ConfigParam { Name = "help", Synonyms = ["?"], Default = false, PType = ParamType.Bool },
        };

        Configs configs = new Configs(paramTypes, false);

        // -action=Copy|Quiet -source="C:\Users\sanut\Downloads" -destination="C:\Users\sanut\OneDrive\Pictures" -speed=faster|2

        configs.ParseArgs(["-filter=takeout-*.zip"]);

        Assert.IsTrue(configs.TryGetString("filter", out string filter));
        Assert.AreEqual("takeout-*.zip", filter);
        Assert.IsTrue(configs.TryGetBool("help", out bool help));
        Assert.AreEqual(help, false);
        Assert.IsFalse(configs.TryGetString("doesNotExist", out string test));

        // no leading '-' or '/'
        try
        {
            configs.ParseArgs(["filter=oops"]);
        }
        catch (InvalidDataException ide)
        {
            var foo = ide;
        }
    }

    [TestMethod]
    public void TestOverrideSettings()
    {
        List<ConfigParam> paramTypes = new List<ConfigParam>
        {
            new ConfigParam { Name = "filter", Default="takeout-*.default", PType = ParamType.String },
        };

        Configs configs = new Configs(paramTypes, false);

        string appDir = AppHelpers.GetApplicationDir();
        Assert.IsNotNull(appDir);

        string appSettings = Path.Combine(appDir, "appsettings.json");

        if (File.Exists(appSettings))
        {
            File.Delete(appSettings);
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("{");
        sb.AppendLine("    \"Logging\": {");
        sb.AppendLine("      \"LogLevel\": {");
        sb.AppendLine("        \"Default\": \"Information\",");
        sb.AppendLine("        \"System\": \"Error\",");
        sb.AppendLine("        \"Microsoft.AspNetCore\": \"Warning\"");
        sb.AppendLine("      }");
        sb.AppendLine("    },");
        sb.AppendLine("    \"AllowedHosts\": \"*\",");
        sb.AppendLine("    \"Data\": {");
        sb.AppendLine("      \"Settings\": {");
        sb.AppendLine("        \"filter\": \"takeout-*.zip\"");
        sb.AppendLine("    }");
        sb.AppendLine("  }");
        sb.AppendLine("}");

        File.WriteAllText(appSettings, sb.ToString());
        configs.LoadAppSettings();
        configs.ParseArgs(["-filter=takeout-*.7z"]);

        Assert.IsTrue(configs.TryGetString("filter", out string filter));
        Assert.AreEqual("takeout-*.7z", filter);
    }

    [TestMethod]
    public void TestOverrideArgs()
    {
        List<ConfigParam> paramTypes = new List<ConfigParam>
        {
            new ConfigParam { Name = "filter", Default="takeout-*.default", PType = ParamType.String },
        };

        Configs configs = new Configs(paramTypes, false);

        string appDir = AppHelpers.GetApplicationDir();
        Assert.IsNotNull(appDir);

        string appSettings = Path.Combine(appDir, "appsettings.json");

        if (File.Exists(appSettings))
        {
            File.Delete(appSettings);
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("{");
        sb.AppendLine("    \"Logging\": {");
        sb.AppendLine("      \"LogLevel\": {");
        sb.AppendLine("        \"Default\": \"Information\",");
        sb.AppendLine("        \"System\": \"Error\",");
        sb.AppendLine("        \"Microsoft.AspNetCore\": \"Warning\"");
        sb.AppendLine("      }");
        sb.AppendLine("    },");
        sb.AppendLine("    \"AllowedHosts\": \"*\",");
        sb.AppendLine("    \"Data\": {");
        sb.AppendLine("      \"Settings\": {");
        sb.AppendLine("        \"filter\": \"takeout-*.zip\"");
        sb.AppendLine("    }");
        sb.AppendLine("  }");
        sb.AppendLine("}");

        File.WriteAllText(appSettings, sb.ToString());
        configs.ParseArgs(["-filter=takeout-*.7z"]);
        configs.LoadAppSettings();

        Assert.IsTrue(configs.TryGetString("filter", out string filter));
        Assert.AreEqual("takeout-*.zip", filter);
    }
}
