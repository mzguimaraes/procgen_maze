using UnityEngine;

using UnityEngine.SceneManagement;

public class RestartR : MonoBehaviour {

	void Update () {
		if (Input.GetKeyDown(KeyCode.R)) {
			Pathmaker.allTiles.Clear();
			Pathmaker.allPathmakers.Clear();
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}
}
