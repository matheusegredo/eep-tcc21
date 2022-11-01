using LiteDB;

namespace Tcc.Text_to_Speech.Domain
{
    public class AudioRepository
    {
        private readonly ILiteCollection<Audio> _audios;

        public AudioRepository(ILiteDatabase db)
        {
            _audios = db.GetCollection<Audio>(nameof(Audio));
            _audios.EnsureIndex(p => p.Key, true);
        }

        public Audio Get(string sentence)
        {
            return _audios.FindById(sentence);
        }

        public void Insert(Audio audio)
        {
            _audios.Insert(audio);
        }
    }
}
