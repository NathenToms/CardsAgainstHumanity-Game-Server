using System.Collections.Generic;

[System.Serializable]
public class Deck
{
	public string Key;
	public List<QuestionCard> questionCard = new List<QuestionCard>();
	public List<AnswerCard> answerCards = new List<AnswerCard>();

	public Deck(string key)
	{
		Key = key;
	}
}
