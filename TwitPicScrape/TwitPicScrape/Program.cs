using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace TwitPicScrape
{
    class Program
    {
        static String _picturesDirectory = "C:\\FacesProject\\TwitPics";
        static HashSet<String> _knownNames = new HashSet<String>();
        static void Main(string[] args)
        {
            foreach (String name in Directory.GetFiles(_picturesDirectory, "*.jpg"))
                _knownNames.Add(name);

            Dictionary<DateTime, string> nameDic = new Dictionary<DateTime, string>();
            foreach (String name in _knownNames)
                nameDic.Add(Directory.GetCreationTime(name), name);

            var lastime = nameDic.Count() > 0 ? nameDic.Last().Key : DateTime.Now;

            StreamReader r = new StreamReader(_picturesDirectory +  "\\list.txt");
            String line;
            while ((line = r.ReadLine()) != null)
            {
                nameDic.Add(lastime, line);
                lastime = lastime.AddSeconds(1);
            }

            foreach (var item in nameDic.OrderByDescending(x=>x.Key))
                FindFollowers(
                    String.Format("https://api.twitter.com/1/followers/ids.json?screen_name=@{0}", Path.GetFileNameWithoutExtension(item.Value)));
        }
        private static void FindFollowers(String url)
        {
            using (var webClient = new WebClient())
            {
                string html = webClient.DownloadString(url);
                JavaScriptSerializer ser = new JavaScriptSerializer();
                var dict = ser.Deserialize<Dictionary<string, object>>(html);
                var ids = ((System.Collections.ArrayList)dict["ids"]);
                foreach (int id in ids)
                {
                    String screenNameURL = String.Format("https://api.twitter.com/1/users/show.json?user_id={0}&include_entities=true", id);
                    string html2 = webClient.DownloadString(screenNameURL);
                    var dict2 = ser.Deserialize<Dictionary<string, object>>(html2);
                    String screenName = (String) dict2["screen_name"];
                    Console.WriteLine(id + ": " + screenName);

                    if (_knownNames.Contains(screenName)) continue;

                    String profilPicURL = String.Format("https://api.twitter.com/1/users/profile_image?screen_name={0}&size=original", screenName);
                    MemoryStream ms = new MemoryStream(webClient.DownloadData(profilPicURL));
                    System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                    img.Save(_picturesDirectory + "\\" + screenName + ".jpg");
                }
            }
        }
    }
}
