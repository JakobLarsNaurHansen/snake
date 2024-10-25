using System;
using System.Collections.Generic;
using Npgsql;
using Microsoft.Extensions.Configuration;

namespace Snake
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(IConfiguration configuration)
        {
            var databaseName = configuration["PostgreSql:DatabaseName"];
            var databaseUser = configuration["PostgreSql:User"];
            var databasePassword = configuration["PostgreSql:Password"];
            var databaseHost = configuration["PostgreSql:Host"];
            var databasePort = configuration["PostgreSql:Port"];

            if (string.IsNullOrEmpty(databaseName) || string.IsNullOrEmpty(databaseUser) ||
                string.IsNullOrEmpty(databasePassword) || string.IsNullOrEmpty(databaseHost) ||
                string.IsNullOrEmpty(databasePort))
            {
                throw new InvalidOperationException("One or more database configuration values are not set.");
            }

            _connectionString = $"Host={databaseHost};Port={databasePort};Username={databaseUser};Password={databasePassword};Database={databaseName}";
        }

        public void PostScore(string? username, int score)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = connection;
                        cmd.CommandText = "INSERT INTO Highscore (UserName, Score) VALUES (@username, @score)";
                        cmd.Parameters.AddWithValue("username", username ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("score", score);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error posting high score: {ex.Message}");
            }
        }
        public List<(string UserName, int Score)> GetTopScores(int topN = 10)
        {
            var topScores = new List<(string UserName, int Score)>();

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = connection;
                        cmd.CommandText = "SELECT UserName, Score FROM Highscore ORDER BY Score DESC LIMIT @topN";
                        cmd.Parameters.AddWithValue("topN", topN);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var userName = reader.GetString(0);
                                var score = reader.GetInt32(1);
                                topScores.Add((userName, score));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching top scores: {ex.Message}");
            }

            return topScores;
        }
    }
}