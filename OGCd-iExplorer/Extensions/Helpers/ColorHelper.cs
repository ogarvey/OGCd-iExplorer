using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace OGCdiExplorer.Extensions.Helpers;

 public static class ColorHelper
 {
   public static List<Color> GenerateColorsFromClutBank(byte[] palette)
   {
     var colors = new List<Color>();
     if (palette.Length > 0)
     {
       var length = (int)palette.Length;
       colors = new List<Color>();
       for (int i = 0; i < length; i += 4)
       {
         var r = palette[i + 1];
         var g = palette[i + 2];
         var b = palette[i + 3];
         var color = Color.FromArgb(255, r, g, b);
         colors.Add(color);
       }

       if (colors.Count >= 256) return colors;
       {
         var remaining = 256 - colors.Count;
         for (int i = 0; i < remaining; i++)
         {
           colors.Add(Color.FromArgb(255, 0, 0, 0));
         }
       }
     }
     return colors;
   }
   public static List<Color> GenerateColors(int count)
   {
     List<Color> colors = new List<Color>();
     // Generate grayscale colors
     for (int i = 0; i < count / 2; i++)
     {
       int grayValue = (int)((i / (float)(count / 2)) * 255);
       colors.Add(Color.FromArgb(grayValue, grayValue, grayValue));
     }
     // Generate spectrum colors
     for (int i = 0; i < count - count / 2; i++)
     {
       float hue = (i / (float)(count - count / 2)) * 360;  // Hue varies from 0 to 360
       colors.Add(ColorFromHSV(hue, (float)1.0, (float)1.0)); // Saturation and lightness are constant
     }

     

     return colors;
   }

   // Convert HSV color to RGB color
   static Color ColorFromHSV(float hue, float saturation, float value)
   {
     int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
     float f = (float)(hue / 60 - Math.Floor(hue / 60));

     value = value * 255;
     int v = Convert.ToInt32(value);
     int p = Convert.ToInt32(value * (1 - saturation));
     int q = Convert.ToInt32(value * (1 - f * saturation));
     int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

     if (hi == 0)
       return Color.FromArgb(255, v, t, p);
     else if (hi == 1)
       return Color.FromArgb(255, q, v, p);
     else if (hi == 2)
       return Color.FromArgb(255, p, v, t);
     else if (hi == 3)
       return Color.FromArgb(255, p, q, v);
     else if (hi == 4)
       return Color.FromArgb(255, t, p, v);
     else
       return Color.FromArgb(255, v, p, q);
   }
   public static List<Color> ConvertBytesToRGB(byte[] bytes)
   {
     List<Color> colors = new List<Color>();

     for (int i = 0; i < bytes.Length - 2; i += 3)
     {
       byte red = bytes[i];
       byte green = bytes[i + 1];
       byte blue = bytes[i + 2];

       Color color = Color.FromArgb(red, green, blue);
       colors.Add(color);
     }
     return colors;
   }

   public static void RotateSubset(List<Color> colors, int startIndex, int endIndex, int permutations)
   {
     int subsetSize = endIndex - startIndex + 1;
     permutations = permutations % subsetSize; // Remove unnecessary full rotations

     if (permutations == 0 || subsetSize <= 1)
     {
       return;
     }

     // Create a temporary list to hold the rotated values
     List<Color> rotatedSubset = new List<Color>(subsetSize);
     for (int i = 0; i < subsetSize; i++)
     {
       rotatedSubset.Add(colors[startIndex + ((i - permutations + subsetSize) % subsetSize)]);
     }

     // Copy the rotated subset back into the original list
     for (int i = 0; i < subsetSize; i++)
     {
       colors[startIndex + i] = rotatedSubset[i];
     }
   }

   public static void ReverseRotateSubset(List<Color> colors, int startIndex, int endIndex, int permutations)
   {
     int subsetSize = endIndex - startIndex + 1;
     permutations = permutations % subsetSize; // Remove unnecessary full rotations

     if (permutations == 0 || subsetSize <= 1)
     {
       return;
     }

     // Create a temporary list to hold the rotated values
     List<Color> rotatedSubset = new List<Color>(subsetSize);

     for (int i = 0; i < subsetSize; i++)
     {
       rotatedSubset.Add(colors[startIndex + ((i + permutations) % subsetSize)]);
     }

     // Copy the rotated subset back into the original list
     for (int i = 0; i < subsetSize; i++)
     {
       colors[startIndex + i] = rotatedSubset[i];
     }
   }

   public static Bitmap CreateLabelledPalette(List<Color> colors)
   {
     int squareSize = 16;
     int cols = 16;
     int rows = colors.Count / cols;

     // Calculate the width and height of the bitmap
     int width = squareSize * cols;
     int height = squareSize * rows;

     // Create a new bitmap with the calculated dimensions
     Bitmap bitmap = new Bitmap(width, height);

     using (Graphics graphics = Graphics.FromImage(bitmap))
     {
       // Set the smoothing mode for better text quality
       graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
       graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

       // Define the font and brush for the text
       using (Font font = new Font("Arial", 8, FontStyle.Bold))
       using (Brush textBrush = new SolidBrush(Color.White))
       {
         for (int i = 0; i < colors.Count; i++)
         {
           int xIndex = i % cols;
           int yIndex = i / cols;

           // Draw the 8x8 square
           using (Brush brush = new SolidBrush(colors[i]))
           {
             graphics.FillRectangle(brush, xIndex * squareSize, yIndex * squareSize, squareSize, squareSize);
           }

           // Draw the text (index) on the square
           string text = i.ToString();
           SizeF textSize = graphics.MeasureString(text, font);
           float x = (xIndex * squareSize) + (squareSize - textSize.Width) / 2;
           float y = (yIndex * squareSize) + (squareSize - textSize.Height) / 2;

           graphics.DrawString(text, font, textBrush, x, y);
         }
       }
     }

     return bitmap;
   }

   public static void WritePalette(string path, List<Color> colors)
   {
     // output a bitmap where each color in the colors array is an 8*8 pixel square
     var bitmap = new Bitmap(256, 256);
     var graphics = Graphics.FromImage(bitmap);
     var brush = new SolidBrush(Color.Black);
     var pen = new Pen(Color.Black);
     var x = 0;
     var y = 0;
     var width = 8;
     var height = 8;
     foreach (var color in colors)
     {
       brush.Color = color;
       graphics.FillRectangle(brush, x, y, width, height);
       x += width;
       if (x >= 256)
       {
         x = 0;
         y += height;
       }
     }
     bitmap.Save(path, ImageFormat.Png);

   }

   public static List<Color> ReadPalette(byte[] data)
   {
     var length = (int)data.Length;
     List<Color> colors = new List<Color>();
     for (int i = 0; i < length; i += 4)
     {
       var color = Color.FromArgb(255, data[i + 1], data[i + 2], data[i + 3]);
       colors.Add(color);
     }
     return colors;
   }

   public static List<Color> ReadClutBankPalettes(byte[] data, byte count)
   {
     var length = 0x100;
     List<Color> colors = new List<Color>();
     for (int i = 4; i < 4 + length; i += 4)
     {
       var color = Color.FromArgb(255, data[i + 1], data[i + 2], data[i + 3]);
       colors.Add(color);
     }
     if (count >= 2)
     {
       for (int i = 264; i < 264 + length; i += 4)
       {
         var color = Color.FromArgb(255, data[i + 1], data[i + 2], data[i + 3]);
         colors.Add(color);
       }
     }
     if (count >= 3)
     {
       for (int i = 524; i < 524 + length; i += 4)
       {
         var color = Color.FromArgb(255, data[i + 1], data[i + 2], data[i + 3]);
         colors.Add(color);
       }
     }
     if (count == 4)
     {
       for (int i = 784; i < 784 + length; i += 4)
       {
         var color = Color.FromArgb(255, data[i + 1], data[i + 2], data[i + 3]);
         colors.Add(color);
       }
     }
     return colors;
   }

   public static Color[] GenerateGrayscalePalette(int numColors)
   {
     Color[] palette = new Color[numColors];
     float step = 1f / (numColors - 1);

     for (int i = numColors-1; i >= 0; i--)
     {
       float intensity = step * i;
       byte value = (byte)(intensity * 255);
       palette[i] = Color.FromArgb(value, value, value);
     }
     return palette;
   }

 }
