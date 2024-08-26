using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    readonly IHubContext<ChatHub> _hubContext;
    public ChatHub(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendMessage(string user, string message)
        => await Clients.All.SendAsync("ReceiveMessage", user, message);

    public void MovePlayer(int moveDirection)
    {      
        var player = Game.Players.First(p => p.Id == Context.ConnectionId);
        if(!player.Moved)
            player.MoveDirection = (MoveDirection)moveDirection;
    }

    public void ChangeName(string newName)
    {
        Game.ChangeName(Context.ConnectionId, newName);
        Clients.All.SendAsync("NameChanged", Context.ConnectionId, newName);
    }

    public override Task OnConnectedAsync()
    {
        Game.AddPlayer(new Player() { Id = Context.ConnectionId, Name = "mikze", PosX = 200, PosY = 200 });

        Clients.All.SendAsync("Connected", Game.Players.ToArray(), Context.ConnectionId);
        Game.GenerateMap();
        Clients.Caller.SendAsync("GetMap", Game.Map);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Game.RemovePlayer(Context.ConnectionId);
        Clients.All.SendAsync("Disconnected", Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}