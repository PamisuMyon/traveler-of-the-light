using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Utils
{

    public static string ToString(Array array)
    {
        if (array == null)
            return "null";
        else
            return "{" + string.Join(", ", array.Cast<object>().Select(o => o.ToString()).ToArray()) + "}";
    }

    public static string ToString<T>(List<T> list)
    {
        if (list == null)
            return "null";
        else
            return "{" + string.Join(", ", list.Cast<object>().Select(o => o.ToString()).ToArray()) + "}";
    }

    public static string ToString<TKey, TValue>(Dictionary<TKey, TValue> dict)
    {
        if (dict == null)
            return "null";
        else
            return "{" + string.Join(", ", dict.Select(kvp => kvp.Key.ToString() + ":" + kvp.Value.ToString()).ToArray()) + "}";
    }

    public static void AddUnique<T>(this List<T> list, T item)
    {
        if (!list.Contains(item))
            list.Add(item);
    }

    public static void AddRangeUnique<T>(this List<T> list, IEnumerable<T> collection)
    {
        foreach (var item in collection)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
            }
        }
    }

    public static int RandomSigned(int start, int end)
    {
        int sign = Random.Range(0, 2) > 0 ? -1 : 1;
        var num = sign * Random.Range(start, end);
        return num;
    }

    public static float RandomSigned(float start, float end)
    {
        int sign = Random.Range(0, 2) > 0 ? -1 : 1;
        var num = sign * Random.Range(start, end);
        return num;
    }

    public static int RandomSign()
    {
        return Random.Range(0, 2) > 0 ? -1 : 1;
    }

    public static void PlayRandomPitch(AudioSource source)
    {
        source.pitch = Random.Range(.8f, 1.2f);
        source.Play();
    }

    public static void PlayRandomPitch(AudioSource source, AudioClip clip)
    {
        source.pitch = Random.Range(.8f, 1.2f);
        source.clip = clip;
        source.Play();
    }

}

public class FloatComparer : IComparer<float>
{
    public int Compare(float x, float y)
    {
        return x.CompareTo(y);
    }
}