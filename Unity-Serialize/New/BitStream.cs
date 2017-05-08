namespace DreamSerialize.New
{
    public class BitStream
    {
        public byte[] Bytes;
        public int Offset;
        public int Head = 0;
        public BitStream(byte[] bytes, int offset)
        {
            Bytes = bytes;
            Offset = offset;
        }
    }
}
