using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CardsAgainstHumanityServer
{
	public static class DeckLoader
	{
		public static List<Deck> DeckList = new List<Deck>();

		public static void LoadCards()
		{
			string path = Directory.GetCurrentDirectory();
			string newPath = Path.GetFullPath(Path.Combine(path, @"..\..\Cards.txt"));

			Console.WriteLine($"Loading Card..\nPath {newPath}\n");

			StreamReader reader = new StreamReader(newPath);

			while (true)
			{
				string data = reader.ReadLine();

				string[] card = data.Split('	');

				// If we have the right amount of data
				if (card.Length == 5)
				{
					string deckID = card[2];

					bool found = false;

					foreach (Deck deck in DeckList)
					{
						if (deck.Key == deckID)
						{
							found = true;

							// Questions
							if (card[0].ToUpper() == "BLACK")
							{
								deck.questionCard.Add(new QuestionCard(card[1], card[2], card[3], card[4]));
							}

							// Answers
							if (card[0].ToUpper() == "WHITE")
							{
								deck.answerCards.Add(new AnswerCard(card[1], card[2]));
							}
						}
					}

					if (found == false)
					{
						Deck deck = new Deck(deckID);

						// Questions
						if (card[0].ToUpper() == "BLACK")
						{
							deck.questionCard.Add(new QuestionCard(card[1], card[2], card[3], card[4]));
						}

						// Answers
						if (card[0].ToUpper() == "WHITE")
						{
							deck.answerCards.Add(new AnswerCard(card[1], card[2]));
						}

						DeckList.Add(deck);
					}
				}

				// Break
				if (reader.EndOfStream) break;
			}

			reader.Close();
		}
	}
}
