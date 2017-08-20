using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Reciprocate
{
    public class RecipeGetter
    {
        public static async Task<string> Get(string ingredient)
        {
            var random = new Random();
            
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-Mashape-Key", Environment.GetEnvironmentVariable("MashapeApiKey"));
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                var url = GetUrl(ingredient);

                try
                {
                    var response = await client.GetAsync(url);

                    Console.WriteLine("External request status: " + response.StatusCode);

                    if (response.StatusCode == HttpStatusCode.OK && response.Content != null)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseBody);

                        var searchResult = JsonConvert.DeserializeObject<SearchResult>(responseBody);

                        var recipeNumber = random.Next(searchResult.results.Count);

                        return $"You should cook {searchResult.results[recipeNumber].title}.";
                    }
                    else
                    {
                        var errorResponse = "I didn't understand. Please try again.";
                        return errorResponse;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("External request failed: " + ex.Message);
                }
            }

            return string.Empty;
        }

        private static string GetUrl(string ingredient)
        {
            var url = "https://spoonacular-recipe-food-nutrition-v1.p.mashape.com/recipes/search?number=100";

            if (!string.IsNullOrWhiteSpace(ingredient))
            {
                url += "&query=" + System.Uri.EscapeUriString(ingredient);
            }

            return url;
        }

        //possible cuisines: african, chinese, japanese, korean, vietnamese, thai, indian, british, irish, french, italian, mexican, spanish, middle eastern, jewish, american, cajun, southern, greek, german, nordic, eastern european, caribbean, or latin american
    }
}
