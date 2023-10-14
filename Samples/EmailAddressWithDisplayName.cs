/*
 * EmailAddressWithDisplayName.cs
 *
 *   Created: 2022-12-28-07:49:41
 *   Modified: 2022-12-28-07:49:43
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

namespace Contacts;

[RegexDto(
    @"^(?<FirstName:string?>\w+)?\s*?(?<LastName:string?>\w+)?\s*?\<(?<Username>\w+)@(?<Domain>((?:\w+\.))(?<Tld>\w+)))\>$"
)]
public partial record struct EmailAddressWithDisplayName { }
