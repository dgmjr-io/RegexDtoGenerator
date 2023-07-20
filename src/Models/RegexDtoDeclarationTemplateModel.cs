/*
 * RegexDtoDeclarationTemplateModel.cs
 *
 *   Created: 2023-02-01-02:41:30
 *   Modified: 2023-02-01-02:47:31
 *
 *   Author: David G. Mooore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022-2023 David G. Mooore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

namespace Dgmjr.RegexDtoGenerator;
using Dgmjr.RegexDtoGenerator.Models;


internal record struct RegexDtoDeclarationTemplateModel
{
    public string NamespaceName { get; set; }
    public string TargetDataStructureType { get; set; }
    public string TypeName { get; set; }
    public string Visibility { get; set; }
    public string Regex { get; set; }
    public RegexOptions RegexOptions { get; set; } = Compiled | IgnoreCase | ExplicitCapture; // Default to Compiled, IgnoreCase, and ExplicitCapturezww
    public RegexDtoPropertyDeclarationModel[] Properties { get; set; }
    public string BaseType { get; set; }
    public string Members { get; set; }
}
