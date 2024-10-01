using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
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
            await Game.RestartGame();
        }
    }
    
    public void MovePlayer(int moveDirection)
    {
        var player = Game.Players.FirstOrDefault(p => p.Id == Context.ConnectionId);
        if (player != null && !player.Moved)
            player.MoveDirection = (MoveDirection)moveDirection;
    }

    public void PlantBomb()
    {
        var player = Game.Players.FirstOrDefault(p => p.Id == Context.ConnectionId);
        if (player != null)
        {
            Clients.All.SendAsync("BombPlanted", Context.ConnectionId);
            player.PlantBomb();
        }
    }

    public void ChangeName(string newName)
    {
        var player = Game.Players.FirstOrDefault(p => p.Id == Context.ConnectionId);
        if (player != null)
        {
            Game.ChangeName(Context.ConnectionId, newName);
            Clients.All.SendAsync("NameChanged", Context.ConnectionId, newName);
        }
    }

        public void ChangeSkin(string newSkinName)
    {
        var player = Game.Players.FirstOrDefault(p => p.Id == Context.ConnectionId);
        if (player != null)
        {
            Game.ChangeSkin(Context.ConnectionId, newSkinName);
            Clients.All.SendAsync("SkinChanged", Context.ConnectionId, newSkinName);
        }
    }

    public async void GetMap()
    {
        await Clients.Caller.SendAsync("GetMap", Game.GenerateMap());
    }

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
            await Clients.All.SendAsync("Start");
        }
    }

    public override async Task OnConnectedAsync()
    {
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
        Game.RemovePlayer(Context.ConnectionId);
        Clients.All.SendAsync("Disconnected", Game.Players.Where(p => p.Live).ToArray(), Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}