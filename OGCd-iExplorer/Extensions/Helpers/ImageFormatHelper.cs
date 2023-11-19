using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using static OGCdiExplorer.Extensions.Helpers.Utils;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace OGCdiExplorer.Extensions.Helpers;

public static class ImageFormatHelper
{
    private static readonly int[] dequantizer = { 0, 1, 4, 9, 16, 27, 44, 79, 128, 177, 212, 229, 240, 247, 252, 255 };

    public static Bitmap DecodeDyuvDoubleImage(byte[] encodedData, int width, int height)
    {
        int encodedIndex = 0; //reader index
       
        byte[] decodedImage = new byte[width * 2 * height * 4]; //decoded image array
        uint initialY = 16; //initial Y value (per line)
        uint initialU = 128; //initial U value (per line)
        uint initialV = 128; //initial V value (per line)
        //create bitmap from RGB array

        try
        {
            //loop through all output lines
            for (int y = 0; y < height; y++)
            {
                //re-initialize previous YUV value
                uint prevY = initialY;
                uint prevU = initialU;
                uint prevV = initialV;

                //loop through each pixel in line
                for (int x = 0; x < width; x += 2)
                {
                    if (encodedIndex >= encodedData.Length || encodedIndex + 1 >= encodedData.Length)
                    {
                        break;
                    }

                    //read DYUV value from source
                    int encodedPixel = ((encodedData[encodedIndex] << 8) | encodedData[encodedIndex + 1]);

                    //parse encoded pixel to each delta value
                    byte dU1 = (byte)((encodedPixel & 0xF000) >> 12);
                    byte dY1 = (byte)((encodedPixel & 0x0F00) >> 8);
                    byte dV1 = (byte)((encodedPixel & 0x00F0) >> 4);
                    byte dY2 = (byte)(encodedPixel & 0x000F);

                    //dequantize dYUV to YUV
                    var Yout1 = (prevY + dequantizer[dY1]) % 256;
                    var Uout2 = (prevU + dequantizer[dU1]) % 256; //Uout2 is the output when dequantizing
                    var Vout2 = (prevV + dequantizer[dV1]) % 256; //Vout2 is the output when dequantizing
                    var Yout2 = (Yout1 + dequantizer[dY2]) % 256; //Yout2 is based on You1, not prevY

                    //interpolate U and V to double resolution and determine UVout1
                    var Uout1 = (prevU + Uout2) / 2;
                    var Vout1 = (prevV + Vout2) / 2;

                    //store latest YUV values for next iteration
                    prevY = (uint)Yout2;
                    prevU = (uint)Uout2;
                    prevV = (uint)Vout2;

                    //decode each YUV set to RGB
                    (int R1, int G1, int B1) = YUVtoRGB((int)Yout1, (int)Uout1, (int)Vout1);
                    (int R2, int G2, int B2) = YUVtoRGB((int)Yout2, (int)Uout2, (int)Vout2);

                    //write RGB to output array
                    int decodedIndex =
                        (y * width + x * 2) *
                        4; //each iteration there are 2 pixels decoded, therefor the index moves 8 steps
                    decodedImage[decodedIndex + 0] = (byte)B1;
                    decodedImage[decodedIndex + 1] = (byte)G1;
                    decodedImage[decodedIndex + 2] = (byte)R1;
                    decodedImage[decodedIndex + 3] = 0xff;
                    decodedImage[decodedIndex + 4] = (byte)B1;
                    decodedImage[decodedIndex + 5] = (byte)G1;
                    decodedImage[decodedIndex + 6] = (byte)R1;
                    decodedImage[decodedIndex + 7] = 0xff;
                    decodedImage[decodedIndex + 8] = (byte)B2;
                    decodedImage[decodedIndex + 9] = (byte)G2;
                    decodedImage[decodedIndex + 10] = (byte)R2;
                    decodedImage[decodedIndex + 11] = 0xff;
                    decodedImage[decodedIndex + 12] = (byte)B2;
                    decodedImage[decodedIndex + 13] = (byte)G2;
                    decodedImage[decodedIndex + 14] = (byte)R2;
                    decodedImage[decodedIndex + 15] = 0xff;

                    //increment reader
                    encodedIndex += 2;
                }
            }
        }
        catch (Exception)
        {
            
        }

        try
        {
            
            Bitmap bitmap = new Bitmap(new MemoryStream(decodedImage));
            return bitmap;
        }
        catch
        {
            // MessageBox.Show("Failed to decode image");
        }

        return null;
    }

