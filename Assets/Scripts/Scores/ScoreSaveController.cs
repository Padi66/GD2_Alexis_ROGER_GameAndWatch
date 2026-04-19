using UnityEngine;
using System.IO;

/// <summary>Gère la lecture et l'écriture des scores dans un fichier JSON par pseudo.</summary>
public class ScoreSaveController
{
    private const string FilePrefix = "/scores_";
    private const string FileExt    = ".json";

    /// <summary>Retourne le chemin du fichier de score pour un pseudo donné.</summary>
    public string GetPath(string pseudo)
    {
        string safePseudo = string.IsNullOrWhiteSpace(pseudo) ? "unknown" : pseudo.Trim().ToLower();
        foreach (char c in Path.GetInvalidFileNameChars())
            safePseudo = safePseudo.Replace(c, '_');

        return Application.persistentDataPath + FilePrefix + safePseudo + FileExt;
    }

    /// <summary>Sauvegarde les données de score pour un pseudo donné.</summary>
    public void Save(ScoreSaveData data, string pseudo)
    {
        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(GetPath(pseudo), json);
    }

    /// <summary>Charge les données de score d'un pseudo. Retourne un objet vierge si le fichier n'existe pas.</summary>
    public ScoreSaveData Load(string pseudo)
    {
        string path = GetPath(pseudo);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<ScoreSaveData>(json) ?? new ScoreSaveData();
        }
        return new ScoreSaveData();
    }
}
