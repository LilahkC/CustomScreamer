using CustomScreamer.Renderer;
using TwitchLib.Api;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using OnConnectedEventArgs = TwitchLib.Client.Events.OnConnectedEventArgs;

namespace CustomScreamer.Utils;

public static class Twitch
{
    private static TwitchAPI twitch;
    private static TwitchClient client;
    private static bool connected;
    
    private const string clientId = "gp762nuuoqcoxypju8c569th9wz7q5";
    private const string secret = "ywvz5vov81jt3di0l0lulmu4ri1z4o";
    private const string accessToken = "ccwsxon63y3ghshyr4hc79vsh0ai8i";
    
    public static string ChannelName { get; set; } = "";

    public static void Initialize()
    {
        connected = true;
        
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
        ImGUIInterface.ShowConfirmation(ImGUIInterface.Section.Twitch, "Connecting...", Color.Orange, 10);
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
        ImGUIInterface.ShowConfirmation(ImGUIInterface.Section.Twitch, $"Connected ! The chat will be able to use !{Folder.GetSubDirectoriesText()} to play a screamer", Color.Green, 10);
        return Task.CompletedTask;
    }
    
    private static Task OnTwitchConnectionError(object sender, OnConnectionErrorArgs args)
    {
        connected = false;
        
        ImGUIInterface.ShowConfirmation(ImGUIInterface.Section.Twitch, $"Error connecting : {args.Error.Message}", Color.Red, 10);
        return Task.CompletedTask;
    }
    
    private static Task OnTwitchDisconnected(object sender, OnDisconnectedArgs args)
    {
        connected = false;
        return Task.CompletedTask;
    }
    
    public static void Disconnect()
    {
        client?.DisconnectAsync();
    }

    public static bool IsConnected()
    {
        return connected;
    }
    
    public static void Quit()
    {
        Disconnect();

        twitch = null;
        client = null;
    }
}
