using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Client Handle is called when we want to unpack a packet
/// </summary>
public class ClientHandle
{
	#region PacketHandler Setup
	public delegate void PacketHandler(Packet packet);
	public static Dictionary<int, PacketHandler> packetHandler;
	#endregion
	public static void InitializeClientData()
	{
		// List of server commands this client can receive 
		// AND the functions they are linked to
		packetHandler = new Dictionary<int, PacketHandler>()
		{
			// When this client connects the server will send us a welcome message with our ID
			{ (int)ServerPackets.welcome, ClientHandle.Welcome },
	
			{ (int)ServerPackets.broadcastClientJoin, ClientHandle.NewConnection },
			{ (int)ServerPackets.broadcastClientDisconnect, ClientHandle.Disconnection },
			
			{ (int)ServerPackets.broadcastNewGame, ClientHandle.NewGame },
			{ (int)ServerPackets.broadcastGameOver, ClientHandle.GameOver },
			{ (int)ServerPackets.broadcastNewRound, ClientHandle.NewRound },
			
			{ (int)ServerPackets.startPickingPhase, ClientHandle.PickingPhase },
			
			{ (int)ServerPackets.broadcastChatMessage, ClientHandle.HandelNewMessage },
		};

		Debug.Log("[Client] Initialized packets.");
	}

	// [Setup Chain (2)] On welcome message received from server
	// We are given an ID and a welcome message. We want to print out that welcome message
	public static void Welcome(Packet packet)
	{
		// [Packet] Sender ID | welcome message

		// [Get] Get our ID from the server packet
		int senderID = packet.ReadInt();

		// [Get] Get the welcome message from the server socket
		string message = packet.ReadString();

		// The amount of current connections
		int size = packet.ReadInt();

		Chat.Print($"There are currently {size} players on the server", MessageType.Default);

		for (int i = 0; i < size; i++)
		{
			int ID = packet.ReadInt();
			string username = packet.ReadString();

			NetworkManager.Instance.AddConnection(username, ID);

			Chat.Print($"[Client] Adding player '{username}' ID {ID}", MessageType.ClientMessage);
		}

		// Print welcome message
		Chat.Print($"[Client] Message from the server: '{message}\n[Client] ID: {senderID}'", MessageType.ClientMessage);

		// Get our client object and set its ID
		Client.Instance.ID = senderID;

		// [UDP] Connect our UDP Client
		Client.Instance.Udp.Connect(((IPEndPoint)Client.Instance.Tcp.socket.Client.LocalEndPoint).Port, Client.Instance.ID);

		// On connect to game
		GameManager.Instance.OnConnectToServer();

		// We want to return a welcome received message to the server, this will send..
		// our Username and confirmation  we have connected
		ClientSend.WelcomeReceived();
	}

	// Player joins and player leaves
	public static void NewConnection(Packet packet)
	{
		// [Packet] Chat message | Size of Array | [(string)Sender username, (int)Sender ID] 

		// The Senders ID
		int senderID = packet.ReadInt();

		// The Senders Username
		string senderUsername = packet.ReadString();

		// A message from the server
		string serverMessage = packet.ReadString();

		if (senderUsername == "NULL")
		{
			// No nothing
		}
		else
		{
			NetworkManager.Instance.AddConnection(senderUsername, senderID);
		}
	}
	public static void Disconnection(Packet packet)
	{
		int ID = packet.ReadInt();

		NetworkManager.Instance.RemoveConnection(ID);
	}

	/// <summary> Packet Format: [ Points to win, server message ] </summary>
	public static void NewGame(Packet packet)
	{
		int points = packet.ReadInt();
		string serverMessage = packet.ReadString();

		Chat.Print($"[Server] Starting a New Game! Points needed {points}", MessageType.ClientMessage);
		
		GameManager.Instance.OnNewGame(points);
	}
	public static void GameOver(Packet packet)
	{
		int winnerID = packet.ReadInt();

		GameManager.Instance.OnGameOver(winnerID);
	}

	/// <summary> Packet Format: [ Round Number, CardCzarID, LastRoundWinner, Array Size => [Card Texts], Server Message ] </summary>
	public static void NewRound(Packet packet)
	{
		int currentRound = packet.ReadInt();
		int nextCardCzarID = packet.ReadInt();
		int lastRoundWinner = packet.ReadInt();

		List<string> cards = new List<string>();

		int size = packet.ReadInt();
		for (int i = 0; i < size; i++)
		{
			cards.Add(packet.ReadString());
		}

		string newQuestionCard = packet.ReadString();
		string serverMessage = packet.ReadString();

		int cardsNeeded = packet.ReadInt();

		// New Round
		Chat.Print($"[Server] Starting a New Round!", MessageType.ClientMessage);

		// The New Rounds Info
		Chat.Print($"[Server] Round Info [ Round: {currentRound}, Card Czar: {nextCardCzarID}, Last Round Winner: {(lastRoundWinner == -1 ? "No Winner" : lastRoundWinner.ToString())} ]", MessageType.ClientMessage);

		// The next question
		Chat.Print($"[Server] New Question: {newQuestionCard}, Cards Needed {cardsNeeded} ", MessageType.ClientMessage);

		GameManager.Instance.OnNewRound(currentRound, nextCardCzarID, lastRoundWinner, cards, newQuestionCard, cardsNeeded);
	}

	//
	public static void PickingPhase(Packet packet)
	{
		List<List<string>> hands = new List<List<string>>();

		int handCount = packet.ReadInt();
		for (int i = 0; i < handCount; i++)
		{
			hands.Add(new List<string>());
			int cardCount = packet.ReadInt();
			for (int j = 0; j < cardCount; j++)
			{
				hands[i].Add(packet.ReadString());
			}
		}

		GameManager.Instance.OnPickingPhase(hands);
	}

	// New chat message
	public static void HandelNewMessage(Packet packet)
	{
		string message = packet.ReadString();

		Chat.Print(message, MessageType.Default);
	}
}
