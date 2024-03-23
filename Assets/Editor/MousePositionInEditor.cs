using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpawnData))]
public class MousePositionInEditor : Editor
{
	private void OnEnable()
	{
		SceneView.duringSceneGui += onSceneGUI;
	}

	private void OnDisable()
	{
		SceneView.duringSceneGui -= onSceneGUI;
	}
	private void onSceneGUI(SceneView sceneView)
	{
		if (Selection.activeObject is SpawnData)
		{
			Event currentEvent = Event.current;
			Vector2 mousePosition = currentEvent.mousePosition;
			Ray worldRay = HandleUtility.GUIPointToWorldRay(mousePosition);

			if (currentEvent.type == EventType.MouseDown)
			{
				RaycastHit hit;
				if (Physics.Raycast(worldRay, out hit))
				{
					Vector3 mouseWorldPosition = hit.point;
					Debug.Log("Mouse position in world space: " + mouseWorldPosition);
				}
			}
		}
	}

}
