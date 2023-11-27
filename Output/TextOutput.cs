using Newtonsoft.Json;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using SEChatGPT.Connections;
using SEChatGPT.Library;
using SpaceEngineers.Game.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Utils;

namespace SEChatGPT.Outputs
{
    public class TextOutput : BaseOutput
    {
        private string responsePrefix = "GPT";
        private List<BaseConnection> registeredConnections = new List<BaseConnection>();

        public TextOutput()
        {
            
        }

        public override void RegisterConnections(List<BaseConnection> connections)
        {
            base.RegisterConnections(connections);

            if (connections == null) return;
            registeredConnections = new List<BaseConnection>(connections);

            foreach (var connection in registeredConnections)
            {
                if (connection is GPTConnection gptConnection)
                {
                    gptConnection.GPTOnResponse += GPTResponse;
                }
            }
        }

        private void GPTResponse(GptResponse response)
        {
            if(response == null) return;

            if (response?.choices[0]?.message?.function_call != null)
            {
                RunGameFunction(response.choices[0].message.function_call);
                return;
            }

            if (response?.choices[0]?.message?.content != null)
            {
                var content = response.choices[0].message.content;
                MyAPIGateway.Utilities.ShowMessage(responsePrefix, content);
            }
        }

        private void RunGameFunction(GptFunctionResponse function_call)
        {
            switch (function_call.name)
            {
                case ("startTimer"):
                    StartTimerFunction(function_call.arguments);
                    break;
                case ("pbFunction"):
                    PBFunction(function_call.arguments);
                    break;
                default:
                    break;
            }
        }

        private void StartTimerFunction(string arguments)
        {
            TimerContainer container = JsonConvert.DeserializeObject<TimerContainer>(arguments);
            var timerName = container.TimerStart;
            if (MyAPIGateway.Session?.Player?.Character == null) return;

            var controller = MyAPIGateway.Session.ControlledObject.Entity as MyShipController;
            if (controller == null || controller.CubeGrid == null || controller.CubeGrid.Physics == null) return;

            var gts = MyAPIGateway.TerminalActionsHelper.GetTerminalSystemForGrid(controller.CubeGrid);
            if (gts == null) return;
            var timer = gts.GetBlockWithName(timerName) as IMyTimerBlock;
            if (timer == null) return;

            MyAPIGateway.Utilities.ShowMessage(responsePrefix, "Running your commands");
            timer.StartCountdown();
        }

        private void PBFunction(string arguments)
        {
            CodeContainer container = JsonConvert.DeserializeObject<CodeContainer>(arguments);
            var rawCode = container.CodeString;

            if (MyAPIGateway.Session?.Player?.Character == null) return;

            var controller = MyAPIGateway.Session.ControlledObject.Entity as MyShipController;
            if (controller == null || controller.CubeGrid == null || controller.CubeGrid.Physics == null) return;

            var gts = MyAPIGateway.TerminalActionsHelper.GetTerminalSystemForGrid(controller.CubeGrid);
            if (gts == null) return;
            var pb = gts.GetBlockWithName("gpt") as IMyProgrammableBlock;
            if (pb == null) return;

            MyAPIGateway.Utilities.InvokeOnGameThread(() => SendCodeToPB(pb, rawCode));
        }

        private void SendCodeToPB(IMyProgrammableBlock pb, string rawCode)
        {
            MyAPIGateway.Utilities.ShowMessage(responsePrefix, "Running your commands");
            pb.ProgramData = rawCode;
        }

        public class CodeContainer
        {
            public string CodeString { get; set; }
        }

        public class TimerContainer
        {
            public string TimerStart { get; set; }
        }

        public override void Unload()
        {
            base.Unload();

            if (registeredConnections != null)
            {
                foreach (var connection in registeredConnections)
                {
                    if (connection is GPTConnection gptConnection)
                    {
                        gptConnection.GPTOnResponse -= GPTResponse;
                    }
                }
            }

            registeredConnections.Clear();
        }
    }
}
