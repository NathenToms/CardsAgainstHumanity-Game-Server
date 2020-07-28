using CardsAgainstHumanityServer;
using System;
using System.Collections;
using System.Collections.Generic;

public class ServerSend
{
	#region TCP Sends
	public static void SendTCPData(int toClient, Packet packet)
	{
		packet.WriteLength();
		Server.Clients[toClient].tcp.SendData(packet);
	}
	public static void SendTCPDataToAll(Packet packet)
	{
		packet.WriteLength();

		for (int index = 1; index <= Server.MaxPlayers; index++)
		{
			Server.Clients[index].tcp.SendData(packet);
		}
	}
	public static void SendTCPDataToAll(int exceptClient, Packet packet)
	{
		packet.WriteLength();

		for (int index = 1; index <= Server.MaxPlayers; index++)
		{
			if (index != exceptClient)
			{
				Server.Clients[index].tcp.SendData(packet);
			}
		}
	}
	#endregion
	#region UDP Sends
	public static void SendUDPData(int toClient, Packet packet)
	{
		packet.WriteLength();
		Server.Clients[toClient].udp.SendData(packet);
	}
	public static void SendUDPDataToAll(Packet packet)
	{
		packet.WriteLength();

		for (int index = 1; index <= Server.MaxPlayers; index++)
		{
			Server.Clients[index].udp.SendData(packet);
		}
	}
	public static void SendUDPDataToAll(int exceptClient, Packet packet)
	{
		packet.WriteLength();

		for (int index = 1; index <= Server.MaxPlayers; index++)
		{
			if (index != exceptClient)
			{
				Server.Clients[index].udp.SendData(packet);
			}
		}
	}
	#endregion

	#region Connect Methods
	public static void BroadcastWelcome(int clientID, string message)
	{
		using (Packet packet = new Packet((int)ServerPackets.welcome))
		{
			packet.Write(clientID);
			packet.Write(message);

			int count = 0;
			foreach (ServerClient client in Server.Clients.Values) if (client.Username != null) count++;

			Console.WriteLine($"There are currently {count} connections!");

			packet.Write(count);
		
			foreach (ServerClient client in Server.Clients.Values) {
				if (client.Username != null)
				{
					packet.Write(client.ID);
					packet.Write(client.Username);
				}
			}

			SendTCPData(clientID, packet);
		}
	}
	#endregion

	#region Chat Join & Disconnect
	public static void BroadcastClientJoin(int clientID, string username, string message)
	{
		using (Packet packet = new Packet((int)ServerPackets.broadcastClientJoin))
		{
			// The joining clients ID
			packet.Write(clientID);

			// The joining clients username
			packet.Write(username);

			// Server message
			packet.Write(message);

			SendTCPDataToAll(packet);
		}
	}

	public static void BroadcastClientDisconnect(int exceptClient, string message)
	{
		using (Packet packet = new Packet((int)ServerPackets.broadcastClientDisconnect))
		{
			// The disconnecting players ID
			packet.Write(exceptClient);

			// Username
			packet.Write(exceptClient);

			SendTCPDataToAll(packet);
		}
	}
	#endregion

	#region Game Methods
	public static void BroadcastNewGame(int pointsToWin, string message)
	{
		Console.WriteLine("--------------------");
		Console.WriteLine($"[Server] Starting a new game!");
		Console.WriteLine($"[Server] You need {pointsToWin} points to win this game\n");

		using (Packet packet = new Packet((int)ServerPackets.broadcastNewGame))
		{
			packet.Write(pointsToWin);

			packet.Write(message);

			SendTCPDataToAll(packet);
		}
	}

	/// <summary> Packet Format: [ Round Number, CardCzarID, LastRoundWinner, Array Size => [Card Texts], Server Message ] </summary>
	public static void BroadcastNewRound(int round, int newCardCzar, int lastCardCzar, int lastRoundWinner, int cardsNeeded, QuestionCard questionCard, string message)
	{
		Console.WriteLine("--------------------");
		Console.WriteLine($"[Server] Broadcasting New Round | Round {round}, CardCzar {lastCardCzar} => {newCardCzar}, Last Round winner {lastRoundWinner}\n" +$"[Server] Sending {cardsNeeded} cards..\n" +$"[Server] New Question Card: {questionCard}. Cards Needed: {questionCard.cardsNeeded}\n");

		foreach (ServerClient player in Server.Clients.Values)
		{
			if (player.Username == null) continue;

			using (Packet packet = new Packet((int)ServerPackets.broadcastNewRound))
			{
				// Round count
				packet.Write(round);

				// new card czar ID
				packet.Write(newCardCzar);

				// Who won last round
				packet.Write(lastRoundWinner);

				// If its the first round of the game
				if (round == 1)
				{
					Console.WriteLine($"[Server] Sending {player.Username} {cardsNeeded} cards!");

					// How many cards we are sending to the client
					packet.Write(cardsNeeded);

					for (int i = 0; i < cardsNeeded; i++)
					{
						// The cards we are sending
						packet.Write(GameManager.DrawFromDeck(player.ID, false));
					}
				}
				else
				{
					if (player.ID == lastCardCzar)
					{
						// How many cards we are sending to the client
						packet.Write(0);
					}
					else
					{
						Console.WriteLine($"[Server] Sending {player.Username} {cardsNeeded} cards!");

						// How many cards we are sending to the client
						packet.Write(cardsNeeded);

						for (int i = 0; i < cardsNeeded; i++)
						{
							// The cards we are sending
							packet.Write(GameManager.DrawFromDeck(player.ID, false));
						}
					}
				}

				// New question
				packet.Write(questionCard.text);

				// Server message
				packet.Write(message);

				// The cards we need from the new question
				packet.Write(questionCard.cardsNeeded);

				SendTCPData(player.ID, packet);
			}
		}	
	}

	public static void StartPickingPhase(Dictionary<int, List<string>> hands)
	{
		using (Packet packet = new Packet((int)ServerPackets.startPickingPhase))
		{
			packet.Write(hands.Count);
			foreach (List<string> hand in hands.Values)
			{
				packet.Write(hand.Count);

				foreach (string item in hand)
				{
					packet.Write(item);
				}
			}

			SendTCPDataToAll(packet);
		}
	}

	public static void SendCards(int toClient, List<string> cards)
	{
		using (Packet packet = new Packet((int)ServerPackets.sendCards))
		{
			packet.Write(cards.Count);
			for (int i = 0; i < cards.Count; i++)
			{
				packet.Write(cards[i]);
			}

			SendTCPData(toClient, packet);
		}
	}

	public static void BroadcastGameOver(int winner)
	{
		using (Packet packet = new Packet((int)ServerPackets.broadcastGameOver))
		{
			packet.Write(winner);

			SendTCPDataToAll(packet);
		}
	}
	#endregion

	#region Chat Methods
	public static void BroadcastChatMessage(string message)
	{
		using (Packet packet = new Packet((int)ServerPackets.broadcastChatMessage))
		{
			// The chat message
			packet.Write(message);

			SendTCPDataToAll(packet);
		}
	}
	#endregion
}
