/*
 * USPostalAddress.cs
 *
 *   Created: 2023-01-05
 */
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

#nullable enable
#nullable enable
namespace Contacts
{
    public partial record class USPostalAddressBase
    {
        public const string RegexString =
            @"""^(?n:(?<address1>(\d{1,5}(\ 1\/[234])?(\x20[A-Z]([a-z])+)+ )|(P\.O\.\ Box\ \d{1,5}))\s{1,2}(?i:(?<address2>(((APT|B LDG|DEPT|FL|HNGR|LOT|PIER|RM|S(LIP|PC|T(E|OP))|TRLR|UNIT)\x20\w{1,5})|(BSMT|FRNT|LBBY|LOWR|OFC|PH|REAR|SIDE|UPPR)\.?)\s{1,2})?)(?<city>[A-Z]([a-z])+(\.?)(\x20[A-Z]([a-z])+){0,2})\, \x20(?<state>A[LKSZRAP]|C[AOT]|D[EC]|F[LM]|G[AU]|HI|I[ADL N]|K[SY]|LA|M[ADEHINOPST]|N[CDEHJMVY]|O[HKR]|P[ARW]|RI|S[CD] |T[NX]|UT|V[AIT]|W[AIVY])\x20(?<zipcode>(?!0{5})\d{5}(-\d {4})?))$""";

#if NET7_0_OR_GREATER
        [GeneratedRegex(
            RegexString,
            RegexOptions.Compiled
                | RegexOptions.CultureInvariant
                | RegexOptions.ExplicitCapture
                | RegexOptions.IgnoreCase
                | RegexOptions.IgnorePatternWhitespace
                | RegexOptions.Multiline
                | RegexOptions.RightToLeft
                | RegexOptions.Singleline
        )]
        public static partial Regx Regex();
#else
        private static readonly Regx _regex = new(RegexString);

        public static Regx Regex() => _regex;
#endif

        public static USPostalAddress Parse(string s)
        {
            var match = Regex().Match(s);
            return match.Success
                ? new USPostalAddress
                {
                    address2 = (string)
                        Convert.ChangeType(match.Groups["address2"]?.Value, typeof(string)),
                    city = (string)Convert.ChangeType(match.Groups["city"]?.Value, typeof(string)),
                    state = (string)
                        Convert.ChangeType(match.Groups["state"]?.Value, typeof(string)),
                    zipcode = (string)
                        Convert.ChangeType(match.Groups["zipcode"]?.Value, typeof(string)),
                }
                : throw new ArgumentException(
                    $"The string \"{s}\" does not match the regular expression \"{RegexString}\".",
                    nameof(s)
                );
        }

        public virtual string address1 { get; set; }
        public virtual string address2 { get; set; }
        public virtual string city { get; set; }
        public virtual string state { get; set; }
        public virtual string zipcode { get; set; }

        protected USPostalAddressBase() { }

        protected USPostalAddressBase(string s)
        {
            var match = Regex().Match(s);
            if (!match.Success)
            {
                throw new ArgumentException(
                    $"The string \"{s}\" does not match the regular expression \"{RegexString}\".",
                    nameof(s)
                );
            }

            address1 = (string)Convert.ChangeType(match.Groups["address1"]?.Value, typeof(string));
            address2 = (string)Convert.ChangeType(match.Groups["address2"]?.Value, typeof(string));
            city = (string)Convert.ChangeType(match.Groups["city"]?.Value, typeof(string));
            state = (string)Convert.ChangeType(match.Groups["state"]?.Value, typeof(string));
            zipcode = (string)Convert.ChangeType(match.Groups["zipcode"]?.Value, typeof(string));
        }
    }
}

#nullable enable
namespace Contacts
{
    public partial record class USPostalAddress : USPostalAddressBase
    {
        public new const string RegexString =
            @"""^(?n:(?<address1>(\d{1,5}(\ 1\/[234])?(\x20[A-Z]([a-z])+)+ )|(P\.O\.\ Box\ \d{1,5}))\s{1,2}(?i:(?<address2>(((APT|B LDG|DEPT|FL|HNGR|LOT|PIER|RM|S(LIP|PC|T(E|OP))|TRLR|UNIT)\x20\w{1,5})|(BSMT|FRNT|LBBY|LOWR|OFC|PH|REAR|SIDE|UPPR)\.?)\s{1,2})?)(?<city>[A-Z]([a-z])+(\.?)(\x20[A-Z]([a-z])+){0,2})\, \x20(?<state>A[LKSZRAP]|C[AOT]|D[EC]|F[LM]|G[AU]|HI|I[ADL N]|K[SY]|LA|M[ADEHINOPST]|N[CDEHJMVY]|O[HKR]|P[ARW]|RI|S[CD] |T[NX]|UT|V[AIT]|W[AIVY])\x20(?<zipcode>(?!0{5})\d{5}(-\d {4})?))$""";

#if NET7_0_OR_GREATER
        [GeneratedRegex(
            RegexString,
            RegexOptions.Compiled
                | RegexOptions.CultureInvariant
                | RegexOptions.ExplicitCapture
                | RegexOptions.IgnoreCase
                | RegexOptions.IgnorePatternWhitespace
                | RegexOptions.Multiline
                | RegexOptions.RightToLeft
                | RegexOptions.Singleline
        )]
        public static partial System.Text.RegularExpressions.Regex Regex();
#else
        private static readonly Regx _regex = new(RegexString);

        public static new Regx Regex() => _regex;
#endif

        public USPostalAddress() { }

        public USPostalAddress(string s)
        {
            var match = Regex().Match(s);
            if (!match.Success)
            {
                throw new ArgumentException(
                    $"The string \"{s}\" does not match the regular expression \"{RegexString}\".",
                    nameof(s)
                );
            }

            address1 = (string)Convert.ChangeType(match.Groups["address1"]?.Value, typeof(string));
            address2 = (string)Convert.ChangeType(match.Groups["address2"]?.Value, typeof(string));
            city = (string)Convert.ChangeType(match.Groups["city"]?.Value, typeof(string));
            state = (string)Convert.ChangeType(match.Groups["state"]?.Value, typeof(string));
            zipcode = (string)Convert.ChangeType(match.Groups["zipcode"]?.Value, typeof(string));
        }
    }
}
