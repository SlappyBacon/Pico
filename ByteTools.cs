namespace Pico.ByteTools;

public static class ByteTools
{
    public static bool IsBitSet(ref byte b, int bitIndex)
    {
        byte mask = (byte)(1 << bitIndex);
        return (b & mask) != 0;
    }
    public static void SetBitHigh(ref byte b, int bitIndex)
    {
        byte mask = (byte)(1 << bitIndex);
        b |= mask;
    }
    public static void SetBitLow(ref byte b, int bitIndex)
    {
        byte mask = (byte)(1 << bitIndex);
        b &= mask;
    }
    public static void FlipBit(ref byte b, int bitIndex)
    {
        byte mask = (byte)(1 << bitIndex);
        b ^= mask;
    }
    public static void FlipRandomBit(ref byte b)
    {
        int bitIndex = Random.Shared.Next(0, 8);
        FlipBit(ref b, bitIndex);
    }
    public static void FlipRandomBits(ref byte b, int count)
    {
        for (int i = 0; i < count; i++)
        {
            FlipRandomBit(ref b);
        }
    }
    public static void FlipRandomBit(ref byte[] bytes)
    {
        int byteIndex = Random.Shared.Next(0, bytes.Length);
        FlipRandomBit(ref bytes[byteIndex]);
    }
    public static void FlipRandomBits(ref byte[] bytes, int count)
    {
        for (int i = 0; i < count; i++)
        {
            FlipRandomBit(ref bytes);
        }
    }
}
