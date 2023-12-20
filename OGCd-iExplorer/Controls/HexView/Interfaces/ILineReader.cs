using System;

namespace OGCdiExplorer.Controls.HexView.Interfaces;

public interface ILineReader: IDisposable
{
    byte[] GetLine(long lineNumber, int width);
}