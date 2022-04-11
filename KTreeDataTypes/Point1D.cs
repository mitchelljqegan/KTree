using System;

namespace KTreeDataTypes
{
    /// <summary>
    /// Represents point in 1-D space.
    /// </summary>
    [Serializable]
    public class Point1D : KTreeDataType<Point1D>
    {
        /// <summary>
        /// The x-coordinate of the point.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Constructor to create a Point object.
        /// </summary>
        /// <param name="x">The x-coordinate to be assigned to the point.</param>
        public Point1D(double x)
        {
            X = x;
        }

        public override Point1D Add(Point1D other)
        {
            return new(X + other.X);
        }

        public override void ClearProperties()
        {
            X = default;
        }

        public override object Clone()
        {
            return new Point1D(X);
        }

        public override int CompareTo(object obj)
        {
            if (obj != null)
            {
                return obj is Point1D otherPoint ? X.CompareTo(otherPoint.X) : throw new ArgumentException("Object is not a Point");
            }
            else
            {
                return 1;
            }
        }

        public override double DistanceFrom(Point1D otherPoint)
        {
            // Squared Euclidean distance
            return Math.Pow(X - otherPoint.X, 2);
        }

        public override Point1D Divide(int integer)
        {
            return new(X / integer);
        }

        public override string ToString()
        {
            return X.ToString();
        }
    }
}
