using Sandbox.ModAPI;
using SEChatGPT.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEChatGPT.Inputs
{
    public class TextInput : BaseInput
    {
        private string activationText = "/gpt ";
        public Action<string> TextMessage;
        public string playerName;

        public TextInput()
        {
            MyAPIGateway.Utilities.MessageEntered += MessageEntered;
            playerName = MyAPIGateway.Session?.Player?.DisplayName;
        }

        private void MessageEntered(string messageText, ref bool sendToOthers)
        {
            if (!messageText.StartsWith(activationText)) return;

            var message = messageText.Substring(activationText.Length).Trim();

            MyAPIGateway.Utilities.ShowMessage(playerName, message);
            TextMessage?.Invoke(message);
            sendToOthers = false;
        }

        public override void Unload()
        {
            MyAPIGateway.Utilities.MessageEntered -= MessageEntered;
        }
    }
}
