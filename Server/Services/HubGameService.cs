
using Microsoft.AspNetCore.SignalR;

public class HubGameService : BackgroundService
{
    public IHubContext<ChatHub> hubContext;
    public HubGameService(IHubContext<ChatHub> hubContext)
    {
        this.hubContext = hubContext;
        Game.hubGameService = this;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ConsumeEvents();
    }

    public Task ConsumeEvents()
    {
        var tsk = new Task(() =>
        {
            while (true)
            {
                try
                {
                    foreach(var e in Game.GetEntities().Where(e => e.Destroyed))
                        hubContext.Clients.All.SendAsync("RemoveEntity", e.Id);

                    lock(Game.LockEntities)
                    {
                        Game.Entities.RemoveAll(e => e.Destroyed);
                    }

                    foreach(var player in Game.Players.Where(p => p.Live && !p.Dead && p.MoveDirection != MoveDirection.none))
                    {
                        player.Moved = true;
                        player.MovePlayer(player.MoveDirection);
                        player.Moved = false;
                    }
                    Thread.Sleep(10);
                    if(Game.Live)
                        hubContext.Clients.All.SendAsync("MovePlayer", Game.Players.Select(p => new PlayerModel(p)).ToArray());
                }
                catch
                {

                }
            }
        });
        tsk.Start();
        return tsk;
    }
}