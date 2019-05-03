using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	public void LoadScene(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}

	public void Quit()
	{
		if (Application.isEditor)
		{
#if UNITY_EDITOR
			EditorApplication.isPlaying = false;
#endif
		}
		else
		{
			Application.Quit();
		}
	}
}
