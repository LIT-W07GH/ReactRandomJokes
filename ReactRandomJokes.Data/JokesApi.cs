using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;

namespace ReactRandomJokes.Data
{
    public static class JokesApi
    {
        private class JokeApiJoke
        {
            public int Id { get; set; }
            public string Setup { get; set; }
            public string Punchline { get; set; }
        }

        public static Joke GetJoke()
        {
            using (var client = new HttpClient())
            {
                var json = client.GetStringAsync("https://official-joke-api.appspot.com/jokes/programming/random")
                    .Result;
                var joke = JsonConvert.DeserializeObject<IEnumerable<JokeApiJoke>>(json).FirstOrDefault();
                return new Joke
                {
                    OriginId = joke.Id,
                    Setup = joke.Setup,
                    Punchline = joke.Punchline
                };
            }
        }
    }
}