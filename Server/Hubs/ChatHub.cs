using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    readonly IHubContext<ChatHub> _hubContext;
    public ChatHub(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendMessage(string user, string message)
        { 
            if(message == "restart")
            {
                Game.RestartGame();
                await Clients.All.SendAsync("GetMap", Game.GenerateMap());
            }
            else
                await Clients.All.SendAsync("ReceiveMessage", Context.ConnectionId, message);
        }

    public void MovePlayer(int moveDirection)
    {      
        var player = Game.Players.FirstOrDefault(p => p.Id == Context.ConnectionId);
        if(player is null)
            Console.WriteLine("Nie znaleziono gracza....");

        if(player != null && !player.Moved)
            player.MoveDirection = (MoveDirection)moveDirection;
    }

    public void PlantBomb()
    {      
        var player = Game.Players.First(p => p.Id == Context.ConnectionId);
        Clients.All.SendAsync("BombPlanted", Context.ConnectionId);
        player.PlantBomb();
    }

    public void ChangeName(string newName)
    {
        Game.ChangeName(Context.ConnectionId, newName);
        Clients.All.SendAsync("NameChanged", Context.ConnectionId, newName);
    }

    public override Task OnConnectedAsync()
    {
        if(Game.Players.Count() == 0)
            Game.AddPlayer(new Player() { Id = Context.ConnectionId, Name = "mikze", PosX = 101, PosY = 100 });
        else if(Game.Players.Count() == 1)
            Game.AddPlayer(new Player() { Id = Context.ConnectionId, Name = "mikze", PosX = 99 + 14 * 50, PosY = 100 });
        else if(Game.Players.Count() == 2)
            Game.AddPlayer(new Player() { Id = Context.ConnectionId, Name = "mikze", PosX = 99 + 14 * 50, PosY = 99 + 13 * 50});
        else if(Game.Players.Count() == 3)
            Game.AddPlayer(new Player() { Id = Context.ConnectionId, Name = "mikze", PosX = 101, PosY = 99 + 13 * 50});

        Clients.All.SendAsync("Connected", Game.Players.Where(p => p.Live).ToArray(), Context.ConnectionId);
        Clients.Caller.SendAsync("GetMap", Game.GenerateMap());
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Game.RemovePlayer(Context.ConnectionId);
        Clients.All.SendAsync("Disconnected", Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}