using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.Utils
{
    public static class FireBaseUtil
    {
        //flashship
        //public const string BrowserAPIKey = "AAAA-1nB5Do:APA91bGkzPEGLTMKfZI0MadiXXH2z6PapzbVAUqwz-7PRs5fw6grpHJCEfE98fHMQSQTDtIKrQ6iYpakCasImuBUB5mzerm1TL8L6kV1wEv8CVOqmNPoT-yV6ss7Eze8_0Gt7spdPjlP";
        //tasetco
        //public const string BrowserAPIKey = "AAAAAI8TvZA:APA91bE7af2t6fDh4evAbx3mshIDIIJz1eNlSVGiVWmoKutUJaP1s1-GSGGXlTvPrOcDaGmhkCiHYb521cfijFNiwyE8WtcVrZSfanJz1MavVS8kRZYRYuNSizwkW8bIkQ5gC0WF3v-Q";
        //vietstar
        //public const string BrowserAPIKey = "AAAAOPH75HQ:APA91bHFsCJ_FRLCZF4gOfu1lfaYD0k4YBboN6y7FM7IUgtJ-SrrEiX8gPgd73QpK0fRrT1locj4fMorrhsmOdL3DI7Mt2GD-KLWIm-IXaL7_1iFGhiHTa07QQtn_Cz5QYZtZH9Oqr70";
        //beep
        public const string BrowserAPIKey = "AAAA4d2UScE:APA91bFQDqwHw5-Ty4X-IxQTFMtlfSv24yHfI9gHrYAL2UHzYH260IQBWjAuLzR5fipz-nmzsFikchGeKgRpkE6AaDrJnl8rkma5j9ZIC7mkMAKRXeCfdwRAB4kk9v5z64bKpvFHdWVN";
        //Airline Express
        //public const string BrowserAPIKey = "AAAAkwOtt6Y:APA91bH1KdoDENb56QrmaSk_aa-J8uNSH5wtsgzah5YVUMnUh61LH38w9omss4YAemAg5b6CsKl_P2q9T5qF_AhI4-l0QO9-EqmZDOCUzDn79TdkkYbI2GVBS6Xoj8_mxKHuls9q72g3";
        //Vintrans
        //public const string BrowserAPIKey = "AAAA-2VAwpg:APA91bE84hyJ7tP6E6So0ErvVGW5252KCxcRd1MYklXBMxWnVKD6AYWCDxdGduVUz6OS2q1NN3wfql3YFdbJr12JcEyoO4JO4NZQqZJrO5Tymyd_ekOaN0OPevax5bo8miRnVJcFtuJu";
        //pcs
        //public const string BrowserAPIKey = "AAAAxbhGYrk:APA91bFJHd2JavDZheIF-iMkXLVtPpdsTVfORq87WCWhls4oSmVfnWDYv8TLsJccw8vUWC_RteyhpG67jbNGKCjoGcqopnn15IdWvEJDEBE1ghifVs2dh7CPen_zbBomicPoTj67TF66";

        public const string CONTENT_TYPE_JSON = "application/json";
        public const string TOPIC_NAME_REQUEST_LADING = "post";//Topic type : YÊU CẦU LẤY HÀNG


        public static async Task<bool> SendNotification(string browserAPIKey,string[] token, string title, string message, int badge)
        {
            if (badge > 0)
            {
                await SendFCMNotification(
                    browserAPIKey
                    , GetPostData(token, title, message, badge)
                    );
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string GetPostData(string[] registrationID, string title, string message, int badge)
        {
            if(registrationID.Length > 1)
            {
                string listToken = "[";
                foreach(var i in registrationID)
                {
                    listToken = listToken + "\"" + i + "\"" + ",";
                }
                listToken = listToken.Remove(listToken.Length - 1) + "]";
                return "{\"registration_ids\":" + listToken + "," +
              "\"priority\": \"high\"," +
              "\"content_available\":true," +
              "\"notification\":{" +
              "\"sound\": \"default\"," +
              "\"badge\": \"" + badge + "\"," +
              "\"title\": \"" + title + "\"," +
              "\"body\":\"" + message + "\"}}"
              ;
            }
            else
            {
                return "{\"to\":\"" + registrationID[0] + "\"," +
             "\"priority\": \"high\"," +
             "\"content_available\":true," +
             "\"notification\":{" +
             "\"sound\": \"default\"," +
             "\"badge\": \"" + badge + "\"," +
             "\"title\": \"" + title + "\"," +
             "\"body\":\"" + message + "\"}}"
             ;
            }
          
        }

        public static async Task<string> SendFCMNotification(string apiKey, string postData)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://fcm.googleapis.com/");

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=" + apiKey);

            HttpContent content = new StringContent(
                postData
                , Encoding.UTF8
                , "application/json");

            HttpResponseMessage response = client.PostAsync("fcm/send", content).Result;

            if (response.IsSuccessStatusCode)
            {
                string sOutput = await response.Content.ReadAsStringAsync();
                return sOutput;
            }
            else
            {
                string sOutput = await response.Content.ReadAsStringAsync();
                return sOutput;
            }
        }
    }
}
