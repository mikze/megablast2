using Microsoft.AspNetCore.SignalR;

namespace Server.Game.Entities;

public class Bomb : EntityBase
{
    private static readonly object Locker = new();
    public bool Touched { get; set; } = true;
    public Player Owner { get; }

    public Bomb(double x, double y, Player owner)
    {
        Id = Guid.NewGuid().ToString();
        Owner = owner;
        PosX = x;
        PosY = y;
        Destructible = true;
        Height = Width = 36;
        Collision = true;

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

    private async Task SendToClients(List<Fire> fires)
    {
        await Game.HubGameService?.HubContext.Clients.All.SendAsync("Fires", fires.ToArray())!;
        await Game.HubGameService.HubContext.Clients.All.SendAsync("BombExplode", new BombModel(this));
    }

    private static async Task<List<Fire>> Calc(List<Fire> fires)
    {
        var fireList = new List<Fire>();

        foreach (var f in fires)
        {
            var stop = false;
            var coll = Game.GetEntities().Where(y => y.Destructible).ToArray();
            foreach (var e in coll)
                if (e is { Destroyed: false } && e.CheckCollision(f))
                {
                    if (e is Bomb bomb)
                    {
                        Console.WriteLine($"Destroy BOMB: {e.PosX} {e.PosY};  fire: {f.PosX}  {f.PosY}");
                        await bomb.Explode();
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
                    if (e is Player player)
                    {
                        Console.WriteLine($"Hit player {e.Id}  {player.Name} with {player.Lives} lives");
                        player.Lives--;
                        if (player.Lives <= 0)
                        {
                            Console.WriteLine($"Killed player {e.Id}  {player.Name}");
                            player.Dead = true;
                            await Game.HubGameService?.HubContext.Clients.All.SendAsync("KillPlayer", e.Id)!;
                        }
                    }

                    stop = true;
                }
            fireList.Add(f);
            if (stop)
            {
                return fireList;
            }
        }

        return fireList;
    }

    private async Task Explode()
    {
        if (!Destroyed)
        {
            Destroyed = true;
            List<Fire> fires1 = new List<Fire>();
            List<Fire> fires2 = new List<Fire>();
            List<Fire> fires3 = new List<Fire>();
            List<Fire> fires4 = new List<Fire>();

            lock (Locker)
            {

                const int size = 4;

                for (var i = 1; i <= size; i++)
                    fires1.Add(new Fire() { PosX = PosX, PosY = PosY + (i * 32) });

                for (var i = 1; i <= size; i++)
                    fires2.Add(new Fire() { PosX = PosX + (i * 32), PosY = PosY });

                for (var i = -1; i >= -size; i--)
                    fires3.Add(new Fire() { PosX = PosX, PosY = PosY + (i * 32) });

                for (var i = -1; i >= -size; i--)
                    fires4.Add(new Fire() { PosX = PosX + (i * 32), PosY = PosY });
            }

            var f = await Calc(fires1);
            f.AddRange(await Calc(fires2));
            f.AddRange(await Calc(fires3));
            f.AddRange(await Calc(fires4));
            await SendToClients(f);
        }
    }
}

public class Fire : EntityBase
{
    public Fire()
    {
        Height = Width = 25;
    }
}