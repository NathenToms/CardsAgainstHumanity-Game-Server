using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour
{
	[SerializeField] Text nameText;
	[SerializeField] Text scoreText;

	string playerUsername;
	int playerID;
	int score;

	public string Name { get { return playerUsername; } }

	public Transform ImageTransform { get { return scoreText.transform.parent; } }

	private void OnValidate()
	{
		if (nameText == null)
			Debug.LogError("The 'nameText' field on this object is required!", this);

		if (scoreText == null)
			Debug.LogError("The 'scoreText' field on this object is required!", this);
	}

	public void Initialized(int ID, string name)
	{
		playerID = ID;
		playerUsername = name;
		nameText.text = $"[{ID}] {name}";
		scoreText.text = "0";
		score = 0;
	}

	public void OnWin()
	{
		score++;
		scoreText.text = score.ToString();
	}

	public void Reset()
	{
		score = 0;
		scoreText.text = "0";
	}
}
