using UnityEditor;

[CustomEditor(typeof(Boss), true)]
public class BossEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Boss boss = (Boss)target;

        EditorGUILayout.MinMaxSlider(
            ref boss.attackRange.closeLimit, ref boss.attackRange.midLimit,
            boss.attackRange.min, boss.attackRange.longLimit);
    }
}
