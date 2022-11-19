using Microsoft.CognitiveServices.Speech;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tcc.Text_to_Speech.Application.Interfaces;
using Tcc.Text_to_Speech.Domain;
using Tcc.Text_to_Speech.Infrastructure;

namespace Tcc.Text_to_Speech.Application.Commands
{
	public sealed class TextToSpeechCommandHandler : ITextToSpeechHandler
	{
		private readonly AudioRepository _repositoy;

		public TextToSpeechCommandHandler(AudioRepository repository)
		{
            _repositoy = repository;
		}

		public async Task<TextToSpeechCommandResponse> Handle(TextToSpeechCommand command)
		{
			var config = SpeechConfig.FromSubscription("8e11fb332a75441ba71e18eda1e66cb2", "brazilsouth");

			config.SpeechSynthesisLanguage = "pt-BR";

            config.SpeechSynthesisVoiceName = "Microsoft Server Speech Text to Speech Voice (pt-BR, AntonioNeural)";

            using var synthesizer = new SpeechSynthesizer(config, null);

            var result = await synthesizer.SpeakTextAsync(command.Text);

			return new TextToSpeechCommandResponse(result.AudioData);
		}

		private Audio FindCachedData(string key)
		{
			return _repositoy.Get(BuildKey(key));
		}

		private void InsertData(string key, byte[] data)
		{
			_repositoy.Insert(new Audio { Key = BuildKey(key), Data = data });
		}

		private static string BuildKey(string sentence)
		{
            var s = sentence.ToLowerInvariant().Replace(" ", string.Empty);

			using MD5 md5 = MD5.Create();

			byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(s));
			return new Guid(hash).ToString();
		}	 
    }
}
