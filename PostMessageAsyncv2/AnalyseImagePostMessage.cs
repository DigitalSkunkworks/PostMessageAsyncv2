using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Threading.Tasks;

namespace AnalyseImagePostMessage
{
    public static class AnalyseImagePostMessage
    {
        [FunctionName("AnalyseImagePostMessagev2")]
        public static async Task Run([QueueTrigger("%MSGQ_NAME_VISION_ANALYSER%", Connection = "ocrmsgqueue")]CloudQueueMessage myQueueItem, TraceWriter log)
        {
            Boolean QueueReturnReponse = false;
            try
            {
                log.Info($"Queue Trigger processed: \n" +
                $"id={myQueueItem.Id}\n" +
                $"expirationTime={myQueueItem.ExpirationTime}\n" +
                $"insertionTime={myQueueItem.InsertionTime}\n" +
                $"dequeueCount={myQueueItem.DequeueCount}");

                // Json Post Call
                log.Info($"Calling Post Message.");
                QueueReturnReponse = await HttpPostMessage.JsonPostMessage(myQueueItem.AsString, log);
                log.Info($"HTTP Reponse Code: {QueueReturnReponse}");
                if (QueueReturnReponse == false)
                {
                    throw new Exception ("HTTP Post Failed.  Throw Exception.");
                }
            }
            catch (Exception ex)
            {
                log.Info($"Trigger Exception found: {ex.Message}");
                throw ex;
            }
            //return QueueReturnReponse;
        }
    }
}
