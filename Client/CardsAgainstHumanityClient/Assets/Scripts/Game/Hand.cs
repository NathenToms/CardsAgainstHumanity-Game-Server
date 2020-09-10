using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{
	// Singleton 
	public static Hand Instance;

	// Dependencies
	// Game Anchors
	[SerializeField] Transform BoardBlocker_Anchor;
	[SerializeField] Transform Board_Anchor;
	[SerializeField] Transform Hand_Anchor;

	// Card Prefabs
	[SerializeField] GameObject CardGroupPrefab;
	[SerializeField] Card CardPrefab;


	// Public
	/// <summary> A list of this hands cards </summary>
	public List<Card> Cards = new List<Card>();
	public List<Card> SelectedCards = new List<Card>();

	public List<Transform> AnswerObjects = new List<Transform>();


	// Properties
	/// <summary> Can we submit our cards </summary>
	public bool CanSubmit() { return (Board_Anchor.childCount) == GameManager.Instance.CardsNeeded; }

	// Start is called before the first frame update
	void Awake()
	{
		if (Instance == null) {
			Instance = this;
		}
		else
		{
			Destroy(Instance);
		}
	}

	//
	public void AddNewCard(string cardText, float delay)
	{
		Card card = Instantiate(CardPrefab, Hand_Anchor);

		card.transform.localScale = new Vector3(0, 1, 1);

		card.InitializeCard(cardText);
		Cards.Add(card);

		TweenUtility.Instance.EaseOut_Scale_QuartX(card.transform, 0, 1, 0.25f, delay, () => {
		});
	}

	//
	public List<string> GetCards()
	{
		List<string> cardsList = new List<string>();
		foreach (Card card in SelectedCards)
		{
			cardsList.Add(card.GetText());
		}
		return cardsList;
	}

	//
	public void DisplayAnswersToAll(List<List<string>> hands, bool IsCardCzar)
	{
		Chat.Print($"DisplayAnswersToAll {IsCardCzar}", MessageType.Default);

		int count = 0;
		foreach (List<string> hand in hands)
		{
			GameObject handObject = Instantiate(CardGroupPrefab, Board_Anchor.parent);

			if (IsCardCzar)
			{
				Button button = handObject.GetComponent<Button>();
				button.onClick.AddListener(() => {
					ClientSend.SendWinner(hand);

					foreach (Transform item in AnswerObjects) {
						item.gameObject.SetActive(false);
					}
				});
			}

			int cardCount = 0;
			foreach (string card in hand)
			{
				Card cardObject = Instantiate(CardPrefab, handObject.transform);
				cardObject.InitializeCard(card);

				if (IsCardCzar)
				{
					Image image = cardObject.GetComponent<Image>();
				
					Destroy(cardObject.GetComponent<Button>());
					image.raycastTarget = false;
				}

				cardObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1);
				cardObject.transform.localScale = new Vector3(1, 0, 1);

				TweenUtility.Instance.EaseOut_Scale_QuartY(cardObject.transform, 0, 1, 0.5f, 0.5f * cardCount);
				cardCount++;
			}

			AnswerObjects.Add(handObject.transform);

			count++;
		}

		BoardBlocker_Anchor.gameObject.SetActive(IsCardCzar == false);
	}

	public void OnNewGame()
	{
		foreach (Transform child in Hand_Anchor) {
			Destroy(child.gameObject);
		}

		foreach (Transform child in Board_Anchor) {
			Destroy(child.gameObject);
		}

		SelectedCards.Clear();
		Cards.Clear();

		ClearAnswerGroup();
	}

	public void ClearAnswerGroup()
	{
		foreach (Transform hand in AnswerObjects) {
			Destroy(hand.gameObject);
		}

		AnswerObjects.Clear();

		BoardBlocker_Anchor.gameObject.SetActive(false);
	}

	public void ShowHand()
	{
		Board_Anchor.GetComponent<RectTransform>().sizeDelta = new Vector2(160, 0);
	}

	public void HideHand()
	{
		Board_Anchor.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
	}

	public void Submit()
	{
		ClientSend.SendCards(SelectedCards);

		GameManager.Instance.CurrentGameState = GameState.SelectingCards_Waiting;

		foreach (Transform child in Board_Anchor) {
			if (child.GetComponent<Card>())
			{
				Destroy(child.gameObject);
			}
		}

		SelectedCards.Clear();
		BoardBlocker_Anchor.gameObject.SetActive(true);
	}

	public void ClickCard(Card card)
	{
		if (GameManager.Instance.CurrentGameState != GameState.SelectingCards) return;

		if (card.InHand)
		{
			if (Board_Anchor.childCount < GameManager.Instance.CardsNeeded)
			{
				card.Select(Board_Anchor);
				SelectedCards.Add(card);
				Cards.Remove(card);
			}
		}
		else
		{
			card.Remove(Hand_Anchor);
			SelectedCards.Remove(card);
			Cards.Add(card);
		}
	}
}
