using HarmonyLib;
using Sandbox.Game.VoiceChat;
using Sandbox.ModAPI;
using SEChatGPT.Config;
using SpaceEngineers.Game.VoiceChat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;
using VRage.Game.Components;
using VRage.Game;
using VRage.Plugins;
using SEChatGPT.Inputs;
using SEChatGPT.Outputs;
using SEChatGPT.Connections;

namespace SEChatGPT
{
    public class SEChatGPTPlugin : IPlugin
    {
        public void Init(object gameInstance)
        {
            RunSetup();
        }

        public void OpenConfigDialog()
        {
            SEChatGPTConfigScreen.Open();
        }

        public void Update()
        {

        }

        public void Dispose()
        {

        }


        private void RunSetup()
        {
            // Initialize config
            ConfigService.Load();
        }
    }

    public class SEChatGPTBot
    { 
        public bool enabled = false;
        public List<BaseInput> activeInputs = new List<BaseInput>();
        public List<BaseOutput> activeOutputs = new List<BaseOutput>();
        public List<BaseConnection> activeConnections = new List<BaseConnection>();

        public SEChatGPTBot(BaseConfig config)
        {
            enabled = config.Enabled;

            // Initialize IO
            if (config.InputType == InputType.Text) activeInputs.Add(new TextInput());
            if (config.OutputType == OutputType.Text) activeOutputs.Add(new TextOutput());

            // Initialize connections
            if (!string.IsNullOrEmpty(config.GPTAPIKey))
            {
                activeConnections.Add(new GPTConnection(config));
            }

            // Register IO
            foreach (var connection in activeConnections)
            {
                connection.RegisterInputs(activeInputs);
                connection.RegisterOutputs(activeOutputs);
            }

            //Register connections
            foreach (var output in activeOutputs)
            {
                output.RegisterConnections(activeConnections);
            }
        }

        public void Close()
        {
            foreach (var input in activeInputs)
            {
                input.Unload();
            }

            foreach (var output in activeOutputs)
            {
                output.Unload();
            }

            foreach (var connection in activeConnections)
            {
                connection.Unload();
            }
        }
    }


    [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
    public class SEChatGPTSession : MySessionComponentBase
    {
        public SEChatGPTBot activeBot;
        bool initialized = false;

        public override void Init(MyObjectBuilder_SessionComponent sessionComponent)
        {

        }

        public override void BeforeStart()
        {

        }

        private void CreateBot(BaseConfig config = null)
        {
            if (config == null) config = ConfigService.ActiveConfig;
            if (config == null) return;

            if (activeBot != null) activeBot.Close();
            activeBot = new SEChatGPTBot(config);
        }

        public override void UpdateAfterSimulation()
        {
            if (!initialized)
            {
                initialized = true;
                CreateBot();
            }
        }

        protected override void UnloadData()
        {
            if (activeBot != null) activeBot.Close();
        }
    }

    [HarmonyPatch]
    public class SEChatGPTVoicePatch
    {
        static OpusDecoder opusDecoder;

        // Get the method we're trying to patch
        static MethodBase TargetMethod()
        {
            // Get the SendBuffer type
            var sendBufferType = AccessTools.Inner(typeof(MyVoiceChatSessionComponent), "SendBuffer");

            // Get the SendVoice method that has a parameter of the SendBuffer type
            return AccessTools.Method(typeof(MyVoiceChatSessionComponent), "SendVoice", new[] { sendBufferType });
        }

        static void Postfix(object receiveBuffer)
        {
            try
            {
                if (opusDecoder == null)
                {
                    opusDecoder = new OpusDecoder(24000, 1);
                }

                // Get the Type of SendBuffer
                Type sendBufferType = typeof(MyVoiceChatSessionComponent).GetNestedType("SendBuffer", BindingFlags.NonPublic);

                if (sendBufferType != null && receiveBuffer.GetType() == sendBufferType)
                {
                    // Use reflection to get the protected fields
                    FieldInfo voiceDataBufferField = sendBufferType.GetField("VoiceDataBuffer", BindingFlags.Public | BindingFlags.Instance);
                    FieldInfo numElementsField = sendBufferType.GetField("NumElements", BindingFlags.Public | BindingFlags.Instance);
                    FieldInfo senderUserIdField = sendBufferType.GetField("SenderUserId", BindingFlags.Public | BindingFlags.Instance);

                    if (voiceDataBufferField != null && numElementsField != null && senderUserIdField != null)
                    {
                        byte[] voiceDataBuffer = (byte[])voiceDataBufferField.GetValue(receiveBuffer);
                        int numElements = (int)numElementsField.GetValue(receiveBuffer);
                        long senderUserId = (long)senderUserIdField.GetValue(receiveBuffer);
                        byte[] uncompressedBuffer = opusDecoder.Decode(voiceDataBuffer).ToArray();
                    }
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
