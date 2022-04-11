using System;

namespace KTreeDataTypes
{
    /// <summary>
    /// Represents an RGB pixel in an image.
    /// </summary>
    [Serializable]
    public class Pixel : KTreeDataType<Pixel>
    {
        /// <summary>
        /// The x-coordinate of the pixel.
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// The y-coordinate of the pixel.
        /// </summary>
        public int Y { get; set; }
        /// <summary>
        /// The red component of the pixel.
        /// </summary>
        public double R { get; set; }
        /// <summary>
        /// The green component of the pixel.
        /// </summary>
        public double G { get; set; }
        /// <summary>
        /// The blue component of the pixel.
        /// </summary>
        public double B { get; set; }

        /// <summary>
        /// Public constructor to create a Pixel object.
        /// </summary>
        /// <param name="x">The x-coordinate to be assigned to the pixel.</param>
        /// <param name="y">The y-coordinate to be assigned to the pixel.</param>
        /// <param name="r">The red component to be assigned to the pixel.</param>
        /// <param name="g">The green component to be assigned to the pixel.</param>
        /// <param name="b">The blue component to be assigned to the pixel.</param>
        public Pixel(int x, int y, int r, int g, int b)
        {
            X = x;
            Y = y;
            R = r;
            G = g;
            B = b;
        }

        /// <summary>
        /// Private constructor to create a Pixel object without coordinates.
        /// </summary>
        /// <param name="r">The red component to be assigned to the pixel.</param>
        /// <param name="g">The green component to be assigned to the pixel.</param>
        /// <param name="b">The blue component to be assigned to the pixel.</param>
        private Pixel(double r, double g, double b)
        {
            R = r;
            G = g;
            B = b;
        }

        public override Pixel Add(Pixel other)
        {
            return new(R + other.R, G + other.G, B + other.B);
        }

        public override void ClearProperties()
        {
            R = default;
            G = default;
            B = default;
        }

        public override object Clone()
        {
            return new Pixel(R, G, B);
        }

        public override int CompareTo(object obj)
        {
            if (obj != null)
            {
                if (obj is Pixel otherPixel)
                {
                    int rOrder = R.CompareTo(otherPixel.R);

                    if (rOrder == 0)
                    {
                        int gOrder = G.CompareTo(otherPixel.G);

                        return gOrder == 0 ? B.CompareTo(otherPixel.B) : gOrder;
                    }
                    else
                    {
                        return rOrder;
                    }
                }
                else
                {
                    throw new ArgumentException("Object is not a Pixel");
                }
            }
            else
            {
                return 1;
            }
        }

        public override double DistanceFrom(Pixel otherPixel)
        {
            // Squared Euclidean distance.
            return Math.Sqrt(Math.Pow(R - otherPixel.R, 2) + Math.Pow(G - otherPixel.G, 2) + Math.Pow(B - otherPixel.B, 2));

        }

        public override Pixel Divide(int integer)
        {
            return new(R / integer, G / integer, B / integer);
        }

        public override string ToString()
        {
            return string.Format("(R:{0} G:{1} B:{2})", R, G, B);
        }
    }
}
