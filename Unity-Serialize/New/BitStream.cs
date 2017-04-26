namespace DreamSerialize.New
{
    public class BitStream
    {
        public byte[] Bytes;
        public int Offset;

        public BitStream(byte[] bytes, int offset)
        {
            Bytes = bytes;
            Offset = offset;
        }
    }
}
