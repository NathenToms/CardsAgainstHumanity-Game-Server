using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatController : MonoBehaviour
{
	// References
	[SerializeField] Transform chatMenu;
	[SerializeField] Transform chatContent;

	// Prefabs
	[SerializeField] Transform messagePrefab;

	public bool chatOpen;

	public bool Toggle() { return (chatOpen = !chatOpen); }

	public void OpenChat()
	{
		RectTransform chatTransform = chatMenu.GetComponent<RectTransform>();

		chatTransform.pivot = new Vector2(1, 0.5f);
	}
	public void CloseChat()
	{
		RectTransform chatTransform = chatMenu.GetComponent<RectTransform>();

		chatTransform.pivot = new Vector2(0, 0.5f);
	}

	public void AddMessage(string message)
	{
		Transform newMessageObject = GameObject.Instantiate(messagePrefab, chatContent);

		newMessageObject.GetComponentInChildren<Text>().text = message;
	}
}
