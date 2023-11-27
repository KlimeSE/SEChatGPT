using Newtonsoft.Json;
using Sandbox.ModAPI;
using SEChatGPT.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SEChatGPT.Library
{
    public class GPTHttpClient : BaseHttpClient
    {
        private readonly string gptAPIKey;
        private readonly string gptModel;
        private readonly string gptURL = "https://api.openai.com/v1/chat/completions";
        private readonly string gptBehaviour = "";

        private Dictionary<GPTModel, string> gptModelLookup = new Dictionary<GPTModel, string>()
        {
            { GPTModel.GPT3, "gpt-3.5-turbo" },
            { GPTModel.GPT4 , "gpt-4" },
        };

        public GPTHttpClient(string gptAPIKey, GPTModel gptModel, string gptBehaviour)
        {
            this.gptAPIKey = gptAPIKey ?? DisplayErrorAndReturnNull("API key cannot be null");
            this.gptModel = gptModelLookup[gptModel] ?? DisplayErrorAndReturnNull("Model cannot be null");
            this.gptBehaviour = gptBehaviour;

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", gptAPIKey);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        }

        public async Task<GptResponse> SendRequestAsync(List<GptMessage> currentConversation)
        {
            List<object> outgoingMessages = new List<object>();
            List<object> outgoingFunctions = new List<object>();
            foreach (var msg in currentConversation)
            {
                outgoingMessages.Add(new
                {
                    msg.role,
                    msg.content
                });
            }


            var startFunctionParameters = new
            {
                type = "object",
                properties = new
                {
                    timerStart = new
                    {
                        type = "string",
                        description = "Name of Timer Block"
                    }
                }
            };

            var startFunction = new
            {
                name = "startTimer",
                description = "Start Timer Block",
                parameters = startFunctionParameters
            };

            outgoingFunctions.Add(startFunction);

            var requestBody = new
            {
                model = gptModel,
                max_tokens = 3000,
                messages = outgoingMessages,
                functions = outgoingFunctions,
                function_call = "auto"
            };

            var response = await PostAsync<GptResponse>(gptURL, requestBody).ConfigureAwait(false);

            StringBuilder fullContentBuilder = new StringBuilder();

            // Handle chunked replies
            if (response?.ObjectValue == "chat.completion.chunk")
    {
                // Append the initial chunk
                fullContentBuilder.Append(response.choices.First().delta.content);

                // Check finish reason to decide if more chunks need to be fetched
                while (response.choices.First().finish_reason != "stop")
                {
                    // You may need to wait or use the provided ID to get the next chunk.
                    // This is just a basic mock and might vary based on the actual API's chunk fetching mechanism.
                    response = await PostAsync<GptResponse>(gptURL, requestBody).ConfigureAwait(false);
                    fullContentBuilder.Append(response.choices.First().delta.content);
                }

                // Update the response's choice message with the full content
                response.choices.First().delta.content = fullContentBuilder.ToString();
            }

            return response;
        }

        private string DisplayErrorAndReturnNull(string message)
        {
            DisplayError(message);
            return null;
        }
    }

    public class GptResponse
    {
        public string id { get; set; }

        [JsonProperty(PropertyName = "object")]
        public string ObjectValue { get; set; }

        public long created { get; set; }
        public string model { get; set; }
        public List<GptChoice> choices { get; set; }
    }

    public class GptChoice
    {
        public int index { get; set; }
        public GptDelta delta { get; set; }
        public string finish_reason { get; set; }
    }

    public class GptDelta
    {
        public string content { get; set; }
    }

    public class GptMessage
    {
        public string role { get; set; }
        public string content { get; set; }

        public GptFunctionResponse function_call { get; set; }
    }

    public class GptFunctionResponse
    {
        public string name { get; set; }
        public string arguments { get; set; }
    }
}
