﻿using Jint.Runtime;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Jint.Native
{
    public sealed class JsNumber : JsValue, IEquatable<JsNumber>
    {
        // .NET double epsilon and JS epsilon have different values
        internal const double JavaScriptEpsilon = 2.2204460492503130808472633361816E-16;

        internal readonly double _value;

        // how many decimals to check when determining if double is actually an int
        internal const double DoubleIsIntegerTolerance = double.Epsilon * 100;

        private static readonly long NegativeZeroBits = BitConverter.DoubleToInt64Bits(-0.0);

        // we can cache most common values, doubles are used in indexing too at times so we also cache
        // integer values converted to doubles
        private const int NumbersMax = 1024 * 10;
        private static readonly JsNumber[] _doubleToJsValue = new JsNumber[NumbersMax];
        private static readonly JsNumber[] _intToJsValue = new JsNumber[NumbersMax];

        internal static readonly JsNumber DoubleNaN = new JsNumber(double.NaN);
        internal static readonly JsNumber DoubleNegativeOne = new JsNumber((double)-1);
        internal static readonly JsNumber DoublePositiveInfinity = new JsNumber(double.PositiveInfinity);
        internal static readonly JsNumber DoubleNegativeInfinity = new JsNumber(double.NegativeInfinity);
        private static readonly JsNumber IntegerNegativeOne = new JsNumber(-1);
        internal static readonly JsNumber NegativeZero = new JsNumber(-0d);
        internal static readonly JsNumber PositiveZero = new JsNumber(+0);

        internal static readonly JsNumber PI = new JsNumber(System.Math.PI);

        static JsNumber()
        {
            for (int i = 0; i < NumbersMax; i++)
            {
                _intToJsValue[i] = new JsNumber(i);
                _doubleToJsValue[i] = new JsNumber((double)i);
            }
        }

        public JsNumber(double value) : base(Types.Number)
        {
            _value = value;
        }

        public JsNumber(int value) : base(Types.Number)
        {
            _value = value;
        }

        public JsNumber(uint value) : base(Types.Number)
        {
            _value = value;
        }

        public override object ToObject()
        {
            var isAbs = System.Math.Abs(_value % 1) <= DoubleIsIntegerTolerance;
            if (isAbs
               && _value >= int.MinValue
               && _value <= int.MaxValue)
                return (int)_value;

            if (isAbs
               && _value >= long.MinValue
               && _value <= long.MaxValue)
                return (long)_value;

            return _value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JsNumber Create(double value)
        {
            // we can cache positive double zero, but not negative, -0 == 0 in C# but in JS it's a different story
            var temp = _doubleToJsValue;
            if ((value == 0 && BitConverter.DoubleToInt64Bits(value) != NegativeZeroBits || value >= 1)
                && value < temp.Length
                && System.Math.Abs(value % 1) <= DoubleIsIntegerTolerance)
            {
                return temp[(uint)value];
            }

            if (value == -1)
            {
                return DoubleNegativeOne;
            }

            return CreateNumberUnlikely(value);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static JsNumber CreateNumberUnlikely(double value)
        {
            if (value <= double.MaxValue && value >= double.MinValue)
            {
                return new JsNumber(value);
            }

            if (value == double.NegativeInfinity)
            {
                return DoubleNegativeInfinity;
            }

            if (value == double.PositiveInfinity)
            {
                return DoublePositiveInfinity;
            }

            if (double.IsNaN(value))
            {
                return DoubleNaN;
            }

            return new JsNumber(value);
        }

        internal static JsNumber Create(int value)
        {
            var temp = _intToJsValue;
            if ((uint)value < (uint)temp.Length)
            {
                return temp[value];
            }

            if (value == -1)
            {
                return IntegerNegativeOne;
            }

            return new JsNumber(value);
        }

        internal static JsNumber Create(uint value)
        {
            var temp = _intToJsValue;
            if (value < (uint)temp.Length)
            {
                return temp[value];
            }

            return new JsNumber(value);
        }

        internal static JsNumber Create(ulong value)
        {
            if (value < (ulong)_intToJsValue.Length)
            {
                return _intToJsValue[value];
            }

            return new JsNumber(value);
        }

        public override string ToString()
        {
            return _value.ToString(CultureInfo.InvariantCulture);
        }

        public override bool Equals(JsValue obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (!(obj is JsNumber number))
            {
                return false;
            }

            return Equals(number);
        }

        public bool Equals(JsNumber other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return _value == other._value;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }
    }
}