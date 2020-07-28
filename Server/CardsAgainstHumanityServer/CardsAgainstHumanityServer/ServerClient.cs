using CardsAgainstHumanityServer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class ServerClient
{
	// This players cards
	public List<string> Cards = new List<string>();


	// Public Members
	public string Username = null;
	
	public int ID;
	public int Points;

	#region Editor
	public bool CardsOpenInEditor;
	#endregion

	#region Network Members
	public static int DataBufferSize = 4096;

	public TCP tcp;
	public UDP udp;
	#endregion

	// Constructor
	public ServerClient(int id)
	{
		this.ID = id;
		this.tcp = new TCP(ID);
		this.udp = new UDP(ID);
	}

	public void Disconnect()
	{
		GameManager.PlayerDisconnecting(ID, Username);

		ServerSend.BroadcastClientDisconnect(ID, $"{Username} has Disconnected");

		Username = null;

		tcp.Disconnect();
		udp.Disconnect();
	}

	#region TCP and UDP Socket Classes
	public class TCP
	{
		private readonly int id;

		private byte[] receiveBuffer;

		public TcpClient socket;
		private NetworkStream stream;

		private Packet receivedData;

		public TCP(int id)
		{
			this.id = id;
		}

		public void Connect(TcpClient socket)
		{
			this.socket = socket;

			socket.ReceiveBufferSize = DataBufferSize;
			socket.SendBufferSize = DataBufferSize;

			stream = socket.GetStream();

			receivedData = new Packet();

			receiveBuffer = new byte[DataBufferSize];

			stream.BeginRead(receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);

			ServerSend.BroadcastWelcome(id, "Welcome to the server!");
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
				Console.WriteLine($"[Server] Error sending data to player {id} via TCP: {e}");
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
					Server.Clients[id].Disconnect();
					return;
				}
				else
				{
					// Create a new EMPTY byte array to copy the new data into
					byte[] data = new byte[byteLength];

					// NOTE:(Nathen) Out data in currently in the "receive Buffer" client member
					// We need to copy  it into the byte array we just created
					Array.Copy(receiveBuffer, data, byteLength);

					receivedData.Reset(HandleData(data));

					// Keep reading
					stream.BeginRead(receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
				}

			}
			catch (Exception e)
			{
				Console.WriteLine("[Server] Exception: " + e);
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

						Console.WriteLine($"\n[Data] Packet ID:{packetID} => {((ClientPackets)packetID)}\n");

						ServerHandle.packetHandler[packetID](id, packet);
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
			socket.Close();
			stream = null;
			receivedData = null;
			receiveBuffer = null;
			socket = null;
		}
	}
	public class UDP
	{
		public IPEndPoint endPoint;

		public int ID;

		public UDP(int id)
		{
			ID = id;
		}

		public void Connect(IPEndPoint endPoint)
		{
			this.endPoint = endPoint;
		}

		public void SendData(Packet packet)
		{
			Server.SendUDPData(endPoint, packet);
		}

		public void HandleData(Packet packetData)
		{
			int packetLength = packetData.ReadInt();
			byte[] packetBytes = packetData.ReadBytes(packetLength);

			ThreadManager.ExecuteOnMainThread(() =>
			{
				using (Packet packet = new Packet(packetBytes))
				{
					int packetID = packet.ReadInt();
					ServerHandle.packetHandler[packetID](ID, packet);
				}
			});
		}

		public void Disconnect()
		{
			endPoint = null;
		}
	}
	#endregion
}
