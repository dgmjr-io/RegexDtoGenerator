/*
 * RegexDtoPropertiesDeclarationModel.cs
 *
 *   Created: 2023-02-01-02:41:19
 *   Modified: 2023-02-01-02:46:27
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

namespace Dgmjr.RegexDtoGenerator.Models;

internal record struct RegexDtoPropertiesDeclarationModel
{
    public string TypeName { get; set; }
    public RegexDtoPropertyDeclarationModel[] Properties { get; set; }
}
