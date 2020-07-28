using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;

public class Server
{
	#region Server Members
	public static Dictionary<int, ServerClient> Clients = new Dictionary<int, ServerClient>();

	public static int MaxPlayers { get; private set; }
	public static int Port { get; private set; }

	public static bool RunnerRoleOpen = true;
	public static bool PlacerRoleOpen = false;

	private static TcpListener _tcpListener;
	private static UdpClient _udpListener;
	#endregion

	#region Start Server
	public static void StartServer(int maxPlayers, int port)
	{
		MaxPlayers = maxPlayers;
		Port = port;

		Console.WriteLine("[Server] Starting server...");
		Console.WriteLine($"[Server] IP: 127.0.0.1\n[Server] Port: {Port}\n");

		InitializeServerData();

		ServerHandle.InitializeServerData();
		
		_tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), Port);
		_tcpListener.Start();
		_tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

		_udpListener = new UdpClient(Port);
		_udpListener.BeginReceive(UDPReceiveCallback, null);

		Console.WriteLine($"[Server] Server started on {Port}.");
	}

	private static void InitializeServerData()
	{
		for (int index = 1; index <= MaxPlayers; index++) {
			Clients.Add(index, new ServerClient(index));
		}

		Console.WriteLine("[Server] Initializing Server Sockets");
	}
	#endregion

	#region Server Callbacks and Sending
	private static void TCPConnectCallback(IAsyncResult AR)
	{
		TcpClient client = _tcpListener.EndAcceptTcpClient(AR);

		_tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

		for (int index = 1; index <= MaxPlayers; index++) {
			if (Clients[index].tcp.socket == null)
			{
				Clients[index].tcp.Connect(client);
				return;
			}
		}
	}
	private static void UDPReceiveCallback(IAsyncResult AR)
	{
		try
		{
			IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
			byte[] data = _udpListener.EndReceive(AR, ref clientEndPoint);
			_udpListener.BeginReceive(UDPReceiveCallback, null);

			if (data.Length < 4)
			{
				return;
			}

			using (Packet packet = new Packet(data))
			{
				int clientID = packet.ReadInt();

				if (clientID == 0)
				{
					return;
				}

				if (Clients[clientID].udp.endPoint == null)
				{
					// If this is a new connection
					Clients[clientID].udp.Connect(clientEndPoint);
					return;
				}

				if (Clients[clientID].udp.endPoint.ToString() == clientEndPoint.ToString())
				{
					Clients[clientID].udp.HandleData(packet);
				}
			}
		}
		catch (Exception e)
		{
			Console.WriteLine($"[Server] Error receiving UDP data: {e}");
		}
	}

	public static void SendUDPData(IPEndPoint clientEndPoint, Packet packet)
	{
		try
		{
			if (clientEndPoint != null)
			{
				_udpListener.BeginSend(packet.ToArray(), packet.Length(), clientEndPoint, null, null);
			}
		}
		catch (Exception e)
		{
			Console.WriteLine($"[Server] Error sending data to {clientEndPoint} via UPD: {e}");
		}
	}
	#endregion
}
