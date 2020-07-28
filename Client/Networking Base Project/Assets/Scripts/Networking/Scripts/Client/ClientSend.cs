using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClientSend : MonoBehaviour
{
	// Send Data to the server
	public static void SendTCPData(Packet packet)
	{
		packet.WriteLength();

		Client client = GameObject.FindObjectOfType<Client>();
		client.Tcp.SendData(packet);
	}
	public static void SendUDPData(Packet packet)
	{
		packet.WriteLength();

		Client client = GameObject.FindObjectOfType<Client>();
		client.Udp.SendData(packet);
	}

	// [Setup Chain (3)] After we received a welcome message from the server
	// We want to return this clients info the server
	public static void WelcomeReceived()
	{
		// [Packet] Sender ID | Sender username |

		// Find our Client
		Client client = GameObject.FindObjectOfType<Client>();

		using (Packet packet = new Packet((int)ClientPackets.welcomeReceived))
		{
			// Add our client ID
			packet.Write(client.ID);

			// Get and Add our username
			string username = GameObject.Find("InputField_Username").GetComponent<TMP_InputField>().text;

			// Catch empty name
			if (username == "") {
				username = "Anonymous Rick";
			}

			// Close the connection panel
			NetworkManager.Instance.CloseConnectionPanel();
			Client.Instance.Username = username;

			// Add it to the packet
			packet.Write(username);

			Chat.Print($"[Client] Sending your info to the server.. Your username is '{username}'", MessageType.ClientMessage);

			// Send the packet
			SendTCPData(packet);
		}
	}

	// Send a chat message to the server
	public static void SendChatMessage(string message)
	{
		// [Packet] Chat message |

		using (Packet packet = new Packet((int)ClientPackets.sendChatMessage))
		{
			packet.Write(message);

			SendTCPData(packet);
		}
	}

	// Host => Server: Start a new game
	public static void StartNewGame()
	{
		using (Packet packet = new Packet((int)ClientPackets.startNewGame))
		{
			packet.Write(true);

			// Send the packet
			SendTCPData(packet);
		}
	}

	// Host => Server: Force A New Round
	public static void ForceNewRound()
	{
		using (Packet packet = new Packet((int)ClientPackets.startNewRound))
		{
			packet.Write(true);

			// Send the packet
			SendTCPData(packet);
		}
	}

	//
	public static void SendWinner(List<string> cards)
	{
		Chat.Print("SendWinner", MessageType.Default);

		using (Packet packet = new Packet((int)ClientPackets.sendWinner))
		{
			Chat.Print($"Sending {cards.Count} cards", MessageType.Default);
			packet.Write(cards.Count);
			foreach (string card in cards)
			{
				packet.Write(card);
			}	

			// Send the packet
			SendTCPData(packet);
		}
	}

	//
	public static void SendCards(List<Card> selectedCards)
	{
		using (Packet packet = new Packet((int)ClientPackets.sendCards))
		{
			packet.Write(selectedCards.Count);
			foreach (Card card in selectedCards)
			{
				packet.Write(card.GetText());
			}

			// Send the packet
			SendTCPData(packet);
		}
	}
}
