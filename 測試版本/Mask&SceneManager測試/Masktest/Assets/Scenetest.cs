using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenetest : MonoBehaviour {

    /*
    IEnumerator LoadYourAsyncScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("masktest");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }*/

    public void Click_load()
    {
        Debug.Log("load");
        SceneManager.LoadScene("masktest");
        //StartCoroutine(LoadYourAsyncScene());
    }

    public void Click_unload()
    {
        Debug.Log("unload");
        SceneManager.LoadScene("start");
    }
}