    public static Bitmap DecodeDYUVImage(byte[] encodedData, int Width, int Height)
    {
        int encodedIndex = 0; //reader index
        int width = Width, height = Height; //output dimensions
        byte[] decodedImage = new byte[width * height * 4]; //decoded image array
        uint initialY = 128; //initial Y value (per line)
        uint initialU = 128; //initial U value (per line)
        uint initialV = 128; //initial V value (per line)

        //loop through all output lines
        for (int y = 0; y < height; y++)
        {
            //re-initialize previous YUV value
            uint prevY = initialY;
            uint prevU = initialU;
            uint prevV = initialV;

            //loop through each pixel in line
            for (int x = 0; x < width; x += 2)
            {
                if (encodedIndex >= encodedData.Length || encodedIndex + 1 >= encodedData.Length)
                {
                    break;
                }

                //read DYUV value from source
                int encodedPixel = ((encodedData[encodedIndex] << 8) | encodedData[encodedIndex + 1]);

                //parse encoded pixel to each delta value
                byte dU1 = (byte)((encodedPixel & 0xF000) >> 12);
                byte dY1 = (byte)((encodedPixel & 0x0F00) >> 8);
                byte dV1 = (byte)((encodedPixel & 0x00F0) >> 4);
                byte dY2 = (byte)(encodedPixel & 0x000F);

                //dequantize dYUV to YUV
                var Yout1 = (prevY + dequantizer[dY1]) % 256;
                var Uout2 = (prevU + dequantizer[dU1]) % 256; //Uout2 is the output when dequantizing
                var Vout2 = (prevV + dequantizer[dV1]) % 256; //Vout2 is the output when dequantizing
                var Yout2 = (Yout1 + dequantizer[dY2]) % 256; //Yout2 is based on You1, not prevY

                //interpolate U and V to double resolution and determine UVout1
                var Uout1 = (prevU + Uout2) / 2;
                var Vout1 = (prevV + Vout2) / 2;

                //store latest YUV values for next iteration
                prevY = (uint)Yout2;
                prevU = (uint)Uout2;
                prevV = (uint)Vout2;

                //decode each YUV set to RGB
                (int R1, int G1, int B1) = YUVtoRGB((int)Yout1, (int)Uout1, (int)Vout1);
                (int R2, int G2, int B2) = YUVtoRGB((int)Yout2, (int)Uout2, (int)Vout2);

                //write RGB to output array
                int decodedIndex =
                    (y * width + x) * 4; //each iteration there are 2 pixels decoded, therefor the index moves 8 steps
                decodedImage[decodedIndex + 0] = (byte)B1;
                decodedImage[decodedIndex + 1] = (byte)G1;
                decodedImage[decodedIndex + 2] = (byte)R1;
                decodedImage[decodedIndex + 3] = 0xff;
                decodedImage[decodedIndex + 4] = (byte)B2;
                decodedImage[decodedIndex + 5] = (byte)G2;
                decodedImage[decodedIndex + 6] = (byte)R2;
                decodedImage[decodedIndex + 7] = 0xff;

                //increment reader
                encodedIndex += 2;
            }
        }

        //create bitmap from RGB array
        Bitmap bitmap = new Bitmap(new MemoryStream(decodedImage));

        return bitmap;
    }

