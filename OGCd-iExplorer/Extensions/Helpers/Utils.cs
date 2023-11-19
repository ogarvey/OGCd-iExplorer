using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OGCdiExplorer.Extensions.Helpers;

public static class Utils
{
    
    public static bool MatchesSequence(BinaryReader reader, byte[] sequence)
    {
        for (int i = 0; i < sequence.Length; i++)
        {
            byte nextByte = reader.ReadByte();
            if (nextByte != sequence[i])
            {
                // rewind to the start of the sequence (including the byte we just read)
                reader.BaseStream.Position -= i + 1;
                return false;
            }
        }

        return true;
    }
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
    {
        return source.Select((item, index) => (item, index));
    }
}