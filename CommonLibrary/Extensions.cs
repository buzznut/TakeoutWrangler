//  <@$&< copyright begin >&$@> 24FE144C2255E2F7CCB65514965434A807AE8998C9C4D01902A628F980431C98:20241017.A:2025:7:1:14:38
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// Copyright © 2024-2025 Stewart A. Nutter - All Rights Reserved.
// No warranty is implied or given.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// <@$&< copyright end >&$@>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary;

public static class Extensions
{
    public static int WriteInt(this Stream stream, int value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        stream.Write(bytes, 0, bytes.Length);
        return bytes.Length;
    }

    public static int ReadInt(this Stream stream)
    {
        byte[] bytes = new byte[sizeof(int)];
        stream.ReadExactly(bytes, 0, bytes.Length);
        return BitConverter.ToInt32(bytes, 0);
    }

    public static int WriteString(this Stream stream, string value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        int length = bytes.Length;
        stream.WriteShort(length); // Write the length of the string
        stream.Write(bytes, 0, length);
        return length + sizeof(int); // Return total bytes written
    }

    public static string ReadString(this Stream stream)
    {
        int length = stream.ReadShort(); // Read the length of the string
        if (length < 0) throw new ArgumentOutOfRangeException(nameof(length), "String length cannot be negative.");
        byte[] bytes = new byte[length];
        stream.ReadExactly(bytes, 0, length);
        return Encoding.UTF8.GetString(bytes);
    }

    public static int WriteShort(this Stream stream, int value)
    {
        if (value < short.MinValue || value > short.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(value), "Value must be between -32768 and 32767.");

        byte[] bytes = BitConverter.GetBytes((short)value);
        stream.Write(bytes, 0, bytes.Length);
        return bytes.Length;
    }

    public static int ReadShort(this Stream stream)
    {
        byte[] bytes = new byte[sizeof(short)];
        stream.ReadExactly(bytes, 0, bytes.Length);
        return BitConverter.ToInt16(bytes, 0);
    }
}
