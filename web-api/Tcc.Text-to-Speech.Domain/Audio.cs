using LiteDB;

namespace Tcc.Text_to_Speech.Domain
{
    public record Audio
    {
        [BsonId]
        public string Key { get; init; }

        public byte[] Data { get; init; }
    }
}
