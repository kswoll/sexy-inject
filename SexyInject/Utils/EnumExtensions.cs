using System;

namespace SexyInject.Utils
{
    internal static class EnumExtensions
    {
        public static bool HasFlag<T>(this T flags, T flag) where T : struct
        {
            return ((Enum)(object)flags).HasFlag((Enum)(object)flag);
        }
    }
}