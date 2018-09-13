using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Navigation.DeterministicMath
{
    public struct DeterministicFloat
    {
        public long RawValue;
        public const int SHIFT_AMOUNT = 12; //12 is 4096

        public const long One = 1 << SHIFT_AMOUNT;
        public const int OneI = 1 << SHIFT_AMOUNT;
        public static DeterministicFloat OneF = new DeterministicFloat(1, true);
        public static readonly DeterministicFloat Epsilon = new DeterministicFloat(10, false);

        #region Constructors

        public DeterministicFloat(long StartingRawValue, bool UseMultiple)
        {
            this.RawValue = StartingRawValue;
            if (UseMultiple)
                this.RawValue = this.RawValue << SHIFT_AMOUNT;
        }

        public DeterministicFloat(long StartingRawValue) : this(StartingRawValue, true)
        {
        }

        public DeterministicFloat(double DoubleValue)
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

        public DeterministicFloat Inverse
        {
            get { return new DeterministicFloat(-this.RawValue, false); }
        }

        #region FromParts
        /// <summary>
        /// Create a fixed-int number from parts.  For example, to create 1.5 pass in 1 and 500.
        /// </summary>
        /// <param name="PreDecimal">The number above the decimal.  For 1.5, this would be 1.</param>
        /// <param name="PostDecimal">The number below the decimal, to three digits.
        /// For 1.5, this would be 500. For 1.005, this would be 5.</param>
        /// <returns>A fixed-int representation of the number parts</returns>
        public static DeterministicFloat FromParts(int PreDecimal, int PostDecimal)
        {
            DeterministicFloat f = new DeterministicFloat(PreDecimal);
            if (PostDecimal != 0)
                f.RawValue += (new DeterministicFloat(PostDecimal) / 1000).RawValue;

            return f;
        }
        #endregion

        #region *
        public static DeterministicFloat operator *(DeterministicFloat one, DeterministicFloat other)
        {
            long result = (one.RawValue * other.RawValue) >> SHIFT_AMOUNT;
            return new DeterministicFloat(result, false);
        }

        public static DeterministicFloat operator *(DeterministicFloat one, int multi)
        {
            return one * (DeterministicFloat)multi;
        }

        public static DeterministicFloat operator *(int multi, DeterministicFloat one)
        {
            return one * (DeterministicFloat)multi;
        }
        #endregion

        #region /
        public static DeterministicFloat operator /(DeterministicFloat one, DeterministicFloat other)
        {
            return new DeterministicFloat((one.RawValue << SHIFT_AMOUNT) / (other.RawValue), false);
        }

        public static DeterministicFloat operator /(DeterministicFloat one, int divisor)
        {
            return one / (DeterministicFloat)divisor;
        }

        public static DeterministicFloat operator /(int divisor, DeterministicFloat one)
        {
            return (DeterministicFloat)divisor / one;
        }
        #endregion

        #region %
        public static DeterministicFloat operator %(DeterministicFloat one, DeterministicFloat other)
        {
            return new DeterministicFloat((one.RawValue) % (other.RawValue), false);
        }

        public static DeterministicFloat operator %(DeterministicFloat one, int divisor)
        {
            return one % (DeterministicFloat)divisor;
        }

        public static DeterministicFloat operator %(int divisor, DeterministicFloat one)
        {
            return (DeterministicFloat)divisor % one;
        }
        #endregion

        #region +
        public static DeterministicFloat operator +(DeterministicFloat one, DeterministicFloat other)
        {
            return new DeterministicFloat(one.RawValue + other.RawValue, false);
        }

        public static DeterministicFloat operator ++(DeterministicFloat one)
        {
            return one + (DeterministicFloat)1;
        }

        public static DeterministicFloat operator --(DeterministicFloat one)
        {
            return one - (DeterministicFloat)1;
        }

        public static DeterministicFloat operator +(DeterministicFloat one, int other)
        {
            return one + (DeterministicFloat)other;
        }

        public static DeterministicFloat operator +(int other, DeterministicFloat one)
        {
            return one + (DeterministicFloat)other;
        }
        #endregion

        #region -
        public static DeterministicFloat operator -(DeterministicFloat one, DeterministicFloat other)
        {
            return new DeterministicFloat(one.RawValue - other.RawValue, false);
        }

        public static DeterministicFloat operator -(DeterministicFloat one)
        {
            return new DeterministicFloat(-one.RawValue, false);
        }

        public static DeterministicFloat operator -(DeterministicFloat one, int other)
        {
            return one - (DeterministicFloat)other;
        }

        public static DeterministicFloat operator -(int other, DeterministicFloat one)
        {
            return (DeterministicFloat)other - one;
        }
        #endregion

        #region ==
        public static bool operator ==(DeterministicFloat one, DeterministicFloat other)
        {
            return one.RawValue == other.RawValue;
        }

        public static bool operator ==(DeterministicFloat one, int other)
        {
            return one == (DeterministicFloat)other;
        }

        public static bool operator ==(int other, DeterministicFloat one)
        {
            return (DeterministicFloat)other == one;
        }
        #endregion

        #region !=
        public static bool operator !=(DeterministicFloat one, DeterministicFloat other)
        {
            return one.RawValue != other.RawValue;
        }

        public static bool operator !=(DeterministicFloat one, int other)
        {
            return one != (DeterministicFloat)other;
        }

        public static bool operator !=(int other, DeterministicFloat one)
        {
            return (DeterministicFloat)other != one;
        }
        #endregion

        #region >=
        public static bool operator >=(DeterministicFloat one, DeterministicFloat other)
        {
            return one.RawValue >= other.RawValue;
        }

        public static bool operator >=(DeterministicFloat one, int other)
        {
            return one >= (DeterministicFloat)other;
        }

        public static bool operator >=(int other, DeterministicFloat one)
        {
            return (DeterministicFloat)other >= one;
        }
        #endregion

        #region <=
        public static bool operator <=(DeterministicFloat one, DeterministicFloat other)
        {
            return one.RawValue <= other.RawValue;
        }

        public static bool operator <=(DeterministicFloat one, int other)
        {
            return one <= (DeterministicFloat)other;
        }

        public static bool operator <=(int other, DeterministicFloat one)
        {
            return (DeterministicFloat)other <= one;
        }
        #endregion

        #region >
        public static bool operator >(DeterministicFloat one, DeterministicFloat other)
        {
            return one.RawValue > other.RawValue;
        }

        public static bool operator >(DeterministicFloat one, int other)
        {
            return one > (DeterministicFloat)other;
        }

        public static bool operator >(int other, DeterministicFloat one)
        {
            return (DeterministicFloat)other > one;
        }
        #endregion

        #region <
        public static bool operator <(DeterministicFloat one, DeterministicFloat other)
        {
            return one.RawValue < other.RawValue;
        }

        public static bool operator <(DeterministicFloat one, int other)
        {
            return one < (DeterministicFloat)other;
        }

        public static bool operator <(int other, DeterministicFloat one)
        {
            return (DeterministicFloat)other < one;
        }
        #endregion

        public static explicit operator int(DeterministicFloat src)
        {
            return (int)(src.RawValue >> SHIFT_AMOUNT);
        }

        public static implicit operator DeterministicFloat(int src)
        {
            return new DeterministicFloat(src, true);
        }

        public static implicit operator DeterministicFloat(long src)
        {
            return new DeterministicFloat(src, true);
        }

        public static implicit operator DeterministicFloat(ulong src)
        {
            return new DeterministicFloat((long)src, true);
        }

        public static DeterministicFloat operator <<(DeterministicFloat one, int Amount)
        {
            return new DeterministicFloat(one.RawValue << Amount, false);
        }

        public static DeterministicFloat operator >>(DeterministicFloat one, int Amount)
        {
            return new DeterministicFloat(one.RawValue >> Amount, false);
        }

        public override bool Equals(object obj)
        {
            if (obj is DeterministicFloat)
                return ((DeterministicFloat)obj).RawValue == this.RawValue;
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
        public static DeterministicFloat PI = new DeterministicFloat(12868, false); //PI x 2^12
        public static DeterministicFloat TwoPIF = PI * 2; //radian equivalent of 260 degrees
        public static DeterministicFloat HalfPIF = PI / 2;
        public static DeterministicFloat PIOver180F = PI / (DeterministicFloat)180; //PI / 180
        #endregion

        #region Sqrt
        public static DeterministicFloat Sqrt(DeterministicFloat f, int NumberOfIterations)
        {
            if (f.RawValue < 0) //NaN in Math.Sqrt
                throw new ArithmeticException("Input Error");
            if (f.RawValue == 0)
                return (DeterministicFloat)0;
            DeterministicFloat k = f + DeterministicFloat.OneF >> 1;
            for (int i = 0; i < NumberOfIterations; i++)
                k = (k + (f / k)) >> 1;

            if (k.RawValue < 0)
                throw new ArithmeticException("Overflow");
            else
                return k;
        }

        public static DeterministicFloat Sqrt(DeterministicFloat f)
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
        public static DeterministicFloat Sin(DeterministicFloat i)
        {
            DeterministicFloat j = (DeterministicFloat)0;
            for (; i < 0; i += new DeterministicFloat(25736, false)) ;
            if (i > new DeterministicFloat(25736, false))
                i %= new DeterministicFloat(25736, false);
            DeterministicFloat k = (i * new DeterministicFloat(10, false)) / new DeterministicFloat(714, false);
            if (i != 0 && i != new DeterministicFloat(6434, false) && i != new DeterministicFloat(12868, false) &&
                i != new DeterministicFloat(19302, false) && i != new DeterministicFloat(25736, false))
                j = (i * new DeterministicFloat(100, false)) / new DeterministicFloat(714, false) - k * new DeterministicFloat(10, false);
            if (k <= new DeterministicFloat(90, false))
                return sin_lookup(k, j);
            if (k <= new DeterministicFloat(180, false))
                return sin_lookup(new DeterministicFloat(180, false) - k, j);
            if (k <= new DeterministicFloat(270, false))
                return sin_lookup(k - new DeterministicFloat(180, false), j).Inverse;
            else
                return sin_lookup(new DeterministicFloat(360, false) - k, j).Inverse;
        }

        private static DeterministicFloat sin_lookup(DeterministicFloat i, DeterministicFloat j)
        {
            if (j > 0 && j < new DeterministicFloat(10, false) && i < new DeterministicFloat(90, false))
                return new DeterministicFloat(SIN_TABLE[i.RawValue], false) +
                    ((new DeterministicFloat(SIN_TABLE[i.RawValue + 1], false) - new DeterministicFloat(SIN_TABLE[i.RawValue], false)) /
                    new DeterministicFloat(10, false)) * j;
            else
                return new DeterministicFloat(SIN_TABLE[i.RawValue], false);
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

        private static DeterministicFloat mul(DeterministicFloat F1, DeterministicFloat F2)
        {
            return F1 * F2;
        }

        #region Cos, Tan, Asin
        public static DeterministicFloat Cos(DeterministicFloat i)
        {
            return Sin(i + new DeterministicFloat(6435, false));
        }

        public static DeterministicFloat Tan(DeterministicFloat i)
        {
            return Sin(i) / Cos(i);
        }

        public static DeterministicFloat Acos(DeterministicFloat f)
        {
            return Asin(f) * -1 + HalfPIF;
        }

        public static DeterministicFloat Asin(DeterministicFloat f)
        {
            bool isNegative = f < 0;
            f = Abs(f);

            if (f > DeterministicFloat.OneF)
                throw new ArithmeticException("Bad Asin Input:" + f.ToDouble());

            DeterministicFloat f1 = mul(mul(mul(mul(new DeterministicFloat(145103 >> DeterministicFloat.SHIFT_AMOUNT, false), f) -
                new DeterministicFloat(599880 >> DeterministicFloat.SHIFT_AMOUNT, false), f) +
                new DeterministicFloat(1420468 >> DeterministicFloat.SHIFT_AMOUNT, false), f) -
                new DeterministicFloat(3592413 >> DeterministicFloat.SHIFT_AMOUNT, false), f) +
                new DeterministicFloat(26353447 >> DeterministicFloat.SHIFT_AMOUNT, false);
            DeterministicFloat f2 = PI / new DeterministicFloat(2, true) - (Sqrt(DeterministicFloat.OneF - f) * f1);

            return isNegative ? f2.Inverse : f2;
        }
        #endregion

        #region ATan, ATan2
        public static DeterministicFloat Atan(DeterministicFloat F)
        {
            return Asin(F / Sqrt(DeterministicFloat.OneF + (F * F)));
        }

        public static DeterministicFloat Atan2(DeterministicFloat F1, DeterministicFloat F2)
        {
            if (F2.RawValue == 0 && F1.RawValue == 0)
                return (DeterministicFloat)0;

            DeterministicFloat result = (DeterministicFloat)0;
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
                result = (F1 >= 0 ? PI : PI.Inverse) / new DeterministicFloat(2, true);

            return result;
        }
        #endregion

        #region Abs
        public static DeterministicFloat Abs(DeterministicFloat F)
        {
            if (F < 0)
                return F.Inverse;
            else
                return F;
        }
        #endregion

        public static DeterministicFloat Max(DeterministicFloat x1, DeterministicFloat x2)
        {
            if (x1 < x2)
                return x2;

            return x1;
        }

        public static DeterministicFloat Min(DeterministicFloat x1, DeterministicFloat x2)
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
