using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(Character))]
public class CharacterEditor : Editor
{
    Character character = null;

    private void OnEnable()
    {
        // Character 컴포넌트를 얻어오기
        character = (Character)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // 속력 수치 표시
        EditorGUILayout.LabelField("속도", character.realSpeed.ToString());
    }
}
