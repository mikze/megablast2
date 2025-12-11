using Game.Game;
using Game.Game.Models;
using GameEngine;

namespace Server.Services;

class GameContext(GameLoop game, string groupName)
{
    public GameLoop Game { get; set; } = game;
    public string GroupName { get; set; } = groupName;
    public List<string> Connections { get; set; } = [];
}
public class GameManager(ICommunicateHandler communicateHandler)
{
    private int gameId;
    
    private List<GameContext> Games { get; } = [];

    public void DestroyGame(Game.Game.Game game) => DestroyGame(GetGameName(game));
    public void DestroyGame(string gameName)
    {
        var game = GetGameByGroupName(gameName);
        if (game is null) return;
        var gameContext = Games.FirstOrDefault(g => g.Game.World.Id == game.Id);
        if (gameContext is null) return;
        
        gameContext.Connections.Clear();
        Games.Remove(gameContext);
        game.Destroy();
    }
    public GameLoop CreateGame(string groupName)
    {
        var oldGame = GetGameByGroupName(groupName);
        
        if (oldGame is not null) throw new Exception($"Game {groupName} already is running");
        if (communicateHandler == null) throw new Exception("Hub game service is null");
        
        var newWorld = new Game.Game.Game(communicateHandler, groupName);
        var newGameLoop = new GameLoop(newWorld, null);
        var gameContext = new GameContext(newGameLoop, groupName);
        var id = Games.Count + 1;
        gameContext.Game.World.Id = id;
        Games.Add(gameContext);
        newGameLoop.RunGameLoopAsync(CancellationToken.None);
        return gameContext.Game;
    }

    public void AddConnectionToGame(string groupName, string connId)
    {
        if(string.IsNullOrWhiteSpace(connId))
            throw new Exception("Connection id is empty");
        
        if(string.IsNullOrWhiteSpace(groupName))
            throw new Exception("Group name is empty");
        
        if(Games.Count == 0)
            throw new Exception("No games found");
        
        if(Games.FirstOrDefault(g => g.GroupName == groupName) is null)
            throw new Exception("Game not found");
        
        var gameContext = Games.FirstOrDefault(g => g.GroupName == groupName);
        gameContext?.Connections.Add(connId);
    }
    
    public Game.Game.Game? GetGameByConnectionId(string connId)
    {
        if(string.IsNullOrWhiteSpace(connId))
            throw new Exception("Connection id is empty");
        
        if(Games.Count == 0)
            throw new Exception("No games found");
        
        var gameContext = Games.FirstOrDefault(g => g.Connections.Contains(connId));
        if(gameContext is null)
            throw new Exception("Game not found");
        
        return gameContext.Game.World as Game.Game.Game;
    }
    
    public bool ClientExist(string connId)
    {
        if (string.IsNullOrWhiteSpace(connId))
            return false;
        
        if(Games.Count == 0)
            return false;
        
        var gameContext = Games.FirstOrDefault(g => g.Connections.Contains(connId));
        if(gameContext is null)
            return false;
        
        return true;
    }
    
    public GameLoop GetGameById(int id)
    {
        var game = Games.FirstOrDefault(g => g.Game.World.Id == id)?.Game;
        
        if(game is null)
            throw new Exception("Game not found");
        
        return game;
    }

    public Game.Game.Game? GetGameByGroupName(string groupName)
        => Games.FirstOrDefault(g => g.GroupName == groupName)?.Game.World as Game.Game.Game;
    


    public string GetGameName(Game.Game.Game game)
    {
        var name = Games.FirstOrDefault(g => g.Game.World.Id == game.Id)?.GroupName;
        if(string.IsNullOrWhiteSpace(name))
            throw new Exception("Game not found");

        return name;
    }
    
    public GameInfo GetGameInfo(Game.Game.Game game)
    {
        var foundGame = Games.FirstOrDefault(g => g.Game.World.Id == game.Id);
        if(foundGame is null || string.IsNullOrEmpty(foundGame.GroupName))
            throw new Exception("Game not found");
        
        var gameInfo = new GameInfo()
        {
            Name = foundGame.GroupName,
            ActivePlayers = (foundGame.Game.World as Game.Game.Game)!.GetPlayers().Count(p => p.Live),
            MaxPlayers = 4,
            PassRequired = false
        };
        
        return gameInfo;
    }

    public Game.Game.Game?[] GetAllGames() => Games.Select(g => g.Game.World as Game.Game.Game).ToArray();
    
}