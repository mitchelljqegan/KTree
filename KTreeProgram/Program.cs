using KTree;
using KTreeDataTypes;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using static RandomArray.RandomArray;

namespace KTreeProgram
{
    internal partial class Program
    {
        static void Main()
        {
            int order = 5;

            // 1-D Point K-tree
            /*
            Point1D[] observations = GenerateRandom1DArray(0, 20, 10);
            KTree<Point1D> kTree = new(observations, order);
            kTree = kTree.Construct();
            //kTree.Save("Point1D.ktree");

            //kTree = KTree<Point1D>.Open("Point1D.ktree");
            kTree.Print();
            */
            // 2-D Point K-tree
            /*
            Point2D[] observations = GenerateRandom2DArray(0, 20, 10);
            KTree<Point2D> kTree = new(observations, order);
            kTree = kTree.Construct();
            //kTree.Save("Point2D.ktree");

            //kTree = KTree<Point2D>.Open("Point2D.ktree");
            kTree.Print();
            */
            // Pixel K-tree
            
            string imageFilePath = "image.jpg";
            Pixel[] observations = GetPixels(imageFilePath);
            KTree<Pixel> kTree = new(observations, order);
            kTree = kTree.Construct();
            //string kTreeFilePath = string.Format("{0}.ktree", Path.GetFileNameWithoutExtension(imageFilePath));
            //kTree.Save(kTreeFilePath);

            //kTree = KTree<Pixel>.Open(kTreeFilePath);
            GenerateClusteredImages(kTree, imageFilePath);
            
        }

        private static Pixel[] GetPixels(string filePath)
        {
            Bitmap image = new(filePath);
            Pixel[] observations = new Pixel[image.Width * image.Height];

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color pixel = image.GetPixel(x, y);
                    observations[x * image.Height + y] = new(x, y, pixel.R, pixel.G, pixel.B);
                }
            }

            return observations;
        }

        private static void GenerateClusteredImages(KTree<Pixel> ktree, string filePath)
        {
            Bitmap image = new(filePath);
            IEnumerator<KTreeKey<Pixel>>[] meansEnumerators = new IEnumerator<KTreeKey<Pixel>>[image.Width * image.Height];

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color pixel = image.GetPixel(x, y);
                    Pixel observation = new(x, y, pixel.R, pixel.G, pixel.B);
                    meansEnumerators[x * image.Height + y] = ktree.ClusterMeansOf(observation).GetEnumerator();
                }
            }

            int depth = 0;
            bool finished = false;

            while (!finished)
            {
                for (int i = 0; i < meansEnumerators.Length; i++)
                {
                    if (meansEnumerators[i].MoveNext())
                    {
                        Pixel mean = meansEnumerators[i].Current.Key;
                        image.SetPixel(i / image.Height, i % image.Height, Color.FromArgb((int)mean.R, (int)mean.G, (int)mean.B));
                    }
                    else
                    {
                        finished = true;
                        break;
                    }
                }

                if (!finished)
                {
                    string clusterFilePath = string.Format("{0}-clustered{1}{2}", Path.GetFileNameWithoutExtension(filePath), depth++, Path.GetExtension(filePath));
                    image.Save(clusterFilePath);
                }
            }
        }
    }
}
