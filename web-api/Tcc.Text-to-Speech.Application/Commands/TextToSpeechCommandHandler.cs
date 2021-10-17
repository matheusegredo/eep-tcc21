using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System;
using System.IO;
using System.Threading.Tasks;
using Tcc.Text_to_Speech.Application.Interfaces;

namespace Tcc.Text_to_Speech.Application.Commands
{
	public sealed class TextToSpeechCommandHandler : ITextToSpeechHandler
	{
		public async Task<TextToSpeechCommandResponse> Handle(TextToSpeechCommand command)
		{
			var config = SpeechConfig.FromSubscription("c597504049ea4fed872b248df66e3bcb", "southcentralus");

			config.SpeechSynthesisLanguage = command.SpeechSynthesisLanguage;
			config.SpeechSynthesisVoiceName = command.SpeechSynthesisVoiceName;

			using var synthesizer = new SpeechSynthesizer(config, null);

			var result = await synthesizer.SpeakTextAsync(command.Text);
			var base64Audio = Convert.ToBase64String(result.AudioData);			

			return new TextToSpeechCommandResponse(base64Audio);
		}
	}
}
