using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
	//#region PacketHandler Setup
	//public delegate void PacketHnadler(int fromClient, Packet packet);
	//public static Dictionary<int, PacketHnadler> packetHandler;
	//#endregion
	//public static void InitializeServerData()
	//{
	//	// List of server commands this client can receive 
	//	// AND the functions they are linked to
	//	packetHandler = new Dictionary<int, PacketHnadler>()
	//	{
	//		// A client has returned the welcome with extra personal info
	//		{ (int) ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived },

	//		// A client has sent a chat message
	//		// We need to send a "print to chat" message to all clients
	//		{ (int) ClientPackets.sendChatMessage, ServerHandle.NewChatMessage },

	//		// A client wants to draw some cards
	//		{ (int) ClientPackets.drawCards, ServerHandle.GiveCards },

	//		//
	//		{ (int) ClientPackets.sendCards, ServerHandle.OnCardsSubmitted },
	//	};
	//}	

	//// [Setup Chain (4)] On Client confirmation and client info received
	//// We want to send a UI update and add this clients into to the server
	//public static void WelcomeReceived(int fromClient, Packet packet)
	//{
	//	int clientIDCleck = packet.ReadInt();
		
	//	string username = packet.ReadString();

	//	Server.Clients[fromClient].Username = username;
	//	ThreadManager.ExecuteOnMainThread(() => { if (fromClient != clientIDCleck) {
	//			Chat.Print($"[Server] Player \"{username}\" (ID: {fromClient}) has assumed the wrong client ID ({clientIDCleck})!", MessageType.ServerMessage);
	//	}; });

	//	ServerSend.BroadcastClientJoin(fromClient, username, "");
	//}

	//// We got a new chat message, now send it to all the clients!
	//public static void NewChatMessage(int fromClient, Packet packet)
	//{
	//	string message =  packet.ReadString();

	//	Chat.Print($"[Server][{fromClient}] " + message, MessageType.ServerMessage);

	//	ServerSend.BroadcastChatMessage($"[{fromClient}] " + message);
	//}

	//// A client wants to draw some cards
	//public static void GiveCards(int fromClient, Packet packet)
	//{
	//	int cardsRequested = packet.ReadInt();

	//	ServerSend.SendCards(fromClient, GameManager.Instance.DrawCards(fromClient, cardsRequested));
	//}

	////
	//public static void OnCardsSubmitted(int fromClient, Packet packet)
	//{
	//	List<string> cards = new List<string>();
	//	int size = packet.ReadInt();
	//	for (int i = 0; i < size; i++) {
	//		string item = packet.ReadString();
	//		cards.Add(item);
	//	}

	//	GameManager.Instance.OnCardsSubmitted(fromClient, cards);
	//}
}
