using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mt_reverse
{
	/// <summary>
	/// Code from http://takel.jp/mt/MersenneTwister.cs
	/// 
	/// C# Version Copyright (C) 2001-2004 Akihilo Kramot (Takel).
	/// C# porting from a C-program for MT19937, originaly coded by
	/// Takuji Nishimura, considering the suggestions by
	/// Topher Cooper and Marc Rieffel in July-Aug. 1997.
	/// This library is free software under the Artistic license:
	///
	/// You can find the original C-program at
	///     http://www.math.keio.ac.jp/~matumoto/mt.html
	/// </summary>
	public class MersenneTwister : Random
	{
		/* Period parameters */

		#region Private fields
		const uint LOWER_MASK = 0x7fffffff; /* least significant r bits */
		const int M = 397;
		const uint MATRIX_A = 0x9908b0df; /* constant vector a */
		const int N = 624;

		/* Tempering parameters */
		const uint TEMPERING_MASK_B = 0x9d2c5680;
		const uint TEMPERING_MASK_C = 0xefc60000;
		const uint UPPER_MASK = 0x80000000; /* most significant w-r bits */
		static readonly uint[] mag01 = { 0x0, MATRIX_A };

		public uint[] mt = new uint[N]; /* the array for the state vector  */

		short mti;
		#endregion

		/* initializing the array with a NONZERO seed */

		public MersenneTwister(uint seed)
		{
			/* setting initial seeds to mt[N] using         */
			/* the generator Line 25 of Table 1 in          */
			/* [KNUTH 1981, The Art of Computer Programming */
			/*    Vol. 2 (2nd Ed.), pp102]                  */
			mt[0] = seed & 0xffffffffU;
			for (mti = 1; mti < N; ++mti)
				mt[mti] = (69069 * mt[mti - 1]) & 0xffffffffU;
		}

		static uint TEMPERING_SHIFT_U(uint y)
		{
			return (y >> 11);
		}

		static uint TEMPERING_SHIFT_S(uint y)
		{
			return (y << 7);
		}

		static uint TEMPERING_SHIFT_T(uint y)
		{
			return (y << 15);
		}

		static uint TEMPERING_SHIFT_L(uint y)
		{
			return (y >> 18);
		}

		public uint GenerateUInt()
		{
			uint y;

			/* mag01[x] = x * MATRIX_A  for x=0,1 */
			if (mti >= N) /* generate N words at one time */
			{
				short kk = 0;

				for (; kk < N - M; ++kk)
				{
					y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
					mt[kk] = mt[kk + M] ^ (y >> 1) ^ mag01[y & 0x1];
				}

				for (; kk < N - 1; ++kk)
				{
					y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
					mt[kk] = mt[kk + (M - N)] ^ (y >> 1) ^ mag01[y & 0x1];
				}

				y = (mt[N - 1] & UPPER_MASK) | (mt[0] & LOWER_MASK);
				mt[N - 1] = mt[M - 1] ^ (y >> 1) ^ mag01[y & 0x1];

				mti = 0;
			}

			y = mt[mti++];
			y ^= TEMPERING_SHIFT_U(y);
			y ^= TEMPERING_SHIFT_S(y) & TEMPERING_MASK_B;
			y ^= TEMPERING_SHIFT_T(y) & TEMPERING_MASK_C;
			y ^= TEMPERING_SHIFT_L(y);

			return y;
		}

		public virtual uint NextUInt()
		{
			return GenerateUInt();
		}

		public virtual uint NextUInt(uint maxValue)
		{
			return (uint)(GenerateUInt() / ((double)uint.MaxValue / maxValue));
		}

		public virtual uint NextUInt(uint minValue, uint maxValue)
		{
			if (minValue >= maxValue)
				throw new ArgumentOutOfRangeException();

			return (uint)(GenerateUInt() / ((double)uint.MaxValue / (maxValue - minValue)) + minValue);
		}

		public override int Next()
		{
			return Next(int.MaxValue);
		}

		public override int Next(int maxValue) /* throws ArgumentOutOfRangeException */
		{
			if (maxValue <= 1)
			{
				if (maxValue < 0)
					throw new ArgumentOutOfRangeException();

				return 0;
			}

			return (int)(NextDouble() * maxValue);
		}

		public override int Next(int minValue, int maxValue)
		{
			if (maxValue < minValue)
				throw new ArgumentOutOfRangeException();
			if (maxValue == minValue)
				return minValue;
			return Next(maxValue - minValue) + minValue;
		}

		public override void NextBytes(byte[] buffer) /* throws ArgumentNullException*/
		{
			int bufLen = buffer.Length;

			if (buffer == null)
				throw new ArgumentNullException();

			for (int idx = 0; idx < bufLen; ++idx)
				buffer[idx] = (byte)Next(256);
		}

		public override double NextDouble()
		{
			return (double)GenerateUInt() / ((ulong)uint.MaxValue + 1);
		}
	}
}
