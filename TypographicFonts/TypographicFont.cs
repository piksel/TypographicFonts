﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace jnm2.TypographicFonts
{
    [DebuggerDisplay("{ToString(),nq}")]
    public sealed partial class TypographicFont
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
        public bool Bold { get; private set; }
        /// <summary>
        /// If true, this font is natively italic.
        /// </summary>
        public bool Italic { get; private set; }
        /// <summary>
        /// If true, this font is natively oblique.
        /// </summary>
        public bool Oblique { get; private set; }
        /// <summary>
        /// If true, this font is natively underlined.
        /// </summary>
        public bool Underlined { get; private set; }
        /// <summary>
        /// If true, this font is natively negative.
        /// </summary>
        public bool Negative { get; private set; }
        /// <summary>
        /// If true, this font is natively outline.
        /// </summary>
        public bool Outlined { get; private set; }
        /// <summary>
        /// If true, this font is natively overstruck.
        /// </summary>
        public bool Strikeout { get; private set; }
        /// <summary>
        /// Characters are in the standard weight/style for the font.
        /// </summary>
        public bool Regular { get; private set; }
        /// <summary>
        /// Gets the font container's location on disk.
        /// </summary>
        public string FileName { get; private set; }
        
        private TypographicFont(string family, string subFamily, string name, TypographicFontWeight weight, bool bold, bool italic, bool oblique, bool underlined, bool negative, bool outlined, bool strikeout, bool regular, string fileName)
        internal Panose Panose { get; private set; }

        private TypographicFont(string family, string subFamily, string name, TypographicFontWeight weight, bool bold, bool italic, bool oblique, bool underlined, bool negative, bool outlined, bool strikeout, bool regular, string fileName, byte[] panose)
        {
            this.Family = family;
            this.SubFamily = subFamily;
            this.Name = name;
            this.Weight = weight;
            this.Bold = bold;
            this.Italic = italic;
            this.Underlined = underlined;
            this.Negative = negative;
            this.Outlined = outlined;
            this.Strikeout = strikeout;
            this.Regular = regular;
            this.FileName = fileName;
            this.Oblique = oblique;
            this.Panose = new Panose(panose);
        }

        public override string ToString()
        {
            return this.SubFamily == null ? this.Family : this.Family + " " + this.SubFamily;
        }
        

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