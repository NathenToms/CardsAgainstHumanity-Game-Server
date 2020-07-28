[System.Serializable]
public class AnswerCard
{
	public string text;
	public string deck;

	public AnswerCard(string text, string deck)
	{
		this.text = text;
		this.deck = deck;
	}
}