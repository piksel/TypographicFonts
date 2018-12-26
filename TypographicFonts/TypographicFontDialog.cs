using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace jnm2.TypographicFonts
{
    public partial class TypographicFontDialog : Form
    {
        public string FontName { get; set; }
        //Dictionary<string, TypoGra fonts;

        public float FontSize { get; set; }

        private TypographicFont selectedFont;

        public TypographicFontDialog()
        {
            InitializeComponent();
        }

        private void TypographicFontDialog_Load(object sender, EventArgs e)
        {
            /*
            for (int i=0; i < listView2.Items.Count; i++)
            {
                var size = float.Parse(listView2.Items[i].Text);
                if (FontSize > size) continue;
                if (FontSize < size)
                {
                    listView2.Items.Insert(i, FontSize.ToString());
                }
                listView2.Items[i].Selected = true;
                break;
            }
            */
            comboBox1.SelectedIndex = -1;
            comboBox1.Text = FontSize.ToString();

            foreach (var ff in TypographicFontFamily.InstalledFamilies)
            {
                //var fc = new PrivateFontCollection();
                var normalFont = ff.NormalFont;
                //fc.AddFontFile(normalFont.FileName);
                var font = new FontFamily(normalFont.Name);
                var famFont = ff.Fonts.FirstOrDefault(tf => tf.Name == FontName);
                if (famFont != null)
                {
                    selectedFont = famFont;
                }
                
                listView1.Items.Add(new ListViewItem()
                {
                    Text = ff.Name,
                    Font = new Font(font, listView1.Font.Size, listView1.Font.Unit),
                    ToolTipText = ff.Name,
                    Tag = ff,
                    Selected = famFont != null,
                    Group = normalFont.Panose.Proportion == Panose.PanoseProportion.Monospaced 
                        ? listView1.Groups["lvgMono"] 
                        : listView1.Groups["lvgNormal"]
                });
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView2.Items.Clear();
            if (listView1.SelectedIndices.Count < 1) return;

            var fontFamily = (listView1.SelectedItems[0].Tag as TypographicFontFamily);
            var normal = fontFamily.NormalFont;

            foreach(var font in fontFamily.Fonts.OrderBy( f => f.Weight ))
            {
                //if (font.Italic) continue;

                var family = new FontFamily(font.Name);
                var newItem = listView2.Items.Add(new ListViewItem()
                {
                    Text = font.SubFamily ?? "Regular" + (font.Bold ? " Bold" : "") + (font.Italic ? " Italic" : ""),
                    Font = new Font(font.Name, listView2.Font.Size, font.FontStyle, listView2.Font.Unit),
                    Tag = font,
                    Selected = (selectedFont != null) ? font == selectedFont : font == normal
                });


            }
            UpdatePreview();
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            if (listView2.SelectedIndices.Count < 1)
            {
                label1.Visible = false;
            }
            else
            {
                if (float.TryParse(comboBox1.Text, out float fontSize))
                {
                    label1.Visible = true;
                    var font = (listView2.SelectedItems[0].Tag as TypographicFont);
                    var family = new FontFamily(font.Name);

                    label1.Font = new Font(family, fontSize, font.FontStyle);
                }
                else
                {
                    label1.Visible = false;
                }
            }
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }
    }
}
