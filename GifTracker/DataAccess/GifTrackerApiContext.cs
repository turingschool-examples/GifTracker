using GifTrackerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GifTrackerAPI.DataAccess
{
    public class GifTrackerApiContext : DbContext
    {
        public DbSet<Gif> Gifs { get; set; }

        public GifTrackerApiContext(DbContextOptions<GifTrackerApiContext> options)
            : base(options) { }
    }
}
