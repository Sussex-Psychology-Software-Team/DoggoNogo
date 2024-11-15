using System.Linq;
using UnityEngine;

public static class StringUtils
{
    private static readonly System.Random Random = new();
    private const string Characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public static string GenerateRandomId(int size)
    {
        return new string(Enumerable.Repeat(Characters, size)
            .Select(s => s[Random.Next(s.Length)])
            .ToArray());
    }
}