using UnityEngine;
using System.IO;

[System.Serializable]
public class PlayerDatas
{
    public string Name;
    public int Score;
    public int Level;
    public int Class;
}

public class SaveController
{
    public string GetPath()
    {
        return Application.persistentDataPath + "/save.json";
    }

    public void Save(PlayerDatas datas)
    {
        string json = JsonUtility.ToJson(datas, prettyPrint: true);
        File.WriteAllText(GetPath(), json);
    }

    public PlayerDatas Load()
    {
        if (File.Exists(GetPath()))
        {
            string json = File.ReadAllText(GetPath());
            return JsonUtility.FromJson<PlayerDatas>(json);
        }
        return new PlayerDatas();
    }
}