using System.Runtime.InteropServices;
using Klayman.Application;
using Klayman.Application.KeyboardLayoutManagement;
using Klayman.Application.KeyboardLayoutSetManagement;
using Klayman.Service;
using Klayman.Domain.JsonConverters;
using Klayman.Infrastructure.KeyboardLayoutSetManagement;
using Klayman.Infrastructure.Windows;
using Klayman.Infrastructure.Windows.KeyboardLayoutManagement;
using Klayman.Infrastructure.Windows.WinApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    services.AddScoped<IWinApiFunctions, WinApiFunctions>();
    services.AddScoped<IRegistryFunctions, RegistryFunctions>();
    services.AddScoped<IKeyboardLayoutManager, WindowsKeyboardLayoutManager>();
    services.AddScoped<IKeyboardLayoutNameProvider, WindowsKeyboardLayoutNameProvider>();
    services.AddWindowsService();
}

services.AddScoped<IKeyboardLayoutFactory, KeyboardLayoutFactory>();
services.AddScoped<IKeyboardLayoutSetManager, KeyboardLayoutSetManager>();
services.AddSingleton<IKeyboardLayoutSetCache, KeyboardLayoutSetCache>();
services.AddSingleton<IKeyboardLayoutSetExporter, KeyboardLayoutSetExporter>();

services.AddControllers()
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.Converters.Add(
            new KeyboardLayoutIdJsonConverter());
    });
services.AddHostedService<Worker>(); 

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapControllers();

app.Run();