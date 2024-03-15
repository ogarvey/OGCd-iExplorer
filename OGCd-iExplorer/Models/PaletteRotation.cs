namespace OGCdiExplorer.Models;

public record PaletteRotation(int StartIndex, int EndIndex, int Permutations, int FrameSkip, bool reverseRotation = false);