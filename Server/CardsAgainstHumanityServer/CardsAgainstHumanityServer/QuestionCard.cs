[System.Serializable]
public class QuestionCard
{
	public string text;
	public string deck;
	public int cardsToDraw;
	public int cardsNeeded;

	public QuestionCard(string text, string deck, string cardsToDraw, string cardsNeeded)
	{
		this.text = text;
		this.deck = deck;
		this.cardsToDraw = int.Parse(cardsToDraw);
		this.cardsNeeded = int.Parse(cardsNeeded);
	}
}
