using System.Diagnostics;
using GameEngine.Interface;
using Microsoft.Extensions.Logging;

namespace GameEngine;

public class GameLoop(IWorld world, ILogger<GameLoop> logger)
{
    public IWorld World { get; } = world;

    public async Task RunGameLoopAsync(CancellationToken stoppingToken)
        => await Task.Run(RunGameLoop, stoppingToken);

    public void RunGameLoop(CancellationToken stoppingToken)
        => RunGameLoop();

    public bool Live { get; set; }

    private void RunGameLoop()
    {
        var tick = TimeSpan.FromMilliseconds(16); // ~60 FPS
        var sw = new Stopwatch();
        sw.Start();
        var next = sw.Elapsed;
        Live = true;
        while (true)
        {
            try
            {
                if (!World.Live)
                {
                    Live = false;
                    return;
                }

                var now = sw.Elapsed;
                if (now < next) continue;
                
                if(!World.Pause)
                    World.Update(tick);

                next += tick;
                Thread.Sleep(10);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Live = false;
                throw;
            }
        }
    }
}