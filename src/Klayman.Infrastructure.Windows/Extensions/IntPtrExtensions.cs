namespace Klayman.Infrastructure.Windows.Extensions;

public static class IntPtrExtensions
{
    public static short LoWord(this IntPtr value)
    {
        return unchecked((short)value.ToInt32());
    }

    public static short HiWord(this IntPtr value)
    {
        return unchecked((short)(value.ToInt32() >> 16));
    }
}