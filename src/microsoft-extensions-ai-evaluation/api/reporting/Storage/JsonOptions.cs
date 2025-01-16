// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Reporting.Storage;

public static class JsonOptions
{
    public static readonly JsonSerializerOptions Default =
        new JsonSerializerOptions(JsonSerializerDefaults.General)
        {
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
            WriteIndented = true,
            AllowTrailingCommas = true,
            IgnoreReadOnlyProperties = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

    public static readonly JsonSerializerOptions Compact =
        new JsonSerializerOptions(Default)
        {
            WriteIndented = false,
        };
}

