using System;

namespace KTreeDataTypes
{
    /// <summary>
    /// Base class to implement data types for use in a k-tree structure.
    /// </summary>
    /// <typeparam name="T">A data type for use in a k-tree structure.</typeparam>
    [Serializable]
    public abstract class KTreeDataType<T> : IComparable, ICloneable where T : KTreeDataType<T>
    {
        /// <summary>
        /// Computes the sum of its operands.
        /// </summary>
        /// <param name="a">The first operand.</param>
        /// <param name="b">The second operand.</param>
        /// <returns>The result of the sum.</returns>
        public static T operator +(KTreeDataType<T> a, T b) => a.Add(b);

        /// <summary>
        /// Divides the first operand by the second operand.
        /// </summary>
        /// <param name="a">The first operand.</param>
        /// <param name="b">The second operand.</param>
        /// <returns>The result of the division.</returns>
        public static T operator /(KTreeDataType<T> a, int b) => a.Divide(b);

        /// <summary>
        /// Adds another object of the same type to the current instance. Used when calculating 

        /// </summary>
        /// <param name="other">Another KTreeDataType to add to this instance.</param>
        /// <returns>The result of the addition as a new object.</returns>
        public abstract T Add(T other);

        /// <summary>
        /// Compares the current instance with another object of the same type and returns
        /// an integer that indicates whether the current instance precedes, follows, or
        /// occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>A value that indicates the relative order of the objects being compared. The
        /// return value has these meanings:
        /// Value – Meaning
        /// Less than zero – This instance precedes obj in the sort order.
        /// Zero – This instance occurs in the same position in the sort order as obj.
        /// Greater than zero – This instance follows obj in the sort order.
        /// </returns>
        public abstract int CompareTo(object obj);

        /// <summary>
        /// Sets all properties of the current instance to their default value. Used when 
        /// re-calculating cluster means.
        /// </summary>
        public abstract void ClearProperties();

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public abstract object Clone();

        /// <summary>
        /// Calculates the distance (using a metric such as Euclidean distance) of the current 
        /// instance from another object of the same type.
        /// </summary>
        /// <param name="other">Another KTreeDataType to calculate the current instance's distance 
        /// from.</param>
        /// <returns>The result of the distance calculation as a double.</returns>
        public abstract double DistanceFrom(T other);

        /// <summary>
        /// Divides the current instance by an integer. Used when calculating cluster means.
        /// </summary>
        /// <param name="integer"></param>
        /// <returns>The result of the division as a new object.</returns>
        public abstract T Divide(int integer);

        /// <summary>
        /// Returns a string that represents the current object. For use when printing in the 
        /// console or debugging.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public abstract override string ToString();
    }
}
