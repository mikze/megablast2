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
        var player = Game.Players.FirstOrDefault(p => p.Id == Context.ConnectionId);
        if (player != null)
        {
            await Clients.All.SendAsync("ReceiveMessage", Context.ConnectionId, message);
        }
    }
    public async Task RestartGame()
    {
        await Game.RestartGame();
    }
    
    public void MovePlayer(int moveDirection)
    {
        var player = Game.Players.FirstOrDefault(p => p.Id == Context.ConnectionId);
        if (player is null)
            Console.WriteLine("Nie znaleziono gracza....");

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

    public async void GetMap()
    {
        await Clients.Caller.SendAsync("GetMap", Game.GenerateMap());
    }

    public void BackToLobby()
    {
        var player = Game.Players.FirstOrDefault(p => p.Id == Context.ConnectionId);
        if (player != null)
        {
            Game.Live = false;
            Clients.All.SendAsync("BackToLobby");
        }
    }

    public void Start()
    {
        Game.Live = true;
        Clients.All.SendAsync("Start");
    }

    public override async Task OnConnectedAsync()
    {
        var players = Game.Players.Where(p => p.Live);

        if (players.Count() > 4)
             await base.OnConnectedAsync();

        if (players.Count() == 0)
            Game.AddPlayer(new Player() { Id = Context.ConnectionId, Name = "mikze", PosX = 101, PosY = 100 });
        else if (players.Count() == 1)
            Game.AddPlayer(new Player() { Id = Context.ConnectionId, Name = "mikze", PosX = 99 + 14 * 50, PosY = 100 });
        else if (players.Count() == 2)
            Game.AddPlayer(new Player() { Id = Context.ConnectionId, Name = "mikze", PosX = 99 + 14 * 50, PosY = 99 + 13 * 50 });
        else if (players.Count() == 3)
            Game.AddPlayer(new Player() { Id = Context.ConnectionId, Name = "mikze", PosX = 101, PosY = 99 + 13 * 50 });

        await Clients.All.SendAsync("Connected", Game.Players.Where(p => p.Live).ToArray(), Context.ConnectionId);
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Game.RemovePlayer(Context.ConnectionId);
        Clients.All.SendAsync("Disconnected", Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}