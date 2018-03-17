namespace jnm2.TypographicFonts
{
    public class Panose
    {
        private byte[] panose;

        public PanoseProportion Proportion => (PanoseProportion)panose[3];

        public Panose(byte[] panose)
        {
            this.panose = panose;
        }


        public enum PanoseProportion : byte
        {
            Any = 0,
            NoFit = 1,
            OldStyle = 2,
            Modern = 3,
            EvenWidth = 4,
            Expanded = 5,
            Condensed = 6,
            VeryExpanded = 7,
            VeryCondensed = 8,
            Monospaced = 9,
        }
    }
}