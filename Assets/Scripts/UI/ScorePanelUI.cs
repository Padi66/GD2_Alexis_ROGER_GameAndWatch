using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Affiche les scores des 3 jeux dans le panel de menu.
/// Les scores sont liés au pseudo courant du joueur.
/// Se met à jour automatiquement à l'ouverture du panel.
/// </summary>
public class ScorePanelUI : MonoBehaviour
{
    [Header("Données")]
    [SerializeField] private SO_ScoreDatas   _scoreDatas;
    [SerializeField] private SO_PlayersDatas _playersDatas;

    [Header("Pseudo")]
    [SerializeField] private TextMeshProUGUI _pseudoText;
    [SerializeField] private Image           _classImage;
    [SerializeField] private Sprite          _spriteHacker;
    [SerializeField] private Sprite          _spriteInfiltrateur;

    [Header("Jeu 1 - Dernier score")]
    [SerializeField] private TextMeshProUGUI _jeu1ScoreText;
    [SerializeField] private TextMeshProUGUI _jeu1DifficultyText;

    [Header("Jeu 2 - Meilleur temps")]
    [SerializeField] private TextMeshProUGUI _jeu2TimeText;
    [SerializeField] private TextMeshProUGUI _jeu2DifficultyText;

    [Header("Jeu 3 - Nombre de coups")]
    [SerializeField] private TextMeshProUGUI _jeu3MovesText;
    [SerializeField] private TextMeshProUGUI _jeu3DifficultyText;

    private void OnEnable()
    {
        Refresh();
    }

    /// <summary>Ouvre le panel et rafraîchit les scores.</summary>
    public void Open()
    {
        gameObject.SetActive(true);
    }

    /// <summary>Ferme le panel.</summary>
    public void Close()
    {
        gameObject.SetActive(false);
    }

    /// <summary>Recharge les scores du pseudo courant et met à jour l'affichage.</summary>
    public void Refresh()
    {
        if (_scoreDatas == null || _playersDatas == null) return;

        _playersDatas.LoadDatas();
        _scoreDatas.LoadScores(_playersDatas.Name);

        RefreshPseudo();
        RefreshJeu1();
        RefreshJeu2();
        RefreshJeu3();
    }

    private void RefreshPseudo()
    {
        if (_pseudoText != null)
            _pseudoText.text = string.IsNullOrWhiteSpace(_playersDatas.Name) ? "-" : _playersDatas.Name;

        if (_classImage != null)
        {
            bool hasClass = _playersDatas.Class == PlayerClass.Hacker
                ? _spriteHacker      != null
                : _spriteInfiltrateur != null;

            _classImage.enabled = hasClass;
            _classImage.sprite  = _playersDatas.Class == PlayerClass.Hacker
                ? _spriteHacker
                : _spriteInfiltrateur;
        }
    }

    private void RefreshJeu1()
    {
        if (_jeu1ScoreText != null)
            _jeu1ScoreText.text = _scoreDatas.Jeu1LastScore > 0
                ? _scoreDatas.Jeu1LastScore.ToString()
                : "-";

        if (_jeu1DifficultyText != null)
            _jeu1DifficultyText.text = _scoreDatas.Jeu1Difficulty;
    }

    private void RefreshJeu2()
    {
        if (_jeu2TimeText != null)
        {
            float t = _scoreDatas.Jeu2BestTime;
            if (t < 0f)
                _jeu2TimeText.text = "-";
            else if (t >= float.MaxValue)
                _jeu2TimeText.text = "infini";
            else
            {
                int minutes = Mathf.FloorToInt(t / 60f);
                int seconds = Mathf.FloorToInt(t % 60f);
                _jeu2TimeText.text = $"{minutes:00}:{seconds:00}";
            }
        }

        if (_jeu2DifficultyText != null)
            _jeu2DifficultyText.text = _scoreDatas.Jeu2BestTime < 0f ? "-" : _scoreDatas.Jeu2Difficulty;
    }

    private void RefreshJeu3()
    {
        if (_jeu3MovesText != null)
            _jeu3MovesText.text = _scoreDatas.Jeu3Moves < 0
                ? "-"
                : _scoreDatas.Jeu3Moves.ToString();

        if (_jeu3DifficultyText != null)
            _jeu3DifficultyText.text = _scoreDatas.Jeu3Moves < 0 ? "-" : _scoreDatas.Jeu3Difficulty;
    }
}
