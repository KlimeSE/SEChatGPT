using HarmonyLib;
using Sandbox.Game.VoiceChat;
using SpaceEngineers.Game.VoiceChat;
using System;
using System.Reflection;
using VRage.Plugins;

namespace SEChatGPT
{
    public class SEChatGPTVoiceHook : IPlugin
    {
        Harmony harmony = new Harmony("SEChatGPT");

        public void Init(object gameInstance)
        {
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public void Update()
        {

        }

        public void Dispose()
        {

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
