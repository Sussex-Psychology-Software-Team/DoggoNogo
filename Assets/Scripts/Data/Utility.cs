using System;

public static class Utility
{
    // Generates a random ID
    public static string GenerateRandomId(int size) {
        var rand = new System.Random();
        const string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var randId = new char[size];
        for (var i = 0; i < size; i++)
        {
            randId[i] = characters[rand.Next(characters.Length)];
        }
        return new string(randId);
    }
}
