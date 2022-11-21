using AutoCompleteDemo.Models;
using AutoCompleteDemo.Repository;
using Microsoft.AspNetCore.Mvc;

namespace AutoCompleteDemo.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class AirportController : ControllerBase
    {
        private readonly AirportRepository _airportRepository;
        public AirportController(AirportRepository airportRepository)
        {
            _airportRepository = airportRepository; 
        }

        
        [HttpGet]
        [Route("search/{query?}")]
        public async Task<ActionResult<List<Suggestion>>> AutoCompleteName(string query)
        {
            var authorsuggestion = await _airportRepository.AutoCompleteName(query, true);
            return Ok(authorsuggestion);
        }
    }
}
