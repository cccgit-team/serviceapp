using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using Firebase.Storage;
using System.Threading.Tasks;
using Firebase.Auth;

namespace CCCGFunctionApp
{
    public static class Function1
    {

        static videoSet videoData = new videoSet();

        [FunctionName("Function1")]
        public static void Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");


            string path = @"..\..\..\json\videos.json";
            string videoDataJson = "";

            if (File.Exists(path))
            {
                //File.Delete(path);
                using (var tw = new StreamReader(path, true))
                {
                    string line;
                    while ((line = tw.ReadLine()) != null)
                    {
                        videoDataJson += line;
                    }
                    
                    tw.Close();
                }

                Console.WriteLine(videoDataJson);

            }

            string[] videoIDs = { "gT7iU-qTl94", "cmMl5yc_Sec", "9dlQ7jWIYYg", "poXNOM1kymQ", "h0D-sU8aR8w", "67W7u_BeZfg" };


            videoData = JsonConvert.DeserializeObject<videoSet>(videoDataJson);

            //getRecentData(videoIDs[0]);
            //getRecentData(videoIDs[1]);
            //getRecentData(videoIDs[2]);
            //getRecentData(videoIDs[3]);



            //(videoIDs[0], "SundayService1");
            setLatestData(videoIDs[0], "BibleStudy");
            setLatestData(videoIDs[1], "SundayService1");
            setLatestData(videoIDs[2], "SundayService2");
            setLatestData(videoIDs[3], "SundayService3");
            //setLatestData(videoIDs[0], "BibleStudy");
            //setLatestData(videoIDs[4], "NightPrayers");
            //setLatestData(videoIDs[4],"BibleStudy");
            //setLatestData(videoIDs[5], "SundayService3");
            //setLatestData(videoIDs[6], "SundayService4");

            //setLatestData(videoIDs[0], "SundayService4");
            //setLatestData(videoIDs[0], "SundayService4");


            //setLiveData(getScheduled());


            //setLatestData(getCompleted());


            videoDataJson = JsonConvert.SerializeObject(videoData);



            Console.WriteLine("");
            Console.WriteLine("Video Data");
            Console.WriteLine(videoDataJson);
            Console.WriteLine(DateTime.Now);

            

            if (File.Exists(path))
            {
                File.Delete(path);
                using (var tw = new StreamWriter(path, true))
                {
                    tw.WriteLine(videoDataJson.ToString());
                    tw.Close();
                }

            }
            else if (!File.Exists(path))
            {
                using (var tw = new StreamWriter(path, true))
                {
                    tw.WriteLine(videoDataJson.ToString());
                    tw.Close();
                }
            }


           uploadToFirebaseAsync();

           //uploadToFirebaseUpdatesAsync();
        }

        public static string getService()
        {
            TimeSpan now = DateTime.Now.TimeOfDay;

            string Service = "";

            //TimeSpan 1stServiceStart = new TimeSpan(9, 0, 0); //10 o'clock
            //TimeSpan 1stServiceStop = new TimeSpan(10, 0, 0); //12 o'clock

            //TimeSpan 2ndServiceStart = new TimeSpan(9, 0, 0); //10 o'clock
            //TimeSpan 2ndServiceStop = new TimeSpan(10, 0, 0); //12 o'clock

            //TimeSpan 3rdServiceStart = new TimeSpan(9, 0, 0); //10 o'clock
            //TimeSpan 3rdServiceStop = new TimeSpan(10, 0, 0); //12 o'clock


            //if((now > 1stServiceStart) && (now < 1stServiceEnd))
            //{
            //    Service = "SundayService1";
            //}
            
            return Service;
        }

        public static async void uploadToFirebaseAsync()
        {
            // Get any Stream - it can be FileStream, MemoryStream or any other type of Stream
            var stream = File.Open(@"..\..\..\json\videos.json", FileMode.Open);

            //authentication
            var auth = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyCiIgEkwZ0VSLdUrF8kDUPa6AYzcsRhHkg"));
            var a = await auth.SignInWithEmailAndPasswordAsync("cccgit@gmail.com", "9249CCCG!!");

            // Constructr FirebaseStorage, path to where you want to upload the file and Put it there
            var task = new FirebaseStorage(
                "cccg-mobile-c246b.appspot.com",
                 new FirebaseStorageOptions
                 {
                     AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                     ThrowOnCancel = true,
                 })
                .Child("data")
                .Child("json")
                .Child("videos.json")
                .PutAsync(stream);

            // Track progress of the upload
            task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");

            //stream.Close();
        }

