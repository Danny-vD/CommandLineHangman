using System;
using System.Collections.Generic;
using System.Linq;

namespace Hangman
{
	public static class GameManager
	{
		private static string solutionWord = "Ruitensproeiervloeistofreservoir"; // Great start word
		private static List<char> guessedLetters = null;

		// Byte, because allowing over 255 mistakes in a game of Hangman seems very unlikely considering the latin alphabet only has 26 characters
		private static byte errorCount = 0;

		private static string currentWord = "";

		public static void SetupNewGame(string word)
		{
			errorCount     = 0;
			guessedLetters = new List<char>(ErrorMessageHandler.MaxErrorCount); // Set capacity to prevent resizing after every guess

			solutionWord = word;

			for (;;) // Keep asking for new words as long as the user does not meet all criteria
			{
				if (!HasValidSolution())
				{
					Console.WriteLine("That is not a valid word/phrase!");
					RequestNewWord();
					continue;
				}

				Console.WriteLine($"Are you sure that you want to play with \"{solutionWord}\"? (Y/N)");

				if (Program.UserAnsweredYes())
				{
					break;
				}

				RequestNewWord();
			}

			SetCurrentWord();

			// Clear the console so that players cannot cheat by scrolling up
			Console.Clear();
		}

		public static void SetupNewGame()
		{
			errorCount     = 0;
			guessedLetters = new List<char>(ErrorMessageHandler.MaxErrorCount); // Set capacity to prevent resizing after every guess

			for (;;) // Keep asking for new words as long as the user does not meet all criteria
			{
				RequestNewWord();

				if (!HasValidSolution())
				{
					Console.WriteLine("That is not a valid word/phrase!");
					continue;
				}

				Console.WriteLine($"Are you sure that you want to play with \"{solutionWord}\"? (Y/N)");

				if (Program.UserAnsweredYes())
				{
					break;
				}
			}

			SetCurrentWord();

			// Clear the console so that players cannot cheat by scrolling up
			Console.Clear();
		}

		/// <summary>
		/// Guess a specific phrase or letter
		/// </summary>
		/// <returns>Whether the game ended (either win or loss)</returns>
		public static bool Guess(string guess)
		{
			// Do not allow an empty guess
			if (string.IsNullOrEmpty(guess.Trim()) || guess.Length == 0)
			{
				return false;
			}

			// Allow the user to guess whole words/phrases
			if (solutionWord.ToLower().Equals(guess.ToLower()))
			{
				EndGame();
				return true;
			}

			// If the correct word was not guessed, take the first letter.
			char letter = guess.ToLower()[0];

			// Ignore punctuation guesses
			if (char.IsPunctuation(letter))
			{
				// If the guess is only punctuation, then ignore it
				if (guess.All(char.IsPunctuation))
				{
					return false;
				}
				
				// When guessing a word, count it as an error if it was wrong, to prevent free word/phrase guesses
				if (guess.Trim().Length > 1)
				{
					return WrongGuess();
				}

				return false;
			}

			if (!guessedLetters.Contains(letter))
			{
				// There's a "Free" word/phrase guess if the word starts with an unguessed letter.. that's ok
				guessedLetters.Add(letter);
			}
			else
			{
				// When guessing a word, count it as an error if it was wrong, to prevent free word/phrase guesses
				if (guess.Trim().Length > 1)
				{
					return WrongGuess();
				}

				// If the letter has already been guessed, disregard it.
				return false;
			}

			// Check if the solution contains the guessed letter
			return solutionWord.ToLower().Contains(letter.ToString()) ? CorrectGuess() : WrongGuess();
		}

		private static void RequestNewWord()
		{
			Console.WriteLine("Please enter a word/phrase to play with.");
			solutionWord = Console.ReadLine();
		}

		private static bool HasValidSolution()
		{
			// Do not allow only whitespace or all punctuation solutions
			return !string.IsNullOrEmpty(solutionWord) && !solutionWord.All(char.IsPunctuation) && !solutionWord.Trim().Equals("");
		}

		private static void SetCurrentWord()
		{
			// Replace all characters (except punctuation and spaces) with an underscore e.g. "_____"
			char solutionLetter = solutionWord[0];

			// If the word starts with punctuation, display that instead of the underscore
			currentWord = char.IsPunctuation(solutionLetter) ? solutionLetter.ToString() : "_";

			for (int i = 1; i < solutionWord.Length; i++)
			{
				solutionLetter = solutionWord[i];

				if (solutionLetter.Equals(' '))
				{
					currentWord += " ";
				}
				else
				{
					// Either the punctuation or an underscore representing a letter
					currentWord += char.IsPunctuation(solutionLetter) ? solutionLetter.ToString() : "_";
				}
			}
		}

		private static void EndGame()
		{
			string outcome = errorCount < ErrorMessageHandler.MaxErrorCount ? "You win!" : "You lose!";

			Console.WriteLine($"\n{outcome}\nThe answer was:\n{solutionWord}");
		}

		/// <returns>Are all the letters guessed?</returns>
		private static bool CorrectGuess()
		{
			currentWord = string.Empty;
			string lowerSolution = solutionWord.ToLower();

			for (int i = 0; i < solutionWord.Length; i++)
			{
				char letter = lowerSolution[i];
				
				if (letter.Equals(' ') || char.IsPunctuation(letter) || guessedLetters.Contains(letter))
				{
					// Use the index from the solutionword to retain any capital letters
					currentWord += solutionWord[i];
				}
				else
				{
					currentWord += "_";
				}
			}

			// Did you guess all the letters?
			if (currentWord.ToLower().Equals(lowerSolution))
			{
				DisplayCurrentWord();
				
				EndGame();
				return true;
			}

			DisplayGuesses();
			DisplayCurrentWord();

			return false;
		}

		/// <returns>Did we reach the error limit?</returns>
		private static bool WrongGuess()
		{
			++errorCount;
			DisplayError();

			// If we reached the limit of allowed mistakes, end the game
			if (errorCount == ErrorMessageHandler.MaxErrorCount)
			{
				EndGame();
				return true;
			}

			DisplayGuesses();
			DisplayCurrentWord();

			return false;
		}

		private static void DisplayError()
		{
			Console.WriteLine(ErrorMessageHandler.GetErrorMessage(errorCount));
		}

		private static void DisplayGuesses()
		{
			string guesses = string.Empty;

			guesses += guessedLetters.FirstOrDefault();

			for (int i = 1; i < guessedLetters.Count; i++)
			{
				guesses += ", " + guessedLetters[i];
			}

			Console.WriteLine(guesses);
		}

		private static void DisplayCurrentWord()
		{
			// Display the currentword (which has underscores in place of unguessed letters) and place spaces between characters e.g. "_ _ _ _ _"
			string displayedCurrent = currentWord[0].ToString();

			for (int i = 1; i < solutionWord.Length; i++)
			{
				char letter = currentWord[i];

				if (char.IsPunctuation(letter))
				{
					// If it matches then we know it's punctuation (and not an underscore respresenting a letter), so no space
					if (letter.Equals(solutionWord[i]))
					{
						displayedCurrent += letter;
						continue;
					}
				}

				displayedCurrent += " " + letter;
			}
			
			Console.WriteLine(Environment.NewLine + displayedCurrent + Environment.NewLine);
		}
	}
}