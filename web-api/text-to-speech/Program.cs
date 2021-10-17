using Microsoft.CognitiveServices.Speech;
using System;
using System.Threading.Tasks;

namespace text_to_speech
{
	class Program
	{
		static void Main(string[] args)
		{
			SynthesizeAudioAsync().ConfigureAwait(false).GetAwaiter().GetResult();
		}

		static async Task SynthesizeAudioAsync()
		{
			var config = SpeechConfig.FromSubscription("c597504049ea4fed872b248df66e3bcb", "southcentralus");
				config.SpeechSynthesisLanguage = "pt-BR";
				config.SpeechSynthesisVoiceName = "Microsoft Server Speech Text to Speech Voice (pt-BR, AntonioNeural)";
			using var synthesizer = new SpeechSynthesizer(config);
			await synthesizer.SpeakTextAsync("Não sou uma vagabunda garota");			
		}
	}
}
