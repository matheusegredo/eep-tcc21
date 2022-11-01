using System.Text;
using System.Threading.Channels;

namespace Tcc.Text_to_Speech.Infrastructure
{
    public class Wave
    {
        public int length;
        public short channels;
        public int samplerate;
        public int DataLength;
        public short BitsPerSample;

        private void WaveHeaderIN(byte[] data)
        {
            using var fs = new MemoryStream(data);
            BinaryReader br = new(fs);
            length = (int)fs.Length - 8;
            fs.Position = 22;
            channels = br.ReadInt16();
            fs.Position = 24;
            samplerate = br.ReadInt32();
            fs.Position = 34;

            BitsPerSample = br.ReadInt16();
            DataLength = (int)fs.Length - 44;
            br.Close();
            fs.Close();
        }

        private void WaveHeaderOUT(Stream fs)
        {
            BinaryWriter bw = new(fs, Encoding.UTF8, true);
            fs.Position = 0;
            bw.Write(new char[4] { 'R', 'I', 'F', 'F' });

            bw.Write(length);

            bw.Write(new char[8] { 'W', 'A', 'V', 'E', 'f', 'm', 't', ' ' });

            bw.Write((int)16);

            bw.Write((short)1);
            bw.Write(channels);

            bw.Write(samplerate);

            bw.Write((int)(samplerate * ((BitsPerSample * channels) / 8)));

            bw.Write((short)((BitsPerSample * channels) / 8));

            bw.Write(BitsPerSample);

            bw.Write(new char[4] { 'd', 'a', 't', 'a' });
            bw.Write(DataLength);
            bw.Close();
        }

        public byte[] Merge(List<byte[]> files)
        {
            var wa_IN = new Wave();

            var wa_out = new Wave
            {
                DataLength = 0,
                length = 0
            };


            //Gather header data
            foreach (var s in files)
            {
                wa_IN.WaveHeaderIN(s);
                wa_out.DataLength += wa_IN.DataLength;
                wa_out.length += wa_IN.length;
            }

            var outStream = new MemoryStream();

            //Recontruct new header
            wa_out.BitsPerSample = wa_IN.BitsPerSample;
            wa_out.channels = wa_IN.channels;
            wa_out.samplerate = wa_IN.samplerate;
            wa_out.WaveHeaderOUT(outStream);

            foreach (var f in files)
            {
                using var fs = new MemoryStream(f);

                byte[] arrfile = new byte[fs.Length - 44];
                fs.Position = 44;
                fs.Read(arrfile, 0, arrfile.Length);
                fs.Close();

                BinaryWriter bw = new(outStream, Encoding.UTF8, leaveOpen: true);
                bw.Write(arrfile);
                bw.Close();
            }
            
            outStream.Close();
            var result = outStream.ToArray();
            outStream.Dispose();

            return result;
        }
    }
}