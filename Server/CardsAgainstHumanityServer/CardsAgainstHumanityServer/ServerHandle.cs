using CardsAgainstHumanityServer;
using System;
using System.Collections;
using System.Collections.Generic;

public class ServerHandle
{
	#region PacketHandler Setup
	public delegate void PacketHnadler(int fromClient, Packet packet);
	public static Dictionary<int, PacketHnadler> packetHandler;
	#endregion
	public static void InitializeServerData()
	{
		// List of server commands this client can receive 
		// AND the functions they are linked to
		packetHandler = new Dictionary<int, PacketHnadler>()
		{
			// A client has returned the welcome with extra personal info
			{ (int) ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived },


			// A client has sent a chat message
			// We need to send a "print to chat" message to all clients
			{ (int) ClientPackets.sendChatMessage, ServerHandle.NewChatMessage },
			

			// Start a new game
			{ (int) ClientPackets.startNewGame, ServerHandle.StartNewGame },
						
			// Start a new game
			{ (int) ClientPackets.startNewRound, ServerHandle.StartNewRound },
			
			// A client has given us there cards
			{ (int) ClientPackets.sendCards, ServerHandle.OnCardsSubmitted },

			// A client / The card czar has submitted there choice for the winner
			{ (int) ClientPackets.sendWinner, ServerHandle.OnWinnerSubmitted },
		};

		Console.WriteLine($"[Server] Initializing Server Dictionary");
	}

	// A Client is sending us their initialization information
	public static void WelcomeReceived(int fromClient, Packet packet)
	{
		int clientIDCleck = packet.ReadInt();
		
		string username = packet.ReadString();

		Server.Clients[fromClient].Username = username;

		if (fromClient != clientIDCleck) Console.WriteLine($"[Server] Player \"{username}\" (ID: {fromClient}) has assumed the wrong client ID ({clientIDCleck})!");

		GameManager.PlayerJoining(fromClient, username);

		ServerSend.BroadcastClientJoin(fromClient, username, "");
	}
	
	// A client want to add a message to the chat
	public static void NewChatMessage(int fromClient, Packet packet)
	{
		string message =  packet.ReadString();

		Console.WriteLine($"[Server][{fromClient}] Chat Message: " + message);

		ServerSend.BroadcastChatMessage($"[{fromClient}] " + message);
	}

	// The "Host" has told us to start a new game
	public static void StartNewGame(int fromClient, Packet packet)
	{
		Console.WriteLine("[Server] Start New Game");

		GameManager.StartNewGame();
	}

	// The "Host" has told us to start a new round
	public static void StartNewRound(int fromClient, Packet packet)
	{
		// NOTE:(Nathen) This does not happen naturally.

		// A client asking the server to start a new round means something went wrong
		// This methods job is to fix that anomaly

		int NO_WINNER = -1;
		GameManager.StartNewRound(NO_WINNER, 0);
	}

	// A client has given us there cards
	// We pass this onto the card czar and enter the picking phase
	public static void OnCardsSubmitted(int fromClient, Packet packet)
	{
		List<string> cards = new List<string>();

		int size = packet.ReadInt();
		for (int i = 0; i < size; i++) {
			cards.Add(packet.ReadString());
		}

		Console.WriteLine($"[Server][{fromClient}] {Server.Clients[fromClient].Username} Has submitted there answers. {size} cards submitted!");

		GameManager.OnCardsSubmitted(fromClient, cards);
	}

	// A client has given us there cards
	// We pass this onto the card czar and enter the picking phase
	public static void OnWinnerSubmitted(int fromClient, Packet packet)
	{
		// Who did the card czar pick to win?
		int size = packet.ReadInt();

		List<string> card = new List<string>();

		for (int i = 0; i < size; i++) {
			card.Add(packet.ReadString());
		}

		GameManager.OnWinnerSubmitted(fromClient, card);
	}
}
