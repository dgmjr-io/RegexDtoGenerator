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
        public const string RegexString = @"""^(?n:(?<address1>(\d{1,5}(\ 1\/[234])?(\x20[A-Z]([a-z])+)+ )|(P\.O\.\ Box\ \d{1,5}))\s{1,2}(?i:(?<address2>(((APT|B LDG|DEPT|FL|HNGR|LOT|PIER|RM|S(LIP|PC|T(E|OP))|TRLR|UNIT)\x20\w{1,5})|(BSMT|FRNT|LBBY|LOWR|OFC|PH|REAR|SIDE|UPPR)\.?)\s{1,2})?)(?<city>[A-Z]([a-z])+(\.?)(\x20[A-Z]([a-z])+){0,2})\, \x20(?<state>A[LKSZRAP]|C[AOT]|D[EC]|F[LM]|G[AU]|HI|I[ADL N]|K[SY]|LA|M[ADEHINOPST]|N[CDEHJMVY]|O[HKR]|P[ARW]|RI|S[CD] |T[NX]|UT|V[AIT]|W[AIVY])\x20(?<zipcode>(?!0{5})\d{5}(-\d {4})?))$""";

#if NET7_0_OR_GREATER
        [GeneratedRegex(RegexString, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.RightToLeft | RegexOptions.Singleline)]
        public static partial Regex Regex();
#else
        private static readonly System.Text.RegularExpressions.Regex _regex = new System.Text.RegularExpressions.Regex(RegexString);
        public static System.Text.RegularExpressions.Regex Regex() => _regex;
#endif

        public static USPostalAddress Parse(string s)
        {
            var match = Regex().Match(s);
            if (!match.Success)
            {
                throw new System.ArgumentException($"The string \"{s}\" does not match the regular expression \"{RegexString}\".", nameof(s));
            }
        
            return new USPostalAddress
            {
                address2 = (string)System.Convert.ChangeType(match.Groups["address2"]?.Value, typeof(string)),
                city = (string)System.Convert.ChangeType(match.Groups["city"]?.Value, typeof(string)),
                state = (string)System.Convert.ChangeType(match.Groups["state"]?.Value, typeof(string)),
                zipcode = (string)System.Convert.ChangeType(match.Groups["zipcode"]?.Value, typeof(string)),
            };
        }
        public virtual string address1 { get; set; }
        public virtual string address2 { get; set; }
        public virtual string city { get; set; }
        public virtual string state { get; set; }
        public virtual string zipcode { get; set; }
        
        protected USPostalAddressBase () { }
        
        protected USPostalAddressBase (string s)
        {
            var match = Regex().Match(s);
            if (!match.Success)
            {
                throw new System.ArgumentException($"The string \"{s}\" does not match the regular expression \"{RegexString}\".", nameof(s));
            }
        
            address1 = (string)System.Convert.ChangeType(match.Groups["address1"]?.Value, typeof(string));
            address2 = (string)System.Convert.ChangeType(match.Groups["address2"]?.Value, typeof(string));
            city = (string)System.Convert.ChangeType(match.Groups["city"]?.Value, typeof(string));
            state = (string)System.Convert.ChangeType(match.Groups["state"]?.Value, typeof(string));
            zipcode = (string)System.Convert.ChangeType(match.Groups["zipcode"]?.Value, typeof(string));
        }
    }
}

#nullable enable
namespace Contacts
{
    public partial record class USPostalAddress  : USPostalAddressBase 
    {
        public const string RegexString = @"""^(?n:(?<address1>(\d{1,5}(\ 1\/[234])?(\x20[A-Z]([a-z])+)+ )|(P\.O\.\ Box\ \d{1,5}))\s{1,2}(?i:(?<address2>(((APT|B LDG|DEPT|FL|HNGR|LOT|PIER|RM|S(LIP|PC|T(E|OP))|TRLR|UNIT)\x20\w{1,5})|(BSMT|FRNT|LBBY|LOWR|OFC|PH|REAR|SIDE|UPPR)\.?)\s{1,2})?)(?<city>[A-Z]([a-z])+(\.?)(\x20[A-Z]([a-z])+){0,2})\, \x20(?<state>A[LKSZRAP]|C[AOT]|D[EC]|F[LM]|G[AU]|HI|I[ADL N]|K[SY]|LA|M[ADEHINOPST]|N[CDEHJMVY]|O[HKR]|P[ARW]|RI|S[CD] |T[NX]|UT|V[AIT]|W[AIVY])\x20(?<zipcode>(?!0{5})\d{5}(-\d {4})?))$""";

