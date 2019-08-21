using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Countdown : MonoBehaviour {
    string countdownText = "";
	private GUIStyle guiNumber = new GUIStyle();
    void OnGUI()
    {
		guiNumber.fontSize = 200;
		guiNumber.normal.textColor = Color.red;
        if (countdownText != "")
			GUI.Label(new Rect((Screen.width - 100) / 2 -100, (Screen.height - 30) / 2, 100, 30), countdownText,guiNumber);
        else
			if (GUI.Button(new Rect(1100, 0, 150, 50), "Play"))
            StartCoroutine(down());
    }

    IEnumerator down()
    {
        countdownText = "  3";
        yield return new WaitForSeconds(1);
        countdownText = "  2";
        yield return new WaitForSeconds(1);
        countdownText = "  1";
        yield return new WaitForSeconds(1);
        countdownText = "  0";
        yield return new WaitForSeconds(1);
        countdownText = "Go!";
        yield return new WaitForSeconds(1);
        countdownText = "";
    }


}
