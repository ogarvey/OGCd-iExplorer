using System.IO;
using System.IO.MemoryMappedFiles;
using OGCdiExplorer.Controls.HexView.Interfaces;

namespace OGCdiExplorer.Controls.HexView.Services;

public class MemoryMappedLineReader: ILineReader
{
    private readonly byte[] _bytes;
    private readonly MemoryMappedFile _file;
    private readonly MemoryMappedViewAccessor _accessor;


    public MemoryMappedLineReader(byte[] data)
    {
        _file = MemoryMappedFile.CreateNew(null, data.Length);
        _bytes = data;
        _accessor = _file.CreateViewAccessor();
        _accessor.WriteArray(0, data, 0, data.Length);
    }
    
    public byte[] GetLine(long lineNumber, int width)
    {
        var bytes = new byte[width];
        var offset = lineNumber * width;

        for (var j = 0; j < width; j++)
        {
            var position = offset + j;
            if (position < _bytes.Length)
            {
                bytes[j] = _accessor.ReadByte(position);
            }
            else
            {
                break;
            }
        }

        return bytes;
    }

    public void Dispose()
    {
        _accessor.Dispose();
        _file.Dispose();
    }
}