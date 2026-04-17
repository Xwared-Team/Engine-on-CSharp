namespace EOCS;

using System.Runtime.Versioning;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using SixLabors.Fonts;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

[SupportedOSPlatform("windows")]
public class FontAtlas : IDisposable
{   
    public int TextureID {get; private set;}
    public int Width {get; private set;}
    public int Height {get; private set;}

    private Dictionary<char, GlyphData> _glyphs = new Dictionary<char, GlyphData>();

    private const string Charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 .,!?-+*/=()[]{}<>@#$%^&";

    public FontAtlas(string fontPath, int fontSize, int atlasSize = 512)
    {
        Width = atlasSize;
        Height = atlasSize;
        GenerateAtlas(fontPath, fontSize);
    }

    private void GenerateAtlas(string fontPath, int fontSize)
    {
        var collection = new FontCollection();
        var family = collection.Add(fontPath);
        var font = family.CreateFont(fontSize, SixLabors.Fonts.FontStyle.Regular);

        using var bitmap = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        using var graphics = Graphics.FromImage(bitmap);

        graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
        graphics.Clear(Color.Transparent);

        float currentX = 0;
        float currentY = 0;
        float maxHeightInRow = 0;
        float padding = 2.0f;

        foreach (char c in Charset)
        {
            string s = c.ToString();
            
            SizeF size = graphics.MeasureString(s, new System.Drawing.Font(family.Name, fontSize));
            
            if (currentX + size.Width > Width)
            {
                currentX = 0;
                currentY += maxHeightInRow + padding;
                maxHeightInRow = 0;
            }
            
            if (currentY + size.Height > Height)
            {
                Console.WriteLine($"Warning: Font atlas overflow at character '{c}'. Increase atlas size.");
                break;
            }

            using (var drawFont = new System.Drawing.Font(family.Name, fontSize))
            {
                graphics.DrawString(s, drawFont, Brushes.White, currentX, currentY);
            }

            float u = currentX / Width;
            float v = currentY / Height;
            float w = size.Width / Width;
            float h = size.Height / Height;
            float advance = size.Width + padding; 

            _glyphs[c] = new GlyphData(c, u, v, w, h, 0, 0, advance);

            currentX += advance;
            if (size.Height > maxHeightInRow) maxHeightInRow = size.Height;
        }

        string outputPath = "generated_atlas.png";
        string? dirName = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(dirName))
        {
            Directory.CreateDirectory(dirName);
        }
        bitmap.Save(outputPath, ImageFormat.Png);
        Console.WriteLine($"Font atlas saved to: {outputPath}");
        LoadTextureToOpenGL(bitmap);
    }

    private void LoadTextureToOpenGL(Bitmap bitmap)
    {
        TextureID = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, TextureID);

        var data = bitmap.LockBits(
            new Rectangle(0, 0, bitmap.Width, bitmap.Height),
            ImageLockMode.ReadOnly,
            System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        GL.TexImage2D(
            TextureTarget.Texture2D,
            0,
            PixelInternalFormat.Rgba,
            bitmap.Width,
            bitmap.Height,
            0,
            PixelFormat.Bgra,
            PixelType.UnsignedByte,
            data.Scan0);

        bitmap.UnlockBits(data);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

        GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    public bool TryGetGlyph(char c, out GlyphData glyph)
    {
        return _glyphs.TryGetValue(c, out glyph);
    }

    public void Dispose()
    {
        if (TextureID != 0)
        {
            GL.DeleteTexture(TextureID);
            TextureID = 0;
        }
    }
}