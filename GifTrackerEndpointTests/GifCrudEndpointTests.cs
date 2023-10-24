using GifTrackerAPI.DataAccess;
using GifTrackerAPI.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Text;

namespace GifTrackerEndpointTests
{
    [Collection("Gifs Controller Tests")]
    public class GifCrudEndpointTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public GifCrudEndpointTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async void GetGifs_ReturnsListOfGifs()
        {
            Gif gif1 = new Gif
            {
                Name = "Cat",
                Url = "www.examplecat.com",
                Rating = 1
            };
            Gif gif2 = new Gif
            {
                Name = "Dog",
                Url = "www.exampledog.com",
                Rating = 1
            };
            List<Gif> gifs = new() { gif1, gif2 };

            GifTrackerApiContext context = GetDbContext();
            HttpClient client = _factory.CreateClient();
            context.Gifs.AddRange(gifs);
            context.SaveChanges();

            HttpResponseMessage response = await client.GetAsync("/gifs");
            string content = await response.Content.ReadAsStringAsync();

            // The method ParseJson is defined below
            string expected = ParseJson(gifs);

            response.EnsureSuccessStatusCode();
            Assert.Equal(expected, content);
        }

        [Fact]
        public async void PostGif_CreatesGifInDb()
        {
            // Create fresh database
            GifTrackerApiContext context = GetDbContext();

            // Set up and send the request
            HttpClient client = _factory.CreateClient();
            var jsonString = "{\"Name\":\"Goat Gif\", \"Url\":\"www.goaturl.com\"}";
            var requestContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/gifs", requestContent);

            // Get the first (and should be only) gif from the db
            var newGif = context.Gifs.First();

            Assert.Equal("Created", response.StatusCode.ToString());
            Assert.Equal(201, (int)response.StatusCode);
            Assert.Equal("Goat Gif", newGif.Name);
        }

        [Fact]
        public async void PutGif_UpdatesDatabaseRecord()
        {
            Gif catGif = new Gif
            {
                Name = "Cat",
                Url = "www.examplecaturl.com",
                Rating = 1
            };

            GifTrackerApiContext context = GetDbContext();
            context.Gifs.Add(catGif);
            context.SaveChanges();

            HttpClient client = _factory.CreateClient();
            var jsonString =
                "{ \"Id\":\"1\", \"Name\":\"Cat New\", \"Url\":\"www.examplecaturl.com\", \"Rating\":1  }";
            var requestContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/gifs/1", requestContent);

            // Clear all previously tracked DB objects to get a new copy of the updated book
            context.ChangeTracker.Clear();

            Assert.Equal(204, (int)response.StatusCode);
            Assert.Equal("Cat New", context.Gifs.Find(1).Name);
            Assert.Equal("www.examplecaturl.com", context.Gifs.Find(1).Url);
        }

        // This method helps us create an expected value. We can use the Newtonsoft JSON serializer to build the string that we expect.  Without this helper method, we would need to manually create the expected JSON string.
        private string ParseJson(object obj)
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };

            string json = JsonConvert.SerializeObject(
                obj,
                new JsonSerializerSettings { ContractResolver = contractResolver }
            );

            return json;
        }

        private GifTrackerApiContext GetDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<GifTrackerApiContext>();
            optionsBuilder.UseInMemoryDatabase("TestDatabase");

            var context = new GifTrackerApiContext(optionsBuilder.Options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            return context;
        }
    }
}
