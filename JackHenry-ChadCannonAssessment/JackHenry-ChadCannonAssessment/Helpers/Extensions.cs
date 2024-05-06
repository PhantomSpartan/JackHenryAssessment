namespace JackHenry_ChadCannonAssessment.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using Domain;
    using Reddit.Controllers;

    public static class Extensions
    {
        public static AssessmentPost ConvertTo(this Post post)
        {
            return new AssessmentPost {Id = post.Id, Title = post.Title, SubReddit = post.Subreddit, UpVotes = post.UpVotes, Author = post.Author};
        }

        public static List<AssessmentPost> ConvertTo(this List<Post> post)
        {
            return post.Select(ConvertTo).ToList();
        }
    }
}