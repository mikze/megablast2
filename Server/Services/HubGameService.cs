
using Microsoft.AspNetCore.SignalR;

public class HubGameService : BackgroundService
{
    IHubContext<ChatHub> _hubContext;
    public HubGameService(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
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
                        _hubContext.Clients.All.SendAsync("RemoveEntity", e.Id);

                    Game.GetEntities().RemoveAll(e => e.Destroyed);

                    foreach(var player in Game.Players.Where(p => p.Live && p.MoveDirection != MoveDirection.none))
                    {
                        player.Moved = true;
                        player.MovePlayer(player.MoveDirection);
                        player.Moved = false;
                    }
                    Thread.Sleep(10);
                    _hubContext.Clients.All.SendAsync("MovePlayer", Game.Players.Select(p => new PlayerModel(p)).ToArray());
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