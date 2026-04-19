using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Panel affiché au démarrage.
/// Le joueur saisit son pseudo, choisit sa classe via 2 images, puis confirme.
/// </summary>
public class ClassSelectionPanel : MonoBehaviour
{
    [Header("Données")]
    [SerializeField] private SO_PlayersDatas _playersDatas;
    [SerializeField] private SO_ScoreDatas   _scoreDatas;

    [Header("Pseudo")]
    [SerializeField] private TMP_InputField _nameInputField;

    [Header("Image Hacker")]
    [SerializeField] private Image _hackerImage;
    [SerializeField] private Sprite _hackerSpriteNormal;
    [SerializeField] private Sprite _hackerSpriteSelected;

    [Header("Image Infiltrateur")]
    [SerializeField] private Image _infiltrateurImage;
    [SerializeField] private Sprite _infiltrateurSpriteNormal;
    [SerializeField] private Sprite _infiltrateurSpriteSelected;

    [Header("Navigation")]
    [SerializeField] private Button _confirmButton;

    private PlayerClass _selectedClass = PlayerClass.Hacker;
    private Vector2 _hackerImageSize;
    private Vector2 _infiltrateurImageSize;

    private void Start()
    {
        _hackerImageSize       = _hackerImage.GetComponent<RectTransform>().sizeDelta;
        _infiltrateurImageSize = _infiltrateurImage.GetComponent<RectTransform>().sizeDelta;

        _playersDatas.LoadDatas();
        _selectedClass = _playersDatas.Class;
        _nameInputField.text = _playersDatas.Name;

        AddClickHandler(_hackerImage,       () => SelectClass(PlayerClass.Hacker));
        AddClickHandler(_infiltrateurImage, () => SelectClass(PlayerClass.Infiltrateur));
        _confirmButton.onClick.AddListener(Confirm);

        RefreshImages();

        // Charge les scores du pseudo existant dès le démarrage.
        if (!string.IsNullOrWhiteSpace(_playersDatas.Name))
        {
            _scoreDatas?.LoadScores(_playersDatas.Name);
            gameObject.SetActive(false);
        }
    }

    /// <summary>Sélectionne une classe et met à jour le sprite des images.</summary>
    public void SelectClass(PlayerClass playerClass)
    {
        _selectedClass = playerClass;
        RefreshImages();
    }

    /// <summary>Ouvre le panel.</summary>
    public void Open()
    {
        gameObject.SetActive(true);
    }

    /// <summary>Valide le pseudo et la classe, sauvegarde et ferme le panel.</summary>
    public void Confirm()
    {
        if (string.IsNullOrWhiteSpace(_nameInputField.text)) return;

        string newPseudo = _nameInputField.text.Trim();
        bool pseudoChanged = !string.Equals(newPseudo, _playersDatas.Name, System.StringComparison.OrdinalIgnoreCase);

        _playersDatas.Name  = newPseudo;
        _playersDatas.Class = _selectedClass;
        _playersDatas.SaveDatas();

        // Si le pseudo a changé, on charge les scores du nouveau pseudo.
        // S'il n'en a pas encore, les scores seront remis à zéro.
        // Si le joueur remet son ancien pseudo, ses anciens scores seront restaurés.
        if (_scoreDatas != null && pseudoChanged)
            _scoreDatas.LoadScores(newPseudo);
        else
            _scoreDatas?.SetCurrentPseudo(newPseudo);

        gameObject.SetActive(false);
    }

    private void RefreshImages()
    {
        ApplyState(_hackerImage,       _hackerSpriteNormal,       _hackerSpriteSelected,       _selectedClass == PlayerClass.Hacker,       _hackerImageSize);
        ApplyState(_infiltrateurImage, _infiltrateurSpriteNormal, _infiltrateurSpriteSelected, _selectedClass == PlayerClass.Infiltrateur, _infiltrateurImageSize);
    }

    private void ApplyState(Image image, Sprite normal, Sprite selected, bool isSelected, Vector2 originalSize)
    {
        if (image == null) return;
        image.sprite = isSelected ? selected : normal;
        image.GetComponent<RectTransform>().sizeDelta = originalSize;
    }

    private void AddClickHandler(Image image, System.Action onClick)
    {
        if (image == null) return;
        image.raycastTarget = true;

        EventTrigger trigger = image.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = image.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerDown
        };
        entry.callback.AddListener(_ => onClick?.Invoke());
        trigger.triggers.Add(entry);
    }
}
