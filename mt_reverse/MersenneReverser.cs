using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mt_reverse
{
	// Code from http://b10l.com/reversing-the-mersenne-twister-rng-temper-function/
	class MersenneReverser
	{
		private const UInt32 TemperingMaskB = 0x9d2c5680;
		private const UInt32 TemperingMaskC = 0xefc60000;

		public static UInt32 undoTemper(UInt32 y)
		{
			y = undoTemperShiftL(y);
			y = undoTemperShiftT(y);
			y = undoTemperShiftS(y);
			y = undoTemperShiftU(y);
			return y;
		}
		public static UInt32 undoTemperShiftL(UInt32 y)
		{
			UInt32 last14 = y >> 18;
			UInt32 final = y ^ last14;
			return final;
		}
		public static UInt32 undoTemperShiftT(UInt32 y)
		{
			UInt32 first17 = y << 15;
			UInt32 final = y ^ (first17 & TemperingMaskC);
			return final;
		}
		public static UInt32 undoTemperShiftS(UInt32 y)
		{
			/*
			 * This one also sucked to figure out, but now I think i could write
			 * a general one.  This basically waterfalls and keeps restoring original
			 * bits then shifts the values down and xors again to restore more bits
			 * and keeps on doing it.
			 */
			UInt32 a = y << 7;
			UInt32 b = y ^ (a & TemperingMaskB);
			UInt32 c = b << 7;
			UInt32 d = y ^ (c & TemperingMaskB); // now we have 14 of the original
			UInt32 e = d << 7;
			UInt32 f = y ^ (e & TemperingMaskB); // now we have 21 of the original
			UInt32 g = f << 7;
			UInt32 h = y ^ (g & TemperingMaskB); // now we have 28 of the original
			UInt32 i = h << 7;  // now we have the original xor

			UInt32 final = y ^ (i & TemperingMaskB);
			return final;
		}
		public static UInt32 undoTemperShiftU(UInt32 y)
		{
			/*
			 * This was confusing to figure out.
			 * We know the first 11 bits are un-altered becuase they were
			 * xored with 0's.  We shift those 11 bits to the right and xor that with the 
			 * original which gives us the first 22 bits(b) of what it orginally was.  Now that we have the
			 * first 22 bits so we can shift that to the right 11 bits which gives us
			 * what the number was orginally xored with.  So then we just xor y with that and
			 * our number is restored!
			 */
			UInt32 a = y >> 11;
			UInt32 b = y ^ a;
			UInt32 c = b >> 11;
			UInt32 final = y ^ c;
			return final;
		}
	}
}
