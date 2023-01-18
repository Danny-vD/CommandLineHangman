using System.IO;

namespace Hangman
{
	public static class ErrorParser
	{
		public const string FILE_NAME = "ErrorMessages.txt";
		
		public static string[] GetErrorMessages()
		{
			string text = File.ReadAllText(FILE_NAME);
			char seperator = text[0]; // Use first character in the text as seperator
			
			// Split the text at the seperator, ignoring the first character
			return text.Remove(0, 1).Split(seperator);
		}
	}
}