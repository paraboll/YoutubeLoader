using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using YTLoader.CLI.Services;

namespace YL.CLI
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Logger logger = LogManager.GetCurrentClassLogger();

            try
            {
                var file = File.ReadAllText("config.json", Encoding.UTF8);
                logger.Debug($"Загруженный конфиг: {file}");
               
                var config = JsonConvert.DeserializeObject<Config>(file);
                config.ValidTo();

                if (!Directory.Exists(config.BaseDir))
                    Directory.CreateDirectory(config.BaseDir);
                
                await LoadVideosAsync(config, logger);
            }
            catch (Exception exc)
            {
                logger.Fatal($"Произошла непредвиденная ошибка: {exc.Message} \n\rStackTrace: {exc.StackTrace}");
            }
        }

        static async Task LoadVideosAsync(Config config, ILogger logger)
        {
            var youTubeLoader = new YTLoaderService(logger, config.BaseDir, config.VideoResolution);

            var giud = Guid.NewGuid();
            logger.Debug($"Загрузка {giud} начинается {DateTime.Now}");

            var tasks = config.VideoUrls
                .AsParallel()
                .WithDegreeOfParallelism(config.ThreadCount)
                .Select(url => youTubeLoader.LoadAsync(url));

            await Task.WhenAll(tasks);

            Console.WriteLine($"Загрузка прошла успешно {DateTime.Now}");
            logger.Debug($"Загрузка {giud} прошла успешно {DateTime.Now}");
        }
    }
}
