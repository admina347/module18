using YoutubeExplode;

namespace PatternCommand;

interface ICommand
{
    Task ExecuteAsync();
    void Undo();
}



class VideoPlayer
{
    YoutubeClient youtube = new YoutubeClient();

    public async Task GetInfoAsync()
    {
        Console.WriteLine("Введите Url video:");
        string videoUrl = Console.ReadLine();
        if (videoUrl != null)
        {
            try
            {
                // You can specify both video ID or URL
                var video = await youtube.Videos.GetAsync(videoUrl);

                var title = video.Title; // "Collections - Blender 2.80 Fundamentals"
                                         //var author = video.Author.ChannelTitle; // "Blender"
                var description = video.Description; // desc
                Console.WriteLine("Title: " + title);
                Console.WriteLine("Description: " + description);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        else
        {
            Console.WriteLine("Вы не ввели Url video! Попробуйте снова.");
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
        await videoPlayer.GetInfoAsync();
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

        // создадим команду 
        var getVideoInfoCommand = new GetVideoInfoCommand(videoPlayer);

        // инициализация команды
        sender.SetCommand(getVideoInfoCommand);

        //  выполнение
        await sender.Run();

    }
}