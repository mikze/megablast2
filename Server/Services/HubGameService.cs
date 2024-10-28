using Microsoft.AspNetCore.SignalR;
using Server.Game;

namespace Server.Services;

public class HubGameService : BackgroundService
{
    public readonly IHubContext<ChatHub> HubContext;
    public HubGameService(IHubContext<ChatHub> hubContext)
    {
        this.HubContext = hubContext;
        Game.Game.HubGameService = this;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ConsumeEvents();
    }

    private Task ConsumeEvents()
    {
        var tsk = new Task(() =>
        {
            while (true)
            {
                try
                {
                    foreach(var e in Game.Game.GetEntities().Where(e => e.Destroyed))
                        HubContext.Clients.All.SendAsync("RemoveEntity", e.Id);

                    lock(Game.Game.LockObject)
                    {
                        Game.Game.Entities.RemoveAll(e => e.Destroyed);
                    }

                    foreach(var player in Game.Game.Players.Where(p => p is { Live: true, Dead: false } && p.MoveDirection != MoveDirection.None))
                    {
                        player.Moved = true;
                        player.MovePlayer(player.MoveDirection);
                        player.Moved = false;
                    }
                    Thread.Sleep(10);
                    if(Game.Game.Live)
                        HubContext.Clients.All.SendAsync("MovePlayer", Game.Game.Players.Select(p => new PlayerModel(p)).ToArray());
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        });
        tsk.Start();
        return tsk;
    }
}