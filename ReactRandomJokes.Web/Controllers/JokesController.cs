using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ReactRandomJokes.Data;
using ReactRandomJokes.Web.Models;

namespace ReactRandomJokes.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JokesController : ControllerBase
    {
        private string _connectionString;
        private const int MinutesAllowedToChangeLike = 5;

        public JokesController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConStr");
        }

        [HttpGet]
        [Route("randomjoke")]
        public Joke GetRandomJoke()
        {
            var joke = JokesApi.GetJoke();
            var jokeRepo = new JokesRepository(_connectionString);
            if (!jokeRepo.JokeExists(joke.OriginId))
            {
                jokeRepo.AddJoke(joke);
            }

            return jokeRepo.GetByOriginId(joke.OriginId);
        }

        [HttpGet]
        [Route("getlikescount/{jokeid}")]
        public object GetLikesCount(int jokeId)
        {
            var repo = new JokesRepository(_connectionString);
            var joke = repo.GetWithLikes(jokeId);
            return new
            {
                likes = joke.UserJokeLikes.Count(u => u.Like),
                dislikes = joke.UserJokeLikes.Count(u => !u.Like)
            };
        }

        [HttpGet]
        [Route("getinteractionstatus/{jokeid}")]
        public object GetInteractionStatus(int jokeId)
        {
            UserJokeInteractionStatus status = GetStatus(jokeId);
            return new { status };
        }

        [HttpPost]
        [Authorize]
        [Route("interactwithjoke")]
        public void InteractWithJoke(InteractViewModel viewModel)
        {
            var userRepo = new UserRepository(_connectionString);
            var user = userRepo.GetByEmail(User.Identity.Name);
            var jokeRepo = new JokesRepository(_connectionString);
            jokeRepo.InteractWithJoke(user.Id, viewModel.JokeId, viewModel.Like);
        }

        [HttpGet]
        [Route("viewall")]
        public IEnumerable<Joke> ViewAll()
        {
            var jokeRepo = new JokesRepository(_connectionString);
            return jokeRepo.GetAll();
        }

        private UserJokeInteractionStatus GetStatus(int jokeId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return UserJokeInteractionStatus.Unauthenticated;
            }
            var userRepo = new UserRepository(_connectionString);
            var user = userRepo.GetByEmail(User.Identity.Name);
            var jokeRepo = new JokesRepository(_connectionString);
            UserJokeLike likeStatus = jokeRepo.GetLike(user.Id, jokeId);
            if (likeStatus == null)
            {
                return UserJokeInteractionStatus.NeverInteracted;
            }
            if (likeStatus.Date.AddMinutes(MinutesAllowedToChangeLike) < DateTime.Now)
            {
                return UserJokeInteractionStatus.CanNoLongerInteract;
            }
            return likeStatus.Like
                ? UserJokeInteractionStatus.Liked
                : UserJokeInteractionStatus.Disliked;
        }
    }
}