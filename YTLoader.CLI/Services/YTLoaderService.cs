using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using VideoLibrary;

namespace YTLoader.CLI.Services
{
    internal class YTLoaderService
    {
        private readonly ILogger _logger;
        private readonly string _directoryPath;
        private readonly int _videoResolution;

        private readonly YouTube _youTubeLoader;

        public YTLoaderService(ILogger logger, string directoryPath, int videoResolution)
        {
            _logger = logger;
            _directoryPath = directoryPath;
            _videoResolution = videoResolution;

            _youTubeLoader = YouTube.Default;
            
        }

        public async Task LoadAsync(string videoUrl, int maxRetries = 5)
        {
            int errorCount = 0;

            while (errorCount <= maxRetries)
            {
                try
                {
                    Console.WriteLine($"Начало скачивания видео: {videoUrl}. Попытка {errorCount}. Поток: {Thread.CurrentThread.ManagedThreadId}");
                    _logger.Debug($"Начало скачивания видео: {videoUrl}. Попытка {errorCount}. Поток: {Thread.CurrentThread.ManagedThreadId}");

                    Stopwatch stopWatch = Stopwatch.StartNew();

                    var (video, audio) = GetVideoAndAudio(videoUrl);
                    if (video == null || audio == null) 
                        throw new InvalidOperationException("Не удалось получить видео или аудио.");

                    Console.WriteLine($"Метаинформация про видео {video.FullName}({video.Resolution}):({videoUrl}) полученна. Выполняется попытка вытянуть видео.");

                    await SaveFilesAsync(video, audio);

                    stopWatch.Stop();
                    LogSuccess(video, stopWatch.Elapsed);
                    return;
                }
                catch (Exception exc)
                {
                    errorCount++;

                    _logger.Error($"URL: {videoUrl} Ошибка({errorCount}): {exc.Message}. \nStackTrace: {exc.StackTrace}");
                    Console.WriteLine($"URL: {videoUrl} Ошибка({errorCount}): {exc.Message}");
                }
            }
        }

        private (YouTubeVideo video, YouTubeVideo audio) GetVideoAndAudio(string videoUrl)
        {
            var videoInfo = _youTubeLoader.GetAllVideos(videoUrl);

            var audio = videoInfo
                .Where(i => i.AudioFormat == AudioFormat.Aac)
                .OrderByDescending(e => e.AudioBitrate)
                .FirstOrDefault();

            var video = videoInfo
                .Where(i => i.Format == VideoFormat.Mp4 && i.Resolution <= _videoResolution)
                .OrderByDescending(e => e.Resolution)
                .FirstOrDefault();

            return (video, audio);
        }

        private async Task SaveFilesAsync(YouTubeVideo video, YouTubeVideo audio)
        {
            var loadPath = Path.Combine(_directoryPath, video.Title);
            if (!Directory.Exists(loadPath))
                Directory.CreateDirectory(loadPath);

            var audiobytes = audio.GetBytes();
            var videoBytes = video.GetBytes();

            Console.WriteLine($"Видео {video.FullName}({video.Resolution}) успешно вытянуто.");

            await File.WriteAllBytesAsync(Path.Combine(loadPath, $"Видео({video.Resolution})_{video.FullName}"), videoBytes);
            await File.WriteAllBytesAsync(Path.Combine(loadPath, $"Аудио({audio.AudioBitrate})_{video.FullName}"), audiobytes);
        }

        private void LogSuccess(YouTubeVideo video, TimeSpan ts)
        {
            string formattedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

            _logger.Debug($"Видео {video.FullName}({ConvertBytesToMegabytes(video.GetBytes().Length)})MB Успешно загруженно за {formattedTime}");
            Console.WriteLine($"Видео {video.FullName}({ConvertBytesToMegabytes(video.GetBytes().Length)})MB Успешно загруженно за {formattedTime}");
        }

        private double ConvertBytesToMegabytes(long bytes)
        {
            return bytes / 1_048_576f;
        }

    }
}
