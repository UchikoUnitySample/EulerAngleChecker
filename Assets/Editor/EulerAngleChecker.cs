using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Editor
{
	public class EulerAngleChecker : EditorWindow
	{
		[MenuItem("EulerAngleChecker/OpenWindow")]
		static void Open()
		{
			EditorWindow.GetWindow<EulerAngleChecker>("EulerAngleChecker");
		}

		void OnGUI()
		{
			GUILayout.Label("Rotation -> Interpolation -> \"Euler Angle\"");

			if (GUILayout.Button("Check")) {
				Check();
			}
		}

		void Check()
		{
			HashSet<string> targetProperties = new HashSet<string>();
			int animationCount = 0;
			int propertiesCount = 0;

			var gameObjectsInScene = FindObjectsOfType(typeof(GameObject)) as GameObject[];
			foreach (var gameObject in gameObjectsInScene) {
				var animator = gameObject.GetComponent<Animator>();

				if (animator == null)
					continue;

				animationCount++;

				var clips = AnimationUtility.GetAnimationClips(gameObject);

				foreach (var clip in clips) {
					string key = "Target: "  + gameObject.name;

					var bindings = AnimationUtility.GetCurveBindings(clip);

					foreach (var bind in bindings) {
						// Euler Angle -> "localEulerAnglesRaw"
						// Euler Angle (Quaternion Approximation) -> "m_LocalRotation"
						if (bind.propertyName.StartsWith("localEulerAnglesRaw")) {
							string v = gameObject.name + "/" + bind.path;
							targetProperties.Add(key + "\t" + v);
						}
					}
					propertiesCount += bindings.Length;
				}
			}

			Debug.Log(
				"--- Interpolation Check Report ---\n"
				+ "Number of target animation:" + animationCount + "\n" 
				+ "Number of target property:" + propertiesCount
			);

			if (targetProperties.Count == 0) {
				Debug.Log("\"Euler Angle\" property doesn't exist.");
			} else {
				Debug.Log("Number of \"Euler Angle\" properties: " + targetProperties.Count);
				foreach (var target in targetProperties) {
					Debug.Log("\"Euler Angle\" property: " + target);
				}
			}
		}
	}
}
