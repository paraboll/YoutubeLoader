using System;
using System.Collections.Generic;
using System.Text;

namespace YL.CLI
{
    sealed class Config
    {
        public string BaseDir { get; set; }
        public List<string> VideoUrls { get; set; }
        public int ThreadCount { get; set; } = 1;
        public int VideoResolution { get; set; }
    }

    internal static class ConfigExtensions
    {
        internal static void ValidTo(this Config config)
        {
            var strBuilder = new StringBuilder();

            if (string.IsNullOrEmpty(config.BaseDir))
                strBuilder.Append("BaseDir не может быть пустым");

            //if (config.ThreadCount < 1 || config.ThreadCount > Environment.ProcessorCount)
            //    strBuilder.Append($"Количество поток должно находится в пределах от 1 до {Environment.ProcessorCount}");

            if (config.VideoResolution < 144 || config.VideoResolution > 5000)
                strBuilder.Append("VideoQuality должно быть от 144 до 5000");

            var errorStr = strBuilder.ToString();
            if (errorStr.Length != 0)
                throw new Exception(errorStr);
        }
    }
}
