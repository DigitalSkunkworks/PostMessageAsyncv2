using Microsoft.Azure;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AnalyseImagePostMessagev2
{
    public static class HttpPostMessage
    {
        // Create a single, static HttpClient.
        private static HttpClient httpClient = new HttpClient();

        // HTTP API Call to AWS Gateway to Post JSON Message
        public static async Task<Boolean> JsonPostMessage(String itemMessage, TraceWriter log)
        {
            Boolean httpPostReturnReponse = false;
            string jsonPostAPIKEY = null;
            string jsonPostContentResponse = string.Empty;
            string jsonPostURI = "https://qd66fmaff8.execute-api.eu-west-2.amazonaws.com/dev/orchestrator";

            // Retrieve API Key.
            jsonPostAPIKEY = CloudConfigurationManager.GetSetting("jsonPostAPIKEY");

            try
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Add("x-api-key", jsonPostAPIKEY);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Temporary code for POC - just POST LabelAnnotation Messages
                var jsonMessageCheck= new JSONHandler();
                Boolean jsonMessageCheckResponse = jsonMessageCheck.CheckJSON(itemMessage);

                if (jsonMessageCheckResponse)
                {
                    // Main part of code that has been wrapped in temporary If statement.  
                    //HTTP JSON Post
                    //string jsonTestString ="{\"description\":\"hello there\",\"locale\":\"en\"}";
                    HttpResponseMessage response = await httpClient.PostAsync(jsonPostURI, new StringContent(itemMessage, Encoding.UTF8, "application/json"));

                    //Log Response Code from AWS.
                    jsonPostContentResponse = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        IEnumerable<string> jsonPostHeaderResponse;
                        if (response.Headers.TryGetValues("x-amzn-RequestId", out jsonPostHeaderResponse))
                        {
                            log.Info($"AWS Post Success.\nAWSRequestID={jsonPostHeaderResponse.First()}\nAWSAPIResponse={jsonPostContentResponse}");
                        }
                        else
                        {
                            log.Info($"AWS Post Success.\nAWSRequestID=Not Available\nAWSAPIResponse={jsonPostContentResponse}");
                        }
                        httpPostReturnReponse = true;
                    }
                    else
                    {
                        log.Error($"AWS Post Failure.\nAWSAPIResponse={jsonPostContentResponse}");
                    }
                }
                else
                {
                    log.Info($"AWS Post Skipped for this Annotation Type");
                    httpPostReturnReponse = true;
                }
            }
            catch (Exception ex)
            {
                log.Info($"Post Exception found {ex.Message}");
            }
            return httpPostReturnReponse;
        }
    }
}
