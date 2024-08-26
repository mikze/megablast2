var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyMethod();
                          policy.AllowCredentials()
                          .SetIsOriginAllowed(hostName => true);

                          policy.AllowAnyHeader();
                      });
});

builder.Services.AddSignalR();
builder.Services.AddHostedService<HubGameService>();

var app = builder.Build();
app.UseCors(MyAllowSpecificOrigins);
app.MapHub<ChatHub>("/Chat");
app.Run();
