// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.AI.Evaluation.Reporting;

namespace Reporting.Storage.Sqlite;

/// <summary>
/// An implementation of <see cref="IResultStore"/> that stores results in a SQLite database.
/// </summary>
public class SqliteResultStore : IResultStore
{
    private readonly string _connectionString;

    public SqliteResultStore(string databaseFilePath)
    {
        _connectionString = SqliteUtilities.GetConnectionString(databaseFilePath);

        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using DbTransaction transaction = connection.BeginTransaction();

        try
        {
            SqliteCommand command1 = connection.CreateCommand();
            command1.CommandText =
                """
                CREATE TABLE IF NOT EXISTS executions (
                    execution_name TEXT PRIMARY KEY,
                    execution_timestamp TEXT NOT NULL);
                """;

            SqliteCommand command2 = connection.CreateCommand();
            command2.CommandText =
                """
                CREATE TABLE IF NOT EXISTS scenario_iterations (
                    execution_name TEXT NOT NULL,
                    scenario_name TEXT NOT NULL,
                    iteration_name TEXT NOT NULL,
                    result_json TEXT NOT NULL,
                    FOREIGN KEY (execution_name) REFERENCES executions (execution_name),
                    PRIMARY KEY (execution_name, scenario_name, iteration_name));
                """;

            command1.ExecuteNonQuery();
            command2.ExecuteNonQuery();

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<ScenarioRunResult> ReadResultsAsync(
        string? executionName = null,
        string? scenarioName = null,
        string? iterationName = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        // Order returned results by execution timestamp first, then by scenario name, and finally by iteration name.
        SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT scenario_iteration.result_json
            FROM scenario_iterations scenario_iteration
            JOIN executions execution ON scenario_iteration.execution_name = execution.execution_name
            WHERE (@execution_name IS NULL OR scenario_iteration.execution_name = @execution_name)
            AND (@scenario_name IS NULL OR scenario_iteration.scenario_name = @scenario_name)
            AND (@iteration_name IS NULL OR scenario_iteration.iteration_name = @iteration_name)
            ORDER BY execution.execution_timestamp, scenario_iteration.scenario_name, scenario_iteration.iteration_name;
            """;
        command.Parameters.AddWithValue("@execution_name", SqliteUtilities.ToDbNullIfNull(executionName));
        command.Parameters.AddWithValue("@scenario_name", SqliteUtilities.ToDbNullIfNull(scenarioName));
        command.Parameters.AddWithValue("@iteration_name", SqliteUtilities.ToDbNullIfNull(iterationName));

        using SqliteDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            string resultJson = reader.GetString(ordinal: 0);
            ScenarioRunResult? result = JsonSerializer.Deserialize<ScenarioRunResult>(resultJson, JsonOptions.Default);

            yield return result is null
                ? throw new JsonException(
                    $"""
                    Failed to deserialize result
                    {nameof(executionName)}: {executionName}
                    {nameof(scenarioName)}: {scenarioName}
                    {nameof(iterationName)}: {iterationName}
                    {nameof(resultJson)}: {resultJson}
                    """)
                : result;
        }
    }

    /// <inheritdoc/>
    public async ValueTask WriteResultsAsync(
        IEnumerable<ScenarioRunResult> results,
        CancellationToken cancellationToken = default)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        using DbTransaction transaction = await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var result in results)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Use ISO 8601 format for the execution timestamp. ISO 8601 formatted strings can be sorted
                // lexicographically. This allows us to sort results by execution timestamp in the SQL queries used
                // throughout the current file.
                string executionTimestamp = DateTime.UtcNow.ToISO8601DateString();

                SqliteCommand command1 = connection.CreateCommand();
                command1.CommandText =
                    """
                    INSERT OR REPLACE INTO executions (execution_name, execution_timestamp)
                    VALUES (@execution_name, @execution_timestamp);
                    """;
                command1.Parameters.AddWithValue("@execution_name", result.ExecutionName);
                command1.Parameters.AddWithValue("@execution_timestamp", executionTimestamp);

                // Serialize the result to JSON. This allows us to store the result in a TEXT column in the
                // scenario_iterations table.
                string resultJson = JsonSerializer.Serialize(result, JsonOptions.Default);

                SqliteCommand command2 = connection.CreateCommand();
                command2.CommandText =
                    """
                    INSERT OR REPLACE INTO scenario_iterations (iteration_name, execution_name, scenario_name, result_json)
                    VALUES (@iteration_name, @execution_name, @scenario_name, @result_json);
                    """;
                command2.Parameters.AddWithValue("@iteration_name", result.IterationName);
                command2.Parameters.AddWithValue("@execution_name", result.ExecutionName);
                command2.Parameters.AddWithValue("@scenario_name", result.ScenarioName);
                command2.Parameters.AddWithValue("@result_json", resultJson);

                await command1.ExecuteNonQueryAsync(cancellationToken);
                await command2.ExecuteNonQueryAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    /// <inheritdoc/>
    public async ValueTask DeleteResultsAsync(
        string? executionName = null,
        string? scenarioName = null,
        string? iterationName = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        using DbTransaction transaction = await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            SqliteCommand command1 = connection.CreateCommand();
            command1.CommandText =
                """
                DELETE FROM scenario_iterations
                WHERE (@execution_name IS NULL OR execution_name = @execution_name)
                AND (@scenario_name IS NULL OR scenario_name = @scenario_name)
                AND (@iteration_name IS NULL OR iteration_name = @iteration_name);
                """;
            command1.Parameters.AddWithValue("@execution_name", SqliteUtilities.ToDbNullIfNull(executionName));
            command1.Parameters.AddWithValue("@scenario_name", SqliteUtilities.ToDbNullIfNull(scenarioName));
            command1.Parameters.AddWithValue("@iteration_name", SqliteUtilities.ToDbNullIfNull(iterationName));

            // Delete any executions that no longer have any associated results in the scenario_iterations table.
            SqliteCommand command2 = connection.CreateCommand();
            command2.CommandText =
                """
                DELETE FROM executions
                WHERE NOT EXISTS (
                    SELECT 1 FROM scenario_iterations 
                    WHERE scenario_iterations.execution_name = executions.execution_name);
                """;

            await command1.ExecuteNonQueryAsync(cancellationToken);
            await command2.ExecuteNonQueryAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<string> GetLatestExecutionNamesAsync(
        int? count = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        SqliteCommand command = connection.CreateCommand();
        if (count.HasValue)
        {
            command.CommandText =
                """
                SELECT execution_name
                FROM executions
                ORDER BY execution_timestamp DESC
                LIMIT @count;
                """;
            command.Parameters.AddWithValue("@count", count.Value);
        }
        else
        {
            command.CommandText =
                """
                SELECT execution_name
                FROM executions
                ORDER BY execution_timestamp DESC;
                """;
        }

        using SqliteDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            yield return reader.GetString(ordinal: 0);
        }
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<string> GetScenarioNamesAsync(
        string executionName,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT scenario_name
            FROM scenario_iterations
            WHERE execution_name = @execution_name
            ORDER BY scenario_name;
            """;
        command.Parameters.AddWithValue("@execution_name", executionName);

        using SqliteDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            yield return reader.GetString(ordinal: 0);
        }
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<string> GetIterationNamesAsync(
        string executionName,
        string scenarioName,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT iteration_name
            FROM scenario_iterations
            WHERE execution_name = @execution_name
            AND scenario_name = @scenario_name
            ORDER BY iteration_name;
            """;
        command.Parameters.AddWithValue("@execution_name", executionName);
        command.Parameters.AddWithValue("@scenario_name", scenarioName);

        using SqliteDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            yield return reader.GetString(ordinal: 0);
        }
    }
}
