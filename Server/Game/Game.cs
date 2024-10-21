using Microsoft.AspNetCore.SignalR;

public enum MoveDirection
{
    Right,
    Left,
    Up,
    Down,
    none
}
public static class Game
{
    public static bool Live { get; set; } = false;
    public static List<IEntity> Entities = new List<IEntity>();
    public static HubGameService hubGameService;
    public static object LockEntities;
    public static List<IEntity> GetEntities()
    {
        if (LockEntities is null)
            LockEntities = new object();

        lock (LockEntities)
        {
            return Entities;
        }
    }
    public static void AddEntities(IEntity entity)
    {
        if (LockEntities is null)
            LockEntities = new object();

        lock (LockEntities)
        {
            Entities.Add(entity);
        }
    }
    public static Player[] Players { get; set; } =
    {
        new Player(){ Id = string.Empty, Live = false},
        new Player(){ Id = string.Empty, Live = false},
        new Player(){ Id = string.Empty, Live = false},
        new Player(){ Id = string.Empty, Live = false}
    };

    public static Player? MasterPlayer => Players.Any() ? Players[0] : null;
    public static bool IsMasterPlayer(string id) => MasterPlayer is null ? false : MasterPlayer.Id == id;
    public static Wall[] GenerateMap()
    {
        GetEntities().AddRange(MapHandler.GenerateMap());
        return GetMap();
    }

    public static Wall[] GetMap() => GetEntities().Where(e => e is Wall).Cast<Wall>().ToArray();

    public static void AddPlayer(string id)
    {
        if (GetFreePlayerSlotNumber(out int idPlayer))
        {
            Player? newPlayer = null;

            switch (idPlayer)
            {
                case 0:
                    newPlayer = new Player() { Id = id, Name = "mikze", PosX = 101, PosY = 100 };
                    break;
                case 1:
                    newPlayer = new Player() { Id = id, Name = "mikze", PosX = 99 + 14 * 50, PosY = 100 };
                    break;
                case 2:
                    newPlayer = new Player() { Id = id, Name = "mikze", PosX = 99 + 14 * 50, PosY = 99 + 13 * 50 };
                    break;
                case 3:
                    newPlayer = new Player() { Id = id, Name = "mikze", PosX = 101, PosY = 99 + 13 * 50 };
                    break;
            }

            if (newPlayer != null)
            {
                Players[idPlayer] = newPlayer;
                GetEntities().Add(newPlayer);
            }
        }
    }

    public static bool GetFreePlayerSlotNumber(out int number)
    {
        for (int i = 0; i < 4; i++)
        {
            if (Players[i].Live == false)
            {
                number = i;
                return true;
            }
        }
        number = -1;
        return false;
    }


    public static void RemovePlayer(string Id)
    {
        var player = Players.FirstOrDefault(p => p.Id == Id);
        if (player != null)
        {
            player.Live = false;
            lock (LockEntities)
            {
                Entities.Remove(player);
            }
        }
    }

    public static void MovePlayer(string id, MoveDirection moveDirection)
    {
        Thread.Sleep(5);
        var player = Players.FirstOrDefault(p => p.Id == id);
        if (player != null)
        {
            player.MovePlayer(moveDirection);
        }
    }

    public static void ChangeName(string id, string newName)
    {
        var player = Players.FirstOrDefault(p => p.Id == id);
        if (player != null)
            player.Name = newName;
    }

    public static void ChangeSkin(string id, string newSkinName)
    {
        var player = Players.FirstOrDefault(p => p.Id == id);
        if (player != null)
            player.Skin = newSkinName;
    }

    public static Bomb PlantBomb(double x, double y, Player owner)
    {
        var bomb = new Bomb(x, y, owner);
        AddEntities(bomb);
        return bomb;
    }

    public static async Task RestartGame()
    {
        GetEntities().RemoveAll(e => e is Wall);

        var newMap = GenerateMap();
        var players = Players.Where(p => p.Live).ToArray();

        for (int i = 0; i < players.Length; i++)
        {
            if (i == 0)
            {
                players[0].PosX = 101;
                players[0].PosY = 100;
                players[0].Dead = false;

            }
            else if (i == 1)
            {
                players[1].PosX = 99 + 14 * 50;
                players[1].PosY = 100;
                players[1].Dead = false;
            }
            else if (i == 2)
            {
                players[2].PosX = 99 + 14 * 50;
                players[2].PosY = 99 + 13 * 50;
                players[2].Dead = false;
            }
            else if (i == 3)
            {
                players[3].PosX = 101;
                players[3].PosY = 99 + 13 * 50;
                players[3].Dead = false;
            }
        }
        await hubGameService.hubContext.Clients.All.SendAsync("Connected", players);
        await hubGameService.hubContext.Clients.All.SendAsync("GetMap", newMap);
    }

    internal static void CreateBonus(IEntity e)
    {
        Random rnd = new Random();
        int result = rnd.Next(1, 4);
        if (result == 1)
        {
            var bonus = new Bonus() { PosX = e.PosX, PosY = e.PosY, Destructible = true };
            GetEntities().Add(bonus);
            hubGameService.hubContext.Clients.All.SendAsync("SetBonus", bonus);
        }
    }
}