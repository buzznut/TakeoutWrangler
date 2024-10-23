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
