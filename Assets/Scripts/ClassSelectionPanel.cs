using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Panel de sélection du joueur.
/// Le joueur saisit son pseudo, choisit sa classe (Blanc ou Noir) via les boutons,
/// puis confirme. Les images sont activées/désactivées selon la classe choisie.
/// </summary>
public class ClassSelectionPanel : MonoBehaviour
{
    [Header("Données")]
    [SerializeField] private SO_PlayersDatas _playersDatas;
    [SerializeField] private SO_ScoreDatas   _scoreDatas;

    [Header("Pseudo")]
    [SerializeField] private TMP_InputField _nameInputField;

    [Header("Chevalier Blanc — ButtonWhite → SelectHacker()")]
    [SerializeField] private GameObject _hackerOverlay;

    [Header("Chevalier Noir — ButtonBlack → SelectInfiltrateur()")]
    [SerializeField] private GameObject _infiltrateurOverlay;

    [Header("Navigation")]
    [SerializeField] private Button _confirmButton;

    private PlayerClass _selectedClass = PlayerClass.Hacker;

    private void Start()
    {
        _playersDatas.LoadDatas();
        _selectedClass       = _playersDatas.Class;
        _nameInputField.text = _playersDatas.Name;

        _confirmButton.onClick.AddListener(Confirm);

        RefreshImages();

        if (!string.IsNullOrWhiteSpace(_playersDatas.Name))
        {
            _scoreDatas?.LoadScores(_playersDatas.Name);
            gameObject.SetActive(false);
        }
    }

    /// <summary>Sélectionne une classe et met à jour les images.</summary>
    public void SelectClass(PlayerClass playerClass)
    {
        _selectedClass = playerClass;
        RefreshImages();
    }

    /// <summary>Sélectionne le chevalier Blanc — à câbler sur ButtonWhite.</summary>
    public void SelectHacker() => SelectClass(PlayerClass.Hacker);

    /// <summary>Sélectionne le chevalier Noir — à câbler sur ButtonBlack.</summary>
    public void SelectInfiltrateur() => SelectClass(PlayerClass.Infiltrateur);

    /// <summary>Ouvre le panel.</summary>
    public void Open() => gameObject.SetActive(true);

    /// <summary>Ferme le panel.</summary>
    public void Close() => gameObject.SetActive(false);

    /// <summary>Valide le pseudo et la classe, sauvegarde et ferme le panel.</summary>
    public void Confirm()
    {
        if (string.IsNullOrWhiteSpace(_nameInputField.text)) return;

        string newPseudo   = _nameInputField.text.Trim();
        bool pseudoChanged = !string.Equals(newPseudo, _playersDatas.Name, System.StringComparison.OrdinalIgnoreCase);

        _playersDatas.Name  = newPseudo;
        _playersDatas.Class = _selectedClass;
        _playersDatas.SaveDatas();

        if (_scoreDatas != null && pseudoChanged)
            _scoreDatas.LoadScores(newPseudo);
        else
            _scoreDatas?.SetCurrentPseudo(newPseudo);

        gameObject.SetActive(false);
    }

    private void RefreshImages()
    {
        if (_hackerOverlay      != null) _hackerOverlay.SetActive(_selectedClass == PlayerClass.Hacker);
        if (_infiltrateurOverlay != null) _infiltrateurOverlay.SetActive(_selectedClass == PlayerClass.Infiltrateur);
    }
}
