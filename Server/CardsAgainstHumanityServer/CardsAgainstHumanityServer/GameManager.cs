using System;
using System.Collections.Generic;

namespace CardsAgainstHumanityServer
{
	public class GameManager
	{
		// NOTE:(Nathen) "TurnOrder" This is the order that players connect to the game

		// Consents
		public const int NO_WINNER = -1;
		public const int FIRST_ROUND = 0;

		// A list of turn orders.
		// This dictates the order the player will take their turns.
		public static List<int> TurnOrder = new List<int>();

		// PLAYER HANDS
		public static Dictionary<int, List<string>> PlayerHands = new Dictionary<int, List<string>>();

		// PLAYER SUBMITTIONS
		public static Dictionary<int, List<string>> PlayerSubmissions = new Dictionary<int, List<string>>();

		// DECKS
		public static List<QuestionCard> QuestionDeck = new List<QuestionCard>();
		public static List<AnswerCard> AnswerDeck = new List<AnswerCard>();

		// DISCARD PILE
		public static List<AnswerCard> DiscardPile = new List<AnswerCard>();

		// Private
		private static QuestionCard currentQuestion;

		private static int currentRound;
		private static int currentTurnIndex;

		public static int cardsNeededToWin = 3;

		private static int lastCardCzar = -1;

		// Properties
		public static int CurrentTurn
		{
			get { return TurnOrder[currentTurnIndex]; }
		}
		public static int NextTurn
		{
			get { return TurnOrder[(currentTurnIndex = (currentTurnIndex + 1) % TurnOrder.Count)]; }
		}


		public static void StartNewGame()
		{
			currentRound = FIRST_ROUND;

			UpdateCurrentQuestion();

			foreach (ServerClient client in Server.Clients.Values) {
				client.Points = 0;
			}

			ServerSend.BroadcastNewGame(cardsNeededToWin, "[Server] Starting new game");

			StartNewRound(NO_WINNER, 8);
		}

		public static void StartNewRound(int _lastWinner, int cardsNeeded)
		{
			// NOTE:(Nathen) "++currentRound" and "currentRound++" are NOT the same thing

			// Check if there is a winner
			bool gameOver = false;
			foreach (ServerClient client in Server.Clients.Values)
			{
				if (client.Points >= cardsNeededToWin) {
					gameOver = true;
				}
			}

			PlayerSubmissions = new Dictionary<int, List<string>>();

			if (gameOver)
			{
				Console.WriteLine($"[Server] GAME OVER! Winner: {_lastWinner}");

				ServerSend.BroadcastGameOver(_lastWinner);
			}
			else
			{
				if ((++currentRound) == 1)
				{
					// NOTE:(Nathen) This is 'TURN ONE' of the game
					ServerSend.BroadcastNewRound(currentRound, TurnOrder[0], lastCardCzar, NO_WINNER, cardsNeeded, currentQuestion, "[Server] First Round of the Game!");
				}
				else
				{
					lastCardCzar = CurrentTurn;

					// The Properties "NextTurn" Auto Increments the turn index and returns the ID for the next card czar
					int nextTurnID = NextTurn;

					// This was an error round
					if (_lastWinner == NO_WINNER)
					{
						// Forced round
						ServerSend.BroadcastNewRound(currentRound, nextTurnID, lastCardCzar, NO_WINNER, cardsNeeded, currentQuestion, $"[Server] A New Round was Forced!\n[Server] Round {currentRound}");
					}
					else
					{
						// Normal Round
						ServerSend.BroadcastNewRound(currentRound, nextTurnID, lastCardCzar, _lastWinner, cardsNeeded, currentQuestion, $"[Server] Round | {currentRound}");
					}
				}
			}
		}

