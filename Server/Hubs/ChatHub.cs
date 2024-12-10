using Microsoft.AspNetCore.SignalR;
using Server.Game;
using Server.Game.Entities;
using Server.Game.Models;

namespace Server.Hubs;

public class ChatHub(GameManager gameManager) : Hub
{
    private Game.Game GetGame(string gameName)
    {
        return gameManager.GetGameByGroupName(gameName);
    }

    private readonly List<string> _clients = [];

    public async Task SendMessage(string user, string message)
    {
        var game = gameManager.GetGameByConnectionId(Context.ConnectionId);
        var player = game.GetPlayers().FirstOrDefault(p => p.Id == Context.ConnectionId);
        Console.WriteLine($"Received {user}: {message}");
        if (player != null)
        {
            Console.WriteLine("Sending");
            await Clients.Group(gameManager.GetGameName(game)).SendAsync("ReceiveMessage",Context.ConnectionId, player.Name, message);
        }
    }
    public async Task RestartGame()
    {
        var game = gameManager.GetGameByConnectionId(Context.ConnectionId);
        if (game.IsMasterPlayer(Context.ConnectionId))
        {
            foreach(var c in _clients)
                Console.WriteLine($"RestartGame to client {c}");

            await game.RestartGame();
        }
    }
    
    public async Task SetConfig(GameConfig config)
    {
        var game = gameManager.GetGameByConnectionId(Context.ConnectionId);
        game.SetGameConfig(config);
        await GetConfigAll();
    }

    public async Task AmIAdmin()
    {
        var game = gameManager.GetGameByConnectionId(Context.ConnectionId);
        var player = game.GetPlayers().FirstOrDefault(p => p.Id == Context.ConnectionId);
        if (player != null)
        {
            Console.WriteLine($"Setting admin {game.GetPlayers()[0].Id == player.Id}");
            await Clients.Caller.SendAsync("IsAdmin", game.GetPlayers()[0].Id == player.Id);
        }
        else
            Console.WriteLine("Player not found");
    }
    public async Task GetConfigAll()
    {
        var game = gameManager.GetGameByConnectionId(Context.ConnectionId);
        await Clients.Group(gameManager.GetGameName(game)).SendAsync("GetConfig", game.GetConfig());
    }

    public async Task GetConfig()
    {
        var game = gameManager.GetGameByConnectionId(Context.ConnectionId);
        await Clients.Caller.SendAsync("GetConfig", game.GetConfig());
    }
    
    public void MovePlayer(int moveDirection)
    {
        var game = gameManager.GetGameByConnectionId(Context.ConnectionId);
        var player = game.GetPlayers().FirstOrDefault(p => p.Id == Context.ConnectionId);
        if (player is { Moved: false })
            player.MoveDirection = (MoveDirection)moveDirection;
    }

    public async Task PlantBomb()
    {
        var game = gameManager.GetGameByConnectionId(Context.ConnectionId);
        var player = game.GetPlayers().FirstOrDefault(p => p.Id == Context.ConnectionId);
        var bomb = player?.PlantBomb();
        if (bomb != null)
            await Clients.Group(gameManager.GetGameName(game)).SendAsync("BombPlanted", new BombModel(bomb));
    }

    public void GetMonsters()
    {
        var game = gameManager.GetGameByConnectionId(Context.ConnectionId);
        var monsters = game.GetMonsters();
        Clients.Caller.SendAsync("GetMonsters", monsters);
    }
    public void ChangeName(string newName)
    {
        var game = gameManager.GetGameByConnectionId(Context.ConnectionId);
        var player = game.GetPlayers().FirstOrDefault(p => p.Id == Context.ConnectionId);
        if (player is null) return;
        
        game.ChangeName(Context.ConnectionId, newName);
        Clients.Group(gameManager.GetGameName(game)).SendAsync("NameChanged", Context.ConnectionId, newName);
    }

