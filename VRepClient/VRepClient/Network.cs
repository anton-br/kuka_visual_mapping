using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace VRepClient
{
    public class Network
    {
        private JObject GET(string url)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json; charset=utf-8";
            var response = (HttpWebResponse)httpWebRequest.GetResponse();
            JObject json;
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                json = JObject.Parse(sr.ReadToEnd());
            }
            return json;
        }

        private void POST(string url, string json)
        {
            WebClient wc = new WebClient();
            wc.UploadString(url, "POST", json);
        }

        public bool PythonIsReady()
        {
            string url = "http://127.0.0.1:5000/ready/";

            JObject json = GET(url);
            return Convert.ToBoolean(json["ready"]);

        }

        public string[] GetNewVal()
        {
            string url = "http://127.0.0.1:5000/new_val/";
            JObject json = new JObject();
            try
            {
                json = GET(url);
            }
            catch (System.Net.WebException)
            {
                return new string[0];
            }
            string perm_mass = Convert.ToString(json["perm_mass"]);
            
            perm_mass = Regex.Replace(perm_mass, "[^0-9.,]", "");

            return perm_mass.Split(',');
        }
        int ind = 0;
        public void GetNewImage(string status, string CoordVis="", string CoordSpeed="")
        {
            string url = "http://127.0.0.1:5000/new_image/";

            if (CoordSpeed != "")
                ind++;    
            string train = "False", robot_num="21", width="224", height="224", quality="150";
            string post_request = "{\"image\":\"" + status + "\"," +
                                  "\"train\":\"" + train + "\"," +
                                  "\"robot_num\":\"" + robot_num + "\"," +
                                  "\"width\":\"" + width + "\"," +
                                  "\"height\":\"" + height + "\"," +
                                  "\"quality\":\"" + quality + "\"," +
                                  "\"coordVis\":\"" + CoordVis + "\"," +
                                  "\"coordSpeed\":\"" + CoordSpeed + "\"," +
                                  "\"ind\":\"" + ind + "\"}";
            POST(url, post_request);
        }
    }
}
