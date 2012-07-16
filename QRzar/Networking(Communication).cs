using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace QRzar
{
    public partial class Networking
    {
        private static string URL;
        public static string ApiToken;

        public static void Init(string url, int port)
        {
            URL = url + ":" + port;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item">The part that is appended to the URL</param>
        /// <param name="eventHandler">The method that will be invoked when a response is given from the server or if the request fails</param>
        public static void GET(DownloadStringCompletedEventHandler eventHandler, string item = "")
        {
            WebClient wc = new WebClient();
            if (ApiToken != "")
                item = String.Format("{0}?apitoken={1}", item, ApiToken);

            wc.DownloadStringAsync(new Uri(URL + item));
            wc.DownloadStringCompleted += eventHandler;
        }

        public static void POST(UploadStringCompletedEventHandler eventHandler, string data, string item = "")
        {
            WebClient wc = new WebClient();
            wc.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            wc.UploadStringAsync(new Uri(URL + item), "POST", data);
            wc.UploadStringCompleted += eventHandler;
        }

        public static void Login(string email, string password, UploadStringCompletedEventHandler eventHandler)
        {
            string data = "data={\"username\":\"" + email + "\",\"password\":\"" + password + "\"}";
            //A login is a post request 
            POST(new Networking().GetUserDetails + eventHandler, data, "/apitokens");
        }

        public static void CreateUser(string email, string password, string name, string photo, UploadStringCompletedEventHandler eventHandler)
        {
            //Create a temporary LocalUser object so it can be packaged easily into a string
            string data = "data=" + JsonConvert.SerializeObject(new LocalUser(email, password, name, photo:photo));

            //Creating a user is a post request
            POST(new Networking().GetUserDetails + eventHandler, data, "/users");
        }

        public static void GetGames()
        {

        }


        static User user;
        public void GetUserDetails(object sender, UploadStringCompletedEventArgs e)
        {
            lock (this)
            {
                try
                {
                    var dict = JsonConvert.DeserializeObject<System.Collections.Generic.Dictionary<string, object>>(e.Result);

                    if (dict.ContainsKey("apitoken"))
                    {
                        Networking.ApiToken = (string)dict["apitoken"];
                        MessageBox.Show(Networking.ApiToken);
                    }

                    if (dict.ContainsKey("user"))
                    {
                        //The key "user" doesn't point to a string, so it has to be parsed
                        //as an object and then converted to a string
                        user = JsonConvert.DeserializeObject<User>(dict["user"].ToString());
                    }
                }
                catch (WebException ex)
                {
                    Networking.ProcessError(ex);
                }
            }
        }

    }
}
