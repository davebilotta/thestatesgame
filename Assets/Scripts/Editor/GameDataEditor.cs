using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class GameDataEditor : EditorWindow {

	public GameData gameData;

	private string gameDataProjectFilePath = "/StreamingAssets/data.json";

	[MenuItem("Window/Game Data Editor")]
	static void Init() {
		GameDataEditor window = (GameDataEditor) EditorWindow.GetWindow(typeof(GameDataEditor));
		window.Show();
	}

	void OnGUI() {
		// Similar to Update in that it runs continuously when window is in editor, or with mouse click/movement

		if (gameData != null) {
			SerializedObject serializedObject = new SerializedObject(this);
			SerializedProperty serializedProperty = serializedObject.FindProperty("gameData");

			EditorGUILayout.PropertyField(serializedProperty, true);
			serializedObject.ApplyModifiedProperties();

			if (GUILayout.Button("Save Data")) {
				SaveGameData();
			}
		}

		if (GUILayout.Button("Load Data")) {
			LoadGameData();
		}
	}

	private void LoadGameData() {
		string filePath = getFilePath();

		if (File.Exists(filePath)) {
			string dataAsJson = File.ReadAllText(filePath);
			gameData = JsonUtility.FromJson<GameData>(dataAsJson);
		}
		else {
			gameData = new GameData();
		}
	}

	private void SaveGameData() {
		string dataAsJson = JsonUtility.ToJson(gameData);
		string filePath = getFilePath();
		File.WriteAllText(filePath,dataAsJson);
	}

	private string getFilePath() {
		return Application.dataPath + gameDataProjectFilePath;
	}
}