    private static (int R, int G, int B) YUVtoRGB(int Y, int U, int V)
    {
        //added additional parenthesis to ensure "/256" is done last
        int R = Clamp((Y * 256 + 351 * (V - 128)) / 256);
        int G = Clamp(((Y * 256) - (86 * (U - 128) + 179 * (V - 128))) / 256);
        int B = Clamp((Y * 256 + 444 * (U - 128)) / 256);

        return (R, G, B);
    }

    private static int Clamp(int value)
    {
        if (value < 0)
        {
            return 0;
        }

        if (value > 255)
        {
            return 255;
        }

        return value;
    }
    
    // public static Bitmap GenerateClutImage(List<Color> palette, byte[] clut7Bytes, int Width, int Height)
    // {
    //     var clutImage = new Bitmap(Width, Height);
    //
    //     try
    //     {
    //         for (int y = 0; y < Height; y++)
    //         {
    //             for (int x = 0; x < Width; x++)
    //             {
    //                 var i = y * Width + x;
    //                 var paletteIndex = clut7Bytes[i];
    //                 var color = paletteIndex < palette.Count ? palette[paletteIndex] : Color.Transparent;
    //                 clutImage.SetPixel(x, y, color);
    //             }
    //         }
    //     }
    //     catch (System.Exception)
    //     {
    //         return clutImage;
    //     }
    //
    //     return clutImage;
    // }
    //
    // public static Bitmap GenerateClut4Image(List<Color> palette, byte[] clut7Bytes, int Width, int Height)
    // {
    //     var clutImage = new Bitmap(Width, Height);
    //     try
    //     {
    //         for (int y = 0; y < Height; y++)
    //         {
    //             for (int x = 0; x < Width; x += 2)
    //             {
    //                 var i = y * Width + x;
    //                 var paletteIndex = clut7Bytes[i];
    //                 var paletteIndex1 = paletteIndex >> 4;
    //                 var paletteIndex2 = paletteIndex & 0x0F;
    //                 var color = paletteIndex1 < palette.Count ? palette[paletteIndex1] : Color.Transparent;
    //                 var color2 = paletteIndex2 < palette.Count ? palette[paletteIndex2] : Color.Transparent;
    //                 clutImage.SetPixel(x, y, color);
    //                 clutImage.SetPixel(x + 1, y, color);
    //             }
    //         }
    //     }
    //     catch (System.Exception)
    //     {
    //         return clutImage;
    //     }
    //
    //     return clutImage;
    // }
    //
    // public static Bitmap GenerateRle3Image(List<Color> palette, byte[] rl7Bytes, int width, int height)
    // {
    //     var rleImage = Rle3(rl7Bytes, width, height);
    //     var rleBitmap = CreateImage(rleImage, palette, width, height);
    //     return rleBitmap;
    // }

    // public static Bitmap GenerateRle7Image(List<Color> palette, byte[] rl7Bytes, int width, int height,
    //     bool useTransparency = false)
    // {
    //     var rleImage = Rle7(rl7Bytes, width, height);
    //     var rleBitmap = CreateImage(rleImage, palette, width, height, useTransparency);
    //     return rleBitmap;
    // }
    //
    public static byte[] Rle3(byte[] dataRLE, int width, int height)
    {
        //initialize variables
        int nrRLEData = dataRLE.Count();
        byte[] dataDecoded = new byte[width * height];
        int posX = 1;
        int outputIndex = 0;
        int inputIndex = 0;

        //decode RLE3
        while ((inputIndex < nrRLEData) && (outputIndex < (width * height)))
        {
            //get run count
            byte byte1 = @dataRLE[inputIndex++];
            if (inputIndex >= nrRLEData)
            {
                break;
            }

            if (byte1 >= 128)
            {
                //draw multiple times
                byte colorNr = (byte)((byte1 - 128) >> 4 & 0x07);
                byte colorNr2 = (byte)((byte1 - 128) & 0x07);

                //get runlength
                byte rl = @dataRLE[inputIndex++];

                //draw x times
                for (int i = 0; i < rl; i++)
                {
                    var index = outputIndex += 2;
                    if (index >= dataDecoded.Length)
                    {
                        break;
                    }

                    dataDecoded[index] = @colorNr;
                    dataDecoded[index + 1] = @colorNr2;
                    posX += 2;
                }

                //draw until end of line
                if (rl == 0)
                {
                    while (posX <= width)
                    {
                        if (outputIndex >= dataDecoded.Length)
                        {
                            break;
                        }

                        dataDecoded[outputIndex++] = @colorNr;
                        dataDecoded[outputIndex++] = @colorNr2;
                        posX += 2;
                    }
                }
            }
            else
            {
                //draw once
                dataDecoded[outputIndex++] = (byte)((byte1 - 128) >> 4 & 0x07);
                dataDecoded[outputIndex++] = (byte)((byte1 - 128) & 0x07);
                posX += 2;
            }

            //reset x to 1 if end of line is reached
            if (posX >= width)
            {
                posX = 1;
            }
        }

        //decode CLUT to bitmap
        return dataDecoded;
    }
    
