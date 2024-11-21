using Microsoft.AspNetCore.SignalR;
using Server.Game;
using Server.Game.Entities;
using Server.Game.Interface;

namespace Server.Services;

public class HubGameService : BackgroundService
{
    public readonly IHubContext<ChatHub> HubContext;

    public HubGameService(IHubContext<ChatHub> hubContext)
    {
        HubContext = hubContext;
        Game.Game.SetHubGameService(this);
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
                MoveLiveMonster();

                Thread.Sleep(10);

                if (Game.Game.Live)
                {
                    SendPlayerLocations();
                    SendMonstersLocations();
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
            Game.Game.RemoveDestroyedEntities();
        }
    }
    
    private void SendMonstersLocations()
    {
        var monsters = Game.Game.GetMonsters().Where(p => p is { Destroyed: false });
        HubContext.Clients.All.SendAsync("MoveMonsters", monsters);
    }
    
    private void MoveLiveMonster()
    {
        foreach (IMonster monster in Game.Game.GetMonsters().Where(p => p is { Destroyed: false }))
            monster.Move(MoveDirection.None);
    }
    
    private void MoveLivePlayers()
    {
        foreach (var player in Game.Game.GetPlayers().Where(p =>
                     p is { Live: true, Dead: false } && p.MoveDirection != MoveDirection.None))
        {
            player.Move(player.MoveDirection);
        }
    }

    private void SendPlayerLocations()
    {
        var playerModels = Game.Game.GetPlayers().Select(p => new PlayerModel(p)).ToArray();
        HubContext.Clients.All.SendAsync("MovePlayer", playerModels);
    }
}