using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SirenOfShame.Uwp.Web.Controllers
{
    public class AudioPattern
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [Route("api/[controller]")]
    public class AudioPatternsController : Controller
    {
        // GET: api/values
        [HttpGet]
        public IEnumerable<AudioPattern> Get()
        {
            return new []
            {
                new AudioPattern() { Id = 1, Name = "Sad Trombone" },
                new AudioPattern() { Id = 2, Name = "Ding" },
                new AudioPattern() { Id = 3, Name = "Plunk" }
            };
        }
    }
}
