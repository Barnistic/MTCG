using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace MTCG.Infrastructure
{
    public class DatabaseInitializer
    {
        private readonly string _connectionString;

        public DatabaseInitializer(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void InitializeDatabase()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            // Drop existing tables
            using (var command = new NpgsqlCommand(@"
                    DROP TABLE IF EXISTS cards CASCADE;
                    DROP TABLE IF EXISTS users CASCADE;
                    DROP TABLE IF EXISTS user_profiles CASCADE;
                ", connection))
            {
                command.ExecuteNonQuery();
            }

            // Create users table
            var createUsersTable = @"
                CREATE TABLE IF NOT EXISTS users (
                    id VARCHAR(255) PRIMARY KEY,
                    username VARCHAR(255) UNIQUE NOT NULL,
                    password VARCHAR(255) NOT NULL,
                    coins INT,
                    elo INT
                )";

            // Create cards table
            var createCardsTable = @"
                CREATE TABLE IF NOT EXISTS cards (
                    id VARCHAR(255) PRIMARY KEY,
                    name VARCHAR(255) NOT NULL,
                    damage INT NOT NULL,
                    element_type VARCHAR(255) NOT NULL,
                    ownerid VARCHAR(255),
                    FOREIGN KEY (ownerid) REFERENCES users(id)
                )";

            var createUserProfilesTable = @"
                CREATE TABLE IF NOT EXISTS user_profiles (
                    user_id VARCHAR(255) PRIMARY KEY,
                    name VARCHAR(255),
                    bio TEXT,
                    image VARCHAR(255),
                    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
                )";

            var createDecksTable = @"
            CREATE TABLE IF NOT EXISTS decks (
                user_id VARCHAR(255) NOT NULL,
                card_id VARCHAR(255) NOT NULL,
                PRIMARY KEY (user_id, card_id),
                FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
                FOREIGN KEY (card_id) REFERENCES cards(id) ON DELETE CASCADE
            )";

            var createAdminUser = @"INSERT INTO users (id, username, password, coins, elo) 
                        VALUES ('1', 'admin', 'admin', 10000, 0)";

            // Execute all CREATE statements in correct order
            using (var command = new NpgsqlCommand(createUsersTable, connection))
            {
                command.ExecuteNonQuery();
            }
            using (var command = new NpgsqlCommand(createCardsTable, connection))
            {
                command.ExecuteNonQuery();
            }
            using (var command = new NpgsqlCommand(createUserProfilesTable, connection))
            {
                command.ExecuteNonQuery();
            }
            using (var command = new NpgsqlCommand(createAdminUser, connection))
            {
                command.ExecuteNonQuery();
            }using (var command = new NpgsqlCommand(createDecksTable, connection))
            {
                command.ExecuteNonQuery();
            }

            // Verify the foreign key constraints
            using (var command = new NpgsqlCommand(@"
                    SELECT
                        tc.table_name, 
                        kcu.column_name,
                        ccu.table_name AS foreign_table_name,
                        ccu.column_name AS foreign_column_name
                    FROM
                        information_schema.table_constraints AS tc
                        JOIN information_schema.key_column_usage AS kcu
                          ON tc.constraint_name = kcu.constraint_name
                        JOIN information_schema.constraint_column_usage AS ccu
                          ON ccu.constraint_name = tc.constraint_name
                    WHERE tc.constraint_type = 'FOREIGN KEY'", connection))
            {
                using var reader = command.ExecuteReader();
                Console.WriteLine("\nForeign Key Constraints:");
                while (reader.Read())
                {
                    Console.WriteLine($"Table {reader["table_name"]}: Column {reader["column_name"]} references {reader["foreign_table_name"]}({reader["foreign_column_name"]})");
                }
            }

            Console.WriteLine("Database initialized successfully");
        }
    }
}



