namespace Tcc.Text_to_Speech.Application.Commands
{
	public sealed class TextToSpeechCommandResponse
	{
		public TextToSpeechCommandResponse(string base64Fila)
		{
			Base64Fila = base64Fila;
		}

		public string Base64Fila { get; set; }		
	}
}
