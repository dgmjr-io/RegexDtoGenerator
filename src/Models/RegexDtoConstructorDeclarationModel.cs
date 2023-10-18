/*
 * RegexDtoConstructorDeclarationModel.cs
 *
 *   Created: 2023-02-01-02:41:13
 *   Modified: 2023-02-01-02:46:10
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

namespace Dgmjr.RegexDtoGenerator.Models;

internal record struct RegexDtoConstructorDeclarationModel(
    string ParameterizedConstructorVisibility,
    string ParameterlessConstructorVisibility,
    string TypeName,
    RegexDtoPropertyDeclarationModel[] Properties
)
{
    public readonly string Declaration =>
        $@"
        {ParameterizedConstructorVisibility} {TypeName}(string s)
        {{
            var match = Regex().Match(s);
            if (!match.Success)
            {{
                throw new ArgumentException($""The string \""{{s}}\"" does not match the regular expression \""{{RegexString}}\""."", nameof(s));
            }}

            {Join("\n", Properties.Select(p => $"{p.Name} = {(p.IsNullable ? $"string.IsNullOrEmpty(match.Groups[\"{p.Name}\"]?.Value) ? null : ({p.Type}?)System.Convert.ChangeType(match.Groups[\"{p.Name}\"]?.Value, typeof({p.Type}))" : $"({p.Type})System.Convert.ChangeType(match.Groups[\"{p.Name}\"]?.Value, typeof({p.Type}))")};"))}

            OriginalString = s;
        }}

        {ParameterlessConstructorVisibility} {TypeName}()
        {{
        }}";
}
