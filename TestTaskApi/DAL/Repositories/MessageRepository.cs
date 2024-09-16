using Npgsql;
using TestTaskApi.DAL.Interfaces;
using TestTaskApi.Domain.Entity;

namespace TestTaskApi.DAL.Repositories
{
    public class MessageRepository : IBaseRepository<MessageModel>
    {
        private readonly string _connectionString;

        public MessageRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task Create(MessageModel entity)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"INSERT INTO Messages (Content, Timestamp, SequenceNumber) 
                      VALUES (@content, @timestamp, @sequenceNumber)";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@content", entity.Content);
                    command.Parameters.AddWithValue("@timestamp", entity.Timestamp);
                    command.Parameters.AddWithValue("@sequenceNumber", entity.SequenceNumber);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public IQueryable<MessageModel> GetAll()
        {
            var messages = new List<MessageModel>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT Id, Content, Timestamp, SequenceNumber FROM Messages";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            messages.Add(new MessageModel
                            {
                                Id = reader.GetGuid(0),
                                Content = reader.GetString(1),
                                Timestamp = reader.GetDateTime(2),
                                SequenceNumber = reader.GetInt32(3)
                            });
                        }
                    }
                }
            }
            return messages.AsQueryable();
        }

        public async Task<int> GetLatestSequenceNumber()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT SequenceNumber FROM Messages ORDER BY Timestamp DESC LIMIT 1";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            return reader.GetInt32(0);
                        }
                    }
                }
            }
            return 0;
        }
    }

}
