
using PdfSharpCore.Fonts;
using System;
using System.IO;
using System.Reflection;

namespace bank_demo.Services;
public class VerdanaFontResolver : IFontResolver
{
    public static readonly VerdanaFontResolver Instance = new VerdanaFontResolver();

    public string DefaultFontName => "Verdana";

    public byte[] GetFont(string faceName)
    {
        var assembly = typeof(VerdanaFontResolver).GetTypeInfo().Assembly;

        var resource = faceName switch
        {
            "Verdana" => "bank_demo.Resources.Fonts.verdana.ttf",
            "Verdanab" => "bank_demo.Resources.Fonts.verdanab.ttf",
            _ => throw new NotImplementedException($"Font {faceName} not implemented"),
        };


        using Stream stream = assembly.GetManifestResourceStream(resource)
            ?? throw new Exception($"Resource '{resource}' not found.");
        using MemoryStream ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }

    public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        if (familyName.Equals("Verdana", StringComparison.OrdinalIgnoreCase))
        {
            if (isBold) return new FontResolverInfo("Verdanab");
            return new FontResolverInfo("Verdana");
        }

        return PlatformFontResolver.ResolveTypeface(familyName, isBold, isItalic);
    }
}
