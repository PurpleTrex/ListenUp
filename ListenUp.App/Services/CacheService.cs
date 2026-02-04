using System.IO;
using System.Text.Json;
using Microsoft.Data.Sqlite;
using ListenUp.App.Models;

namespace ListenUp.App.Services;

public sealed class CacheService : IDisposable
{
    private readonly string _dbPath;
    private SqliteConnection? _connection;

    public CacheService()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var folder = Path.Combine(appData, "ListenUp");
        Directory.CreateDirectory(folder);
        _dbPath = Path.Combine(folder, "cache.db");
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        _connection = new SqliteConnection($"Data Source={_dbPath}");
        _connection.Open();

        var cmd = _connection.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS SearchCache (
                Query TEXT PRIMARY KEY,
                Results TEXT NOT NULL,
                Timestamp INTEGER NOT NULL
            );
            
            CREATE TABLE IF NOT EXISTS Favorites (
                Id TEXT PRIMARY KEY,
                Title TEXT NOT NULL,
                Author TEXT,
                Data TEXT NOT NULL,
                AddedDate INTEGER NOT NULL
            );
        ";
        cmd.ExecuteNonQuery();
    }

    public bool TryGetCachedSearch(string query, out List<AggregatedResult>? results, TimeSpan maxAge)
    {
        results = null;
        if (_connection == null) return false;

        var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT Results, Timestamp FROM SearchCache WHERE Query = @query";
        cmd.Parameters.AddWithValue("@query", query);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            var timestamp = reader.GetInt64(1);
            var age = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - timestamp;
            if (age <= maxAge.TotalSeconds)
            {
                var json = reader.GetString(0);
                results = JsonSerializer.Deserialize<List<AggregatedResult>>(json);
                return results != null;
            }
        }
        return false;
    }

    public void CacheSearch(string query, List<AggregatedResult> results)
    {
        if (_connection == null) return;

        var json = JsonSerializer.Serialize(results);
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var cmd = _connection.CreateCommand();
        cmd.CommandText = "INSERT OR REPLACE INTO SearchCache (Query, Results, Timestamp) VALUES (@query, @results, @timestamp)";
        cmd.Parameters.AddWithValue("@query", query);
        cmd.Parameters.AddWithValue("@results", json);
        cmd.Parameters.AddWithValue("@timestamp", timestamp);
        cmd.ExecuteNonQuery();
    }

    public void AddFavorite(AggregatedResult item)
    {
        if (_connection == null) return;

        var id = $"{item.Title}|{item.AuthorDisplay}".ToLowerInvariant();
        var json = JsonSerializer.Serialize(item);
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var cmd = _connection.CreateCommand();
        cmd.CommandText = "INSERT OR REPLACE INTO Favorites (Id, Title, Author, Data, AddedDate) VALUES (@id, @title, @author, @data, @date)";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@title", item.Title);
        cmd.Parameters.AddWithValue("@author", item.AuthorDisplay);
        cmd.Parameters.AddWithValue("@data", json);
        cmd.Parameters.AddWithValue("@date", timestamp);
        cmd.ExecuteNonQuery();
    }

    public void RemoveFavorite(AggregatedResult item)
    {
        if (_connection == null) return;

        var id = $"{item.Title}|{item.AuthorDisplay}".ToLowerInvariant();
        var cmd = _connection.CreateCommand();
        cmd.CommandText = "DELETE FROM Favorites WHERE Id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public bool IsFavorite(AggregatedResult item)
    {
        if (_connection == null) return false;

        var id = $"{item.Title}|{item.AuthorDisplay}".ToLowerInvariant();
        var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM Favorites WHERE Id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    public List<AggregatedResult> GetFavorites()
    {
        var favorites = new List<AggregatedResult>();
        if (_connection == null) return favorites;

        var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT Data FROM Favorites ORDER BY AddedDate DESC";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var json = reader.GetString(0);
            var item = JsonSerializer.Deserialize<AggregatedResult>(json);
            if (item != null) favorites.Add(item);
        }
        return favorites;
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}
