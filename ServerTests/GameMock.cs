using Game.Game.Entities;
using GameEngine.Interface;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using Server.Services;

namespace ServerTests;

public class GameMock : IWorld
{
    public List<IEntity> Entities = [];
    public void CreateBonus(IEntity e)
    {
        //Entities.Add(new BonusMock() { PosX = e.PosX, PosY = e.PosY });
    }

    public void Destroy()
    {
        // Live = false;
        // Entities.Clear();
        // hubGameService = null;
    }

    public void RemoveDestroyedEntities()
    {
        //Entities.RemoveAll(e => e.Destroyed);
    }

    public void RemoveEntities()
    {
        //ntities.Clear();
    }

    public async Task RestartGame()
    {
        Destroy();
        Live = true;
        await Task.CompletedTask;
    }

    public Task SendToAll(string methodName, object? args)
    {
        return Task.CompletedTask;
    }

    public Task SendToAll(string methodName, object? args, object? args2)
    {
        return Task.CompletedTask;
    }

    public Task SendToAll(string methodName, object? args, object? args2, object? args3)
    {
        return Task.CompletedTask;
    }

    public Task SendToPlayer(string methodName, string playerId, object? args)
    {
        return Task.CompletedTask;
    }



    public IReadOnlyList<IEntity> GetEntities()
    {
        return Entities.AsReadOnly();
    }

    public IReadOnlyList<IEntity> GetEntities<T>()
    {
        throw new NotImplementedException();
    }

    public void AddEntity(IEntity entity)
    {
        throw new NotImplementedException();
    }

    public void AddEntities(IEnumerable<IEntity> entities)
    {
        throw new NotImplementedException();
    }

    public void RemoveEntity(IEntity entity)
    {
        throw new NotImplementedException();
    }

    public void RemoveEntities(IEnumerable<IEntity> entities)
    {
        throw new NotImplementedException();
    }

    public void RemoveEntities<T>() where T : IEntity
    {
        throw new NotImplementedException();
    }

    public double MonsterSpeed { get; }
    public bool Live { get; set; }
    public bool Pause { get; set; }
    public string GroupName { get; set; }
    public int Id { get; set; }

    public void Update(TimeSpan delta)
    {
        throw new NotImplementedException();
    }

    public Bomb? PlantBomb(double posX, double posY, Player player)
    {
        var bomb = new Bomb(1, 1, null, this);
        Entities.Add(bomb);
        return bomb;
    }

    public Player[] GetPlayers()
    {
        return Entities.OfType<Player>().ToArray();
    }

    public Wall[] GetMap()
    {
        return Entities.OfType<Wall>().ToArray();
    }

    public Wall[] GetAllMap()
    {
        return Entities.OfType<Wall>().ToArray();
    }
}