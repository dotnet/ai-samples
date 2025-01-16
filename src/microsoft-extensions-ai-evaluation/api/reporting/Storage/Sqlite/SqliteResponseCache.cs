// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Caching.Distributed;

namespace Reporting.Storage.Sqlite;

/// <summary>
/// An implementation of <see cref="IDistributedCache"/> that uses a SQLite database to cache LLM responses.
/// </summary>
public partial class SqliteResponseCache : IDistributedCache
{
    private readonly string _connectionString;
    private readonly string _scenarioName;
    private readonly string _iterationName;
    private readonly uint _timeToLiveInDays;

    public SqliteResponseCache(
        string connectionString,
        string scenarioName,
        string iterationName,
        uint timeToLiveInDays)
    {
        _connectionString = connectionString;
        _scenarioName = scenarioName;
        _iterationName = iterationName;
        _timeToLiveInDays = timeToLiveInDays;
    }

    private static SqliteCommand GetCommandForGet(SqliteConnection connection, string key)
    {
        SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT value
            FROM cache
            WHERE key = @key
            AND datetime(expiration_timestamp) > datetime('now');
            """;
        command.Parameters.AddWithValue("@key", key);
        return command;
    }

    private SqliteCommand GetCommandForSet(
        SqliteConnection connection,
        string key,
        byte[] value,
        DistributedCacheEntryOptions options)
    {
        // Use ISO 8601 format for the expiration timestamp. This allows us to use SQLite's datetime function to
        // compare the expiration timestamp to the current time.
        string expirationTimestamp =
            (options.AbsoluteExpiration?.UtcDateTime ?? DateTime.UtcNow.AddDays(_timeToLiveInDays))
                .ToISO8601DateString();

        SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            INSERT OR REPLACE INTO cache (key, value, expiration_timestamp, scenario_name, iteration_name)
            VALUES (@key, @value, @expiration_timestamp, @scenario_name, @iteration_name);
            """;
        command.Parameters.AddWithValue("@key", key);
        command.Parameters.AddWithValue("@value", value);
        command.Parameters.AddWithValue("@expiration_timestamp", expirationTimestamp);
        command.Parameters.AddWithValue("@scenario_name", _scenarioName);
        command.Parameters.AddWithValue("@iteration_name", _iterationName);
        return command;
    }

    private SqliteCommand GetCommandForRefresh(SqliteConnection connection, string key)
    {
        string expirationTimestamp =
            DateTime.UtcNow.AddDays(_timeToLiveInDays).ToISO8601DateString();

        SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            UPDATE cache
            SET expiration_timestamp = @expiration_timestamp
            WHERE key = @key;
            """;
        command.Parameters.AddWithValue("@key", key);
        command.Parameters.AddWithValue("@expiration_timestamp", expirationTimestamp);
        return command;
    }

    private static SqliteCommand GetCommandForRemove(SqliteConnection connection, string key)
    {
        SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            DELETE FROM cache
            WHERE key = @key;
            """;
        command.Parameters.AddWithValue("@key", key);
        return command;
    }

    /// <inheritdoc/>
    public byte[]? Get(string key)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        SqliteCommand command = GetCommandForGet(connection, key);
        object? result = command.ExecuteScalar();
        return result is byte[] resultBytes ? resultBytes : null;
    }

    /// <inheritdoc/>
    public async Task<byte[]?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        SqliteCommand command = GetCommandForGet(connection, key);
        object? result = await command.ExecuteScalarAsync(cancellationToken);
        return result is byte[] resultBytes ? resultBytes : null;
    }

    /// <inheritdoc/>
    public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        SqliteCommand command = GetCommandForSet(connection, key, value, options);
        command.ExecuteNonQuery();
    }

    /// <inheritdoc/>
    public async Task SetAsync(
        string key,
        byte[] value,
        DistributedCacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        SqliteCommand command = GetCommandForSet(connection, key, value, options);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public void Refresh(string key)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        SqliteCommand command = GetCommandForRefresh(connection, key);
        command.ExecuteNonQuery();
    }

    /// <inheritdoc/>
    public async Task RefreshAsync(string key, CancellationToken cancellationToken = default)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        SqliteCommand command = GetCommandForRefresh(connection, key);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public void Remove(string key)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        SqliteCommand command = GetCommandForRemove(connection, key);
        command.ExecuteNonQuery();
    }

    /// <inheritdoc/>
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        SqliteCommand command = GetCommandForRemove(connection, key);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
