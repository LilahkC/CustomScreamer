using CustomScreamer.Renderer;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;
using OnConnectedEventArgs = TwitchLib.Client.Events.OnConnectedEventArgs;

namespace CustomScreamer.Utils;

public static class Twitch
{
    private static TwitchClient client;
    private static ConnectionCredentials credentials;
    
    public static string ChannelName { get; set; }
    public static string TwitchOAuth { get; set; }
    
    public static void Initialize()
    {
        client = new();
        credentials = new(ChannelName, TwitchOAuth);
        client.OnMessageReceived += OnClientOnOnMessageReceived;
        client.OnConnected += OnTwitchConnectionSuccess;
        client.OnConnectionError += OnTwitchConnectionError;
        client.Initialize(credentials, ChannelName);
        client.ConnectAsync();
        ImGUIInterface.ShowTwitchConfirmation("Connecting...", ImGUIInterface.Orange);
    }
    
    private static Task OnClientOnOnMessageReceived(object sender, OnMessageReceivedArgs args)
    {
        if (args.ChatMessage.Message.Contains($"!{Folder.GetSubRepositories()}"))
            Game.Game.PlayScreamer(true);
        return Task.CompletedTask;
    }
    
    private static Task OnTwitchConnectionSuccess(object sender, OnConnectedEventArgs args)
    {
        ImGUIInterface.ShowTwitchConfirmation("Connected", ImGUIInterface.Green);
        return Task.CompletedTask;
    }
    
    private static Task OnTwitchConnectionError(object sender, OnConnectionErrorArgs args)
    {
        ImGUIInterface.ShowTwitchConfirmation($"Error connecting : {args.Error}", ImGUIInterface.Red);
        return Task.CompletedTask;
    }
    
    public static void Quit()
    {
    }
}