        public static async void uploadToFirebaseUpdatesAsync()
        {
            // Get any Stream - it can be FileStream, MemoryStream or any other type of Stream
            var stream = File.Open(@"..\..\..\json\updates.json", FileMode.Open);

            //authentication
            var auth = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyCiIgEkwZ0VSLdUrF8kDUPa6AYzcsRhHkg"));
            var a = await auth.SignInWithEmailAndPasswordAsync("cccgit@gmail.com", "9249CCCG!!");

            // Constructr FirebaseStorage, path to where you want to upload the file and Put it there
            var task = new FirebaseStorage(
                "cccg-mobile-c246b.appspot.com",
                 new FirebaseStorageOptions
                 {
                     AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                     ThrowOnCancel = true,
                 })
                .Child("data")
                .Child("json")
                .Child("updates.json")
                .PutAsync(stream);

            // Track progress of the upload
            task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");

            //stream.Close();
        }

        public static string getScheduled()
        {
            video set = new video();

            YouTubeService yt = new YouTubeService(new BaseClientService.Initializer() { ApiKey = "AIzaSyBuZWspufZ2JMzXPh6W5mUitfezsOhUiaM" });

            var searchListRequest = yt.Search.List("snippet");
            searchListRequest.ChannelId = "UCP7vi_uJF52wJ9d-oWdVVkA";
            searchListRequest.EventType = SearchResource.ListRequest.EventTypeEnum.Completed;
            searchListRequest.MaxResults = 1;
            searchListRequest.Order = SearchResource.ListRequest.OrderEnum.Date;
            searchListRequest.Type = "video";
            var searchListResult = searchListRequest.Execute();

            var videoID = searchListResult.Items[0].Id.VideoId;

            string path = @"C:\Users\yemi.ilupeju.da\source\repos\CCCGFunctionApp\json\updates.txt";

            if (File.Exists(path))
            {
                File.Delete(path);
                using (var tw = new StreamWriter(path, true))
                {
                    tw.WriteLine(videoID);
                    tw.Close();
                }

            }
            else if (!File.Exists(path))
            {
                using (var tw = new StreamWriter(path, true))
                {
                    tw.WriteLine(videoID);
                    tw.Close();
                }
            }

            return videoID;
        }


        public static string getCompleted()
        {

            string path = @"C:\Users\yemi.ilupeju.da\source\repos\CCCGFunctionApp\json\updates.txt";
            var videoID = "";

            if (File.Exists(path))
            {
                using (var tw = new StreamReader(path, true))
                {
                    string line;
                    while ((line = tw.ReadLine()) != null)
                    {
                        videoID = line;
                    }

                    tw.Close();
                }

                Console.WriteLine(videoID);

            }

            return videoID;
        }

        public static void getRecentData(string VideoID)
        {
            YouTubeService yt = new YouTubeService(new BaseClientService.Initializer() { ApiKey = "AIzaSyBuZWspufZ2JMzXPh6W5mUitfezsOhUiaM" });

            var searchListRequest = yt.Videos.List("snippet");
            //searchListRequest. = "UCP7vi_uJF52wJ9d-oWdVVkA";
            searchListRequest.Id = VideoID;
            //searchListRequest.PlaylistId = "PLKsphOLhItmu6SDaydqKiclApN16ynUiP";
            //searchListRequest.MaxResults = 1;
            var searchListResult = searchListRequest.Execute();

            foreach (var item in searchListResult.Items)
            {
                video CurrentVideo = new video();

                Console.WriteLine("ID:" + VideoID);
                CurrentVideo.videoId = VideoID;

                Console.WriteLine("snippet:" + item.Snippet.Title);
                CurrentVideo.videoTitle = item.Snippet.Title;

                Console.WriteLine("Published:" + item.Snippet.PublishedAt);
                CurrentVideo.videoPub = item.Snippet.PublishedAt.ToString();

                Console.WriteLine("Image:" + item.Snippet.Thumbnails.High.Url);
                CurrentVideo.videoImg = item.Snippet.Thumbnails.High.Url;

                CurrentVideo.count = "1";

                videoData.liveVideo = CurrentVideo;
                videoData.latestVideo.videoId = "";
                videoData.latestVideo.videoImg = "";
                videoData.latestVideo.videoPub = "";
                videoData.latestVideo.videoTitle = "";
                videoData.latestVideo.count = "0";
            }

            
        }

