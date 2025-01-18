global using BlazorApp.Client.Services.UserService;
global using BlazorApp.Client.Services.SessionService;
global using BlazorApp.Client.Services.QuestionService;
global using BlazorApp.Client.Authentication;
global using BlazorApp.Shared;
global using Microsoft.AspNetCore.Components.Authorization;
global using Blazored.LocalStorage;
using BlazorApp.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress), Timeout= TimeSpan.FromMinutes(5) });
builder.Services.AddScoped<AuthenticationStateProvider,CustomAuthenticationStateProvider>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();

builder.Services.AddTransient<IUserService,UserService>();
builder.Services.AddTransient<ISessionService, SessionService>();
builder.Services.AddTransient<IQuestionService,QuestionService>();




await builder.Build().RunAsync();
