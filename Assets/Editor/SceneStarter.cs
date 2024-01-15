using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.SceneManagement;

public class SceneStarter : EditorWindow
{
    [MenuItem("Play/PlayMe _%h")]
    public static void RunMainScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Electricity.unity");
        EditorApplication.isPlaying = true;
    }
}
