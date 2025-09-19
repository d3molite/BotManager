using BotManager.Core.Helpers;
using BotManager.Main.Components;
using BotManager.Main.Startup;
using BotManager.WebUi.Modules.Admin;
using MudBlazor.Services;
using Serilog;

ServiceInit.ConfigureLogging();

// begin by loading .env files
var loaded = Env.Load("./env/oauth.env");

if (!loaded)
	Log.Error("Could not load ./env/oauth.env");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

ServiceInit.RegisterServices(builder);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddMudServices();

var app = builder.Build();

await ServiceInit.ApplyMigrations(app);
ServiceInit.SetContainer(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
}
else
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);

	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAntiforgery();
app.UseAuthorization();

app.MapControllers();
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode()
	.AddAdditionalAssemblies(typeof(Admin).Assembly);

Task.Run(async () => await ServiceInit.StartBots(app));

await app.RunAsync();