using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class EnumUtil
{
    public static IEnumerable<T> GetValues<T>()
    {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }

    public static string GetName(this Enum e)
    {
        return Enum.GetName(e.GetType(), e);
    }
}
