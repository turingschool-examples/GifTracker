using GifTrackerAPI.DataAccess;
using Microsoft.AspNetCore.Mvc;
using GifTrackerAPI.Models;

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

        [HttpPost]
        public void CreateGif(Gif gif)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 400;
                return;
            }
            _context.Gifs.Add(gif);
            _context.SaveChanges();

            Response.StatusCode = 201;

            return;
        }

        //Make sure to pass in the id in the request body aswell.
        [HttpPut("{id}")]
        public void UpdateGif(int id, Gif gif)
        {
            _context.Gifs.Update(gif);
            _context.SaveChanges();

            Response.StatusCode = 204;

            return;
        }

        [HttpDelete("{id}")]
        public void DeleteBook(int id)
        {
            Gif gif = _context.Gifs.Find(id);
            _context.Gifs.Remove(gif);
            _context.SaveChanges();

            Response.StatusCode = 204;

            return;
        }
    }
}
