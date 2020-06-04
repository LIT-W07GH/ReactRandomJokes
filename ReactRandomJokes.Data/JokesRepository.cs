using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ReactRandomJokes.Data
{
    public class JokesRepository
    {
        private readonly string _connectionString;

        public JokesRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool JokeExists(int originId)
        {
            using (var context = new JokesContext(_connectionString))
            {
                return context.Jokes.Any(j => j.OriginId == originId);
            }
        }

        public Joke GetByOriginId(int originId)
        {
            using (var context = new JokesContext(_connectionString))
            {
                return context.Jokes.FirstOrDefault(j => j.OriginId == originId);
            }
        }

        public void AddJoke(Joke joke)
        {
            using (var context = new JokesContext(_connectionString))
            {
                context.Jokes.Add(joke);
                context.SaveChanges();
            }
        }

        public UserJokeLike GetLike(int userId, int jokeId)
        {
            using (var context = new JokesContext(_connectionString))
            {
                return context.UserJokeLikes.FirstOrDefault(u => u.UserId == userId && u.JokeId == jokeId);
            }
        }

        public void InteractWithJoke(int userId, int jokeId, bool like)
        {
            using (var context = new JokesContext(_connectionString))
            {
                var userLike = context.UserJokeLikes.FirstOrDefault(u => u.UserId == userId && u.JokeId == jokeId);
                if (userLike == null)
                {
                    context.UserJokeLikes.Add(new UserJokeLike
                    {
                        UserId = userId,
                        JokeId = jokeId,
                        Like = like,
                        Date = DateTime.Now
                    });
                }
                else
                {
                    userLike.Like = like;
                    userLike.Date = DateTime.Now;
                }

                context.SaveChanges();
            }
        }

        public Joke GetWithLikes(int jokeId)
        {
            using (var context = new JokesContext(_connectionString))
            {
                return context.Jokes.Include(u => u.UserJokeLikes)
                    .FirstOrDefault(j => j.Id == jokeId);
            }
        }

        public IEnumerable<Joke> GetAll()
        {
            using (var context = new JokesContext(_connectionString))
            {
                return context.Jokes.Include(j => j.UserJokeLikes).ToList();
            }
        }
    }
}
