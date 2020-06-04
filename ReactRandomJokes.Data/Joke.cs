using System.Collections.Generic;
using Newtonsoft.Json;

namespace ReactRandomJokes.Data
{
    public class Joke
    {
        public int Id { get; set; }
        public int OriginId { get; set; }
        public string Setup { get; set; }
        public string Punchline { get; set; }

        public List<UserJokeLike> UserJokeLikes { get; set; }
    }
}