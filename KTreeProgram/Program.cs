using KTree;
using KTreeDataTypes;
using System.Drawing;
using static RandomArray.RandomArray;

namespace KTreeProgram
{
    internal partial class Program
    {
        static void Main()
        {
            int order = 5;

            //ConstructAndSavePoint1D(order, 0, 20, 10);
            //ConstructAndSavePoint2D(order, 0, 20, 10);
            //ConstructAndSavePixel(order);

            //KTree<Point1D> ktree = KTree<Point1D>.Open("Point1D.ktree");
            //KTree<Point2D> ktree = KTree<Point2D>.Open("Point2D.ktree");
            //ktree.Print();

            //KTree<Pixel> ktree = KTree<Pixel>.Open("image.ktree");
            //SaveClusteredImage(ktree, 0);
            //SaveClusteredImage(ktree, 1);
            //SaveClusteredImage(ktree, 2);
            //SaveClusteredImage(ktree, 3);
            //SaveClusteredImage(ktree, 4);
        }

        private static void ConstructAndSavePixel(int order)
        {
            // Generate observation array from image file
            Bitmap image = new("image.jpg");
            Pixel[] observations = new Pixel[image.Width * image.Height];

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color pixel = image.GetPixel(x, y);
                    observations[x * image.Height + y] = new(x, y, pixel.R, pixel.G, pixel.B);
                }
            }

            KTree<Pixel> kTree = new(observations, order);

            kTree = kTree.Construct();
            kTree.Save("image.ktree");
        }
        private static void ConstructAndSavePoint1D(int order, int minValue, int maxValue, int numValues)
        {
            Point1D[] observations = GenerateRandom1DArray(minValue, maxValue, numValues);
            KTree<Point1D> kTree = new(observations, order);
            kTree = kTree.Construct();
            kTree.Save("Point1D.ktree");
        }

        private static void ConstructAndSavePoint2D(int order, int minValue, int maxValue, int numValues)
        {
            Point2D[] observations = GenerateRandom2DArray(minValue, maxValue, numValues);
            KTree<Point2D> kTree = new(observations, order);
            kTree = kTree.Construct();
            kTree.Save("Point2D.ktree");
        }

        private static void SaveClusteredImage(KTree<Pixel> ktree, int depth)
        {
            Bitmap image = new("image.jpg");

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color pixel = image.GetPixel(x, y);
                    Pixel observation = new(x, y, pixel.R, pixel.G, pixel.B);

                    Pixel mean = ktree.FindMeanOf(observation, depth).Key;
                    image.SetPixel(observation.X, observation.Y, Color.FromArgb((int)mean.R, (int)mean.G, (int)mean.B));
                }
            }

            image.Save(string.Format("image-clustered{0}.jpg", depth));
        }
    }
}
