using System.Collections.Generic;
using Microsoft.AspNet.Mvc;

namespace SirenOfShame.Uwp.Web.Controllers
{
    [Route("api/[controller]")]
    public class AudioPatternsController : Controller
    {
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new [] { "Sad Trombone", "Ding", "Plunk" };
        }
    }
}
