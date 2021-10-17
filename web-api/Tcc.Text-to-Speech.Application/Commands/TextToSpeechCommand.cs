namespace Tcc.Text_to_Speech.Application.Commands
{
	public sealed class TextToSpeechCommand
	{
		public string SpeechSynthesisLanguage { get; set; } = "pt-BR";

		public string SpeechSynthesisVoiceName { get; set; } = "Microsoft Server Speech Text to Speech Voice (pt-BR, AntonioNeural)";

		public string Text { get; set; }

		public string RequestUri { get; set; }
	}
}
