using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace AnalyseImagePostMessagev2
{
    class JSONHandler
    {
        public Boolean CheckJSON(string rawJSON)
        {
            Boolean jsonPostMessageReturnResponse = false;
            JsonTextReader reader = new JsonTextReader(new StringReader(rawJSON));

            // Parse the JSON remaining in the message stripping some elements. 
            while (reader.Read())
            {
                // strip elements from returned JSON data
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    // need only LabelAnnotation message
                    if (string.Equals("LabelAnnotations", reader.Value.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        jsonPostMessageReturnResponse = true;
                        break;
                    }
                    continue;
                }                
            }
            return jsonPostMessageReturnResponse;
        }
    }
}