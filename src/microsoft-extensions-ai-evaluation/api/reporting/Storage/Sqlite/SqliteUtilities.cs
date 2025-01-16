// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace Reporting.Storage.Sqlite;

public static class SqliteUtilities
{
    private const string ISO8601DateFormat = "o";

    public static string GetConnectionString(string databaseFilePath)
        => $"Data Source={Path.GetFullPath(databaseFilePath)}";

    public static string ToISO8601DateString(this DateTime dateTime)
        => dateTime.ToString(ISO8601DateFormat, CultureInfo.InvariantCulture);

    public static object ToDbNullIfNull(string? value)
        => (object?)value ?? DBNull.Value;
}
