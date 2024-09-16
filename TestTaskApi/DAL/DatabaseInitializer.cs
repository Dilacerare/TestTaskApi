using Npgsql;

namespace TestTaskApi.DAL
{
    public class DatabaseInitializer
    {
        private readonly string _connectionString;
        private readonly string _serverConnectionString;

        public DatabaseInitializer(string connectionString)
        {
            _connectionString = connectionString;
            string updatedConnectionString = connectionString.Substring(0, connectionString.IndexOf("Database="));
            _serverConnectionString = updatedConnectionString;
        }

        public void Initialize()
        {
            using (var connection = new NpgsqlConnection(_serverConnectionString))
            {
                connection.Open();

                // Check if the database exists
                var databaseName = GetDatabaseNameFromConnectionString(_connectionString);
                var databaseExists = CheckDatabaseExists(connection, databaseName);

                if (!databaseExists)
                {
                    CreateDatabase(connection, databaseName);
                }

                using (var targetConnection = new NpgsqlConnection(_connectionString))
                {
                    targetConnection.Open();
                    var commandText = @"
                    CREATE TABLE IF NOT EXISTS Messages (
                    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),    -- UUID для уникального идентификатора
                    Content VARCHAR(128) NOT NULL,                    -- Сообщение длиной до 128 символов
                    Timestamp TIMESTAMPTZ NOT NULL DEFAULT now(),     -- Время создания (устанавливается автоматически)
                    SequenceNumber SERIAL NOT NULL                   -- Автоинкрементируемый порядковый номер
                    );";
                    using (var command = new NpgsqlCommand(commandText, targetConnection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private string GetDatabaseNameFromConnectionString(string connectionString)
        {
            var builder = new NpgsqlConnectionStringBuilder(connectionString);
            return builder.Database;
        }

        private bool CheckDatabaseExists(NpgsqlConnection connection, string databaseName)
        {
            var checkDbCommandText = $"SELECT 1 FROM pg_database WHERE datname = '{databaseName}';";
            using (var checkDbCommand = new NpgsqlCommand(checkDbCommandText, connection))
            {
                var result = checkDbCommand.ExecuteScalar();
                return result != null;
            }
        }

        private void CreateDatabase(NpgsqlConnection connection, string databaseName)
        {
            var createDbCommandText = $"CREATE DATABASE \"{databaseName}\";";
            using (var createDbCommand = new NpgsqlCommand(createDbCommandText, connection))
            {
                createDbCommand.ExecuteNonQuery();
            }
        }
    }
}
