using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SirenOfShame.Uwp.Web.Controllers
{
    public class LedPattern
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [Route("api/[controller]")]
    public class LedPatternsController : Controller
    {
        // GET: api/values
        [HttpGet]
        public IEnumerable<LedPattern> Get()
        {
            return new[]
            {
                new LedPattern { Id = 1, Name = "Chase" },
                new LedPattern { Id = 2, Name = "Fade Chase" },
            };
        }

        [HttpPost]
        public void Post(int? id, int duration)
        {
            Console.WriteLine("Post " + id + " duration: " + duration);
        }
    }
}
