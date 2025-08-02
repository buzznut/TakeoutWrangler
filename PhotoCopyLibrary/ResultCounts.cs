//  <@$&< copyright begin >&$@> 24FE144C2255E2F7CCB65514965434A807AE8998C9C4D01902A628F980431C98:20241017.A:2025:7:24:7:12
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024-2025 Stewart A. Nutter - All Rights Reserved.
// No warranty is implied or given.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// <@$&< copyright end >&$@>

// using Library "CompactExifLib" https://www.codeproject.com/Articles/5251929/CompactExifLib-Access-to-EXIF-Tags-in-JPEG-TIFF-an
// for reading and writing EXIF data in JPEG, TIFF and PNG image files.
// © Copyright 2021 Hans-Peter Kalb

using System.Collections.Concurrent;

namespace PhotoCopyLibrary;

public class ResultCounts
{
    private object locker = new object();
    public enum CountKeys
    {
        Total,
        Error,
        Skip,
        Duplicate,
        Change,
        Progress
    }

    private readonly ConcurrentDictionary<CountKeys, int?> Values = new ConcurrentDictionary<CountKeys, int?>();

    public readonly string Text;
    public readonly ResultCounts Parent;
    public readonly List<ResultCounts> Children = new List<ResultCounts>();

    public ResultCounts(ResultCounts parent, string text)
    {
        lock (locker)
        {
            Children.Clear();
            foreach (CountKeys key in Enum.GetValues<CountKeys>())
            {
                Values[key] = null;
            }

            parent?.Children.Add(this);
            Parent = parent;
            Text = text;
        }
    }

    public void Increment(CountKeys key)
    {
        lock (locker)
        {
            Values[key] = Values[key].GetValueOrDefault() + 1;
        }
    }

    public void Set(CountKeys key, int value)
    {
        lock (locker)
        {
            Values[key] = value;
        }
    }

    public void Add(CountKeys key, int value)
    {
        lock (locker)
        {
            Values[key] += value;
        }
    }

    public int? Get(CountKeys key)
    {
        lock (locker)
        {
            return Values[key];
        }
    }
}
