/*
 * RegexDtoPropertyDeclarationModel.cs
 *
 *   Created: 2023-02-01-02:41:46
 *   Modified: 2023-02-01-02:49:03
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

namespace Dgmjr.RegexDtoGenerator.Models;

internal record struct RegexDtoPropertyDeclarationModel(
    string Name,
    string Type,
    bool IsNullable,
    string Overridability,
    bool IsClass = false
)
{
    public readonly string Declaration =>
        $@"
        /// <summary>The <see cref=""{Name}"" /> property</summary>
        public {Overridability} {Type}{(IsNullable ? "?" : "")} {Name} {{ get; init; }}
        ";
}
