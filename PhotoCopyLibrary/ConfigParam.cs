//  <@$&< copyright begin >&$@> D50225522CB19A3A2E3CA10257DC538D19677A6406D028F0BBE01DE33387A4EA:20241017.A:2024:11:16:13:40
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024 Stewart A. Nutter - All Rights Reserved.
// No warranty is implied or given.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// <@$&< copyright end >&$@>

namespace PhotoCopyLibrary;

public enum ParamType
{
    String,
    Bool,
    Int
}

public class ConfigParam
{
    private string _name;
    public string Name
    {
        get
        {
            return _name;
        }

        set
        {
            if (value == null) throw new ArgumentNullException("Name");
            if (_name == null && value != null)
            {
                _name = value;
                _synonyms.Add(value);
            }
        }
    }

    public object Default { get; set; }

    private HashSet<string> _synonyms = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    public string[] Synonyms
    {
        get
        {
            return _synonyms.ToArray();
        }

        set
        {
            if (value == null) throw new ArgumentNullException("Synonyms");
            _synonyms.Clear();
            if (_name != null) _synonyms.Add(_name);
            if (value?.Length > 0)
            {
                foreach (string v in value)
                {
                    _synonyms.Add(v);
                }
            }
        }
    }

    public ParamType PType { get; set; } = ParamType.String;
}
