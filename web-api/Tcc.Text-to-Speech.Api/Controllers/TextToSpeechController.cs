using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Tcc.Text_to_Speech.Application.Commands;
using Tcc.Text_to_Speech.Application.Interfaces;

namespace Tcc.Text_to_Speech.Api.Controllers
{
	[ApiController]
	[Route("api/v1/texttospeech")]
	public class TextToSpeechController : Controller
	{
		private readonly ITextToSpeechHandler _textToSpeechHandler;

		public TextToSpeechController(ITextToSpeechHandler textToSpeechHandler)
		{
			_textToSpeechHandler = textToSpeechHandler;
		}

		[HttpPost]
		public async Task<IActionResult> TextToSpeech([FromBody] TextToSpeechCommand command)
		{
			return Ok(await _textToSpeechHandler.Handle(command));
		}
	}
}
