namespace Tcc.Text_to_Speech.Application.Commands
{
	public sealed class TextToSpeechCommandResponse
	{
		public TextToSpeechCommandResponse(byte[] content)
		{
            Content = content;
		}

		public byte[] Content { get; set; }		
	}
}
