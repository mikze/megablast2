
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.SignalR;

public class Bomb : IEntity
{
    static readonly object Locker = new object();

    public string Id { get; init; }
    public bool Collision { get; set; } = true;
    public double PosX { get; set; }
    public double PosY { get; set; }
    public int Width { get; set; } = 36;
    public int Height { get; set; } = 36;
    public bool Destroyed { get; set; }
    public bool Destructible { get; set; } = true;
    public bool Touched { get; set; } = true;
    public Player Owner { get; set; }

    public Bomb(double x, double y, Player _owner)
    {
        Id = Guid.NewGuid().ToString();
        Owner = _owner;
        PosX = x;
        PosY = y;

        new Task(async () =>
        {
            try
            {
                Console.WriteLine($"PLANT BOMB");
                await Task.Delay(2000);
                await Explode();
            }
            catch(Exception e)
            {
                Console.WriteLine($"nie udalo sie splantowac paki\n {e.Message}");
            }
        }).Start();

    }

    async Task SendToClients(List<Fire> fires)
    {
        await Game.hubGameService.hubContext.Clients.All.SendAsync("Fires", fires.ToArray());
        await Game.hubGameService.hubContext.Clients.All.SendAsync("BombExplode", new BombModel(this));
    }

    public bool CheckCollistion(IEntity entity)
    {
        if (entity.Destroyed)
            return false;

        var h1 = this;
        var h2 = entity;
        return h1 != h2 && h1.PosX < h2.PosX + h2.Width &&
         h1.PosX + h1.Width > h2.PosX &&
         h1.PosY < h2.PosY + h2.Height &&
         h1.Height + h1.PosY > h2.PosY;
    }

    async Task<List<Fire>> Calc(List<Fire> fires)
    {
        List<Fire> firs = new List<Fire>();

        foreach (var f in fires)
        {
            bool stop = false;
            var coll = Game.GetEntities().Where(a => a is Bomb || a is Bonus || a is Wall || a is Player).Where(y => y is object && y.Destructible).ToArray();
            foreach (var e in coll)
                if (e is object && e.Destructible && !e.Destroyed && e.CheckCollistion(f))
                {
                    if (e is Bomb)
                    {
                        Console.WriteLine($"Destroy BOMB: {e.PosX} {e.PosY};  fire: {f.PosX}  {f.PosY}");
                        await (e as Bomb).Explode();
                    }
                    if (e is Wall)
                    {
                        Console.WriteLine($"Destroy block: {e.PosX} {e.PosY};  fire: {f.PosX}  {f.PosY}");
                        e.Destroyed = true;
                        Game.CreateBonus(e);
                    }
                    if (e is Bonus)
                    {
                        Console.WriteLine($"Destroy bonus: {e.PosX} {e.PosY};  fire: {f.PosX}  {f.PosY}");
                        e.Destroyed = true;
                    }
                    if (e is Player)
                    {
                        (e as Player).Lives--;
                        if ((e as Player).Lives <= 0)
                        {
                            Console.WriteLine($"Killed player {e.Id}  {(e as Player).Name}");
                            (e as Player).Dead = true;
                            await Game.hubGameService.hubContext.Clients.All.SendAsync("KillPlayer", e.Id);
                        }
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

    public async Task Explode()
    {
        if (!Destroyed)
        {
            Destroyed = true;
            List<Fire> f;
            List<Fire> fires1 = new List<Fire>();
            List<Fire> fires2 = new List<Fire>();
            List<Fire> fires3 = new List<Fire>();
            List<Fire> fires4 = new List<Fire>();

            lock (Locker)
            {

                int size = 4;

                for (int i = 1; i <= size; i++)
                    fires1.Add(new Fire() { PosX = PosX, PosY = PosY + (i * 32) });

                for (int i = 1; i <= size; i++)
                    fires2.Add(new Fire() { PosX = PosX + (i * 32), PosY = PosY });

                for (int i = -1; i >= -size; i--)
                    fires3.Add(new Fire() { PosX = PosX, PosY = PosY + (i * 32) });

                for (int i = -1; i >= -size; i--)
                    fires4.Add(new Fire() { PosX = PosX + (i * 32), PosY = PosY });
            }

            f = await Calc(fires1);
            f.AddRange(await Calc(fires2));
            f.AddRange(await Calc(fires3));
            f.AddRange(await Calc(fires4));
            await SendToClients(f);
        }
    }
}

public class Fire : IEntity
{
    public bool Collision { get; set; }
    public double PosX { get; set; }
    public double PosY { get; set; }
    public int Width { get; set; } = 25;
    public int Height { get; set; } = 25;
    public string Id { get; init; } = "";
    public bool Destroyed { get; set; }
    public bool Destructible { get; set; } = false;

    public bool CheckCollistion(IEntity entity) => false;
}