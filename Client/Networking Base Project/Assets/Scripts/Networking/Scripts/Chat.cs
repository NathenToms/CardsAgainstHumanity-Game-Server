using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MessageType
{
	Default,
	ServerMessage,
	ClientMessage,
	EventMessage,
	ErrorMessage
}

public class Chat
{
	public static void Print(string message, MessageType messageType)
	{
		switch ((int)messageType)
		{
			case (int) MessageType.Default: DefaultMessage(message); break;

			case (int) MessageType.ServerMessage: ServerMessage(message); break;
			case (int) MessageType.ClientMessage: ClientMessage(message); break;

			case (int) MessageType.EventMessage: EventMessage(message);  break;

			case (int) MessageType.ErrorMessage: ErrorMessage(message);  break;

			default: break;
		}
	}

	static void DefaultMessage(string message)
	{
		if (NetworkManager.Instance.ShowChatMessages)
		{
			ThreadManager.ExecuteOnMainThread(() =>
			{
				NetworkManager.Instance.AddChatMessage($"<color=Black>{message}</color>");
			});
		}
	}

	static void ServerMessage(string message)
	{
		if (NetworkManager.Instance.ShowChatMessages)
		{
			ThreadManager.ExecuteOnMainThread(() =>
			{
				NetworkManager.Instance.AddChatMessage($"<color=Purple>{message}</color>");
			});
		}
	}
	static void ClientMessage(string message)
	{
		if (NetworkManager.Instance.ShowChatMessages)
		{
			ThreadManager.ExecuteOnMainThread(() =>
			{
				NetworkManager.Instance.AddChatMessage($"<color=Blue>{message}</color>");
			});
		}
	}

	static void EventMessage(string message)
	{
		if (NetworkManager.Instance.ShowChatMessages)
		{
			ThreadManager.ExecuteOnMainThread(() =>
			{
				NetworkManager.Instance.AddChatMessage($"<color=Orange>{message}</color>");
			});
		}
	}

	static void ErrorMessage(string message)
	{
		if (NetworkManager.Instance.ShowChatMessages)
		{
			ThreadManager.ExecuteOnMainThread(() =>
			{
				NetworkManager.Instance.AddChatMessage($"<color=Red>{message}</color>");
			});
		}
	}
}
