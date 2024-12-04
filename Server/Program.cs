using Server.Game;
using Server.Hubs;
using Server.Services;

var builder = WebApplication.CreateBuilder(args);
const string myAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyMethod();
                          policy.AllowCredentials()
                          .SetIsOriginAllowed(hostName => true);

                          policy.AllowAnyHeader();
                      });
});

builder.Services.AddSignalR();
builder.Services.AddSingleton<GameManager>();
builder.Services.AddHostedService<HubGameService>();

var app = builder.Build();
app.UseCors(myAllowSpecificOrigins);
app.MapHub<ChatHub>("/Chat");
app.Run();
