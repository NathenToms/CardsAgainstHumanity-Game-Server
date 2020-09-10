using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

#region RequireComponent
[RequireComponent(typeof(ThreadManager))]
[RequireComponent(typeof(Client))]

// Interfaces
[RequireComponent(typeof(ChatController))]
[RequireComponent(typeof(ConnectionStarter))]
#endregion

public class NetworkManager : MonoBehaviour
{
	// Singleton
	public static NetworkManager Instance;

	// Dependencies
	// Menu References
	[SerializeField] private Transform connectionMenu;


	// Public
	public bool ServerStarted = false;
	public bool ClientStarted = false;

	public bool ShowChatMessages = true;


	// Private
	private ConnectionStarter connectionController;
	private ChatController chatController;

	#region Editor
	[HideInInspector] public bool ShowDefaultEditor;
	#endregion

	// Start is called before the first frame update
	void Awake()
	{
		// Setup Singleton
		if (Instance == null) { Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}

		// Dependencies
		connectionController = GetComponent<ConnectionStarter>();
		chatController = GetComponent<ChatController>();

		// Position the connect window
		connectionMenu.transform.localPosition = Vector3.zero;

		// Initialize server and Client dictionaries
		ClientHandle.InitializeClientData();
	}

	// Start and Stop Server & Client
	//
	public void StartServerAndClient() => connectionController.StartServerClient();

	public void StartServer() => connectionController.StartServer();
	public void StartClient() => connectionController.StartClient();

	public void StopClient() => connectionController.StopClient();


	// Close the "Connect Menu"
	//
	public void CloseConnectionPanel() => connectionMenu.gameObject.SetActive(false);


	// Add & Remove UI representing players from the lobby 
	//
	public void AddConnection(string username, int ID) => GameManager.Instance.AddNewPlayer(ID, username);

	public void RemoveConnection(int ID) => GameManager.Instance.RemovePlayer(ID);


	// Manage Chat	
	//
	// Add a Message to the Chat
	public void AddChatMessage(string message) => chatController.AddMessage(message);

	// [Chat] On Send Button Press
	// Tell the server to send a message to all clients
	public void SendChatMessage() 
	{
		string message = "";

		TMP_InputField inputField = GameObject.Find("[Input] Message Box").GetComponent<TMP_InputField>();

		message = inputField.text;
		inputField.text = "";

		ClientSend.SendChatMessage(message);
	}

	// Open and close chat
	public void ToggleChat()
	{
		if (chatController.Toggle())
		{
			chatController.OpenChat();
		}
		else
		{
			chatController.CloseChat();
		}
	}
}
