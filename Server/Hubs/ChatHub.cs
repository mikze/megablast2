using Microsoft.AspNetCore.SignalR;
using Server.Game;

public class ChatHub : Hub
{
    private static readonly List<string> clients = [];
    public async Task SendMessage(string user, string message)
    {
        var player = Game.Players.FirstOrDefault(p => p.Id == Context.ConnectionId);
        if (player != null)
        {
            await Clients.All.SendAsync("ReceiveMessage", Context.ConnectionId, message);
        }
    }
    public async Task RestartGame()
    {
        if (Game.IsMasterPlayer(Context.ConnectionId))
        {
            foreach(var c in clients)
                Console.WriteLine($"RestartGame to client {c}");

            await Game.RestartGame();
        }
    }
    
    public void MovePlayer(int moveDirection)
    {
        var player = Game.Players.FirstOrDefault(p => p.Id == Context.ConnectionId);
        if (player is { Moved: false })
            player.MoveDirection = (MoveDirection)moveDirection;
    }

    public void PlantBomb()
    {
        var player = Game.Players.FirstOrDefault(p => p.Id == Context.ConnectionId);
        var bomb = player?.PlantBomb();
        if (bomb != null)
            Clients.All.SendAsync("BombPlanted", new BombModel(bomb));
    }

    public void GetMonsters()
    {
        Console.WriteLine("Getting Monsters");
        var monsters = Game.GetMonsters();
        Clients.Caller.SendAsync("GetMonsters", monsters);
    }
    public void ChangeName(string newName)
    {
        var player = Game.Players.FirstOrDefault(p => p.Id == Context.ConnectionId);
        if (player is null) return;
        
        Game.ChangeName(Context.ConnectionId, newName);
        Clients.All.SendAsync("NameChanged", Context.ConnectionId, newName);
    }

        public void ChangeSkin(string newSkinName)
    {
        var player = Game.Players.FirstOrDefault(p => p.Id == Context.ConnectionId);
        if (player is null) return;
        
        Game.ChangeSkin(Context.ConnectionId, newSkinName);
        Clients.All.SendAsync("SkinChanged", Context.ConnectionId, newSkinName);
    }

    public async void GetMap() => await Clients.Caller.SendAsync("GetMap", Game.GetMap());
    
    public void BackToLobby()
    {
        if (Game.IsMasterPlayer(Context.ConnectionId))
        {
            var player = Game.Players.FirstOrDefault(p => p.Id == Context.ConnectionId);
            if (player != null)
            {
                Game.Live = false;
                Clients.All.SendAsync("BackToLobby");
            }
        }
    }

    public async Task Start()
    {
        if (Game.IsMasterPlayer(Context.ConnectionId))
        {
            Game.Live = true;
            await Game.RestartGame();
            foreach(var c in clients)
                Console.WriteLine($"START to client {c}");

            await Clients.All.SendAsync("Start");
        }
    }

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"Connected {Context.ConnectionId}");
        clients.Add(Context.ConnectionId);
        var players = Game.Players.Where(p => p.Live);

        if (players.Count() >= 4)
        {
            await base.OnConnectedAsync();
            await Clients.Caller.SendAsync("ServerIsFull");
            await Clients.Caller.SendAsync("Connected", Game.Players.Where(p => p.Live).ToArray(), Context.ConnectionId);
        }
        else
        {
            await base.OnConnectedAsync();
            Game.AddPlayer(Context.ConnectionId);
            await Clients.All.SendAsync("Connected", Game.Players.Where(p => p.Live).ToArray(), Context.ConnectionId);
        }
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"Disconnected {Context.ConnectionId}");
        clients.Remove(Context.ConnectionId);
        Game.RemovePlayer(Context.ConnectionId);
        Clients.All.SendAsync("Disconnected", Game.Players.Where(p => p.Live).ToArray(), Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}