
using Microsoft.AspNetCore.SignalR;
using Server.Game.Interface;

namespace Server.Game.Entities;

public class Player : EntityBase
{
    public bool Dead { get; set; }
    public double Speed { get; set; } = 1;
    public string? Name { get; set; }
    public bool Moved { get; set; }
    public MoveDirection MoveDirection { get; set; }
    public bool Live { get; internal set; } = true;
    public int MaxBombs { get; set; } = 1;
    private int Lives { get; set; } = 1;
    public string Skin { get; set; } = "playerSprite";
    public int BombDelay { get; set; }

    private int _fireSize = 3;
    public int MaxBullets { get; set; } = 2;

    public Player(Game game) : base(game)
    {
        BombDelay = game.BombDelay;
        Destructible = true;
        Width = 36;
        Height = 43;
        Collision = true;
    }
    public override bool CheckCollision(IEntity entity)
    {
        var coll = base.CheckCollision(entity);
        
        if(coll && entity is Bonus)
            entity.Destroyed = true;
        
        // if(coll && entity is Bullet && Bullet)
        //     entity.Destroyed = true;
        
        return coll;
    }

    public void Move(MoveDirection moveDirection)
    {
        if (Dead || !Game.Live) return;
        var oldPosX = PosX;
        var oldPosY = PosY;

        switch (moveDirection)
        {
            case MoveDirection.Right:
                PosX += Speed;
                break;
            case MoveDirection.Left:
                PosX -= Speed;
                break;
            case MoveDirection.Up:
                PosY -= Speed;
                break;
            case MoveDirection.Down:
                PosY += Speed;
                break;
        }

        foreach (var entity in Game.GetEntities().Where(e => e != this && e.Collision))
        {
            if (entity.CheckCollision(this))
            {
                if (entity is Bullet bullet && bullet.Owner == this)
                        break;
                
                if (entity is Bomb bomb && bomb.Owner == this)
                    if(bomb.Touched)
                        break;

                PosX = oldPosX;
                PosY = oldPosY;
                break;
            }

            if (entity is not Bomb bomb1 || bomb1.Owner != this) continue;
            bomb1.Touched = false;
        }
    }

    internal Bomb? PlantBomb()
    {
        if (Game.GetEntities().Where(e => e is Bomb).Count(e => (e as Bomb)?.Owner == this && !e.Destroyed) >=
            MaxBombs) return null;
        return !Dead ? Game.PlantBomb(PosX + 16, PosY + 16, this) : null;
    }

    public void TakeLife(int amount = 1)
    {
        Lives -= amount;
        _ = Game.SendToPlayer("GetStats", Id, GetStats());
        if (LifeAmount() > 0) return;
        
        Console.WriteLine($"Killed player {Id}  {Name}");
        Dead = true;
        Game.GetHubGameService()?.HubContext.Clients.All.SendAsync("KillPlayer", Id);
        _ = Game.SendToAll("ReceiveMessage","","GAME", $"{Name} is killed"); //await Clients.Group(gameManager.GetGameName(game)).SendAsync("ReceiveMessage",Context.ConnectionId, player.Name, message);
        if (Game.GetPlayers().Count(p => p is { Dead: false, Live: true })  > 1) return;
        
        Game.Live = false;
        _ = Game.SendToAll("BackToLobby", null);
    }
    
    public void AddLife(int amount = 1)
    {
        Lives += amount;
    }

    public int LifeAmount() => Lives;
    
    public int GetFireSize() => _fireSize;
    public void IncreaseFireSize() => _fireSize += 1;
    public void DecreaseFireSize() => _fireSize -= 1;

    public PlayerStats GetStats()
    {
        return new PlayerStats()
        {
            Lives = Lives,
            Bombs = MaxBombs,
            Speed = Speed,
            Range = GetFireSize()
        };
    }
}