    public static byte[] Rle7(byte[] dataRLE, int width, int height)
    {
        //initialize variables
        int nrRLEData = dataRLE.Count();
        byte[] dataDecoded = new byte[width * height];
        int posX = 1;
        int outputIndex = 0;
        int inputIndex = 0;

        //decode RLE7
        while ((inputIndex < nrRLEData) && (outputIndex < (width * height)))
        {
            //get run count
            byte byte1 = dataRLE[inputIndex++];
            if (inputIndex >= nrRLEData)
            {
                break;
            }

            if (byte1 >= 128)
            {
                //draw multiple times
                byte colorNr = (byte)(byte1 - 128);

                //get runlength
                byte rl = dataRLE[inputIndex++];

                //draw x times
                for (int i = 0; i < rl; i++)
                {
                    if (outputIndex >= dataDecoded.Length)
                    {
                        break;
                    }

                    var index = outputIndex++;
                    if (index >= dataDecoded.Length)
                    {
                        break;
                    }

                    dataDecoded[index] = colorNr;
                    posX++;
                }

                //draw until end of line
                if (rl == 0)
                {
                    while (posX <= width)
                    {
                        if (outputIndex >= dataDecoded.Length)
                        {
                            break;
                        }

                        dataDecoded[outputIndex++] = colorNr;
                        posX++;
                    }
                }
            }
            else
            {
                //draw once
                dataDecoded[outputIndex++] = byte1;
                posX++;
            }

            //reset x to 1 if end of line is reached
            if (posX >= width)
            {
                posX = 1;
            }
        }

        //decode CLUT to bitmap
        return dataDecoded;
    }

    // requires file to contain iff headers
    public static (byte[], byte[]) ExtractIFFPaletteAndImageBytes(string filePath)
    {
        byte[] plteSequence = new byte[] { 0x50, 0x4C, 0x54, 0x45 };
        byte[] idatSequence = new byte[] { 0x49, 0x44, 0x41, 0x54 };
        byte[] plteBytes = null;
        byte[] idatBytes = null;

        using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
        {
            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                byte currentByte = reader.ReadByte();

                if (currentByte == plteSequence[0] && MatchesSequence(reader, plteSequence.Skip(1).ToArray()))
                {
                    reader.BaseStream.Position += 2; // skip two bytes
                    var bytes = reader.ReadBytes(2);
                    ushort numberOfBytesToRead = BitConverter.ToUInt16(bytes.Reverse().ToArray());
                    plteBytes = reader.ReadBytes(numberOfBytesToRead);
                }
                else if (currentByte == idatSequence[0] && MatchesSequence(reader, idatSequence.Skip(1).ToArray()))
                {
                    reader.BaseStream.Position += 4;
                    idatBytes = reader.ReadBytes(0x1a400);
                    break; // no need to continue reading after this
                }
            }
        }

