using Game.Game.Models;
using GameEngine.Core;
using GameEngine.Interface;

namespace Game.Game.Entities;

public class Bomb : EntityBase
{
    public bool Touched { get; set; } = true;
    public Player Owner { get; }
    private int FireSize => Owner.GetFireSize();
    private ICommunicateHandler? CommunicateHandler => Game as ICommunicateHandler;
    private Game? AsGame => Game as Game;

    public Bomb(double x, double y, Player owner, IWorld game) : base(game)
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
            await Task.Delay(Owner.BombDelay);
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
        await CommunicateHandler?.SendToAll("Fires",Game.GroupName, fires.ToArray())!;
        await CommunicateHandler.SendToAll("BombExplode",Game.GroupName, new BombModel(this));
        //await Game.GetHubGameService()?.HubContext.Clients.All.SendAsync("Fires", fires.ToArray())!;
        //await Game.GetHubGameService()?.HubContext.Clients.All.SendAsync("BombExplode", new BombModel(this))!;
    }

    private async Task<List<Fire>> CalcAsync(List<Fire> fires)
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
    
    private async Task HandleEntityDestructionAsync(IEntity entity, Fire fire)
    {
        switch (entity)
        {
            case BasicMonster monster:
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
                AsGame?.CreateBonus(entity);
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
            
                var fires1 = CreateFireList(PosX, PosY, 1, 0);
                var fires2 = CreateFireList(PosX, PosY, 0, 1);
                var fires3 = CreateFireList(PosX, PosY, -1, 0);
                var fires4 = CreateFireList(PosX, PosY, 0, -1);

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
            fires.Add(new Fire(Game) 
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
    public Fire(IWorld game) : base(game)
    {
        Height = Width = 25;
    }
}