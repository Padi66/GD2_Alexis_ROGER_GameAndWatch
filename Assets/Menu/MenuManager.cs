using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    /// <summary>Charge la scène spécifiée par son nom.</summary>
    public void LoadMap(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>Désactive le panel passé en paramètre.</summary>
    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    /// <summary>Active le panel passé en paramètre.</summary>
    public void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
    }
}