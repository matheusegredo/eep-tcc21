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

			config.SpeechSynthesisLanguage = command.SpeechSynthesisLanguage;
			config.SpeechSynthesisVoiceName = command.SpeechSynthesisVoiceName;

			using var synthesizer = new SpeechSynthesizer(config, null);

			var sentences = command.Text.Split(' ');

			var files = new List<byte[]>();            	

            foreach (var sentence in sentences)
			{
                using var stream = new MemoryStream();
                //var cachedData = FindCachedData(sentence);

                //if (cachedData is not null)
                //{
                //	Console.WriteLine($"{sentence} retrived from cached data");
                //  stream.Write(cachedData.Data);
                //	continue;
                //}

                //Console.WriteLine($"Getting {sentence} from azure");
                var result = await synthesizer.SpeakTextAsync(command.Text);

				stream.Write(result.AudioData);
				//InsertData(sentence, result.AudioData);
				files.Add(stream.ToArray());
            }

			var wave = new Wave();

			return new TextToSpeechCommandResponse(wave.Merge(files));
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
