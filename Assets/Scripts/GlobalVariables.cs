using UnityEngine;
using System.Collections;

public static class GlobalVariables
{
    private static bool thomassong;

    public static bool Thomassong
    {
        get
        {
            return thomassong;
        }
        set
        {
            thomassong = value;
        }
    }
}
