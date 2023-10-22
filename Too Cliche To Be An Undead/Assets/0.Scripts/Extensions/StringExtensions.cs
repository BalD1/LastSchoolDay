using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringExtensions
{
    public static bool NotNullOrEmpty(this string s)
        => !(s == null) && !(s.Length == 0);
}
