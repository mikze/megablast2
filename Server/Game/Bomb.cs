
using Microsoft.AspNetCore.SignalR;

public static class Bomb
{
    static readonly object Locker = new object(); 
    public static Task Plant(int x, int y)
    {
        return new Task(async () =>
        {
            try{
            Console.WriteLine($"PLANT BOMB");
            await Task.Delay(2000);
            await SendToClients(Explode(x, y));
            }
            catch
            {
                Console.WriteLine("nie udalo sie splantowac paki");
            }
        });

        static async Task SendToClients(List<Fire> fires)
        {
            await Game.hubGameService.hubContext.Clients.All.SendAsync("Fires", fires.ToArray());
        }

        static List<Fire> Explode(int x, int y)
        {        
            lock(Locker)
            {  

            int size = 4;
            List<Fire> fires1 = new List<Fire>();
            List<Fire> fires2 = new List<Fire>();
            List<Fire> fires3 = new List<Fire>();
            List<Fire> fires4 = new List<Fire>();

            for (int i = 1; i <= size; i++)
                fires1.Add(new Fire() { PosX = x, PosY = y + (i * 32) });

            for (int i = 1; i <= size; i++)
                fires2.Add(new Fire() { PosX = x + (i * 32), PosY = y });

            for (int i = -1; i >= -size; i--)
                fires3.Add(new Fire() { PosX = x, PosY = y + (i * 32) });

            for (int i = -1; i >= -size; i--)
                fires4.Add(new Fire() { PosX = x + (i * 32), PosY = y });
            
            var f = Calc(fires1);
            f.AddRange(Calc(fires2));
            f.AddRange(Calc(fires3));
            f.AddRange(Calc(fires4));
                   
            return f;

            }
        }

        static List<Fire> Calc(List<Fire> fires)
        {
            List<Fire> firs = new List<Fire>();
                     
                foreach (var f in fires)
                {
                    bool stop = false;

                    foreach (var e in Game.GetEntities().Where(a => a is Wall || a is Player).Where(y => y is object && y.Destructible))
                        if (e is object && e.Destructible && !e.Destroyed && e.CheckCollistion(f))
                        {                     
                            if(e is Wall)
                            {
                                Console.WriteLine($"Destroy block: {e.PosX} {e.PosY};  fire: {f.PosX}  {f.PosY}");
                                e.Destroyed = true;
                            }
                            if(e is Player)
                            {
                                Console.WriteLine($"Killed player {e.Id}  {(e as Player).Name}");
                                (e as Player).Dead = true;
                                Game.hubGameService.hubContext.Clients.All.SendAsync("KillPlayer", e.Id);
                            }

                            stop = true;
                        }
                    firs.Add(f);
                    if (stop)
                    {
                        return firs;
                    }
                }
            
            return firs;
        }
    }
}

public class Fire : IEntity
{
    public bool Collision { get; set; }
    public int PosX { get; set; }
    public int PosY { get; set; }
    public int Width { get; set; } = 10;
    public int Height { get; set; } = 10;
    public string Id { get; init; } = "";
    public bool Destroyed { get; set; }
    public bool Destructible { get; set; } = false;

    public bool CheckCollistion(IEntity entity) => false;
}