        public static void setLiveData(string VideoID)
        {
            YouTubeService yt = new YouTubeService(new BaseClientService.Initializer() { ApiKey = "AIzaSyBuZWspufZ2JMzXPh6W5mUitfezsOhUiaM" });

            var searchListRequest = yt.Videos.List("snippet");
            searchListRequest.Id = VideoID;
            var searchListResult = searchListRequest.Execute();

            foreach (var item in searchListResult.Items)
            {
                video CurrentVideo = new video();

                Console.WriteLine("ID:" + VideoID);
                CurrentVideo.videoId = VideoID;

                Console.WriteLine("snippet:" + item.Snippet.Title);
                CurrentVideo.videoTitle = item.Snippet.Title;

                Console.WriteLine("Published:" + item.Snippet.PublishedAt);
                CurrentVideo.videoPub = item.Snippet.PublishedAt.ToString();

                Console.WriteLine("Image:" + item.Snippet.Thumbnails.Medium.Url);
                CurrentVideo.videoImg = item.Snippet.Thumbnails.Medium.Url;

                CurrentVideo.count = "1";

                videoData.liveVideo = CurrentVideo;
                videoData.latestVideo.videoId = "";
                videoData.latestVideo.videoImg = "";
                videoData.latestVideo.videoPub = "";
                videoData.latestVideo.videoTitle = "";
                videoData.latestVideo.count = "0";
            }


        }


        public static void setLatestData(string videoID, string Service)
        {
            YouTubeService yt = new YouTubeService(new BaseClientService.Initializer() { ApiKey = "AIzaSyBuZWspufZ2JMzXPh6W5mUitfezsOhUiaM" });

            var searchListRequest = yt.Videos.List("snippet");

            searchListRequest.Id = videoID;
            var searchListResult = searchListRequest.Execute();

            var item = searchListResult.Items[0];

            video CurrentVideo = new video();

            Console.WriteLine("ID:" + videoID);
            CurrentVideo.videoId = videoID;

            Console.WriteLine("snippet:" + item.Snippet.Title);
            CurrentVideo.videoTitle = item.Snippet.Title;

            Console.WriteLine("Published:" + item.Snippet.PublishedAt);
            CurrentVideo.videoPub = item.Snippet.PublishedAt.ToString();

            Console.WriteLine("Image:" + item.Snippet.Thumbnails.High.Url);
            CurrentVideo.videoImg = item.Snippet.Thumbnails.High.Url;

            CurrentVideo.count = "1";

            if (Service == "SundayService1")
            {
                videoData.latestVideo = CurrentVideo;
                videoData.SundayVideos[0] = CurrentVideo;
            }

            if (Service == "SundayService2")
            {
                videoData.latestVideo = CurrentVideo;
                videoData.SundayVideos[1] = CurrentVideo;
            }

            if (Service == "SundayService3")
            {
                videoData.latestVideo = CurrentVideo;
                videoData.SundayVideos[2] = CurrentVideo;
            }

            if (Service == "SundayService4")
            {
                videoData.latestVideo = CurrentVideo;
                videoData.SundayVideos[3] = CurrentVideo;
            }

            if (Service == "AskPastorE")
            {
                videoData.latestVideo = CurrentVideo;
                videoData.TuesdayVideo[0] = CurrentVideo;
            }

            if (Service == "BibleStudy")
            {
                videoData.latestVideo = CurrentVideo;
                videoData.ThursdayVideo[0] = CurrentVideo;
            }

            if (Service == "NightPrayers")
            {
                videoData.latestVideo = CurrentVideo;
                videoData.FridayPrayersVideo[0] = CurrentVideo;
            }

            
            //videoData.latestVideo = CurrentVideo;
            

            videoData.liveVideo.videoId = "";
            videoData.liveVideo.videoImg = "";
            videoData.liveVideo.videoPub = "";
            videoData.liveVideo.videoTitle = "";
            videoData.liveVideo.count = "0";


        }

