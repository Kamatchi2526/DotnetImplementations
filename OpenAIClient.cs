using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Mvc;
using Newtonsoft.Json;
using Sitecore.Diagnostics;
using Newtonsoft.Json.Linq;
using Sitecore.Data.Validators.FieldValidators;

namespace Common
{
    public class OpenAIClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        

        public HttpResponseMessage GetResponsefromOpenAI(string _content_or_transcript,string _OpenAIApiKey)

        {
            
            string apiKey = _OpenAIApiKey
            
            var _httpClient = new HttpClient();
            
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            var payloadObj = new Payload();
            payloadObj.model = "gpt-3.5-turbo";
            payloadObj.max_tokens = 256;
            var msgObj = new message();
            msgObj.role = "user";
            msgObj.content = _content + " can you provide Title Subtitle Description";
            List<message> msobjlst = new List<message>();
            msobjlst.Add(msgObj);
            payloadObj.messages = msobjlst;
            string url = "https://api.openai.com/v1/chat/completions";
            string msgBody = JsonConvert.SerializeObject(payloadObj);
            
            var content = new StringContent(msgBody, Encoding.UTF8, "application/json");
            HttpResponseMessage result = _httpClient.PostAsync(url, content).Result;
            HttpContent content1 = result.Content;
            var results = content1.ReadAsStringAsync().Result;
            if (results!= null)
            {
                String AIResult = Convert.ToString(JObject.Parse(results)["choices"][0]["message"]["content"]);
                String title = GetstringBetweenPhrases(AIResult, "Title:", "Subtitle");
                String Subtitle = GetstringBetweenPhrases(AIResult, "Subtitle:", "Description");
                String Description = GetstringBetweenPhrases(AIResult, "Description:", "");
                Console.WriteLine("Please check the GPT Response: " + AIResult);

            }
            return result;


        }

        public  string GetstringBetweenPhrases(string Text, string FirstString, string LastString)
        {
            string STR = Text;
            string STRFirst = FirstString;
            string STRLast = LastString;
            string FinalString;
            int Pos1 = STR.IndexOf(FirstString) + FirstString.Length;
            int Pos2;
            if (String.IsNullOrEmpty(LastString))
            {
                Pos2 = STR.Length;
            }
            else
            {
                Pos2 = STR.IndexOf(LastString);
            }
            FinalString = STR.Substring(Pos1, Pos2 - Pos1);

            return FinalString;
        }

    }


    public class message
    {
        public string role { get; set; }
        public string content { get; set; }
    }

    public class Payload
    {
        public string model { get; set; }
        //public int temparature { get; set; }
        //public int top_p { get; set; }
        public int max_tokens { get; set; }
        //public int frequency_penalty { get; set; }
        //public int presence_penalty { get; set; }
        public List<message> messages;
    }

}

