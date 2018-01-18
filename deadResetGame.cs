using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class deadResetGame : MonoBehaviour {
    public int win;

	public IEnumerator EventTest() {
        if(win == 1) {
            yield return new WaitForSeconds(5f);
        }
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(0);
    }
}
