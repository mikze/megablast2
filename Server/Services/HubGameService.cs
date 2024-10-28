using Microsoft.AspNetCore.SignalR;
using Server.Game;

namespace Server.Services;

public class HubGameService : BackgroundService
{
    public readonly IHubContext<ChatHub> HubContext;

    public HubGameService(IHubContext<ChatHub> hubContext)
    {
        HubContext = hubContext;
        Game.Game.HubGameService = this;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ConsumeEventsAsync();
    }

    private Task ConsumeEventsAsync()
    {
        var task = new Task(EventProcessingLoop);
        task.Start();
        return task;
    }

    private void EventProcessingLoop()
    {
        while (true)
        {
            try
            {
                RemoveDestroyedEntities();

                MoveLivePlayers();

                Thread.Sleep(10);

                if (Game.Game.Live)
                {
                    SendPlayerLocations();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    private void RemoveDestroyedEntities()
    {
        foreach (var entity in Game.Game.GetEntities().Where(e => e.Destroyed))
        {
            HubContext.Clients.All.SendAsync("RemoveEntity", entity.Id);
        }

        lock (Game.Game.LockObject)
        {
            Game.Game.Entities.RemoveAll(e => e.Destroyed);
        }
    }

    private void MoveLivePlayers()
    {
        foreach (var player in Game.Game.Players.Where(p =>
                     p is { Live: true, Dead: false } && p.MoveDirection != MoveDirection.None))
        {
            player.MovePlayer(player.MoveDirection);
        }
    }

    private void SendPlayerLocations()
    {
        var playerModels = Game.Game.Players.Select(p => new PlayerModel(p)).ToArray();
        HubContext.Clients.All.SendAsync("MovePlayer", playerModels);
    }
}