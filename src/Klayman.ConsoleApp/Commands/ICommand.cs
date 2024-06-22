namespace Klayman.ConsoleApp.Commands;

public interface ICommand
{
    Task ExecuteAsync(KlaymanServiceClient klaymanServiceClient);
}