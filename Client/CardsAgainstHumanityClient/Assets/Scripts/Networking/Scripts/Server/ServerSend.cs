using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSend
{
	//#region TCP Sends
	//public static void SendTCPData(int toClient, Packet packet)
	//{
	//	packet.WriteLength();
	//	Server.Clients[toClient].tcp.SendData(packet);
	//}
	//public static void SendTCPDataToAll(Packet packet)
	//{
	//	packet.WriteLength();

	//	for (int index = 1; index <= Server.MaxPlayers; index++)
	//	{
	//		Server.Clients[index].tcp.SendData(packet);
	//	}
	//}
	//public static void SendTCPDataToAll(int exceptClient, Packet packet)
	//{
	//	packet.WriteLength();

	//	for (int index = 1; index <= Server.MaxPlayers; index++)
	//	{
	//		if (index != exceptClient)
	//		{
	//			Server.Clients[index].tcp.SendData(packet);
	//		}
	//	}
	//}
	//#endregion
	//#region UDP Sends
	//public static void SendUDPData(int toClient, Packet packet)
	//{
	//	packet.WriteLength();
	//	Server.Clients[toClient].udp.SendData(packet);
	//}
	//public static void SendUDPDataToAll(Packet packet)
	//{
	//	packet.WriteLength();

	//	for (int index = 1; index <= Server.MaxPlayers; index++)
	//	{
	//		Server.Clients[index].udp.SendData(packet);
	//	}
	//}
	//public static void SendUDPDataToAll(int exceptClient, Packet packet)
	//{
	//	packet.WriteLength();

	//	for (int index = 1; index <= Server.MaxPlayers; index++)
	//	{
	//		if (index != exceptClient)
	//		{
	//			Server.Clients[index].udp.SendData(packet);
	//		}
	//	}
	//}
	//#endregion

	//#region Connect Methods
	//public static void BroadcastWelcome(int clientID, string message)
	//{
	//	using (Packet packet = new Packet((int)ServerPackets.welcome))
	//	{
	//		packet.Write(clientID);
	//		packet.Write(message);

	//		SendTCPData(clientID, packet);
	//	}
	//}
	//#endregion

	//#region Chat Join & Disconnect
	//public static void BroadcastClientJoin(int clientID, string username, string message)
	//{
	//	using (Packet packet = new Packet((int)ServerPackets.broadcastClientJoin))
	//	{
	//		// The joining clients ID
	//		packet.Write(clientID);

	//		// The joining clients username
	//		packet.Write(username);

	//		// Server message
	//		packet.Write(message);

	//		SendTCPDataToAll(packet);
	//	}
	//}
	
	//public static void BroadcastClientDisconnect(int exceptClient, string message)
	//{
	//	using (Packet packet = new Packet((int)ServerPackets.broadcastClientDisconnect))
	//	{
	//		// The disconnecting players ID
	//		packet.Write(exceptClient);

	//		// Username
	//		packet.Write(exceptClient);

	//		SendTCPDataToAll(packet);
	//	}
	//}
	//#endregion

	//#region Game Methods
	//public static void BroadcastNewGame(int cardsNeeded, string message)
	//{
	//	using (Packet packet = new Packet((int)ServerPackets.broadcastNewGame))
	//	{
	//		packet.Write(cardsNeeded);

	//		packet.Write(message);

	//		SendTCPDataToAll(packet);
	//	}		
	//}

	//public static void BroadcastNewRound(int round, int newCardCzar, int lastRoundWinner, string message)
	//{
	//	using (Packet packet = new Packet((int)ServerPackets.broadcastNewRound))
	//	{
	//		packet.Write(round);

	//		packet.Write(newCardCzar);

	//		packet.Write(lastRoundWinner);

	//		packet.Write(message);

	//		SendTCPDataToAll(packet);
	//	}
	//}

	//public static void StartPickingPhase(Dictionary<int, List<string>> hands)
	//{
	//	using (Packet packet = new Packet((int)ServerPackets.startPickingPhase))
	//	{
	//		packet.Write(hands.Count);
	//		foreach (List<string> hand in hands.Values)
	//		{
	//			packet.Write(hand.Count);

	//			foreach (string item in hand) {
	//				packet.Write(item);
	//			}
	//		}

	//		SendTCPDataToAll(packet);
	//	}
	//}

	//public static void SendCards(int toClient, List<string> cards)
	//{
	//	using (Packet packet = new Packet((int)ServerPackets.sendCards))
	//	{
	//		packet.Write(cards.Count);
	//		for (int i = 0; i < cards.Count; i++) {
	//			packet.Write(cards[i]);
	//		}

	//		SendTCPData(toClient, packet);
	//	}
	//}
	//#endregion

	//#region Chat Methods
	//public static void BroadcastChatMessage(string message)
	//{
	//	using (Packet packet = new Packet((int)ServerPackets.broadcastChatMessage))
	//	{
	//		// The chat message
	//		packet.Write(message);

	//		SendTCPDataToAll(packet);
	//	}
	//}
	//#endregion
}
