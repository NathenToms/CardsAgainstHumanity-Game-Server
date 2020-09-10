using System;

namespace CardsAgainstHumanityServer
{
	class Program
	{
		public static bool running = true;

		static void Main(string[] args)
		{
			DeckLoader.LoadCards();
			GameManager.LoadCards();

			Server.StartServer(6, 25565);

			while (running)
			{
				ThreadManager.UpdateMain();
			}
		}
	}
}
