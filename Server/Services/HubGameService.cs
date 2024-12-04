using Microsoft.AspNetCore.SignalR;
using Server.Game;
using Server.Hubs;

namespace Server.Services;

public class HubGameService : BackgroundService
{
    public HubGameService(IHubContext<ChatHub> hubContext, GameManager gameManager)
    {
        HubContext = hubContext;
        _gameManager = gameManager;
        _gameManager.SetHubGameService(this);
    }
    private readonly GameManager _gameManager;
    public readonly IHubContext<ChatHub> HubContext;

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
            foreach (var game in _gameManager.GetAllGames())
            {
                try
                {
                    RemoveDestroyedEntities(game);

                    MoveLivePlayers(game);
                    MoveLiveMonster(game);

                    Thread.Sleep(10);

                    if (!game.Live) continue;
                    SendPlayerLocations(game);
                    SendMonstersLocations(game);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }

    private void RemoveDestroyedEntities(Game.Game game)
    {
        foreach (var entity in game.GetEntities().Where(e => e.Destroyed))
        {
            HubContext.Clients.All.SendAsync("RemoveEntity", entity.Id);
        }
        
        game.RemoveDestroyedEntities();
    }
    
    private void SendMonstersLocations(Game.Game game)
    {
        var monsters = game.GetMonsters().Where(p => p is { Destroyed: false });
        HubContext.Clients.All.SendAsync("MoveMonsters", monsters);
    }
    
    private void MoveLiveMonster(Game.Game game)
    {
        foreach (var monster in game.GetMonsters().Where(p => p is { Destroyed: false }))
            monster.Move(MoveDirection.None);
    }
    
    private void MoveLivePlayers(Game.Game game)
    {
        foreach (var player in game.GetPlayers().Where(p =>
                     p is { Live: true, Dead: false } && p.MoveDirection != MoveDirection.None))
        {
            player.Move(player.MoveDirection);
        }
    }

    private void SendPlayerLocations(Game.Game game)
    {
        var playerModels = game.GetPlayers().Select(p => new PlayerModel(p)).ToArray();
        HubContext.Clients.All.SendAsync("MovePlayer", playerModels);
    }
}