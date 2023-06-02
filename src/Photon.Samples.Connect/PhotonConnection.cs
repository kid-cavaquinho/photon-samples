using ExitGames.Client.Photon;
using Photon.Realtime;

namespace Photon.Samples.Connect;

public sealed class PhotonConnection : IConnectionCallbacks
{
    private readonly LoadBalancingClient _client;
    private bool _quit;

    public PhotonConnection()
    {
        _client = new LoadBalancingClient();
        _client.AddCallbackTarget(this);
        _client.StateChanged += OnStateChange;
        _client.LoadBalancingPeer.DebugOut = DebugLevel.ALL;
    }

    ~PhotonConnection()
    {
        _client.Disconnect();
        _client.RemoveCallbackTarget(this);
    }

    public void Start()
    {
        _client.ConnectUsingSettings(new AppSettings
        {
            AppIdRealtime = "replace-me",
            FixedRegion = "replace-me"
        });

        Task.Run(Loop);
        Console.WriteLine("Running until key pressed.");
        Console.ReadKey();
        _quit = true;
        Disconnect();
    }

    public void OnConnected()
    {
    }

    public void OnConnectedToMaster()
    {
        Console.WriteLine($"OnConnectedToMaster Server: {_client.LoadBalancingPeer.ServerIpAddress}");
    }

    public void OnDisconnected(DisconnectCause cause)
    {
        Console.WriteLine($"Client is disconnected with cause: {cause}");
    }

    public void OnRegionListReceived(RegionHandler regionHandler)
    {
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {
    }

    /// <summary>
    /// This method allows to fine control when to handle incoming messages and how often send messages.
    /// Internally, the messages are buffered until this method is called.
    /// </summary>
    private void Loop()
    {
        while (!_quit)
        {
            // Received events and data gets executed. This is done when you can handle the updates.
            // As the sequence order is usually kept intact, everything that the client receives is queued and ordered.
            // Service internally calls DispatchIncomingCommands to hand over available messages one by one.
            // Outgoing data of your client is sent to the server. This includes acknowledgments (created in the background) which are important to keep the connection to the server.
            // Service internally calls SendOutgoingCommands to do this task. Controlling the frequency of SendOutgoingCommands calls controls the number of packages you use to send a client's produced data.
            _client.Service();
        }
    }

    private static void OnStateChange(ClientState arg1, ClientState arg2)
    {
        Console.WriteLine($"{arg1} -> {arg2}");
    }

    private void Disconnect()
    {
        if (_client.IsConnected)
        {
            _client.Disconnect();
        }
    }
}