        return (plteBytes, idatBytes);
    }
    //
    // public static void Rle7_AllBytes(byte[] dataRLE, List<Color> palette, int width, List<Bitmap> images)
    // {
    //     //initialize variables
    //     int nrRLEData = dataRLE.Count();
    //     byte[] dataDecoded = new byte[0x16800];
    //     int posX = 1;
    //     int outputIndex = 0;
    //     int inputIndex = 0;
    //     int initialIndex;
    //
    //     //decode RLE7
    //     while (inputIndex < nrRLEData)
    //     {
    //         initialIndex = inputIndex;
    //         //get run count
    //         byte byte1 = @dataRLE[inputIndex++];
    //         if (inputIndex >= nrRLEData)
    //         {
    //             break;
    //         }
    //
    //         if (byte1 >= 128)
    //         {
    //             //draw multiple times
    //             byte colorNr = (byte)(byte1 - 128);
    //
    //             //get runlength
    //             byte rl = @dataRLE[inputIndex++];
    //
    //             //draw x times
    //             for (int i = 0; i < rl; i++)
    //             {
    //                 if (outputIndex >= dataDecoded.Length)
    //                 {
    //                     break;
    //                 }
    //
    //                 var index = outputIndex++;
    //                 if (index >= dataDecoded.Length)
    //                 {
    //                     break;
    //                 }
    //
    //                 dataDecoded[index] = @colorNr;
    //                 posX++;
    //             }
    //
    //             //draw until end of line
    //             if (rl == 0)
    //             {
    //                 while (posX <= width)
    //                 {
    //                     if (outputIndex >= dataDecoded.Length)
    //                     {
    //                         break;
    //                     }
    //
    //                     dataDecoded[outputIndex++] = @colorNr;
    //                     posX++;
    //                 }
    //             }
    //         }
    //         else
    //         {
    //             //draw once
    //             dataDecoded[outputIndex++] = @byte1;
    //             posX++;
    //         }
    //
    //         //reset x to 1 if end of line is reached
    //         if (posX >= width)
    //         {
    //             posX = 1;
    //         }
    //
    //         if (outputIndex >= 0x16800)
    //         {
    //             var offsets = $"{initialIndex:X8}_{inputIndex:X8}";
    //             var image = GenerateRle7Image(palette, dataDecoded, width, outputIndex / width, true);
    //             images.Add(image);
    //             dataDecoded = new byte[0x16800];
    //             outputIndex = 0;
    //             posX = 1;
    //         }
    //     }
    //     /* int requiredSize = dataDecoded.Length - 1;
    //     while (dataDecoded[requiredSize] == 0x00)
    //     {
    //       requiredSize--;
    //     }
    //     byte[] dataDecoded2 = new byte[requiredSize + 1];
    //     Array.Copy(dataDecoded, dataDecoded2, requiredSize + 1); */
    //     //decode CLUT to bitmap
    //     //return dataDecoded2;
    // }
    //
    public static byte[] CreateImageData(byte[] imageBin, List<Color> colors, int width, int height, bool useTransparency = false)
    {
        int bytesPerPixel = 4; // Assuming 24-bit RGB (3 bytes per pixel)
        byte[] imageData = new byte[width * height * bytesPerPixel];

        if (useTransparency)
        {
            colors[0] = Color.Transparent;
        }

        for (int i = 0; i < imageBin.Length; i++)
        {
            Color color;
            if (imageBin[i] >= colors.Count)
            {
                color = colors[0];
            }
            else
            {
                color = colors[imageBin[i]];
            }

            int pixelIndex = i * bytesPerPixel;
            imageData[pixelIndex] = color.A;
            imageData[pixelIndex + 1] = color.R;
            imageData[pixelIndex + 2] = color.G;
            imageData[pixelIndex + 3] = color.B;
        }

        return imageData;
    }

}