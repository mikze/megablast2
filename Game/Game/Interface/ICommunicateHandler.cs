namespace Game.Game.Interface;

public interface ICommunicateHandler
{
    public Task SendToAll(string methodName, string groupName, object? args);
    public Task SendToAll(string methodName, string groupName, object? args, object? args2);
    public Task SendToAll(string methodName, string groupName, object? args, object? args2, object? args3);
    public Task SendToPlayer(string methodName, string playerId, object? args);
}