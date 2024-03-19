using Cocona;
using Console.App;
using Console.App.Services;

var builder = CoconaApp.CreateBuilder();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<ReflectService>();

var app = builder.Build();

app.AddCommands<Commands>();
app.Run();