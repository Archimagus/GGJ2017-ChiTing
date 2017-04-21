using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MazeGenerator), true)]
public class MazeGeneratorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		if (GUILayout.Button("Generate"))
		{
			var m = target as MazeGenerator;
			m.GenerateMaze();
		}
		if (GUILayout.Button("Generate Random"))
		{
			var m = target as MazeGenerator;
			m.GenerateMaze(Random.Range(0, int.MaxValue));
		}
		DrawDefaultInspector();
	}
}