    public void ChangeSkin(string newSkinName)
    {
        var game = gameManager.GetGameByConnectionId(Context.ConnectionId);
        var player = game.GetPlayers().FirstOrDefault(p => p.Id == Context.ConnectionId);
        if (player is null) return;
        
        game.ChangeSkin(Context.ConnectionId, newSkinName);
        Clients.Group(gameManager.GetGameName(game)).SendAsync("SkinChanged", Context.ConnectionId, newSkinName);
    }

    public async Task GetMap() => await Clients.Caller.SendAsync("GetMap", gameManager.GetGameByConnectionId(Context.ConnectionId).GetMap());
    
    public async Task BackToLobby()
    {
        var game = gameManager.GetGameByConnectionId(Context.ConnectionId);
        if (game.IsMasterPlayer(Context.ConnectionId))
        {
            var player = game.GetPlayers().FirstOrDefault(p => p.Id == Context.ConnectionId);
            if (player != null)
            {
                game.Live = false;
                await Clients.Group(gameManager.GetGameName(game)).SendAsync("BackToLobby");
            }
        }
    }

    public async Task Start()
    {
        var game = gameManager.GetGameByConnectionId(Context.ConnectionId);
        if (game.IsMasterPlayer(Context.ConnectionId))
        {
            game.Live = true;
            await game.RestartGame();
            var gameName = gameManager.GetGameName(game);
            await Clients.Group(gameName).SendAsync("Start");
        }
    }

    public async Task CreateGame(string gameName)
    {
        if (!string.IsNullOrWhiteSpace(gameName))
        {
            Console.WriteLine("Create game");
            gameManager.CreateGame(gameName);
            await Clients.Caller.SendAsync("JoinToGame", gameName);
        }
    }
    
    public Task BackToServerList()
    {
        try
        {
            var game = gameManager.GetGameByConnectionId(Context.ConnectionId);
            if (game.IsMasterPlayer(Context.ConnectionId))
                gameManager.DestroyGame(game);
            
            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }
    
    public Task CloseGame(string gameName)
    {
        Console.WriteLine("Destroy game");
        gameManager.DestroyGame(gameName);
        return Task.CompletedTask;
    }
    
    public async Task GetRunningAllGames()
    {
        await Clients.Caller.SendAsync("RunningAllGames", gameManager.GetAllGames().Select(gameManager.GetGameName));
    }
    
    public async Task JoinToGroup(string groupName)
    {
        if (!gameManager.ClientExist(Context.ConnectionId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            gameManager.AddConnectionToGame(groupName, Context.ConnectionId);
            Console.WriteLine($"{Context.ConnectionId} has joined the group {groupName}.");

            var game = gameManager.GetGameByGroupName(groupName);
            var players = game?.GetPlayers().Where(p => p.Live);
            if (players != null)
            {
                var enumerable = players as Player[] ?? players.ToArray();
                if (enumerable.Length >= 4)
                {
                    await base.OnConnectedAsync();
                    await Clients.Caller.SendAsync("ServerIsFull");
                    await Clients.Caller.SendAsync("Connected", enumerable.Where(p => p.Live).ToArray(),
                        Context.ConnectionId);
                }
                else
                {
                    await base.OnConnectedAsync();
                    game?.AddPlayer(Context.ConnectionId);
                    if (game != null)
                        await Clients.Group(gameManager.GetGameName(game)).SendAsync("Connected",
                            game.GetPlayers().Where(p => p.Live).ToArray(), Context.ConnectionId);
                }
            }
        }
    }
    

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"Disconnected {Context.ConnectionId}");
        _clients.Remove(Context.ConnectionId);
        try
        {
            var game = gameManager.GetGameByConnectionId(Context.ConnectionId);
            game.RemovePlayer(Context.ConnectionId);
            Clients.Group(gameManager.GetGameName(game)).SendAsync("Disconnected", game.GetPlayers().Where(p => p.Live).ToArray(), Context.ConnectionId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return base.OnDisconnectedAsync(exception);
    }
}