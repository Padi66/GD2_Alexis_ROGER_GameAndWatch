using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ClassSelectionPanel))]
public class ClassSelectionPanelEditor : Editor
{
    SerializedProperty _playersDatas;
    SerializedProperty _scoreDatas;
    SerializedProperty _nameInputField;
    SerializedProperty _hackerOverlay;
    SerializedProperty _infiltrateurOverlay;
    SerializedProperty _confirmButton;

    bool _foldWhite = true;
    bool _foldBlack = true;

    private void OnEnable()
    {
        _playersDatas        = serializedObject.FindProperty("_playersDatas");
        _scoreDatas          = serializedObject.FindProperty("_scoreDatas");
        _nameInputField      = serializedObject.FindProperty("_nameInputField");
        _hackerOverlay       = serializedObject.FindProperty("_hackerOverlay");
        _infiltrateurOverlay = serializedObject.FindProperty("_infiltrateurOverlay");
        _confirmButton       = serializedObject.FindProperty("_confirmButton");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // ── Données ───────────────────────────────────────────────────────────
        SectionHeader("Données");
        EditorGUILayout.PropertyField(_playersDatas, new GUIContent("Players Datas"));
        EditorGUILayout.PropertyField(_scoreDatas,   new GUIContent("Score Datas"));

        Space();

        // ── Pseudo ────────────────────────────────────────────────────────────
        SectionHeader("Pseudo");
        EditorGUILayout.PropertyField(_nameInputField, new GUIContent("Champ de saisie"));

        Space();

        // ── Chevalier Blanc ───────────────────────────────────────────────────
        _foldWhite = EditorGUILayout.BeginFoldoutHeaderGroup(_foldWhite, "⬜  Chevalier Blanc");
        if (_foldWhite)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_hackerOverlay, new GUIContent("Image (ImageWhite)", "Activée quand Hacker est sélectionné"));
            HelpNote("ButtonWhite.onClick → SelectHacker()");
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        Space();

        // ── Chevalier Noir ────────────────────────────────────────────────────
        _foldBlack = EditorGUILayout.BeginFoldoutHeaderGroup(_foldBlack, "⬛  Chevalier Noir");
        if (_foldBlack)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_infiltrateurOverlay, new GUIContent("Image (ImageBlack)", "Activée quand Infiltrateur est sélectionné"));
            HelpNote("ButtonBlack.onClick → SelectInfiltrateur()");
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        Space();

        // ── Navigation ────────────────────────────────────────────────────────
        SectionHeader("Navigation");
        EditorGUILayout.PropertyField(_confirmButton, new GUIContent("Bouton Confirmer", "Câblé automatiquement en Start()"));

        Space();
        EditorGUILayout.HelpBox(
            "Câblage attendu dans la scène :\n" +
            "  • ButtonWhite.onClick  →  SelectHacker()\n" +
            "  • ButtonBlack.onClick  →  SelectInfiltrateur()\n" +
            "  • EditProfileButton.onClick  →  Open()\n" +
            "  • ConfiermeClass  →  câblé automatiquement en Start()",
            MessageType.Info);

        serializedObject.ApplyModifiedProperties();
    }

    private static void SectionHeader(string label)
    {
        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        var rect = EditorGUILayout.GetControlRect(false, 1);
        EditorGUI.DrawRect(rect, new Color(0.4f, 0.4f, 0.4f, 0.5f));
        EditorGUILayout.Space(2);
    }

    private static void HelpNote(string text)
    {
        var style = new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = new Color(0.5f, 0.8f, 0.5f) } };
        EditorGUILayout.LabelField(text, style);
    }

    private static void Space() => EditorGUILayout.Space(6);
}
