namespace JackHenry_ChadCannonAssessment.AC
{
    using System;
    using System.Collections.Generic;
    using System.Runtime;
    using System.Threading.Tasks;
    using Domain;
    using Helpers;
    using Reddit;
    using Reddit.Controllers;
    using Reddit.Controllers.EventArgs;

    /// <summary>
    /// AntiCorruption Layer for Reddit Client
    /// </summary>
    public class RedditAc
    {
        private RedditClient client;
        private Action<List<AssessmentPost>> updatedDelegate;
        
        public RedditAc(string appId, string appSecret, string refreshToken)
        {
            client = new RedditClient(appId: appId, appSecret: appSecret, refreshToken: refreshToken);            
        }

        public async Task<List<AssessmentPost>> GetNewFromSubreddit(string subReddit)
        {
            var sub = client.Subreddit(subReddit);
            return sub.Posts.GetNew().ConvertTo();
        }

        public void MonitorNew(string subReddit, Action<List<AssessmentPost>> updated)
        {
            var sub = client.Subreddit(subReddit);
            updatedDelegate = updated;
            sub.Posts.GetNew();
            sub.Posts.NewUpdated += PostsAdded;
            sub.Posts.MonitorNew(2000);
        }
        
        private void PostsAdded(object sender, PostsUpdateEventArgs e)
        {
            updatedDelegate(e.Added.ConvertTo());
        }
    }
}