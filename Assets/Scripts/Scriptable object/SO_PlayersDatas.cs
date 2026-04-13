using UnityEngine;

public enum PlayerClass { Hacker, Infiltrateur }

[CreateAssetMenu(fileName = "SO_PlayersDatas", menuName = "Scriptable Objects/SO_PlayersDatas")]
public class SO_PlayersDatas : ScriptableObject
{
    public string Name;
    public int Score;
    public int Level;
    public PlayerClass Class;

    private SaveController saveSystem;

    /// <summary>Charge les données depuis le fichier de sauvegarde.</summary>
    public void LoadDatas()
    {
        CheckSaveSystem();
        PlayerDatas datas = saveSystem.Load();
        Name  = datas.Name;
        Score = datas.Score;
        Level = datas.Level;
        Class = (PlayerClass)datas.Class;
    }

    /// <summary>Sauvegarde les données dans le fichier de sauvegarde.</summary>
    public void SaveDatas()
    {
        CheckSaveSystem();
        PlayerDatas datas = new PlayerDatas
        {
            Name  = Name,
            Score = Score,
            Level = Level,
            Class = (int)Class
        };
        saveSystem.Save(datas);
    }

    private void CheckSaveSystem()
    {
        if (saveSystem == null)
            saveSystem = new SaveController();
    }
}