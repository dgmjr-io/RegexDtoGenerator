/*
 * RegexDtoConstructorDeclarationModel.cs
 *
 *   Created: 2023-02-01-02:41:13
 *   Modified: 2023-02-01-02:46:10
 *
 *   Author: David G. Mooore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022-2023 David G. Mooore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

namespace Dgmjr.RegexDtoGenerator.Models;

internal record struct RegexDtoConstructorDeclarationModel
{
    public string ParameterizedConstructorVisibility { get; set; }
    public string ParameterlessConstructorVisibility { get; set; }
    public string TypeName { get; set; }
    public RegexDtoPropertyDeclarationModel[] Properties { get; set; }
}
