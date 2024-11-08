using Microsoft.AspNetCore.SignalR;
using Server.Game.Interface;

namespace Server.Game.Entities;

public class Bomb : EntityBase
{
    public bool Touched { get; set; } = true;
    public Player Owner { get; }
    private int FireSize => Owner.GetFireSize();

    public Bomb(double x, double y, Player owner)
    {
        Id = Guid.NewGuid().ToString();
        Owner = owner;
        PosX = x;
        PosY = y;
        Destructible = true;
        Height = Width = 36;
        Collision = true;
        PlantBombAsync();

    }
    
    private async void PlantBombAsync()
    {
        try
        {
            Console.WriteLine($"PLANT BOMB {Id}");
            await Task.Delay(2000);  // Initial delay for bomb planting
            Console.WriteLine($"Explode BOMB {Id}");
            await ExplodeAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Could not plant bomb, {e.Message}");
        }
    }

    private async Task SendToClients(List<Fire> fires)
    {
         await Game.GetHubGameService()?.HubContext.Clients.All.SendAsync("Fires", fires.ToArray())!;
         await Game.GetHubGameService()?.HubContext.Clients.All.SendAsync("BombExplode", new BombModel(this))!;
    }

    private static async Task<List<Fire>> CalcAsync(List<Fire> fires)
    {
        var fireList = new List<Fire>();
        var entities = Game.GetEntities().Where(y => y.Destructible).ToArray();

        foreach (var fire in fires)
        {
            var stop = false;

            foreach (var entity in entities)
            {
                if (entity is not { Destroyed: false } || !entity.CheckCollision(fire)) continue;
                await HandleEntityDestructionAsync(entity, fire);
                stop = true;
            }

            fireList.Add(fire);
            if (stop)
            {
                break;
            }
        }

        return fireList;
    }
    
    private static async Task HandleEntityDestructionAsync(IEntity entity, Fire fire)
    {
        switch (entity)
        {
            case Monster monster:
                Console.WriteLine($"Destroy monster: {entity.PosX} {entity.PosY}; fire: {fire.PosX} {fire.PosY}");
                monster.Destroyed = true;
                break;
            case Bomb bomb:
                Console.WriteLine($"Destroy BOMB: {entity.PosX} {entity.PosY}; fire: {fire.PosX} {fire.PosY}");
                await bomb.ExplodeAsync();
                break;
            case Wall:
                Console.WriteLine($"Destroy block: {entity.PosX} {entity.PosY}; fire: {fire.PosX} {fire.PosY}");
                entity.Destroyed = true;
                Game.CreateBonus(entity);
                break;
            case Bonus:
                Console.WriteLine($"Destroy bonus: {entity.PosX} {entity.PosY}; fire: {fire.PosX} {fire.PosY}");
                entity.Destroyed = true;
                break;
            case Player player:
                Console.WriteLine($"Hit player {entity.Id} {player.Name} with {player.LifeAmount()} lives");
                player.TakeLife();
                break;
        }
    }

    private async Task ExplodeAsync()
    {
        if (!Destroyed)
        {
            Destroyed = true;
            
                List<Fire> fires1 = CreateFireList(PosX, PosY, 1, 0);
                List<Fire> fires2 = CreateFireList(PosX, PosY, 0, 1);
                List<Fire> fires3 = CreateFireList(PosX, PosY, -1, 0);
                List<Fire> fires4 = CreateFireList(PosX, PosY, 0, -1);

                var fireList = await CalcAsync(fires1);
                fireList.AddRange(await CalcAsync(fires2));
                fireList.AddRange(await CalcAsync(fires3));
                fireList.AddRange(await CalcAsync(fires4));

                await SendToClients(fireList);
        }
    }
    
    private List<Fire> CreateFireList(double x, double y, int xMultiplier, int yMultiplier)
    {
        var fires = new List<Fire>();

        for (var i = 1; i <= FireSize; i++)
        {
            fires.Add(new Fire() 
            { 
                PosX = x + (i * 32 * xMultiplier), 
                PosY = y + (i * 32 * yMultiplier)
            });
        }

        return fires;
    }
}

public class Fire : EntityBase
{
    public Fire()
    {
        Height = Width = 25;
    }
}