        public static void getOldData()
        {
            YouTubeService yt = new YouTubeService(new BaseClientService.Initializer() { ApiKey = "AIzaSyBuZWspufZ2JMzXPh6W5mUitfezsOhUiaM" });

            var searchListRequest = yt.PlaylistItems.List("snippet");
            searchListRequest.PlaylistId = "PLKsphOLhItmtHXAUcYOwBKlObFiqQ7yea";
            searchListRequest.MaxResults = 20;
            var searchListResult = searchListRequest.Execute();

            foreach (var item in searchListResult.Items)
            {
                video CurrentVideo = new video();

                Console.WriteLine("ID:" + item.Snippet.ResourceId.VideoId);
                CurrentVideo.videoId = item.Snippet.ResourceId.VideoId;

                Console.WriteLine("snippet:" + item.Snippet.Title);
                CurrentVideo.videoTitle = item.Snippet.Title;

                Console.WriteLine("Published:" + item.Snippet.PublishedAt);
                CurrentVideo.videoPub = item.Snippet.PublishedAt.ToString();

                Console.WriteLine("Image:" + item.Snippet.Thumbnails.Medium.Url);
                CurrentVideo.videoImg = item.Snippet.Thumbnails.Medium.Url;

                CurrentVideo.count = "1";

                videoData.oldVideos.Add(CurrentVideo);
            }
        }

        public static void getLiveData()
        {
            YouTubeService yt = new YouTubeService(new BaseClientService.Initializer() { ApiKey = "AIzaSyBuZWspufZ2JMzXPh6W5mUitfezsOhUiaM" });

            var searchListRequest = yt.Search.List("snippet");
            searchListRequest.ChannelId = "UCP7vi_uJF52wJ9d-oWdVVkA";
            searchListRequest.EventType = SearchResource.ListRequest.EventTypeEnum.Live;
            searchListRequest.Type = "video";
            var searchListResult = searchListRequest.Execute();
           
            if (searchListResult.Items.Count > 0)
            {
                foreach (var item in searchListResult.Items)
                {
                    video CurrentVideo = new video();

                    Console.WriteLine("ID:" + item.Id.VideoId);
                    CurrentVideo.videoId = item.Id.VideoId;

                    Console.WriteLine("snippet:" + item.Snippet.Title);
                    CurrentVideo.videoTitle = item.Snippet.Title;

                    Console.WriteLine("Published:" + item.Snippet.PublishedAt);
                    CurrentVideo.videoPub = item.Snippet.PublishedAt.ToString();

                    Console.WriteLine("Image:" + item.Snippet.Thumbnails.High.Url);
                    CurrentVideo.videoImg = item.Snippet.Thumbnails.High.Url;

                    CurrentVideo.count = "1";

                    videoData.liveVideo = CurrentVideo;
                }
                
            }
            else
            {
                video CurrentVideo = new video();

                Console.WriteLine("No live data");
                CurrentVideo.videoId = "";
                CurrentVideo.videoTitle = "";
                CurrentVideo.videoPub = "";
                CurrentVideo.videoImg = "";
                CurrentVideo.count = "0";

                videoData.liveVideo = CurrentVideo;
            }
            
        }

        public static DateTime getDate(string day)
        {
            DateTime dt = DateTime.Now;
            if(day == "Night Prayers")
            {
                List<DateTime> fridays = new List<DateTime>();
                for (var month = 1; month <= 12; month++)
                {
                    var date = new DateTime(DateTime.Today.Year, month, 1).AddMonths(1).AddDays(-1);
                    while (date.DayOfWeek != DayOfWeek.Friday)
                    {
                        date = date.AddDays(-1);
                    }
                    fridays.Add(date);
                }
                if (DateTime.Now < fridays[DateTime.Now.Month - 1])
                {
                    dt = fridays[DateTime.Now.Month - 2];
                }
                else
                {
                    dt = fridays[DateTime.Now.Month - 1];
                }
            }
            if (day == "Bible Study")
            {
                while (dt.DayOfWeek != DayOfWeek.Thursday ) dt = dt.AddDays(-1);
            }
            if (day == "Ask Pastor E")
            {
                while (dt.DayOfWeek != DayOfWeek.Tuesday) dt = dt.AddDays(-1);
                dt = new DateTime(dt.Year, dt.Month, dt.Day, 19, 0, 0);
            }
            if (day == "Sunday Services")
            {
                while (dt.DayOfWeek != DayOfWeek.Sunday) dt = dt.AddDays(-1);
            }

            return dt;
        }

