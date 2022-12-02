using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace PatternCommand;

interface ICommand
{
    Task ExecuteAsync();
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
            Description = video.Description; // desc
            PrintCaption("Title:");
            Console.WriteLine(Title);
            PrintCaption("Description:");
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
            PrintCaption("Скачиваю видео:");
            Console.WriteLine(Title);
            //var progress = new Progress<double>
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoUrl);
            // Get highest quality muxed stream
            var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();
            // Download the stream to a file
            //Progress<double> pb = new Progress<double>(p => Console.WriteLine($"Progress updated: {p}"));
            await youtube.Videos.Streams.DownloadAsync(streamInfo, $"video.{streamInfo.Container}");
            PrintCaption("Скачивание завершено.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    public void PrintCaption(string caption)
    {
        Console.WriteLine();
        Console.WriteLine("============================");
        Console.WriteLine(caption);
        Console.WriteLine("============================");
        Console.WriteLine();
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
}

class Program
{
    static async Task Main()
    {
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