using Cocona;
using Console.App;
using Console.App.Services;

var builder = CoconaApp.CreateBuilder();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<ReflectService>();
builder.Services.AddSingleton<GitLabService>();
builder.Services.AddSingleton<RepositoryService>();

var app = builder.Build();

app.AddCommands<Commands>();
app.Run();