using GifTrackerAPI.DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace GifTrackerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GifsController : ControllerBase
    {
        private GifTrackerApiContext _context;

        public GifsController(GifTrackerApiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult GetGifs()
        {
            var gifs = _context.Gifs.ToList();

            Response.StatusCode = 200;

            // API endpoints should return JSON, we are creating a new JSON result with our list of gifs.
            return new JsonResult(gifs);
        }
    }
}
