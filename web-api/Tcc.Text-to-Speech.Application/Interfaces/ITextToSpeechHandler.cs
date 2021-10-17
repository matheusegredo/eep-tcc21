using System.Threading.Tasks;
using Tcc.Text_to_Speech.Application.Commands;

namespace Tcc.Text_to_Speech.Application.Interfaces
{
	public interface ITextToSpeechHandler
	{
		Task<TextToSpeechCommandResponse> Handle(TextToSpeechCommand command);
	}
}
