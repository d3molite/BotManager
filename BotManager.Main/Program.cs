using BotManager.Main.Components;
using BotManager.Main.Startup;
using MudBlazor.Services;

ServiceInit.ConfigureLogging();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
# if DEBUG
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

# endif

ServiceInit.RegisterServices(builder);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

# if DEBUG

builder.Services.AddMudServices();

#endif

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

# if DEBUG
app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
# endif

Task.Run(async () => await ServiceInit.StartBots(app));

await app.RunAsync();