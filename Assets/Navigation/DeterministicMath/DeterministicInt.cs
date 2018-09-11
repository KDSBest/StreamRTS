using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Navigation.DeterministicMath
{
    public struct DeterministicInt
    {
        public long RawValue;
        public const int SHIFT_AMOUNT = 12; //12 is 4096

        public const long One = 1 << SHIFT_AMOUNT;
        public const int OneI = 1 << SHIFT_AMOUNT;
        public static DeterministicInt OneF = new DeterministicInt(1, true);
        public static readonly DeterministicInt Epsilon = new DeterministicInt(10, false);

        #region Constructors

        public DeterministicInt(long StartingRawValue, bool UseMultiple)
        {
            this.RawValue = StartingRawValue;
            if (UseMultiple)
                this.RawValue = this.RawValue << SHIFT_AMOUNT;
        }

        public DeterministicInt(long StartingRawValue) : this(StartingRawValue, true)
        {
        }

        public DeterministicInt(double DoubleValue)
        {
            DoubleValue *= (double)One;
            this.RawValue = (int)Math.Round(DoubleValue);
        }
        #endregion

        public int IntValue
        {
            get { return (int)(this.RawValue >> SHIFT_AMOUNT); }
        }

        public int ToInt()
        {
            return (int)(this.RawValue >> SHIFT_AMOUNT);
        }

        public double ToDouble()
        {
            return (double)this.RawValue / (double)One;
        }

        public DeterministicInt Inverse
        {
            get { return new DeterministicInt(-this.RawValue, false); }
        }

        #region FromParts
        /// <summary>
        /// Create a fixed-int number from parts.  For example, to create 1.5 pass in 1 and 500.
        /// </summary>
        /// <param name="PreDecimal">The number above the decimal.  For 1.5, this would be 1.</param>
        /// <param name="PostDecimal">The number below the decimal, to three digits.
        /// For 1.5, this would be 500. For 1.005, this would be 5.</param>
        /// <returns>A fixed-int representation of the number parts</returns>
        public static DeterministicInt FromParts(int PreDecimal, int PostDecimal)
        {
            DeterministicInt f = new DeterministicInt(PreDecimal);
            if (PostDecimal != 0)
                f.RawValue += (new DeterministicInt(PostDecimal) / 1000).RawValue;

            return f;
        }
        #endregion

        #region *
        public static DeterministicInt operator *(DeterministicInt one, DeterministicInt other)
        {
            return new DeterministicInt((one.RawValue * other.RawValue) >> SHIFT_AMOUNT, false);
        }

        public static DeterministicInt operator *(DeterministicInt one, int multi)
        {
            return one * (DeterministicInt)multi;
        }

        public static DeterministicInt operator *(int multi, DeterministicInt one)
        {
            return one * (DeterministicInt)multi;
        }
        #endregion

        #region /
        public static DeterministicInt operator /(DeterministicInt one, DeterministicInt other)
        {
            return new DeterministicInt((one.RawValue << SHIFT_AMOUNT) / (other.RawValue), false);
        }

        public static DeterministicInt operator /(DeterministicInt one, int divisor)
        {
            return one / (DeterministicInt)divisor;
        }

        public static DeterministicInt operator /(int divisor, DeterministicInt one)
        {
            return (DeterministicInt)divisor / one;
        }
        #endregion

        #region %
        public static DeterministicInt operator %(DeterministicInt one, DeterministicInt other)
        {
            return new DeterministicInt((one.RawValue) % (other.RawValue), false);
        }

        public static DeterministicInt operator %(DeterministicInt one, int divisor)
        {
            return one % (DeterministicInt)divisor;
        }

        public static DeterministicInt operator %(int divisor, DeterministicInt one)
        {
            return (DeterministicInt)divisor % one;
        }
        #endregion

        #region +
        public static DeterministicInt operator +(DeterministicInt one, DeterministicInt other)
        {
            return new DeterministicInt(one.RawValue + other.RawValue, false);
        }

        public static DeterministicInt operator ++(DeterministicInt one)
        {
            return one + (DeterministicInt)1;
        }

        public static DeterministicInt operator --(DeterministicInt one)
        {
            return one - (DeterministicInt)1;
        }

        public static DeterministicInt operator +(DeterministicInt one, int other)
        {
            return one + (DeterministicInt)other;
        }

        public static DeterministicInt operator +(int other, DeterministicInt one)
        {
            return one + (DeterministicInt)other;
        }
        #endregion

        #region -
        public static DeterministicInt operator -(DeterministicInt one, DeterministicInt other)
        {
            return new DeterministicInt(one.RawValue - other.RawValue, false);
        }

        public static DeterministicInt operator -(DeterministicInt one)
        {
            return new DeterministicInt(-one.RawValue, false);
        }

        public static DeterministicInt operator -(DeterministicInt one, int other)
        {
            return one - (DeterministicInt)other;
        }

        public static DeterministicInt operator -(int other, DeterministicInt one)
        {
            return (DeterministicInt)other - one;
        }
        #endregion

        #region ==
        public static bool operator ==(DeterministicInt one, DeterministicInt other)
        {
            return one.RawValue == other.RawValue;
        }

        public static bool operator ==(DeterministicInt one, int other)
        {
            return one == (DeterministicInt)other;
        }

        public static bool operator ==(int other, DeterministicInt one)
        {
            return (DeterministicInt)other == one;
        }
        #endregion

        #region !=
        public static bool operator !=(DeterministicInt one, DeterministicInt other)
        {
            return one.RawValue != other.RawValue;
        }

        public static bool operator !=(DeterministicInt one, int other)
        {
            return one != (DeterministicInt)other;
        }

        public static bool operator !=(int other, DeterministicInt one)
        {
            return (DeterministicInt)other != one;
        }
        #endregion

        #region >=
        public static bool operator >=(DeterministicInt one, DeterministicInt other)
        {
            return one.RawValue >= other.RawValue;
        }

        public static bool operator >=(DeterministicInt one, int other)
        {
            return one >= (DeterministicInt)other;
        }

        public static bool operator >=(int other, DeterministicInt one)
        {
            return (DeterministicInt)other >= one;
        }
        #endregion

        #region <=
        public static bool operator <=(DeterministicInt one, DeterministicInt other)
        {
            return one.RawValue <= other.RawValue;
        }

        public static bool operator <=(DeterministicInt one, int other)
        {
            return one <= (DeterministicInt)other;
        }

        public static bool operator <=(int other, DeterministicInt one)
        {
            return (DeterministicInt)other <= one;
        }
        #endregion

        #region >
        public static bool operator >(DeterministicInt one, DeterministicInt other)
        {
            return one.RawValue > other.RawValue;
        }

        public static bool operator >(DeterministicInt one, int other)
        {
            return one > (DeterministicInt)other;
        }

        public static bool operator >(int other, DeterministicInt one)
        {
            return (DeterministicInt)other > one;
        }
        #endregion

        #region <
        public static bool operator <(DeterministicInt one, DeterministicInt other)
        {
            return one.RawValue < other.RawValue;
        }

        public static bool operator <(DeterministicInt one, int other)
        {
            return one < (DeterministicInt)other;
        }

        public static bool operator <(int other, DeterministicInt one)
        {
            return (DeterministicInt)other < one;
        }
        #endregion

        public static explicit operator int(DeterministicInt src)
        {
            return (int)(src.RawValue >> SHIFT_AMOUNT);
        }

        public static implicit operator DeterministicInt(int src)
        {
            return new DeterministicInt(src, true);
        }

        public static implicit operator DeterministicInt(long src)
        {
            return new DeterministicInt(src, true);
        }

        public static implicit operator DeterministicInt(ulong src)
        {
            return new DeterministicInt((long)src, true);
        }

        public static DeterministicInt operator <<(DeterministicInt one, int Amount)
        {
            return new DeterministicInt(one.RawValue << Amount, false);
        }

        public static DeterministicInt operator >>(DeterministicInt one, int Amount)
        {
            return new DeterministicInt(one.RawValue >> Amount, false);
        }

        public override bool Equals(object obj)
        {
            if (obj is DeterministicInt)
                return ((DeterministicInt)obj).RawValue == this.RawValue;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return RawValue.GetHashCode();
        }

        public override string ToString()
        {
            return this.RawValue.ToString();
        }

        #region PI, DoublePI
        public static DeterministicInt PI = new DeterministicInt(12868, false); //PI x 2^12
        public static DeterministicInt TwoPIF = PI * 2; //radian equivalent of 260 degrees
        public static DeterministicInt PIOver180F = PI / (DeterministicInt)180; //PI / 180
        #endregion

        #region Sqrt
        public static DeterministicInt Sqrt(DeterministicInt f, int NumberOfIterations)
        {
            if (f.RawValue < 0) //NaN in Math.Sqrt
                throw new ArithmeticException("Input Error");
            if (f.RawValue == 0)
                return (DeterministicInt)0;
            DeterministicInt k = f + DeterministicInt.OneF >> 1;
            for (int i = 0; i < NumberOfIterations; i++)
                k = (k + (f / k)) >> 1;

            if (k.RawValue < 0)
                throw new ArithmeticException("Overflow");
            else
                return k;
        }

        public static DeterministicInt Sqrt(DeterministicInt f)
        {
            byte numberOfIterations = 8;
            if (f.RawValue > 0x64000)
                numberOfIterations = 12;
            if (f.RawValue > 0x3e8000)
                numberOfIterations = 16;
            return Sqrt(f, numberOfIterations);
        }
        #endregion

        #region Sin
        public static DeterministicInt Sin(DeterministicInt i)
        {
            DeterministicInt j = (DeterministicInt)0;
            for (; i < 0; i += new DeterministicInt(25736, false)) ;
            if (i > new DeterministicInt(25736, false))
                i %= new DeterministicInt(25736, false);
            DeterministicInt k = (i * new DeterministicInt(10, false)) / new DeterministicInt(714, false);
            if (i != 0 && i != new DeterministicInt(6434, false) && i != new DeterministicInt(12868, false) &&
                i != new DeterministicInt(19302, false) && i != new DeterministicInt(25736, false))
                j = (i * new DeterministicInt(100, false)) / new DeterministicInt(714, false) - k * new DeterministicInt(10, false);
            if (k <= new DeterministicInt(90, false))
                return sin_lookup(k, j);
            if (k <= new DeterministicInt(180, false))
                return sin_lookup(new DeterministicInt(180, false) - k, j);
            if (k <= new DeterministicInt(270, false))
                return sin_lookup(k - new DeterministicInt(180, false), j).Inverse;
            else
                return sin_lookup(new DeterministicInt(360, false) - k, j).Inverse;
        }

        private static DeterministicInt sin_lookup(DeterministicInt i, DeterministicInt j)
        {
            if (j > 0 && j < new DeterministicInt(10, false) && i < new DeterministicInt(90, false))
                return new DeterministicInt(SIN_TABLE[i.RawValue], false) +
                    ((new DeterministicInt(SIN_TABLE[i.RawValue + 1], false) - new DeterministicInt(SIN_TABLE[i.RawValue], false)) /
                    new DeterministicInt(10, false)) * j;
            else
                return new DeterministicInt(SIN_TABLE[i.RawValue], false);
        }

        private static int[] SIN_TABLE = {
        0, 71, 142, 214, 285, 357, 428, 499, 570, 641,
        711, 781, 851, 921, 990, 1060, 1128, 1197, 1265, 1333,
        1400, 1468, 1534, 1600, 1665, 1730, 1795, 1859, 1922, 1985,
        2048, 2109, 2170, 2230, 2290, 2349, 2407, 2464, 2521, 2577,
        2632, 2686, 2740, 2793, 2845, 2896, 2946, 2995, 3043, 3091,
        3137, 3183, 3227, 3271, 3313, 3355, 3395, 3434, 3473, 3510,
        3547, 3582, 3616, 3649, 3681, 3712, 3741, 3770, 3797, 3823,
        3849, 3872, 3895, 3917, 3937, 3956, 3974, 3991, 4006, 4020,
        4033, 4045, 4056, 4065, 4073, 4080, 4086, 4090, 4093, 4095,
        4096
    };
        #endregion

        private static DeterministicInt mul(DeterministicInt F1, DeterministicInt F2)
        {
            return F1 * F2;
        }

        #region Cos, Tan, Asin
        public static DeterministicInt Cos(DeterministicInt i)
        {
            return Sin(i + new DeterministicInt(6435, false));
        }

        public static DeterministicInt Tan(DeterministicInt i)
        {
            return Sin(i) / Cos(i);
        }

        public static DeterministicInt Asin(DeterministicInt F)
        {
            bool isNegative = F < 0;
            F = Abs(F);

            if (F > DeterministicInt.OneF)
                throw new ArithmeticException("Bad Asin Input:" + F.ToDouble());

            DeterministicInt f1 = mul(mul(mul(mul(new DeterministicInt(145103 >> DeterministicInt.SHIFT_AMOUNT, false), F) -
                new DeterministicInt(599880 >> DeterministicInt.SHIFT_AMOUNT, false), F) +
                new DeterministicInt(1420468 >> DeterministicInt.SHIFT_AMOUNT, false), F) -
                new DeterministicInt(3592413 >> DeterministicInt.SHIFT_AMOUNT, false), F) +
                new DeterministicInt(26353447 >> DeterministicInt.SHIFT_AMOUNT, false);
            DeterministicInt f2 = PI / new DeterministicInt(2, true) - (Sqrt(DeterministicInt.OneF - F) * f1);

            return isNegative ? f2.Inverse : f2;
        }
        #endregion

        #region ATan, ATan2
        public static DeterministicInt Atan(DeterministicInt F)
        {
            return Asin(F / Sqrt(DeterministicInt.OneF + (F * F)));
        }

        public static DeterministicInt Atan2(DeterministicInt F1, DeterministicInt F2)
        {
            if (F2.RawValue == 0 && F1.RawValue == 0)
                return (DeterministicInt)0;

            DeterministicInt result = (DeterministicInt)0;
            if (F2 > 0)
                result = Atan(F1 / F2);
            else if (F2 < 0)
            {
                if (F1 >= 0)
                    result = (PI - Atan(Abs(F1 / F2)));
                else
                    result = (PI - Atan(Abs(F1 / F2))).Inverse;
            }
            else
                result = (F1 >= 0 ? PI : PI.Inverse) / new DeterministicInt(2, true);

            return result;
        }
        #endregion

        #region Abs
        public static DeterministicInt Abs(DeterministicInt F)
        {
            if (F < 0)
                return F.Inverse;
            else
                return F;
        }
        #endregion

        public static DeterministicInt Max(DeterministicInt x1, DeterministicInt x2)
        {
            if (x1 < x2)
                return x2;

            return x1;
        }

        public static DeterministicInt Min(DeterministicInt x1, DeterministicInt x2)
        {
            if (x2 < x1)
                return x2;

            return x1;
        }

        public float ToFloat()
        {
            return (float) ToDouble();
        }
    }

}
