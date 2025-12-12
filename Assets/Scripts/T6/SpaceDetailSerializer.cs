using UnityEngine;
using System;

public static class T6SpaceDetailSerializer
{
    public static string ToJson(T6SpaceDetail detail)
    {
        return JsonUtility.ToJson(detail, true);
    }

    public static T6SpaceDetail FromJson(string json)
    {
        return JsonUtility.FromJson<T6SpaceDetail>(json);
    }
}
