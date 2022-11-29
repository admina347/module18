using System;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;

namespace PatternCommand;

/// <summary>
/// Базовый класс команды
/// </summary>
abstract class Command
{
    public abstract void Run();
    public abstract void Cancel();
}

/// <summary>
/// Конкретная реализация команды.
/// </summary>
class CommandOne : Command
{
    Receiver receiver;

    public CommandOne(Receiver receiver)
    {
        this.receiver = receiver;
    }

    // Выполнить
    public override async void Run()
    {
        Console.WriteLine("Команда отправлена");
        //receiver.Operation();
        await receiver.GetvideoInfoAsync();
    }

    // Отменить
    public override void Cancel()
    { }
}

/// <summary>
/// Адресат команды
/// </summary>
class Receiver
{
    public void Operation()
    {
        Console.WriteLine("Процесс запущен");
    }

    public async Task GetvideoInfoAsync()
    {
        var youtube = new YoutubeClient();

        // You can specify both video ID or URL
        var video = await youtube.Videos.GetAsync("https://youtube.com/watch?v=u_yIGGhubZs");

        var title = video.Title; // "Collections - Blender 2.80 Fundamentals"
        var author = video.Author.ChannelTitle; // "Blender"
        var duration = video.Duration; // 00:07:20
        Console.WriteLine("Title: " + title);
    }
}

/// <summary>
/// Отправитель команды
/// </summary>
class Sender
{
    Command _command;

    public void SetCommand(Command command)
    {
        _command = command;
    }

    // Выполнить
    public void Run()
    {
        _command.Run();
    }

    // Отменить
    public void Cancel()
    {
        _command.Cancel();
    }
}


/// <summary>
/// Клиентский код
/// </summary>
class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        // создадим отправителя 
        var sender = new Sender();

        // создадим получателя 
        var receiver = new Receiver();

        // создадим команду 
        var commandOne = new CommandOne(receiver);

        // инициализация команды
        sender.SetCommand(commandOne);

        //  выполнение
        sender.Run();
    }
}