using SEChatGPT.Config;
using SEChatGPT.Inputs;
using SEChatGPT.Outputs;
using System;
using System.Collections.Generic;
using SEChatGPT.Library;
using Sandbox.ModAPI;

namespace SEChatGPT.Connections
{
    public class GPTConnection : BaseConnection
    {
        private List<BaseInput> registeredInputs = new List<BaseInput>();
        private List<BaseOutput> registeredOutputs = new List<BaseOutput>();
        public Action<GptResponse> GPTOnResponse;

        //GPT HTTP
        private readonly GPTHttpClient gptClient;

        //Core
        Queue<string> mQueue = new Queue<string>();
        List<GptMessage> currentConversation = new List<GptMessage>();

        public GPTConnection(BaseConfig config)
        {
            gptClient = new GPTHttpClient(config.GPTAPIKey, config.GPTModel, config.GPTBehaviour);

            //Set behaviour
            currentConversation.Add(new GptMessage()
            {
                role = "system",
                content = config.GPTBehaviour
            });
        }

        public override void RegisterInputs(List<BaseInput> inputs)
        {
            if (inputs == null) return;
            registeredInputs = new List<BaseInput>(inputs);

            foreach (var input in registeredInputs)
            {
                if (input is TextInput textinput)
                {
                    textinput.TextMessage += InputTextMessage;
                }
            }
        }

        public override void RegisterOutputs(List<BaseOutput> outputs)
        {
            if (outputs == null) return;
            registeredOutputs = new List<BaseOutput>(outputs);
        }

        private void InputTextMessage(string msg)
        {
            if(gptClient == null) return;

            mQueue.Enqueue(msg);
            ProcessQueue();
        }

        private async void ProcessQueue()
        {
            string nextMsg = mQueue.Dequeue();
            if (string.IsNullOrEmpty(nextMsg)) return;

            GptMessage nextGptMessage = new GptMessage()
            {
                role = "user",
                content = nextMsg
            };
            currentConversation.Add(nextGptMessage);

            GptResponse nextResponse = await gptClient.SendRequestAsync(currentConversation).ConfigureAwait(false);
            if (nextResponse?.choices[0]?.message != null)
            {
                currentConversation.Add(nextResponse.choices[0].message);
            }

            GPTOnResponse?.Invoke(nextResponse);
        }

        public override void Unload()
        {
            base.Unload();

            if (registeredInputs != null)
            {
                foreach (var input in registeredInputs)
                {
                    if(input is TextInput textinput)
                    {
                        textinput.TextMessage -= InputTextMessage;
                    }
                }
            }

            registeredInputs.Clear();
            registeredOutputs.Clear();
        }
    }
}
