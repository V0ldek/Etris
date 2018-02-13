using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PolygonCollider2D))]
public class PolygonCollider2DEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		var collider = (PolygonCollider2D)target;

		if(GUILayout.Button ("Add point"))
		{
			Vector2[] newPoints = new Vector2[collider.points.Length + 1];

			for(int i = 0; i < newPoints.Length - 1; ++i)
				newPoints[i] = collider.points[i];

			newPoints[newPoints.Length - 1] = new Vector2(0, 0);

			collider.points = newPoints;
		}		
		if(GUILayout.Button ("Remove point"))
		{
			Vector2[] newPoints = new Vector2[collider.points.Length - 1];
			
			for(int i = 0; i < newPoints.Length - 1; ++i)
				newPoints[i] = collider.points[i];

			collider.points = newPoints;
		}

		var points = collider.points;

		for (int i = 0; i < points.Length; i++)
		{
			points[i] = EditorGUILayout.Vector2Field(i.ToString(), points[i]);
		}
		collider.points = points;
		EditorUtility.SetDirty(target);
	}
}