        #if NET7_0_OR_GREATER
        [GeneratedRegex(RegexString, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.RightToLeft | RegexOptions.Singleline)]
        public static partial System.Text.RegularExpressions.Regex Regex();
        #else
        private static readonly System.Text.RegularExpressions.Regex _regex = new System.Text.RegularExpressions.Regex(RegexString);
        public static System.Text.RegularExpressions.Regex Regex() => _regex;
        #endif

        
        
        public USPostalAddress () { }
        
        public USPostalAddress (string s)
        {
            var match = Regex().Match(s);
            if (!match.Success)
            {
                throw new System.ArgumentException($"The string \"{s}\" does not match the regular expression \"{RegexString}\".", nameof(s));
            }
        
            address1 = (string)System.Convert.ChangeType(match.Groups["address1"]?.Value, typeof(string));
            address2 = (string)System.Convert.ChangeType(match.Groups["address2"]?.Value, typeof(string));
            city = (string)System.Convert.ChangeType(match.Groups["city"]?.Value, typeof(string));
            state = (string)System.Convert.ChangeType(match.Groups["state"]?.Value, typeof(string));
            zipcode = (string)System.Convert.ChangeType(match.Groups["zipcode"]?.Value, typeof(string));
        }
    }
}
/*
    baseType: System.Object
    baseTypeSymbolKind: Type
    baseTypeValue: 
    baseTypeValueType: 
    isClass: True
    targetDataStructureType: record class
    typeName: USPostalAddress
    namespaceName: Contacts
    matches: 5: (?<address1>(\d{1,5}(\ 1\/[234])
8: address1
0: 
95: (?<address2>(((APT|B LDG|DEPT|FL|HNGR|LOT|PIER|RM|S(LIP|PC|T(E|OP)
98: address2
0: 
243: (?<city>[A-Z]([a-z])
246: city
0: 
301: (?<state>A[LKSZRAP]|C[AOT]|D[EC]|F[LM]|G[AU]|HI|I[ADL N]|K[SY]|LA|M[ADEHINOPST]|N[CDEHJMVY]|O[HKR]|P[ARW]|RI|S[CD] |T[NX]|UT|V[AIT]|W[AIVY])
304: state
0: 
445: (?<zipcode>(?!0{5})
448: zipcode
0: 
    regex: ^(?n:(?<address1>(\d{1,5}(\ 1\/[234])?(\x20[A-Z]([a-z])+)+ )|(P\.O\.\ Box\ \d{1,5}))\s{1,2}(?i:(?<address2>(((APT|B LDG|DEPT|FL|HNGR|LOT|PIER|RM|S(LIP|PC|T(E|OP))|TRLR|UNIT)\x20\w{1,5})|(BSMT|FRNT|LBBY|LOWR|OFC|PH|REAR|SIDE|UPPR)\.?)\s{1,2})?)(?<city>[A-Z]([a-z])+(\.?)(\x20[A-Z]([a-z])+){0,2})\, \x20(?<state>A[LKSZRAP]|C[AOT]|D[EC]|F[LM]|G[AU]|HI|I[ADL N]|K[SY]|LA|M[ADEHINOPST]|N[CDEHJMVY]|O[HKR]|P[ARW]|RI|S[CD] |T[NX]|UT|V[AIT]|W[AIVY])\x20(?<zipcode>(?!0{5})\d{5}(-\d {4})?))$
    visibility: public
*/
