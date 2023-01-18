namespace Hangman
{
	public static class ErrorMessageHandler
	{
		private const string placeholderFail = "==========\n" +
											   "\\|/     |\n" +
											   " |      |\n" +
											   " |      ()\n" +
											   " |     /||\\\n" +
											   " |      ||\n" +
											   " |      /\\\n" +
											   " |     \n" +
											   "/|\\\n";

		private static string[] errorMessages;

		public static byte MaxErrorCount => (byte) errorMessages.Length;

		static ErrorMessageHandler()
		{
			FillErrorMessages();
		}

		public static string GetErrorMessage(byte errorCount)
		{
			// If there are no drawings (file empty) then use my own very cool drawing instead
			return errorMessages.Length == 0 ? placeholderFail : errorMessages[errorCount - 1];
		}

		private static void FillErrorMessages()
		{
			errorMessages = ErrorParser.GetErrorMessages();
		}
	}
}