// See https://aka.ms/new-console-template for more information

using System.Reflection;
using System.Runtime.InteropServices;
using CommandLine;
using Klayman.Application;
using Klayman.ConsoleApp.Commands;
using Klayman.Infrastructure.Windows;
using Klayman.Infrastructure.Windows.WinApi;

var keyboardLayoutManager = CreateKeyboardLayoutManager();

var commandTypes = Assembly.GetExecutingAssembly().GetTypes()
    .Where(t => t.GetCustomAttribute<VerbAttribute>() != null).ToArray();		 

Parser.Default.ParseArguments(args, commandTypes)
    .WithParsed(command =>
    {
        try
        {
            (command as ICommand)?.Execute(keyboardLayoutManager);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    });
return;

IKeyboardLayoutManager CreateKeyboardLayoutManager()
{
    // ReSharper disable once InvertIf
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
        var winApiFunctions = new WinApiFunctions();
        var registryFunctions = new RegistryFunctions(winApiFunctions);
        return new WindowsKeyboardLayoutManager(
            winApiFunctions,
            registryFunctions,
            new LanguageTagFunctions(),
            new KeyboardLayoutFactory(
                new WindowsKeyboardLayoutNameProvider(
                    registryFunctions)));
    }

    throw new NotSupportedException("The OS platform is not supported yet.");
}