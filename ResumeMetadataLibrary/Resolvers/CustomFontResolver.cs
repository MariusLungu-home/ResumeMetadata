using PdfSharp.Fonts;

namespace ResumeMetadataLibrary.Resolvers
{
    public class CustomFontResolver : IFontResolver
    {
        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            string fontName = "arial";
            return new FontResolverInfo(fontName);
        }

        public byte[] GetFont(string faceName)
        {
            string directory = Directory.GetCurrentDirectory();
            string path = Path.Combine(directory, "Fonts", "arial.ttf");
            return File.ReadAllBytes(path);
        }
    }
}
