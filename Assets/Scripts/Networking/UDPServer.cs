using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

public class UDPServer : MonoBehaviour, INetEventListener
{
    public delegate void CommandDelegate(string command);
    public CommandDelegate OnCommandDelegate;

    private readonly string kCardboardRealityKey = "cardboardReality";

    private NetManager _netServer;
    private NetPeer _ourPeer;
    private NetDataWriter _dataWriter;

    // Use this for initialization
    void Start ()
    {
        _dataWriter = new NetDataWriter();
        _netServer = new NetManager(this, kCardboardRealityKey);
        _netServer.Start(5000);
        _netServer.DiscoveryEnabled = true;
        _netServer.UpdateTime = 15;
    }
	
	// Update is called once per frame
	void Update ()
    {
        _netServer.PollEvents();
    }

    void FixedUpdate()
    {
        // TODONOW
        if (_ourPeer != null)
        {
            //_serverBall.transform.Translate(1f * Time.fixedDeltaTime, 0f, 0f);
            // _dataWriter.Reset();
            //_dataWriter.Put(_serverBall.transform.position.x);
            //_ourPeer.Send(_dataWriter, SendOptions.Sequenced);
        }
    }

    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log("[SERVER] We have new peer " + peer.EndPoint);
        _ourPeer = peer;
    }

    public void OnNetworkError(NetEndPoint endPoint, int socketErrorCode)
    {
        Debug.Log("[SERVER] error " + socketErrorCode);
    }

    public void OnNetworkReceiveUnconnected(NetEndPoint remoteEndPoint, NetDataReader reader,
        UnconnectedMessageType messageType)
    {
        if (messageType == UnconnectedMessageType.DiscoveryRequest)
        {
            Debug.Log("[SERVER] Received discovery request. Send discovery response");
            _netServer.SendDiscoveryResponse(new byte[] { 1 }, remoteEndPoint);
        }
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log("[SERVER] peer disconnected " + peer.EndPoint + ", info: " + disconnectInfo.Reason);
        if (peer == _ourPeer)
        {
            _ourPeer = null;
        }
    }

    public void OnNetworkReceive(NetPeer peer, NetDataReader reader)
    {
        string command = reader.GetString();
        if (!string.IsNullOrEmpty(command))
        {
            Debug.Log("[SERVER] received command " + command);

            if (OnCommandDelegate != null)
            {
                OnCommandDelegate(command);
            }

            _dataWriter.Reset();
            _dataWriter.Put(command);
            peer.Send(_dataWriter, SendOptions.Sequenced);
        }
        else
        {
            Debug.Log("[SERVER] received nullcommand ");
        }
    }
}
