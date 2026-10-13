using CustomScreamer.Renderer;
using TwitchLib.Api;
using TwitchLib.Api.Core;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using OnConnectedEventArgs = TwitchLib.Client.Events.OnConnectedEventArgs;

namespace CustomScreamer.Utils;

public static class Twitch
{
    private static TwitchAPI twitch;
    private static TwitchClient client;
    
    private const string clientId = "gp762nuuoqcoxypju8c569th9wz7q5";
    private const string secret = "ywvz5vov81jt3di0l0lulmu4ri1z4o";
    private const string accessToken = "ccwsxon63y3ghshyr4hc79vsh0ai8i";
    
    public static string ChannelName { get; set; } = "LilahkC";

    public static void Initialize()
    {
        twitch = new()
        {
            Settings =
            {
                ClientId = clientId,
                Secret = secret,
                AccessToken = accessToken
            }
        };

        string authToken = twitch.Auth.GetAccessTokenAsync().Preserve().GetAwaiter().GetResult();
    
        client = new();
        client.Initialize(new("CustomScreamer", authToken), ChannelName);
        client.OnMessageReceived += OnClientOnOnMessageReceived;
        client.OnConnected += OnTwitchConnectionSuccess;
        client.OnConnectionError += OnTwitchConnectionError;
        client.OnDisconnected += OnTwitchDisconnected;
        client.ConnectAsync();
        ImGUIInterface.ShowTwitchConfirmation("Connecting...", ImGUIInterface.Orange);
    }
    
    private static Task OnClientOnOnMessageReceived(object sender, OnMessageReceivedArgs args)
    {
        string[] directoriesToCheck = Folder.GetSubDirectories();

        foreach (string directoryString in directoriesToCheck)
        {
            Console.WriteLine($"Checking directory: {directoryString}");
            if (args.ChatMessage.Message.Contains($"!{directoryString}"))
                Game.Game.PlayScreamer(false);
        }
        return Task.CompletedTask;
    }
    
    private static Task OnTwitchConnectionSuccess(object sender, OnConnectedEventArgs args)
    {
        ImGUIInterface.ShowTwitchConfirmation($"Connected ! The chat will be able to use !{Folder.GetSubDirectoriesText()} to play a screamer", ImGUIInterface.Green);
        return Task.CompletedTask;
    }
    
    private static Task OnTwitchConnectionError(object sender, OnConnectionErrorArgs args)
    {
        ImGUIInterface.ShowTwitchConfirmation($"Error connecting : {args.Error.Message}", ImGUIInterface.Red);
        return Task.CompletedTask;
    }
    
    private static Task OnTwitchDisconnected(object sender, OnDisconnectedArgs args)
    {
        ImGUIInterface.ShowTwitchConfirmation($"Disconnected from channel : {ChannelName}", ImGUIInterface.Green);
        return Task.CompletedTask;
    }
    
    public static void Disconnect()
    {
        client.DisconnectAsync();
    }

    public static void Refresh()
    {
        ImGUIInterface.ShowTwitchConfirmation($"Connected ! The chat will be able to use !{Folder.GetSubDirectoriesText()} to play a screamer", ImGUIInterface.Green);
    }
    
    public static void Quit()
    {
        Disconnect();

        twitch = null;
        client = null;
    }
}
