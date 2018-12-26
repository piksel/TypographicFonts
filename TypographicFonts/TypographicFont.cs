using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace jnm2.TypographicFonts
{
    [DebuggerDisplay("{ToString(),nq}")]
    public sealed class TypographicFont
    {
        /// <summary>
        /// The typographic family. This returns "Arial" for "Arial Black" and "Arial Narrow," which allows them to be grouped with the other "Arial" fonts even though the names are different.
        /// The native font picker groups fonts by typographic family.
        /// </summary>
        public string Family { get; private set; }

        /// <summary>
        /// The typographic subfamily. This returns "Black" and "Narrow" for "Arial Black" and "Arial Narrow," alongside Arial's "Regular", "Bold", etc.
        /// </summary>
        public string SubFamily { get; private set; }

        /// <summary>
        /// This is the formal font name. Together with the style indicators (e.g. Bold and Italic), this is what uniquely identifies the font to Windows.
        /// Use this property to create a System.Drawing.Font.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the native weight of this font.
        /// </summary>
        public TypographicFontWeight Weight { get; private set; }

        /// <summary>
        /// If true, this font is natively bold.
        /// </summary>
        public bool Bold => OS2Info.Style.HasFlag(FontReader.FontStyle.Bold);
        /// <summary>
        /// If true, this font is natively italic.
        /// </summary>
        public bool Italic => OS2Info.Style.HasFlag(FontReader.FontStyle.Italic);
        /// <summary>
        /// If true, this font is natively oblique.
        /// </summary>
        public bool Oblique => OS2Info.Style.HasFlag(FontReader.FontStyle.Oblique);
        /// <summary>
        /// If true, this font is natively underlined.
        /// </summary>
        public bool Underlined => OS2Info.Style.HasFlag(FontReader.FontStyle.Underscore);
        /// <summary>
        /// If true, this font is natively negative.
        /// </summary>
        public bool Negative => OS2Info.Style.HasFlag(FontReader.FontStyle.Negative);
        /// <summary>
        /// If true, this font is natively outline.
        /// </summary>
        public bool Outlined => OS2Info.Style.HasFlag(FontReader.FontStyle.Outlined);
        /// <summary>
        /// If true, this font is natively overstruck.
        /// </summary>
        public bool Strikeout => OS2Info.Style.HasFlag(FontReader.FontStyle.Strikeout);
        /// <summary>
        /// Characters are in the standard weight/style for the font.
        /// </summary>
        public bool Regular => OS2Info.Style.HasFlag(FontReader.FontStyle.Regular);
        /// <summary>
        /// Gets the font container's location on disk.
        /// </summary>
        public string FileName { get; private set; }

        public FontStyle FontStyle
        {
            get
            {
                var fs = (FontStyle)0;
                if (Bold) fs |= FontStyle.Bold;
                if (Italic) fs |= FontStyle.Italic;
                if (Underlined) fs |= FontStyle.Underline;
                if (Strikeout) fs |= FontStyle.Strikeout;

                return fs;
            }
        }

        public Panose Panose { get; private set; }

        public FontReader.FamilyNamesInfo FamilyNames { get; private set; }
        public FontReader.OS2Info OS2Info { get; private set; }

        public TypographicFont(FontReader.FamilyNamesInfo names, FontReader.OS2Info os2info, string fileName)
        {
            FamilyNames = names;
            OS2Info = os2info;

            Name = names.FontName;
            Family = names.TypographicFamily;
            SubFamily = names.TypographicSubfamily;

            Weight = os2info.Weight;
            FileName = fileName;

            Panose = new Panose(os2info.Panose);
        }


        public override string ToString()
            => SubFamily == null ? Family : Family + " " + SubFamily;


        /// <summary>
        /// Gets a cached list of all OpenType fonts installed on the current system. This includes TTF, OTF and TTC formats.
        /// </summary>
        public static IReadOnlyList<string> GetInstalledFontFiles()
        {
            var r = new List<string>();
            var defaultFolder = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);

            using (var installedFontsKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Fonts"))
                if (installedFontsKey != null)
                    foreach (var valueName in installedFontsKey.GetValueNames())
                    {
                        var filename = installedFontsKey.GetValue(valueName) as string;
                        if (string.IsNullOrWhiteSpace(filename)) continue;
                        if (!Path.IsPathRooted(filename)) filename = Path.Combine(defaultFolder, filename);
                        r.Add(filename);
                    }

            return r;
        }

        /// <summary>
        /// Parses an OpenType font container file. Supports TTF, OTF and TTC formats.
        /// </summary>
        public static TypographicFont[] FromFile(string filename)
        {
            return FontReader.Read(filename);
        }
    }
}