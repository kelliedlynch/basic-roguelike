using System;

namespace Roguelike.Utility;

public static class ExtensionMethods
{
    public static T Next<T>(this T src) where T : Enum
    {
        // Returns the next value in an Enum
        T[] Arr = (T[])Enum.GetValues(src.GetType());
        int j = Array.IndexOf<T>(Arr, src) + 1;
        return (Arr.Length==j) ? Arr[0] : Arr[j];            
    }
}