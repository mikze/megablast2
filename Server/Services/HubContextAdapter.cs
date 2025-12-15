using Game.Game;
using Game.Game.Interface;
using Microsoft.AspNetCore.SignalR;
using Server.Hubs;

namespace Server.Services;

public class HubContextAdapter(IHubContext<ChatHub> hubContext) : ICommunicateHandler
{

    public async Task SendToPlayer(string methodName, string playerId, object? args)
    {
        await hubContext.Clients.Clients(playerId).SendAsync(methodName, args)!;
    }
    
    public async Task SendToAll(string methodName, string groupName, object? args)
    {
        await hubContext.Clients.Group(groupName).SendAsync(methodName, args)!; 
    }
    
    public async Task SendToAll(string methodName, string groupName, object? args, object? args2)
    {
        await hubContext.Clients.Group(groupName).SendAsync(methodName, args, args2)!; 
    }
    
    public async Task SendToAll(string methodName, string groupName, object? args, object? args2, object? args3)
    {
        await hubContext.Clients.Group(groupName).SendAsync(methodName, args, args2, args3)!; 
    }
}