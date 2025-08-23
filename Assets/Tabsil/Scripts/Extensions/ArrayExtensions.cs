using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class ArrayExtensions
{
    private static System.Random random = new System.Random();

    public static T GetRandom<T>(this T[] array)
    {
        return array[random.Next(array.Length)]; 
    }
}
