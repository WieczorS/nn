using System.Data;
using interfaces;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using solutions;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    public class NewsRepository : INewsRepository
    {
        private readonly NpgsqlConnection _conn;

        public NewsRepository(NpgsqlConnection conn)
        {
            _conn = conn;
        }


        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
                .ToArray();
        }
        [HttpPut]
        public async Task InsertAsync(News newsCreate)
        {
//        await using (var cmd = dataSource.CreateCommand(
//                         $"INSERT INTO news (title, content, create_date, event_id, create_user_id) VALUES (@title,@content,@create_date,@event_id,@create_user_id);"))
            await using (var ins = new NpgsqlCommand(
                             $"INSERT INTO news (title, content, create_date, event_id, create_user_id) VALUES (@title,@content,@create_date,@event_id,@create_user_id) returning id;",
                             _conn))


            {
                ins.Parameters.AddWithValue("title", newsCreate.Title);
                ins.Parameters.AddWithValue("content", newsCreate.Content);
                ins.Parameters.AddWithValue("create_date", DateTime.Now);
                ins.Parameters.AddWithValue("event_id", newsCreate.EventId);
                ins.Parameters.AddWithValue("create_user_id", newsCreate.UserId);
                var index = await ins.ExecuteScalarAsync();
                newsCreate.Id = Convert.ToInt32(index);
            }
        }

        //READ 1 Odczytywanie Newsa po id - return obiektu
        [HttpPost]
        public async Task<News> GetNewsById(int NewsId)
        {
            var n = new News();
            if (_conn.State != ConnectionState.Open)
            {
                throw new Exception("Db Connection is not open");
            }

            await using (var cmd = new NpgsqlCommand(
                             $"SELECT id, title, content, event_id, create_user_id FROM news WHERE ID = @id;", _conn))
            {
                cmd.Parameters.AddWithValue("id", NewsId);
                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        n.Id = (reader["id"]) is int ? (int)(reader["id"]) : throw new Exception("Id must be int");
                        n.Title = ((reader["title"])) as string;
                        n.Content = (reader["content"]) as string;
                        n.EventId = (int)reader["event_id"];
                        n.UserId = (int)reader["create_user_id"];
                    }
                }

                return n;
            }
        }

        //READ 2 Odczytywanie wszystkich newsow - return listy obiektów
        [HttpGet]
        public async Task<List<News>> GetNewsAsync()
        {
            var list = new List<News>();
            if (_conn.State != ConnectionState.Open)
            {
                throw new Exception("Db is not open!");
            }


            await using (var command =
                         new NpgsqlCommand("SELECT id, title, content, event_id, create_user_id  FROM news;", _conn))
            {
                await using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var n = new News();
                        n.Id = (reader["id"]) is int ? (int)(reader["id"]) : throw new Exception("Id must be int");
                        n.Title = ((reader["title"])) as string;
                        n.Content = (reader["content"]) as string;
                        n.EventId = (int)reader["event_id"];
                        n.UserId = (int)reader["create_user_id"];

                        list.Add(n);
                        Console.WriteLine();
                    }
                }
            }

            return list;
        }


        //UPDATE
        [HttpPut]
        public async Task UpdateAsync(News updateNews)
        {
            await using (var cmd = new NpgsqlCommand(@"update news set 
                    title = COALESCE(@Title,news.title), 
                    content = COALESCE(@Content,news.content) 
                where id = @id ", _conn))
            {
                cmd.Parameters.AddWithValue("id", updateNews.Id);
                cmd.Parameters.AddWithValue("Title", updateNews.Title);
                cmd.Parameters.AddWithValue("Content", updateNews.Content);
                await cmd.ExecuteNonQueryAsync();
            }
        }


        //DELETE - usuwanie istniejacego artykulu ładne
        [HttpDelete]
        public async Task<int> DeleteNewsAsync(int NewsToDelete)
        {
            await using (var cmd = new NpgsqlCommand($"delete from news where id = @id;", _conn))
            {
                cmd.Parameters.AddWithValue("id", NewsToDelete);
                return await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}