		public static string DrawFromDeck(int _id, bool _removeFromDeck)
		{
			// TODO => Random

			// If the deck in empty
			if (AnswerDeck.Count == 0)
			{
				if (DiscardPile.Count == 0)
				{
					Console.WriteLine("ERROR!! No cards in the deck");
				}
				else
				{
					// Put discard pile into card pile
					foreach (AnswerCard card in DiscardPile) {
						AnswerDeck.Add(card);
					}

					DiscardPile.Clear();
				}
			}

			// Draw a card
			AnswerCard newCard = AnswerDeck[0];
			AnswerDeck.RemoveAt(0);

			// Add to a players hand
			if (PlayerHands.ContainsKey(_id) == false) PlayerHands.Add(_id, new List<string>());

			PlayerHands[_id].Add(newCard.text);

			// Return
			return newCard.text;
		}

		public static QuestionCard UpdateCurrentQuestion()
		{
			QuestionCard newCard = QuestionDeck[0];
			QuestionDeck.RemoveAt(0);

			currentQuestion = newCard;

			return currentQuestion;
		}

		public static void OnCardsSubmitted(int _sendingClientId, List<string> _cards)
		{
			if (PlayerSubmissions.ContainsKey(_sendingClientId))
			{
				Console.WriteLine($"[Server] ERROR! This player has already submitted there answers - {_sendingClientId}");
			}
			else
			{
				PlayerSubmissions.Add(_sendingClientId, _cards);
			}

			// If we have all our answers
			if (PlayerSubmissions.Count == TurnOrder.Count - 1)
			{
				Console.WriteLine("[Server] All answers submitted, Time to pick hes answer");

				ServerSend.StartPickingPhase(PlayerSubmissions);
			}
		}

		public static void OnWinnerSubmitted(int _cardCzarId, List<string> _winnerCards)
		{
			Console.WriteLine("On Winner Submitted\n");

			foreach (int key in PlayerHands.Keys) {
				Console.WriteLine($"\nPlayer {key}'s Hand");
				foreach (string card in PlayerHands[key])
				{
					Console.WriteLine(card);
				}
			}

			Console.WriteLine($"\nWinners Hand");
			foreach (string card in _winnerCards) {
				Console.WriteLine(card);
			}

			foreach (int key in PlayerHands.Keys)
			{
				bool match = true;
				foreach (string card in _winnerCards)
				{
					if (PlayerHands[key].Contains(card) == false) {
						match = false;
					}
				}

				if (match)
				{
					Console.WriteLine($"We have a winner => {key} {Server.Clients[key].Username}\n");
					Server.Clients[key].Points++;

					int cardsNeeded = currentQuestion.cardsNeeded;

					QuestionCard card = UpdateCurrentQuestion();
					StartNewRound(key, cardsNeeded);

					return;
				}
			}
		}

		public static void OnNoPlayersLeftInGame()
		{
			Console.WriteLine("There are no players left in the game");
		}

		public static void PlayerJoining(int _Id, string _username)
		{
			// If this player is NOT CURRENTLY in the game
			if (TurnOrder.Contains(_Id) == false)
			{
				Console.Write($"[Server] Welcome '{_username}' has joined the server!\n");
				Console.Write($"[Server] {_username} has claimed the ID of [{_Id}]\n");

				// Add the new player to the turn queue
				TurnOrder.Add(_Id);
			}
			else
			{
				// This player is already in the game!
				Console.WriteLine($"ERROR: The player with ID: {_Id} is already in the turn order!!");
			}
		}

		public static void PlayerDisconnecting(int _Id, string _username)
		{
			// If this player is NOT CURRENTLY in the game
			if (TurnOrder.Contains(_Id))
			{
				Console.Write($"[Server] {_username} has disconnect\n\n");

				// Add the new player to the turn queue
				TurnOrder.Remove(_Id);

				if (TurnOrder.Count == 0) {
					OnNoPlayersLeftInGame();
				}
			}
			else
			{
				// This player is already in the game!
				Console.WriteLine($"ERROR: The player is not in the turn queue!!");
			}
		}

		public static void LoadCards()
		{
			foreach (Deck deck in DeckLoader.DeckList)
			{
				// TODO: Check if deck is active

				foreach (AnswerCard answerDeck in deck.answerCards) {
					AnswerDeck.Add(answerDeck);
				}

				foreach (QuestionCard questionDeck in deck.questionCard) {
					QuestionDeck.Add(questionDeck);
				}
			}
		}
	}
}
