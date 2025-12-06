using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ProgressBar), true)]
public class ProgressBarEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		ProgressBar bar = (ProgressBar)target;

		if (GUILayout.Button("Update object"))
		{
			bar.MaxValue = bar.MaxValue;
			bar.Current = bar.Current;
		}
	}
}
