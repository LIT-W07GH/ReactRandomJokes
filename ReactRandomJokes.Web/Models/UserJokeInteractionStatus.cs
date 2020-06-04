namespace ReactRandomJokes.Web.Models
{
    public enum UserJokeInteractionStatus
    {
        Unauthenticated,
        Liked,
        Disliked,
        NeverInteracted,
        CanNoLongerInteract
    }
}