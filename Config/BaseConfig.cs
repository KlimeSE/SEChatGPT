using Newtonsoft.Json;
using System;
using System.IO;

namespace SEChatGPT.Config
{
    public class ConfigService
    {
        private readonly string defaultPath = Path.Combine(
               Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
               "SpaceEngineers", "Storage", "SEChatGPT.cfg");

        public BaseConfig ActiveConfig;

        public ConfigService()
        {
            Load();
        }

        public void Save(BaseConfig config, string path = null)
        {
            if (string.IsNullOrEmpty(path))
                path = defaultPath;

            var jsonString = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(path, jsonString);
        }

        public void Load(string path = null)
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
            ActiveConfig = JsonConvert.DeserializeObject<BaseConfig>(jsonString);
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
        /// Gets or sets the prompt for GPT.
        /// </summary>
        public string GPTPrompt { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseConfig"/> class.
        /// </summary>
        public BaseConfig()
        {
            Enabled = true;
            InputType = InputType.Voice;
            OutputType = OutputType.BasicVoice;
            GPTModel = GPTModel.GPT3;
            GPTAPIKey = "<GPT_API_KEY>";
            ElevanLabsAPIKey = "<ELEVAN_LABS_API_KEY_OPTIONAL>";
            GPTPrompt = "<GPT_PROMPT>";
        }
    }

    public enum InputType
    {
        Voice,
        Text
    }

    public enum OutputType
    {
        BasicVoice,
        AdvancedVoice,
        Text
    }

    public enum GPTModel
    {
        GPT3,
        GPT4
    }
}
