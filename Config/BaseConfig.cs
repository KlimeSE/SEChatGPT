using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;

namespace SEChatGPT.Config
{
    public static class ConfigService
    {
        private static string defaultPath = Path.Combine(
               Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
               "SpaceEngineers", "Storage", "SEChatGPT.cfg");

        public static BaseConfig ActiveConfig;

        public static void Save(BaseConfig config, string path = null)
        {
            ActiveConfig = config.Clone();

            if (string.IsNullOrEmpty(path))
                path = defaultPath;

            var jsonString = JsonConvert.SerializeObject(ActiveConfig, Formatting.Indented, new StringEnumConverter());
            File.WriteAllText(path, jsonString);
        }

        public static void Load(string path = null)
        {
            if (string.IsNullOrEmpty(path))
                path = defaultPath;

            if (!File.Exists(path))
            {
                // Create a new configuration with default values and save it
                var newConfig = new BaseConfig();
                ActiveConfig = newConfig;
                Save(newConfig, path);
            }

            var jsonString = File.ReadAllText(path);
            ActiveConfig = JsonConvert.DeserializeObject<BaseConfig>(jsonString, new StringEnumConverter());
        }
    }

    public class BaseConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether the feature is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the type of input.
        /// </summary>
        public InputType InputType { get; set; }

        /// <summary>
        /// Gets or sets the type of output.
        /// </summary>
        public OutputType OutputType { get; set; }

        /// <summary>
        /// Gets or sets the GPT model version.
        /// </summary>
        public GPTModel GPTModel { get; set; }

        /// <summary>
        /// Gets or sets the API key for GPT.
        /// </summary>
        public string GPTAPIKey { get; set; }

        /// <summary>
        /// Gets or sets the API key for ElevanLabs.
        /// </summary>
        public string ElevanLabsAPIKey { get; set; }

        /// <summary>
        /// Gets or sets the behaviour for GPT.
        /// </summary>
        public string GPTBehaviour { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseConfig"/> class.
        /// </summary>
        public BaseConfig()
        {
            Enabled = true;
            InputType = InputType.Text;
            OutputType = OutputType.Text;
            GPTModel = GPTModel.GPT3;
            GPTAPIKey = "";
            ElevanLabsAPIKey = "";
            GPTBehaviour = "You are a helpful assistant";
        }

        internal BaseConfig Clone()
        {
            return new BaseConfig()
            {
                Enabled = Enabled,
                InputType = InputType,
                OutputType = OutputType,
                GPTModel = GPTModel,
                GPTAPIKey = GPTAPIKey,
                ElevanLabsAPIKey = ElevanLabsAPIKey,
                GPTBehaviour = GPTBehaviour
            };
        }
    }

    public enum InputType
    {
        Text,
        Voice,
    }

    public enum OutputType
    {
        Text,
        BasicVoice,
        AdvancedVoice,
    }

    public enum GPTModel
    {
        GPT3,
        GPT4
    }
}
