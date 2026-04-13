using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Second panel affiché après la sélection de classe.
/// Le joueur saisit son pseudo puis confirme.
/// </summary>
public class PlayerNamePanel : MonoBehaviour
{
    [Header("Données")]
    [SerializeField] private SO_PlayersDatas _playersDatas;

    [Header("Pseudo")]
    [SerializeField] private TMP_InputField _nameInputField;

    [Header("Bouton de confirmation")]
    [SerializeField] private Button _confirmButton;

    [Header("Panel")]
    [SerializeField] private GameObject _panel;

    private void OnEnable()
    {
        _nameInputField.text = _playersDatas.Name;
    }

    private void Start()
    {
        _confirmButton.onClick.AddListener(Confirm);
    }

    /// <summary>Valide le pseudo, sauvegarde et ferme le panel.</summary>
    public void Confirm()
    {
        if (string.IsNullOrWhiteSpace(_nameInputField.text)) return;

        _playersDatas.Name = _nameInputField.text.Trim();
        _playersDatas.SaveDatas();

        _panel.SetActive(false);
    }
}