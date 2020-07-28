using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
	// If this card in the currently in the players hand?
	public bool InHand = true;


	// Start is called before the first frame update
	private void Awake()
	{
		// Link the "Hand" scrips "ClickCard" method..
		// To the OnClick event for this card panels button.
		GetComponent<Button>().onClick.AddListener(() =>
		{
			Hand.Instance.ClickCard(this);
		});
	}


	/// <summary> Set this cards text to the given text </summary>
	public void InitializeCard(string text) => GetComponentInChildren<Text>().text = text;
	
	/// <summary> Get this cards text </summary>
	public string GetText()
	{
		return GetComponentInChildren<Text>().text;
	}

	/// <summary> Move to the board and toggle "InHand" to true </summary>
	public void Select(Transform anchor)
	{
		transform.SetParent(anchor, false);
		InHand = false;
	}

	/// <summary> Move to the Hand and toggle "InHand" to off </summary>
	public void Remove(Transform anchor)
	{
		transform.SetParent(anchor, false);
		InHand = true;
	}
}
