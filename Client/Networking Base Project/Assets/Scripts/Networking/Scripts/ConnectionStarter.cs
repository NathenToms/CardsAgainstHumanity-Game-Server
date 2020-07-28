using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionStarter : MonoBehaviour
{
	public void StartServer()
	{
		Server.StartServer(5, 25565);
		NetworkManager.Instance.ServerStarted = true;
	}

	public void StopServer() { }

	public void StartClient()
	{
		Client.Instance.Connected();
		NetworkManager.Instance.ServerStarted = true;
	}

	public void StopClient()
	{
		Client.Instance.Disconnect();
	}

	public void StartServerClient()
	{
		Server.StartServer(5, 25565);

		NetworkManager.Instance.ServerStarted = true;
		NetworkManager.Instance.ClientStarted = true;

		Client.Instance.Connected();
	}

	public void StopServerClient() { }
}
