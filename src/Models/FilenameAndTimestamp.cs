/*
 * Constants.cs
 *
 *   Created: 2022-12-28-04:55:15
 *   Modified: 2022-12-28-04:55:17
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

namespace Dgmjr.RegexDtoGenerator;

internal record struct FilenameAndTimestamp(string Filename)
{
    public string Timestamp { get; } = Now.ToString(Constants.DateFormat);
}
