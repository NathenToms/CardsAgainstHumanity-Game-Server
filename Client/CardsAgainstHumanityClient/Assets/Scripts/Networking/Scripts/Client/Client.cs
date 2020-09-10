using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System;
using TMPro;

public class Client : MonoBehaviour
{
	// Singleton
	public static Client Instance;


	// Public
	// This client's ID. This is used by the server to identify you
	public int ID;

	// The Port we want to connect to
	public int Port = 25565;

	// The Host we want to connect to
	public string IP = "127.0.0.1";

	// The data buffer for this client
	public static int DataBufferSize = 4096;

	// Username
	public string Username = "";

	// Sockets
	public TCP Tcp;
	public UDP Udp;


	// Private
	// Is this client connected?
	private bool isConnected = false;

	private int currentRound;
	private int pointsNeeded;


	#region Editor
	[HideInInspector] public bool ShowDefaultEditor;
	#endregion

	// Initialize singleton and the server data
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Debug.Log("This Instance of Client is already created!");
			Destroy(gameObject);
		}

		// Initialize this clients data
		ClientHandle.InitializeClientData();
	}

	// When this script is destroyed
	private void OnApplicationQuit()
	{
		Disconnect();
	}

	// Connect to the server
	public void Connected()
	{
		#region Setup -- IP / Host
		string IP = GameObject.Find("Input_Host").GetComponentInChildren<TMP_InputField>().text;

		if (IP == "")
		{
			IP = "127.0.0.1";
		}

		this.IP = IP;
		#endregion
		#region Setup -- Port
		string portString = GameObject.Find("Input_Port").GetComponentInChildren<TMP_InputField>().text;
		int port;
		if (portString == "")
		{
			port = int.Parse("25565");
		}
		else
		{
			port = int.Parse(portString);
		}
		this.Port = port;
		#endregion

		// Initialize TCP and UDP sockets
		Tcp = new TCP();
		Udp = new UDP(this.IP, Port);

		// Connect
		Tcp.Connect(IP, port);

		// Make sure this client in connected
		isConnected = true;
	}

	// Disconnect from the server
	public void Disconnect()
	{
		if (isConnected)
		{
			isConnected = false;
			Tcp.socket.Close();
			if (Udp.socket != null) Udp.socket.Close();
			
			Chat.Print("[Client] Disconnected from server", MessageType.ClientMessage);
		}
	}

	#region TCP Class
	public class TCP
	{
		private byte[] receiveBuffer;

		public TcpClient socket;
		private NetworkStream stream;

		private Packet receivedData;

		public void Connect(string IP, int port)
		{
			this.socket = new TcpClient
			{
				ReceiveBufferSize = DataBufferSize,
				SendBufferSize = DataBufferSize
			};

			receiveBuffer = new byte[DataBufferSize];

			socket.BeginConnect(IP, port, ConnectCallback, socket);

			Chat.Print("[Client] TCP Connecting...", MessageType.ClientMessage);
		}

		private void ConnectCallback(IAsyncResult AR)
		{
			socket.EndConnect(AR);

			Chat.Print("[Client] TCP Connected!", MessageType.ClientMessage);

			if (socket.Connected == false)
			{
				return;
			}

			stream = socket.GetStream();

			receivedData = new Packet();

			stream.BeginRead(receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
		}

		public void SendData(Packet packet)
		{
			try
			{
				if (socket != null)
				{
					stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error sending data to server via TCP: {e}", true, 2);
			}
		}

		private void ReceiveCallback(IAsyncResult AR)
		{
			try
			{
				// Size of bytes received
				int byteLength = stream.EndRead(AR);
				if (byteLength <= 0)
				{
					Client client = GameObject.FindObjectOfType<Client>();
					client.Disconnect();
					return;
				}
				else
				{
					// Create a new EMPTY byte array to copy the new data into
					byte[] data = new byte[byteLength];

					// NOTE:(Nathen) Out data in currently in the "receive Buffer" client member
					// We need to copy  it into the byte array we just created
					Array.Copy(receiveBuffer, data, byteLength);

					// NOTE:(Nathen) TCP does not always return ALL the data, sometimes we can receive,
					// only some of the data. So we need to make sure that we have All of your data be from
					// doing something with it

					// Do something with that data
					receivedData.Reset(HandleData(data));

					// Keep reading
					stream.BeginRead(receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
				}

			}
			catch (Exception e)
			{
				Disconnect();
			}
		}

		private bool HandleData(byte[] data)
		{
			int packetLength = 0;

			// Set our received data to the bytes we just read from the data stream
			receivedData.SetBytes(data);

			// Check is received data contains more than  4 unread bytes
			// If it does that means we have the START of one of our packets..

			// Because an Int consists of 4 bytes and every pack starts with and Int
			// representing the length of the packet
			if (receivedData.UnreadLength() >= 4)
			{
				packetLength = receivedData.ReadInt();

				// We want to return true
				// Because we want to reset received Data
				if (packetLength <= 0)
				{
					return true;
				}
			}

			// While we still have packets we can handle
			while (packetLength > 0 && packetLength <= receivedData.UnreadLength())
			{
				byte[] packetBytes = receivedData.ReadBytes(packetLength);
				ThreadManager.ExecuteOnMainThread(() =>
				{
					using (Packet packet = new Packet(packetBytes))
					{
						int packetID = packet.ReadInt();

						ClientHandle.packetHandler[packetID](packet);
					}
				});

				packetLength = 0;
				if (receivedData.UnreadLength() >= 4)
				{
					packetLength = receivedData.ReadInt();

					// We want to return true
					// Because we want to reset received Data
					if (packetLength <= 0)
					{
						return true;
					}
				}
			}

			if (packetLength <= 1)
			{
				return true;
			}


			// If the packet length is greater than 1,
			// we don't want to reset received data because
			// theres a partial packet left in there
			return false;
		}

		public void Disconnect()
		{
			Client client = GameObject.FindObjectOfType<Client>();
			client.Disconnect();

			stream = null;
			receivedData = null;
			receiveBuffer = null;
			socket = null;
		}
	}

	#endregion
	#region UDP Class
	public class UDP
	{
		public UdpClient socket;
		public IPEndPoint endPoint;

		public int ID;

		public UDP(string ip, int port)
		{
			endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
		}

		// We want to bind to the local port
		// This is different than the server's port number
		public void Connect(int localPort, int ID)
		{
			this.ID = ID;

			socket = new UdpClient(localPort);

			Chat.Print("[Client] UDP Connecting", MessageType.ClientMessage);
			Chat.Print($"[Client] You are Connected!", MessageType.ClientMessage);

			socket.Connect(endPoint);
			socket.BeginReceive(ReceiveCallback, null);

			using (Packet packet = new Packet())
			{
				SendData(packet);
			}
		}

		public void SendData(Packet packet)
		{
			try
			{
				packet.InsertInt(ID);
				if (socket != null)
				{
					socket.BeginSend(packet.ToArray(), packet.Length(), null, null);
				}
			}
			catch (Exception e)
			{
				Chat.Print($"[Client] Error sending data to server via UDP: {e}", MessageType.ErrorMessage);
			}
		}

		private void ReceiveCallback(IAsyncResult AR)
		{
			try
			{
				byte[] data = socket.EndReceive(AR, ref endPoint);
				socket.BeginReceive(ReceiveCallback, null);

				if (data.Length < 4)
				{
					Client client = GameObject.FindObjectOfType<Client>();
					client.Disconnect();

					return;
				}

				HandleData(data);
			}
			catch (Exception e)
			{
				Disconnect();
			}
		}

		private void HandleData(byte[] data)
		{
			using (Packet packet = new Packet(data))
			{
				int packetLength = packet.ReadInt();
				data = packet.ReadBytes(packetLength);
			}

			ThreadManager.ExecuteOnMainThread(() =>
			{
				using (Packet packet = new Packet(data))
				{
					int packetID = packet.ReadInt();
					ClientHandle.packetHandler[packetID](packet);
				}
			});
		}
		public void Disconnect()
		{
			Client client = GameObject.FindObjectOfType<Client>();
			client.Disconnect();

			endPoint = null;
			socket = null;
		}
	}
	#endregion
}
