using System;
using ShellProgressBar;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace PatternCommand;

interface ICommand
{
    Task ExecuteAsync();
    void Undo();
}

class VideoPlayer
{
    public string videoUrl { private get; set;}
    public string Title { get; private set;}
    public string Description { get; private set;}
    YoutubeClient youtube = new YoutubeClient();
    

    public async Task GetVideoInfoAsync()
    {
        //Пробуем получить инфо
        try
        {
            // You can specify both video ID or URL
            var video = await youtube.Videos.GetAsync(videoUrl);

            Title = video.Title; // "Collections - Blender 2.80 Fundamentals"
            //var author = video.Author.ChannelTitle; // "Blender"
            Description = video.Description; // desc
            Console.WriteLine();
            Console.WriteLine("============================");
            Console.WriteLine("Title:");
            Console.WriteLine("============================");
            Console.WriteLine();
            Console.WriteLine(Title);
            Console.WriteLine();
            Console.WriteLine("============================");
            Console.WriteLine("Description:");
            Console.WriteLine("============================");
            Console.WriteLine();
            Console.WriteLine(Description);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public async Task DownloadVideoAsync()
    {
        // You can specify both video ID or URL
        //var video = await youtube.Videos.GetAsync("https://youtube.com/watch?v=u_yIGGhubZs");
        try
        {
            Console.WriteLine();
            Console.WriteLine("============================");
            Console.WriteLine("Скачиваю видео:");
            Console.WriteLine("============================");
            Console.WriteLine();
            Console.WriteLine(Title);
            //var progress = new Progress<double>
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoUrl);
            // Get highest quality muxed stream
            var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();
            // Download the stream to a file
            //Progress<double> pb = new Progress<double>(p => Console.WriteLine($"Progress updated: {p}"));
            await youtube.Videos.Streams.DownloadAsync(streamInfo, $"video.{streamInfo.Container}");
            Console.WriteLine();
            Console.WriteLine("============================");
            Console.WriteLine("Скачивание завершено.");
            Console.WriteLine("============================");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    public void Off()
    {
        Console.WriteLine("Телевизор выключен...");
    }   
}

class GetVideoInfoCommand : ICommand
{
    VideoPlayer videoPlayer;
    public GetVideoInfoCommand(VideoPlayer videoPlayer)
    {
        this.videoPlayer = videoPlayer;
    }
    public async Task ExecuteAsync()
    {
        await videoPlayer.GetVideoInfoAsync();
    }
    public void Undo()
    {
        videoPlayer.Off();
    }
}

class DownloadVideoCommand : ICommand
{
    VideoPlayer videoPlayer;
    public DownloadVideoCommand(VideoPlayer videoPlayer)
    {
        this.videoPlayer = videoPlayer;
    }
    public async Task ExecuteAsync()
    {
        await videoPlayer.DownloadVideoAsync();
    }
    public void Undo()
    {
        videoPlayer.Off();
    }
}

// Invoker - инициатор
class Sender
{
    ICommand _command;

    public void SetCommand(ICommand command)
    {
        _command = command;
    }

    public async Task Run()
    {
        await _command.ExecuteAsync();
    }
    public void Cancel()
    {
        _command.Undo();
    }
}




class Program
{
    static async Task Main()
    {
        //Video video = new Video();
        //await video.GetInfoAsync();

        // создадим отправителя 
        var sender = new Sender();

        // создадим получателя 
        var videoPlayer = new VideoPlayer();

        // получить url
        Console.WriteLine("Введите Url видео:");
        videoPlayer.videoUrl = Console.ReadLine();

        // создадим команду 
        var getVideoInfoCommand = new GetVideoInfoCommand(videoPlayer);
        var downloadVideoCommand = new DownloadVideoCommand(videoPlayer);

        // инициализация команды
        sender.SetCommand(getVideoInfoCommand);

        //  выполнение
        await sender.Run();

        // инициализация команды
        sender.SetCommand(downloadVideoCommand);

        //  выполнение
        await sender.Run();
    }
}