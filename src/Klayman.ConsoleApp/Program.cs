// See https://aka.ms/new-console-template for more information

using System.Reflection;
using CommandLine;
using Klayman.ConsoleApp;
using Klayman.ConsoleApp.Commands;

var commandTypes = Assembly.GetExecutingAssembly().GetTypes()
    .Where(t => t.GetCustomAttribute<VerbAttribute>() != null).ToArray();

if (args.Length == 0 || args.First() == "-i")
{
    Console.WriteLine("Welcome to the Keyboard Layout Manager.");
    Console.WriteLine("Type \"help\" to see the list of possible commands.");
    Console.WriteLine("Type \"exit\" to close the program.");
    Console.Write("> ");
    var enteredArgs = Console.ReadLine()?.Split() ?? [];
    while (enteredArgs.FirstOrDefault() != "exit")
    {
        ParseArguments(enteredArgs.ToArray());
        Console.Write("> ");
        enteredArgs = Console.ReadLine()?.Split() ?? [];
    }
}
else
{
    ParseArguments(args);
}
return;

void ParseArguments(string[] args)
{
    Parser.Default.ParseArguments(args, commandTypes)
        .WithParsed(command =>
        {
            try
            {
                (command as ICommand)?.ExecuteAsync(new KlaymanServiceClient()).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        });
}