using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Navigation.DeterministicMath;

namespace Navigation
{
    public struct DeterministicVector2
    {
        public DeterministicInt X;
        public DeterministicInt Y;
        public DeterministicVector2(int X, int Y)
        {
            this.X = new DeterministicInt(X, true);
            this.Y = new DeterministicInt(Y, true);
        }

        public DeterministicVector2(DeterministicInt X, DeterministicInt Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public DeterministicVector2(DeterministicVector2 pt)
        {
            this.X = pt.X;
            this.Y = pt.Y;
        }

        public static bool operator ==(DeterministicVector2 a, DeterministicVector2 b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(DeterministicVector2 a, DeterministicVector2 b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public static DeterministicVector2 operator -(DeterministicVector2 a, DeterministicVector2 b)
        {
            return new DeterministicVector2(a.X - b.X, a.Y - b.Y);
        }

        public static DeterministicVector2 operator +(DeterministicVector2 a, DeterministicVector2 b)
        {
            return new DeterministicVector2(a.X + b.X, a.Y + b.Y);
        }
        public static DeterministicVector2 operator *(DeterministicVector2 a, int b)
        {
            return new DeterministicVector2(a.X * b, a.Y * b);
        }

        public static DeterministicVector2 operator /(DeterministicVector2 a, int b)
        {
            return new DeterministicVector2(a.X / b, a.Y / b);
        }

        public DeterministicInt ManhattanHeuristic()
        {
            return DeterministicInt.Abs(X) + DeterministicInt.Abs(Y);
        }

        public DeterministicInt CrossProduct(DeterministicVector2 b)
        {
            return this.X * b.Y - this.Y * b.X;
        }

        public DeterministicInt DotProduct(DeterministicVector2 b)
        {
            return this.X * b.X + this.Y * b.Y;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is DeterministicVector2)
            {
                DeterministicVector2 a = (DeterministicVector2)obj;
                return (X == a.X) && (Y == a.Y);
            }
            else return false;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        /// <summary>
        /// Deeps the copy.
        /// </summary>
        /// <returns></returns>
        public DeterministicVector2 DeepCopy()
        {
            return new DeterministicVector2(X, Y);
        }

        public DeterministicInt GetLengthSquared()
        {
            return X * X + Y * Y;
        }

        public DeterministicInt GetLength()
        {
            return DeterministicInt.Sqrt(GetLengthSquared());
        }
    }
}
