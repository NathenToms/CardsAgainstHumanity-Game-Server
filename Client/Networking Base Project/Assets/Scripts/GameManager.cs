using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
	/// <summary> Not playing </summary>
	InLobby,

	/// <summary> You are selecting you cards to submit </summary>
	SelectingCards,

	/// <summary> You have submitted and are new waiting for the card czar to pick the winner </summary>
	SelectingCards_Waiting,

	/// <summary> You are the card czar. Waiting for other players to submit </summary>
	Picking_Waiting,

	/// <summary> You are the card czar. Picking the best answers </summary>
	Picking_Winner
}

// TODO:PICKING and SUBMISSION PHASE

public class GameManager : MonoBehaviour
{
	// Singleton
	public static GameManager Instance;

	// Constants
	public const int NO_WINNER = -1;
	public const int FIRST_ROUND = 1;

	// Dependencies / Prefabs
	[SerializeField] private PlayerPanel playerPanelPrefab;
	[SerializeField] private RectTransform questionCard;
	[SerializeField] private Transform roundWinnerPanel;
	[SerializeField] private Transform gameWinnerPanel;
	[SerializeField] private Transform lobbyAnchor;
	[SerializeField] private Transform handBlocker;
	[SerializeField] private Transform startButton;
	[SerializeField] private Text roundText;
	[SerializeField] private Text clientText;

	// The Current game state for this client
	public GameState CurrentGameState = GameState.InLobby;

	// How many cards do we need to submit?
	[HideInInspector] public int CardsNeeded;
	[HideInInspector] public int PointsToWin;


	// Private
	// List of player panels that are currently playing
	private Dictionary<int, PlayerPanel> playerUIs = new Dictionary<int, PlayerPanel>();


	// Something went wrong => Force a New Round!
	public void ForceNewRound() => ClientSend.ForceNewRound();
	public void StartNewGame() => ClientSend.StartNewGame();


	// Start is called before the first frame update
	void Awake()
	{
		// Setup Singleton
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}

		#region TODO:Fix this
		// TODO: Put this in a better spot {
		Vector2 size = new Vector2(
		questionCard.sizeDelta.x,
		questionCard.parent.GetComponent<RectTransform>().rect.height * 0.80f);

		questionCard.sizeDelta = size;
		questionCard.localPosition = Vector3.zero;
		//}
		#endregion

		// If the Winner panel is open
		if (gameWinnerPanel.gameObject.activeSelf) {
			TweenUtility.Instance.EaseOut_Scale_ElasticY(gameWinnerPanel, 0, 1, 1.0f, 0, () => 
			{
				gameWinnerPanel.gameObject.SetActive(false);
			});
		}

