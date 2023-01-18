using System;
using System.IO;

namespace Hangman
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			Console.Title = "Hangman Game";

			if (!VerifyIntegrity())
			{
				try
				{
					throw new FileLoadException($"The {ErrorParser.FILE_NAME} must be in the same folder as the .exe!");
				}
				finally
				{
					Console.ReadKey(true);
				}
			}
			
			if (args.Length > 0)
			{
				// Use the first argument as starter word
				GameManager.SetupNewGame(args[0]);
			}
			else
			{
				GameManager.SetupNewGame();
			}

			for (;;)
			{
				while (!Step())
				{
				}

				if (PlayAgain())
				{
					GameManager.SetupNewGame();
					continue;
				}

				break;
			}
		}

		public static bool UserAnsweredYes()
		{
			string answer = Console.ReadLine();

			// If empty: treat as 'no' and contains 'y': treat as 'yes'
			return !string.IsNullOrEmpty(answer) && answer.ToLower().Contains("y");
		}

		private static bool Step()
		{
			return GameManager.Guess(Console.ReadLine());
		}

		private static bool PlayAgain()
		{
			Console.WriteLine("\nPlay again? (Y/N)");

			return UserAnsweredYes();
		}

		private static bool VerifyIntegrity()
		{
			// The game requires the .txt file to be present
			return File.Exists(ErrorParser.FILE_NAME);
		}
	}
}