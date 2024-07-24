using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    public async Task SendMessage(string user, string message)
        => await Clients.All.SendAsync("ReceiveMessage", user, message);

     public async Task MoveText()
        => await Clients.All.SendAsync("MoveText", 10);

    public override Task OnConnectedAsync()
    {
        Clients.All.SendAsync("Connected","blabla");
        return base.OnConnectedAsync();
    }
}