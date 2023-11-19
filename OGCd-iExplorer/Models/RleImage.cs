namespace OGCdiExplorer.Models;

public class RleImage : ImageBase<byte>
{
    public RleImage(byte[] image, int width, int height)
    {
        Image = image;
        Width = width;
        Height = height;
    }
    
    public override void Render()
    {
        throw new System.NotImplementedException();
    }
}