using System;

namespace KTreeDataTypes
{
    /// <summary>
    /// Represents a point in 2-D space.
    /// </summary>
    [Serializable]
    public class Point2D : KTreeDataType<Point2D>
    {
        /// <summary>
        /// The x-coordinate of the point.
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// The y-coordinate of the point.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Constructor to create a Point2D object.
        /// </summary>
        /// <param name="x">The x-coordinate to be assigned to the point.</param>
        /// <param name="y">The y-coordinate to be assigned to the point.</param>
        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override Point2D Add(Point2D other)
        {
            return new(X + other.X, Y + other.Y);
        }

        public override void ClearProperties()
        {
            X = default;
            Y = default;
        }

        public override object Clone()
        {
            return new Point2D(X, Y);
        }

        public override int CompareTo(object obj)
        {
            if (obj != null)
            {
                if (obj is Point2D otherPoint)
                {
                    int xOrder = X.CompareTo(otherPoint.X);

                    return xOrder == 0 ? Y.CompareTo(otherPoint.Y) : xOrder;
                }
                else
                {
                    throw new ArgumentException("Object is not a Point2D");
                }
            }
            else
            {
                return 1;
            }
        }

        public override double DistanceFrom(Point2D otherPoint2D)
        {
            // Squared Euclidean distance
            return Math.Pow(X - otherPoint2D.X, 2) + Math.Pow(Y - otherPoint2D.Y, 2);
        }

        public override Point2D Divide(int integer)
        {
            return new(X / integer, Y / integer);
        }

        public override string ToString()
        {
            return string.Format("({0},{1})", X, Y);
        }
    }
}
