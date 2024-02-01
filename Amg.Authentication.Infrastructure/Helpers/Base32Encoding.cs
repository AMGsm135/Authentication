using System;
using System.Linq;

namespace Amg.Authentication.Infrastructure.Helpers
{
    public static class Base32Encoding
    {
        /// <summary>
        /// تبدیل متن کد شده در مبنای 32 به معادل باینری
        /// </summary>
        /// <param name="input">متن کد شده</param>
        /// <returns>آرایه بایت</returns>
        public static byte[] Decode(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException(nameof(input));
            }

            input = input.TrimEnd('=');
            var byteCount = input.Length * 5 / 8;
            var returnArray = new byte[byteCount];

            byte curByte = 0, bitsRemaining = 8;
            int mask, arrayIndex = 0;

            foreach (var cValue in input.Select(CharToValue))
            {
                if (bitsRemaining > 5)
                {
                    mask = cValue << (bitsRemaining - 5);
                    curByte = (byte)(curByte | mask);
                    bitsRemaining -= 5;
                }
                else
                {
                    mask = cValue >> (5 - bitsRemaining);
                    curByte = (byte)(curByte | mask);
                    returnArray[arrayIndex++] = curByte;
                    curByte = (byte)(cValue << (3 + bitsRemaining));
                    bitsRemaining += 3;
                }
            }

            if (arrayIndex != byteCount)
                returnArray[arrayIndex] = curByte;

            return returnArray;
        }

        /// <summary>
        /// تبدیل آرایه بایت به متن کد شده در مبنای 32
        /// </summary>
        /// <param name="input">آرایه بایت</param>
        /// <returns>متن کد شده در مبنای 32</returns>
        public static string Encode(byte[] input)
        {
            if (input == null || input.Length == 0)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var charCount = (int)Math.Ceiling(input.Length / 5d) * 8;
            var returnArray = new char[charCount];

            byte nextChar = 0, bitsRemaining = 5;
            var arrayIndex = 0;

            foreach (var b in input)
            {
                nextChar = (byte)(nextChar | (b >> (8 - bitsRemaining)));
                returnArray[arrayIndex++] = ValueToChar(nextChar);

                if (bitsRemaining < 4)
                {
                    nextChar = (byte)((b >> (3 - bitsRemaining)) & 31);
                    returnArray[arrayIndex++] = ValueToChar(nextChar);
                    bitsRemaining += 5;
                }

                bitsRemaining -= 3;
                nextChar = (byte)((b << bitsRemaining) & 31);
            }

            if (arrayIndex != charCount)
            {
                returnArray[arrayIndex++] = ValueToChar(nextChar);
                while (arrayIndex != charCount) returnArray[arrayIndex++] = '='; //padding
            }

            return new string(returnArray);
        }


        private static int CharToValue(char c)
        {
            var value = (int)c;
            
            return value switch
            {
                //65-90 == uppercase letters
                var i when i < 91 && i > 64 => value - 65,
                //50-55 == numbers 2-7
                var i when i < 56 && i > 49 => value - 24,
                //97-122 == lowercase letters
                var i when i < 123 && i > 96 => value - 97,
                _ => throw new ArgumentException("Character is not a Base32 character.", nameof(c))
            };
        }

        private static char ValueToChar(byte b)
        {
            return b switch
            {
                var i when i < 26 => (char)(b + 65),
                var i when i < 32 => (char)(b + 24),
                _ => throw new ArgumentException("Byte is not a value Base32 value.", nameof(b))
            };
        }
    }
}
