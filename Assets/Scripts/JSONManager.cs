using System.IO;
using UnityEngine;

public static class JSONManager
{
    public static void SerializeData(User data, string path)
    {
        string jsonDataString = JsonUtility.ToJson(data, false);

        File.WriteAllText(path, jsonDataString);
    }

    public static T DeserealizeData<T>(string path)
    {
        string loadedJSONDataString = File.ReadAllText(path);

        return JsonUtility.FromJson<T>(loadedJSONDataString);
    }
}
