using System;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;

namespace PatternCommand;

interface ICommand
{
    void ExecuteAsync();
    void Undo();
}



class Video
{
    YoutubeClient youtube = new YoutubeClient();

    public async Task GetInfoAsync()
    {
        // You can specify both video ID or URL
        var video = await youtube.Videos.GetAsync("https://youtube.com/watch?v=u_yIGGhubZs");

        var title = video.Title; // "Collections - Blender 2.80 Fundamentals"
        //var author = video.Author.ChannelTitle; // "Blender"
        var description = video.Description; // desc
        Console.WriteLine("Title: " + title);
        Console.WriteLine("Description: " + description);
    }

    public void Off()
    {
        Console.WriteLine("Телевизор выключен...");
    }
}

class GetVideoInfoCommand : ICommand
{
    Video video;
    public GetVideoInfoCommand(Video video)
    {
        this.video = video;
    }
    public async void ExecuteAsync()
    {
        await video.GetInfoAsync();
    }
    public void Undo()
    {
        video.Off();
    }
}

class Sender
{
    ICommand _command;

    public void SetCommand(ICommand command)
    {
        _command = command;
    }

    // Выполнить
    public void Run()
    {
        _command.ExecuteAsync();
    }

    // Отменить
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
        var video = new Video();

        // создадим команду 
        var getVideoInfoCommand = new GetVideoInfoCommand(video);

        // инициализация команды
        sender.SetCommand(getVideoInfoCommand);

        //  выполнение
        sender.Run();

    }
}