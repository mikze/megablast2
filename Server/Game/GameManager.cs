using Server.Game.Models;
using Server.Services;

namespace Server.Game;

class GameContext(Game game, string groupName)
{
    public Game Game { get; set; } = game;
    public string GroupName { get; set; } = groupName;
    public List<string> Connections { get; set; } = [];
}
public class GameManager
{
    private HubGameService? _hubGameService;
    private int gameId;
    
    public void SetHubGameService(HubGameService hubGameService)
    {
        _hubGameService = hubGameService;
    }
    private List<GameContext> Games { get; } = [];

    public void DestroyGame(Game game) => DestroyGame(GetGameName(game));
    public void DestroyGame(string gameName)
    {
        var game = GetGameByGroupName(gameName);
        if (game is null) return;
        var gameContext = Games.FirstOrDefault(g => g.Game.Id == game.Id);
        if (gameContext is null) return;
        
        gameContext.Connections.Clear();
        Games.Remove(gameContext);
        game.Destroy();
    }
    public Game CreateGame(string groupName)
    {
        var oldGame = GetGameByGroupName(groupName);
        
        if (oldGame is not null) throw new Exception($"Game {groupName} already is running");
        if (_hubGameService == null) throw new Exception("Hub game service is null");
        
        var gameContext = new GameContext(new Game(_hubGameService, groupName), groupName);
        var id = Games.Count + 1;
        gameContext.Game.Id = id;
        Games.Add(gameContext);
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
    
    public Game GetGameByConnectionId(string connId)
    {
        if(string.IsNullOrWhiteSpace(connId))
            throw new Exception("Connection id is empty");
        
        if(Games.Count == 0)
            throw new Exception("No games found");
        
        var gameContext = Games.FirstOrDefault(g => g.Connections.Contains(connId));
        if(gameContext is null)
            throw new Exception("Game not found");
        
        return gameContext.Game;
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
    
    public Game GetGameById(int id)
    {
        var game = Games.FirstOrDefault(g => g.Game.Id == id)?.Game;
        
        if(game is null)
            throw new Exception("Game not found");
        
        return game;
    }
    
    public Game? GetGameByGroupName(string groupName) => 
        Games.FirstOrDefault(g => g.GroupName == groupName)?.Game;
    

    public string GetGameName(Game game)
    {
        var name = Games.FirstOrDefault(g => g.Game.Id == game.Id)?.GroupName;
        if(string.IsNullOrWhiteSpace(name))
            throw new Exception("Game not found");

        return name;
    }
    
    public GameInfo GetGameInfo(Game game)
    {
        var foundGame = Games.FirstOrDefault(g => g.Game.Id == game.Id);
        if(foundGame is null || string.IsNullOrEmpty(foundGame?.GroupName))
            throw new Exception("Game not found");
        
        var gameInfo = new GameInfo()
        {
            Name = foundGame.GroupName,
            ActivePlayers = foundGame.Game.GetPlayers().Count(p => p.Live),
            MaxPlayers = 4,
            PassRequired = false
        };
        
        return gameInfo;
    }

    public IEnumerable<Game> GetAllGames() => Games.Select(g => g.Game).ToArray();
    
}