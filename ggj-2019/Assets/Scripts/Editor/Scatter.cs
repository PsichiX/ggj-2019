using UnityEditor;
using UnityEngine;

public class Scatter : EditorWindow
{

	// Add menu item named "My Window" to the Window menu
	[MenuItem("Window/My Window")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(Scatter));
	}

	void OnGUI()
	{
		GUILayout.Label("Base Settings", EditorStyles.boldLabel);

		if (GUILayout.Button("Test"))
		{
			var obj = Instantiate(Resources.Load("Tree01")) as GameObject;
			obj.transform.position = new Vector3(Random.Range(-60, 120), 0, Random.Range(20, 75));
			obj.transform.Rotate(new Vector3(0, Random.Range(-179, 179), 0));
			var random = Random.Range(0.8f, 1.6f);
			obj.transform.localScale = new Vector3(random, random, random);
		}
	}
}