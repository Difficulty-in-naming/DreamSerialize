using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit,Size = 4)]
public unsafe struct IntUnion
{
    [FieldOffset(0)]
    public fixed byte bytes[4];
    [FieldOffset(0)]
    public int Value;
}
[StructLayout(LayoutKind.Explicit)]
public struct UIntUnion
{
    [FieldOffset(0)]
    public byte byte0;
    [FieldOffset(1)]
    public byte byte1;
    [FieldOffset(2)]
    public byte byte2;
    [FieldOffset(3)]
    public byte byte3;
    [FieldOffset(0)]
    public uint Value;
}
[StructLayout(LayoutKind.Explicit)]
public struct ShortUnion
{
    [FieldOffset(0)]
    public byte byte0;
    [FieldOffset(1)]
    public byte byte1;
    [FieldOffset(0)]
    public short Value;
}

[StructLayout(LayoutKind.Explicit)]
public struct UShortUnion
{
    [FieldOffset(0)]
    public byte byte0;
    [FieldOffset(1)]
    public byte byte1;
    [FieldOffset(0)]
    public ushort Value;
}

[StructLayout(LayoutKind.Explicit)]
public struct LongUnion
{
    [FieldOffset(0)]
    public byte byte0;
    [FieldOffset(1)]
    public byte byte1;
    [FieldOffset(2)]
    public byte byte2;
    [FieldOffset(3)]
    public byte byte3;
    [FieldOffset(4)]
    public byte byte4;
    [FieldOffset(5)]
    public byte byte5;
    [FieldOffset(6)]
    public byte byte6;
    [FieldOffset(7)]
    public byte byte7;
    [FieldOffset(0)]
    public long Value;
}
[StructLayout(LayoutKind.Explicit)]
public struct ULongUnion
{
    [FieldOffset(0)]
    public byte byte0;
    [FieldOffset(1)]
    public byte byte1;
    [FieldOffset(2)]
    public byte byte2;
    [FieldOffset(3)]
    public byte byte3;
    [FieldOffset(4)]
    public byte byte4;
    [FieldOffset(5)]
    public byte byte5;
    [FieldOffset(6)]
    public byte byte6;
    [FieldOffset(7)]
    public byte byte7;
    [FieldOffset(0)]
    public ulong Value;
}
[StructLayout(LayoutKind.Explicit)]
public struct FloatUnion
{
    [FieldOffset(0)]
    public byte byte0;
    [FieldOffset(1)]
    public byte byte1;
    [FieldOffset(2)]
    public byte byte2;
    [FieldOffset(3)]
    public byte byte3;
    [FieldOffset(0)]
    public float Value;
}

[StructLayout(LayoutKind.Explicit)]
public struct DoubleUnion
{
    [FieldOffset(0)]
    public byte byte0;
    [FieldOffset(1)]
    public byte byte1;
    [FieldOffset(2)]
    public byte byte2;
    [FieldOffset(3)]
    public byte byte3;
    [FieldOffset(4)]
    public byte byte4;
    [FieldOffset(5)]
    public byte byte5;
    [FieldOffset(6)]
    public byte byte6;
    [FieldOffset(7)]
    public byte byte7;
    [FieldOffset(0)]
    public double Value;
}

[StructLayout(LayoutKind.Explicit)]
public struct DecimalUnion
{
    [FieldOffset(0)]
    public byte byte0;
    [FieldOffset(1)]
    public byte byte1;
    [FieldOffset(2)]
    public byte byte2;
    [FieldOffset(3)]
    public byte byte3;
    [FieldOffset(4)]
    public byte byte4;
    [FieldOffset(5)]
    public byte byte5;
    [FieldOffset(6)]
    public byte byte6;
    [FieldOffset(7)]
    public byte byte7;
    [FieldOffset(8)]
    public byte byte8;
    [FieldOffset(9)]
    public byte byte9;
    [FieldOffset(10)]
    public byte byte10;
    [FieldOffset(11)]
    public byte byte11;
    [FieldOffset(12)]
    public byte byte12;
    [FieldOffset(13)]
    public byte byte13;
    [FieldOffset(14)]
    public byte byte14;
    [FieldOffset(15)]
    public byte byte15;
    [FieldOffset(0)]
    public decimal Value;
}