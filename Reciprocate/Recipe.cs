using System.Collections.Generic;

namespace Reciprocate
{
    public class Recipe
    {
        public int id { get; set; }
        public string title { get; set; }
        public int readyInMinutes { get; set; }
        public string image { get; set; }
        public IList<string> imageUrls { get; set; }
    }

    public class SearchResult
    {
        public IList<Recipe> results { get; set; }
        public string baseUri { get; set; }
        public int offset { get; set; }
        public int number { get; set; }
        public int totalResults { get; set; }
        public int processingTimeMs { get; set; }
        public long expires { get; set; }
    }
}