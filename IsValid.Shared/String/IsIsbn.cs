﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace IsValid
{
    public enum IsbnVersion
    {
        Any = 0,
        Ten = 10,
        Thirteen = 13
    }

    public static class IsIsbn
    {
        /// <summary>
        /// Indicates whether supplied input is either in ISBN-10 digit format or ISBN-13 digit format.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="version">Valid options are: IsbnVersion.Ten, IsbnVersion.Thirteen or IsbnVersion.Any</param>
        /// <returns></returns>
        /// IsbnVersion
        public static bool Isbn(this ValidatableValue<string> inputVal, IsbnVersion version = IsbnVersion.Any)
        {
            var input = RemoveSpacesAndHyphens(inputVal.Value);
            switch (version)
            {
                case IsbnVersion.Any:
                    if (!IsIsbn10(input) && !IsIsbn13(input))
                    {
                        inputVal.AddError("Not ISBN 10 or 13");
                    }
                    return inputVal.IsValid;
                case IsbnVersion.Thirteen:
                    if (!IsIsbn13(input))
                    {
                        inputVal.AddError("Not ISBN 13");
                    }
                    return inputVal.IsValid;
                case IsbnVersion.Ten:
                    if (!IsIsbn10(input))
                    {
                        inputVal.AddError("Not ISBN 10");
                    }
                    return inputVal.IsValid;
            }
            throw new ArgumentOutOfRangeException(
                "version",
                string.Format("Isbn version {0} is not supported.", version));
        }

        /// <summary>
        /// Indicates whether supplied input is in ISBN 13 digit format.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static bool IsIsbn13(string input)
        {
            var checksum = 0;
            // Ensure that input only contains 13 numbers.
            if (!Regex.IsMatch(input, "^[0-9]{13}$"))
            {
                return false;
            }
            var factor = new[] { 1, 3 };
            for (var i = 0; i < 12; i++)
            {
                checksum += factor[i % 2] * int.Parse(input[i].ToString());
            }
            return int.Parse(input[12].ToString()) - ((10 - (checksum % 10)) % 10) == 0;
        }

        /// <summary>
        /// Indicates whether supplied input is in ISBN 10 digit format.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static bool IsIsbn10(string input)
        {
            var checksum = 0;
            // Ensure that input only contains 10 numbers OR 9 numbers and the letter X.
            if (!Regex.IsMatch(input, "^[0-9]{9}X|[0-9]{10}$"))
            {
                return false;
            }
            // Automatically multiply 9 (of the 10) numbers by their weight.
            for (var i = 0; i < 9; i++)
            {
                checksum += (i + 1) * int.Parse(input[i].ToString());
            }
            // Manually multiply the 10th number.
            if (input[9] == 'X')
            {
                checksum += 10 * 10;
            }
            else
            {
                checksum += 10 * int.Parse(input[9].ToString());
            }
            // Ensure that the checksum is a multiple of 11.
            return checksum % 11 == 0;
        }

        /// <summary>
        /// Remove all white-space and hyphen characters from input.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string RemoveSpacesAndHyphens(string input)
        {
            return Regex.Replace(input, "[\\s-]+", "");
        }
    }
}

