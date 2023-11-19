using System;
using System.IO;

namespace OGCdiExplorer.Extensions;

public static class BinaryReaderExtensions
{
    public static byte[] ReadBytesToEnd(this BinaryReader binaryReader)
    {
        var length = binaryReader.BaseStream.Length - binaryReader.BaseStream.Position;
        return binaryReader.ReadBytes((int)length);
    }  
    
    public static byte? PeekByte(this BinaryReader reader)
    {
        if (reader.BaseStream.Position >= reader.BaseStream.Length)
        {
            return null;
        }

        byte nextByte = reader.ReadByte();
        reader.BaseStream.Seek(-1, SeekOrigin.Current);
        return nextByte;
    }

    public static byte[] ReadAllBytes(this BinaryReader binaryReader)
    {

        binaryReader.BaseStream.Position = 0;
        return binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
    }

    public static byte[] ReadBytes(this BinaryReader binaryReader, Range range)
    {

        var (offset, length) = range.GetOffsetAndLength((int)binaryReader.BaseStream.Length);
        binaryReader.BaseStream.Position = offset;
        return binaryReader.ReadBytes(length);
    }
}