		startButton.gameObject.SetActive(false);
	}

	// Validate that all the components we need to run are referenced correctly
	private void OnValidate()
	{
		bool valid = true;

		if (playerPanelPrefab == null) valid = false;
		if (roundWinnerPanel == null) valid = false;
		if (questionCard == null) valid = false;
		if (lobbyAnchor == null) valid = false;
		if (handBlocker == null) valid = false;
		if (startButton == null) valid = false;
		if (roundText == null) valid = false;

		if (valid == false) Debug.LogError("Not all scripts on 'GameManager' are references correctly! You must fix this", this);
	}



	/// <summary> When we connect to the server </summary>
	public void OnConnectToServer()
	{
		if (Client.Instance.ID == 1) {
			startButton.gameObject.SetActive(true);
		}
	}

	/// <summary> The server has told us to start a new game </summary>
	public void OnNewGame(int points)
	{
		startButton.gameObject.SetActive(false);

		Hand.Instance.OnNewGame();

		Chat.Print($"Setting Points to Win to.. '{points}'", MessageType.Default);
		PointsToWin = points;

		// If the Winner panel is open
		gameWinnerPanel.gameObject.SetActive(false);

		// Set name text
		clientText.text = $"[{Client.Instance.ID}] " + Client.Instance.Username;

		foreach (PlayerPanel player in playerUIs.Values) {
			player.Reset();
		}
	}

	/// <summary> The server has told us the game is over </summary>
	public void OnGameOver(int winnerID)
	{
		// Clear the current hands
		Hand.Instance.OnNewGame();

		// USER INTERFACE
		// Update the "Score Display" for the winning player
		UpdatePlayerScors(winnerID);

		gameWinnerPanel.gameObject.SetActive(true);
		gameWinnerPanel.GetComponentInChildren<Text>().text = $"{playerUIs[winnerID].Name} Wins!!";

		TweenUtility.Instance.EaseOut_Scale_ElasticY(gameWinnerPanel, 0, 1, 1.0f, 0, () => { });
	}

	/// <summary> The server has told us to start a new round </summary>
	public void OnNewRound(int round, int newCardCzar, int lastRoundWinner, List<string> cards, string newQuestion, int cardsNeeded)
	{
		// Activate the "Winner Display UI Panel"
		roundWinnerPanel.gameObject.transform.localScale = new Vector3(0, 1, 1);
		roundWinnerPanel.gameObject.SetActive(true);

		// Set "Winner Display UI Panel's"
		string winnerName = lastRoundWinner == NO_WINNER ? "STARTING A NEW GAME!" : $"[{lastRoundWinner	}] {playerUIs[lastRoundWinner].Name} Wins!";
		roundWinnerPanel.GetComponentInChildren<Text>().text = winnerName;

		TweenUtility.Instance.EaseOut_Scale_ElasticX(roundWinnerPanel, 0, 1, 0.75f, 0, () => {
			TweenUtility.Instance.EaseIn_Scale_ElasticX(roundWinnerPanel, 1, 0, 0.75f, 2.5f, () => {

				// Disable the "Winner Display UI Panel"
				roundWinnerPanel.gameObject.SetActive(false);

				// USER INTERFACE
				// Update the "Score Display" for the winning player
				UpdatePlayerScors(lastRoundWinner);

				// Update the current round UI
				UpdateRoundText(round.ToString());
			
				// QUESTION
				// Update the question and card needed
				UpdateQuestion(newQuestion, cardsNeeded);

				// CLIENT
				// Update this clients game state
				OnSubmissionPhase(newCardCzar);

				// HAND
				// Clear last rounds answers and show the plays hand
				UpdateHand();

				// Add new cards to the players hand
				AddCardsToHand(cards);

			});
		});
	}

	/// <summary> A new round has started and we have to submit our cards </summary>
	public void OnSubmissionPhase(int cardCzarID)
	{
		//If we are the card czar
		if (cardCzarID == Client.Instance.ID)
		{
			Chat.Print("You care the CARD CZAR!!", MessageType.Default);
			CurrentGameState = GameState.Picking_Waiting;
			handBlocker.gameObject.SetActive(true);
		}
		// If we are a normal player
		// We want to submit answers
		else
		{
			Chat.Print("You can submit your answers now!", MessageType.Default);
			CurrentGameState = GameState.SelectingCards;
			handBlocker.gameObject.SetActive(false);
		}
	}

	/// <summary> All answers have been submitted and we are entering the picking phase </summary>
	public void OnPickingPhase(List<List<string>> hands)
	{
		// Check if we are the card czar
		bool isCardCzar = CurrentGameState == GameState.Picking_Waiting;

		// Hide our hand and display the answers summited from all players
		Hand.Instance.DisplayAnswersToAll(hands, isCardCzar);
		Hand.Instance.HideHand();

		// If we are the card czar
		if (CurrentGameState == GameState.Picking_Waiting)
		{
			CurrentGameState = GameState.Picking_Winner;
			handBlocker.gameObject.SetActive(true);
		}

		// If we are NOT the card czar
		if (CurrentGameState == GameState.SelectingCards)
		{
			CurrentGameState = GameState.SelectingCards_Waiting;
			handBlocker.gameObject.SetActive(true);
		}
	}


	#region Private Methods
	void UpdateQuestion(string question, int cardsNeeded)
	{
		// Update the question card
		UpdateQuestionCard(question, questionCard);

		// Set the number of cards we need to submit
		CardsNeeded = cardsNeeded;
	}

	void UpdateQuestionCard(string newQuestion, RectTransform cardTransform)
	{
		// Where we started
		Vector3 pos = cardTransform.position;

		// Our size
		float height = cardTransform.sizeDelta.y;
		float width = cardTransform.sizeDelta.x;

		TweenUtility.Instance.EaseIn_Rotation_QuartY(cardTransform, 0, 90, 0.5f, 0, () =>
		{
			cardTransform.GetComponentInChildren<Text>().text = "";
			TweenUtility.Instance.EaseOut_Rotation_QuartY(cardTransform, 90, 180, 0.5f, 0, () => {

				TweenUtility.Instance.EaseIn_Size_QuartHeight(cardTransform, height, 0, 0.5f, 0, () =>
				{
					// Resize / Reset current card
					cardTransform.sizeDelta = new Vector2(width, height);
					cardTransform.eulerAngles = Vector3.zero;

					// Move above the screen
					cardTransform.position = new Vector3(pos.x, pos.y + Screen.height, 0);

					// Set the new question cards text
					cardTransform.GetComponentInChildren<Text>().text = newQuestion;

					// Move to target position
					// From our current y position
					//cardTransform.pivot = new Vector2(0.5f, 0.5f);
					TweenUtility.Instance.EaseOut_Transform_QuartY(cardTransform, cardTransform.position.y, cardTransform.parent.position.y, 0.4f, 0.2f, () => {
						cardTransform.localPosition = Vector3.zero;
					});
				});
			});
		});
	}

	// Update the "Player Scores Display" for the winning player
	void UpdatePlayerScors(int lastRoundWinner)
	{
		if (playerUIs.ContainsKey(lastRoundWinner))
		{
			TweenUtility.Instance.EaseOut_Scale_ElasticAll(playerUIs[lastRoundWinner].ImageTransform, 1, 1.2f, 0.25f, 0, () => {
				playerUIs[lastRoundWinner].OnWin();
				TweenUtility.Instance.EaseIn_Scale_ElasticAll(playerUIs[lastRoundWinner].ImageTransform, 1.2f, 1, 0.25f, 0.5f);
			});
		}
	}

	void UpdateRoundText(string round)
	{
		TweenUtility.Instance.EaseOut_Scale_ElasticAll(roundText.transform, 1, 1.2f, 0.25f, 0, () => {
			roundText.text = $"Cards Against Humanity | Round {round} | Points To Win: {PointsToWin}";
			TweenUtility.Instance.EaseIn_Scale_ElasticAll(roundText.transform, 1.2f, 1, 0.25f, 0.5f);
		});
	}

	void UpdateHand()
	{
		// Remove the answers from last round
		Hand.Instance.ClearAnswerGroup();

		// Show the hand
		Hand.Instance.ShowHand();
	}

	void AddCardsToHand(List<string> cards)
	{
		// Add the new cards to our hand
		for (int i = 0; i < cards.Count; i++)
		{
			Hand.Instance.AddNewCard(cards[i], i * 0.3f);
		}
	}
	#endregion


	/// <summary>  Send our selected cards to the server </summary>
	public void Submit()
	{
		if (Hand.Instance.CanSubmit())  {
			Hand.Instance.Submit();
			handBlocker.gameObject.SetActive(true);
		}
	}


	/// <summary> Add a player to the lobby </summary>
	public void AddNewPlayer(int ID, string username)
	{
		if (playerUIs.ContainsKey(ID)) {
			Chat.Print("A play who is already connected to the lobby attempted to join!", MessageType.Default);
		}
		else
		{
			PlayerPanel playerPanel = Instantiate(playerPanelPrefab, lobbyAnchor);
			playerPanel.Initialized(ID, username);
			playerUIs.Add(ID, playerPanel);
		}
	}

	/// <summary> Remove a player to the lobby </summary>
	public void RemovePlayer(int ID)
	{
		if (playerUIs.ContainsKey(ID))
		{
			Destroy(playerUIs[ID].gameObject);
			playerUIs.Remove(ID);
		}
	}
}
