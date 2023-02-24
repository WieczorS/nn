using API.Config;
using interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Npgsql;
using solutions;

namespace API.Controllers
{
    [ApiController]
    //[Authorize]
    [Route("[controller]")]
    public class NewsController : Controller
    {
        private readonly INewsRepository _repository;

        public NewsController(INewsRepository repository)
        {
            _repository = repository;
        }

        public static string ConnString = "Server=192.168.88.180; Port = 7777; Database=hackaton; User ID=postgres; Password=admin;";
        

        [HttpGet]
        public async Task<List<News>> GetAllNews()
        {
            var lst = new List<News>();
           
            
                lst.AddRange(await _repository.GetNewsAsync());
                

            return lst;
        }

        [HttpDelete("{idNews}")]
        public async Task<IActionResult> DeleteNews([FromRoute] int idNews)
        {
          
                await _repository.DeleteNewsAsync(idNews);
            
            return Ok();
        }

        [HttpPut] 
        public async Task InsertNews(News newsToInsert)
        {
            
                await _repository.InsertAsync(newsToInsert);
            
        }

        [HttpPatch]
        public async Task UpdateNews(News newsToUpdate)
        {
            using (var connection = new NpgsqlConnection(ConnString))
            
                await _repository.UpdateAsync(newsToUpdate);
            }
        }
    
    }
