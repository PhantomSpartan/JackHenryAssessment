namespace JackHenry_ChadCannonAssessment
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AC;
    using Domain;

    public class MainLoop
    {
        private RedditAc ac;
        private List<AssessmentPost> newPosts = new List<AssessmentPost>();
        private string subRedditTitle;
        private int incrementor = 0;
        
        private MainLoop(string appId, string appSecret, string refreshToken, string subReddit)
        {
            subRedditTitle = subReddit;
            ac = new RedditAc(appId, appSecret, refreshToken);
        }
        
        public static MainLoop Init(string appId, string appSecret, string refreshToken, string subReddit)
        {
            return new MainLoop(appId, appSecret, refreshToken, subReddit);
            
        }

        public async Task Monitor()
        {
            //_timer = new Timer(Monitor, null, 0, Timeout.Infinite);
            ac.MonitorNew(subRedditTitle, PostAdded);
        }

        private void PostAdded(List<AssessmentPost> addedPosts)
        {
            foreach (AssessmentPost post in addedPosts)
            {
                newPosts.Add(post);
                Console.WriteLine("[" + post.SubReddit + "] New Post by " + post.Author + ": " + post.Title);
            }
        }
        
        public async Task LogRollUp()
        {
            
            var updatedPosts = await ac.GetNewFromSubreddit(subRedditTitle);
            foreach (var item in newPosts)
            {
                var updatedPost = updatedPosts.SingleOrDefault(x => x.Id == item.Id);
                item.UpVotes = updatedPost?.UpVotes ?? item.UpVotes;
            }

            var mostUpvoted = newPosts.OrderByDescending(x => x.UpVotes).Take(1).ToList();
            var mostPostedUser = newPosts.GroupBy(x => x.Author).OrderByDescending(y => y.Count()).Take(1).ToList();
            Console.WriteLine("------------------------------------------------------------------");
            Console.WriteLine("Roll Up:");
            if (mostUpvoted.Count == 0)
            {
                Console.WriteLine("No New Posts have been made");
            }
            else
            {
                Console.WriteLine("[{0}] Most UpVotes: {1}, Author: {2}, Name:{3}", mostUpvoted[0].SubReddit, mostUpvoted[0].UpVotes, mostUpvoted[0].Author, mostUpvoted[0].Title);
                Console.WriteLine("[{0}] Author with most Posts: Author: {1}, PostCount:{2}",subRedditTitle, mostPostedUser[0].Key, mostPostedUser[0].Count());
            }
            
            Console.WriteLine("------------------------------------------------------------------");

            incrementor++;

            if (incrementor >= 10)
            {
                Console.WriteLine();
                Console.WriteLine("------------------------------------------------------------------");
                Console.WriteLine("Top 5 Roll up");
                var posts = newPosts.OrderByDescending(x => x.UpVotes).Take(5).ToList();
                var mostPostedUsers = newPosts.GroupBy(x => x.Author).OrderByDescending(y => y.Count()).Take(5).ToList();
                foreach (var post in posts)
                {
                    Console.WriteLine("[{0}] Most UpVotes: {1}, Author: {2}, Name:{3}", post.SubReddit, post.UpVotes, post.Author, post.Title);   
                }
                foreach (var user in mostPostedUsers)
                {
                    Console.WriteLine("[{0}] User with most posts: {1}, PostCount:{2}", subRedditTitle, user.Key, user.Count());   
                }
                Console.WriteLine("------------------------------------------------------------------");
                incrementor = 0;
            }
        }
    }
}