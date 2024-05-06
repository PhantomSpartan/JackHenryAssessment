namespace JackHenry_ChadCannonAssessment
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    class Program
    {
        private static List<Task> tasks;
        private static Timer _timer;
        private const int TIMEOUT = 60000;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Chad Cannon: Jack Henry Assessment.  The system will monitor all posts from the given subreddit with the NEW filtering.");
            Console.WriteLine("To Run, Please add the AppId, AppSecret, and RefreshToken for Reddit OAuth as CommandLineArguments, in that order.");
            Console.WriteLine("A 4th argument may be added to CommandLineArguments to specify the specific SubReddit to monitor.  If a 4th argument is not found, the system will default to the 'r/funny' SubReddit");
            Console.WriteLine("When a NEW post is captured, the system will log it.");
            Console.WriteLine("Every 1 minute, a rollup will print to the Console with the most upVoted and the User that has the most posts from the New filter.");
            Console.WriteLine("Every 10 minutes, a rollup will print to the Console with the top 5 Upvoted Posts and top 5 Users with the most posts from the New filter.");
            tasks = new List<Task>();
        
            var loop = MainLoop.Init(args[0], args[1], args[2],  args.Count() > 3 ? args[3] : "funny");
            
            tasks.Add(loop.Monitor());
            _timer = new Timer(Log, loop, TIMEOUT, Timeout.Infinite);
            await Task.WhenAll(tasks);
        }

        private static async void Log(object? sender)
        {
            var loop = (MainLoop) sender;
            await loop.LogRollUp();
            _timer.Change( TIMEOUT, Timeout.Infinite );
        }
        
    }

}