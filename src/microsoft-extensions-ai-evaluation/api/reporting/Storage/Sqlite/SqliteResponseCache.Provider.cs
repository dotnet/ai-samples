// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.AI.Evaluation.Reporting;
using Microsoft.Extensions.Caching.Distributed;

namespace Reporting.Storage.Sqlite;

public partial class SqliteResponseCache
{
    /// <summary>
    /// An implementation of <see cref="IResponseCacheProvider"/> that uses a SQLite database to cache LLM responses.
    /// </summary>
    public class Provider : IResponseCacheProvider
    {
        private readonly string _connectionString;
        private readonly uint _timeToLiveInDays;

        public Provider(string databaseFilePath, uint timeToLiveInDays = 14)
        {
            _connectionString = SqliteUtilities.GetConnectionString(databaseFilePath);
            _timeToLiveInDays = timeToLiveInDays;

            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                $"""
                CREATE TABLE IF NOT EXISTS cache (
                    key TEXT PRIMARY KEY,
                    value BLOB NOT NULL,
                    expiration_timestamp TEXT NOT NULL DEFAULT (datetime('now', '+{_timeToLiveInDays} days')),
                    scenario_name TEXT NOT NULL,
                    iteration_name TEXT NOT NULL);
                """;

            command.ExecuteNonQuery();
        }

        /// <inheritdoc/>
        public ValueTask<IDistributedCache> GetCacheAsync(
            string scenarioName,
            string iterationName,
            CancellationToken cancellationToken = default)
        {
            IDistributedCache cache =
                new SqliteResponseCache(
                    _connectionString,
                    scenarioName,
                    iterationName,
                    _timeToLiveInDays);

            return new ValueTask<IDistributedCache>(cache);
        }

        /// <inheritdoc/>
        public async ValueTask ResetAsync(CancellationToken cancellationToken = default)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "DROP TABLE cache;";

            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async ValueTask DeleteExpiredCacheEntriesAsync(CancellationToken cancellationToken = default)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                DELETE FROM cache
                WHERE datetime(expiration_timestamp) <= datetime('now');
                """;

            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}