        public static List<video> getLatestData(DateTime current)
        {

            List<video> set = new List<video>();

            YouTubeService yt = new YouTubeService(new BaseClientService.Initializer() { ApiKey = "AIzaSyBuZWspufZ2JMzXPh6W5mUitfezsOhUiaM" });

            var searchListRequest = yt.Search.List("snippet");
            searchListRequest.ChannelId = "UCP7vi_uJF52wJ9d-oWdVVkA";
            searchListRequest.EventType = SearchResource.ListRequest.EventTypeEnum.Completed;
            if(current.DayOfWeek == DayOfWeek.Sunday)
            {
                searchListRequest.MaxResults = 2;
            }
            else
            {
                searchListRequest.MaxResults = 1;
            }
            
            searchListRequest.Order = SearchResource.ListRequest.OrderEnum.Date;
            searchListRequest.Type = "video";
            searchListRequest.PublishedBefore = current.AddDays(1);
            searchListRequest.PublishedAfter = current.AddDays(-1);
            var searchListResult = searchListRequest.Execute();

            foreach (var item in searchListResult.Items)
            {

                video CurrentVideo = new video();

                Console.WriteLine("ID:" + item.Id.VideoId);
                CurrentVideo.videoId = item.Id.VideoId;

                Console.WriteLine("snippet:" + item.Snippet.Title);
                CurrentVideo.videoTitle = item.Snippet.Title.Replace("&amp;", "and");

                Console.WriteLine("Published:" + item.Snippet.PublishedAt);
                CurrentVideo.videoPub = item.Snippet.PublishedAt.ToString();

                Console.WriteLine("Image:" + item.Snippet.Thumbnails.Medium.Url);
                CurrentVideo.videoImg = item.Snippet.Thumbnails.Medium.Url;

                CurrentVideo.count = "1";

                set.Add(CurrentVideo);
            }

            return set;

        }


        public static video getLastVideo()
        {

            video set = new video();

            YouTubeService yt = new YouTubeService(new BaseClientService.Initializer() { ApiKey = "AIzaSyBuZWspufZ2JMzXPh6W5mUitfezsOhUiaM" });

            var searchListRequest = yt.Search.List("snippet");
            searchListRequest.ChannelId = "UCP7vi_uJF52wJ9d-oWdVVkA";
            searchListRequest.EventType = SearchResource.ListRequest.EventTypeEnum.Completed;
            searchListRequest.MaxResults = 1;
            searchListRequest.Order = SearchResource.ListRequest.OrderEnum.Date;
            searchListRequest.Type = "video";
            var searchListResult = searchListRequest.Execute();

            foreach (var item in searchListResult.Items)
            {

                video CurrentVideo = new video();

                Console.WriteLine("ID:" + item.Id.VideoId);
                CurrentVideo.videoId = item.Id.VideoId;

                Console.WriteLine("snippet:" + item.Snippet.Title);
                CurrentVideo.videoTitle = item.Snippet.Title.Replace("&amp;", "and");

                Console.WriteLine("Published:" + item.Snippet.PublishedAt);
                CurrentVideo.videoPub = item.Snippet.PublishedAt.ToString();

                Console.WriteLine("Image:" + item.Snippet.Thumbnails.Medium.Url);
                CurrentVideo.videoImg = item.Snippet.Thumbnails.Medium.Url;

                CurrentVideo.count = "1";

                set = CurrentVideo;
            }

            return set;

        }


    }

    public class video
    {
        public string videoId;
        public string videoTitle;
        public string videoImg;
        public string videoPub;
        public string count;
    }

    public class videoSet
    {
        public video liveVideo;
        public video latestVideo;
        public List<video> videos;
        public List<video> oldVideos;
        public List<video> SundayVideos;
        public List<video> TuesdayVideo;
        public List<video> ThursdayVideo;
        public List<video> FridayPrayersVideo;
    }
}
