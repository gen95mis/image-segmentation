using System.Drawing;

namespace ImageSegmentation.Model.Helpers
{
    class Pixel
    {
        private int x;
        private int y;
        private int claster;  // Номер области
        private Color color;

        public Pixel(int x, int y, Color color)
        {
            X = x;
            Y = y;
            claster = -1;
            this.color = color;
        }

        public Pixel(int x, int y, Color color, int claster)
        {
            this.x = x;
            this.y = y;
            this.claster = claster;
            this.color = color;
        }

        public int X
        {
            get => x; 
            set => x = value;
        }

        public int Y
        {
            get => y; 
            set => y = value;
        }

        public int Claster
        {
            get => claster;
            set => claster = value;
        }

        public Color Color
        {
            get => color;
            set => color = value;
        }
    }
}
 