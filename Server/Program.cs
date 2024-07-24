var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          //policy.WithOrigins("http://192.168.100.100:8080","http://127.0.0.1:8080", "http://127.0.0.1","http://localhost","http://localhost:8080", "localhost", "127.0.0.1");
                          //policy.AllowAnyOrigin();
                          policy.AllowAnyMethod();
                          //policy.WithMethods("GET", "POST");
                          policy.AllowCredentials()
                          .SetIsOriginAllowed(hostName => true);

                          policy.AllowAnyHeader();
                      });
});

builder.Services.AddSignalR();

var app = builder.Build();
app.UseCors(MyAllowSpecificOrigins);
app.MapGet("/", () => "Hello World!");
app.MapHub<ChatHub>("/Chat");
app.Run();
