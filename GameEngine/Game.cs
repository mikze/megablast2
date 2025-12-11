using GameEngine.Interface;
using Microsoft.Extensions.Logging;

namespace GameEngine;

public class GameConfig
{
    public string GameName { get; set; }
}
public class Game
{
    public static Game Create(GameConfig gameConfig, IWorld world)
    {
        var gameLoop = new GameLoop(world, new Logger<GameLoop>(null));
        var newGame = new Game(gameConfig, gameLoop);
        gameLoop.RunGameLoopAsync(CancellationToken.None).Start();
       return newGame;
    }


    private GameLoop _gameLoop;
    public string GameName { get; set; }
    private Game(GameConfig gameConfig, GameLoop gameLoop)
    {
        GameName = gameConfig.GameName;
        _gameLoop = gameLoop;
    }
}