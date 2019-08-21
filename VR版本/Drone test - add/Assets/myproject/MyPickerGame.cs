using UnityEngine;
using System.Collections;

public enum Game { playing, won, lost };

public class MyPickerGame : MonoBehaviour
{
    public static MyPickerGame SP;
    public float movementSpeed = 6.0f;

    private int totalGems;
    private int foundGems;
    private Game gameState;

	public float time_f = 0;
	public bool time_b = false;

	public int mode_flag = 1;

    void Awake()
    {
        SP = this;
        foundGems = 0;
        gameState = Game.playing;
        totalGems = GameObject.FindGameObjectsWithTag("Picker").Length;
        Time.timeScale = 1.0f;
    }

    void Update()
    {
		if (time_b == true) {
			time_f = time_f + Time.deltaTime;
		}
    }

	private GUIStyle guiStyle = new GUIStyle(); //create a new variable
	string countdownText = "";
	private GUIStyle guiNumber = new GUIStyle();
    void OnGUI()
    {
		guiNumber.fontSize = 200;
		guiNumber.normal.textColor = Color.red;
		guiStyle.fontSize = 20; //change the font size
		GUILayout.Label("Found gems: " + foundGems + "/" + totalGems, guiStyle);
		switch (mode_flag) {
			case 1: 
				GUILayout.Label ("Times: " + (int)time_f / 60 + "m" + (int)time_f % 60 + "s", guiStyle);
				break;
			case 2: 
				GUILayout.Label ("Times: " + (int)time_f/2/60 + "m" + (int)time_f/2%60 + "s", guiStyle);
				break;
			case 3:	
				GUILayout.Label ("Times: " + (int)time_f/3/60 + "m" + (int)time_f/3%60 + "s", guiStyle);
				break;
		}


		if (countdownText != "")
			GUI.Label(new Rect((Screen.width - 100) / 2 -100, (Screen.height - 30) / 2, 100, 30), countdownText,guiNumber);
		
		if (GUILayout.Button ("Play")) {
			StartCoroutine(down());
		}

		if (GUILayout.Button ("Pause")) {
			Time.timeScale = 0;
		}

		if (GUILayout.Button ("Continue")) {
			Time.timeScale = 1;
		}

		if (GUILayout.Button("Reset"))
		{
			Application.LoadLevel(Application.loadedLevel);
		}

		if (GUILayout.Button("Mode1"))
		{
			Time.timeScale = 1;
			mode_flag = 1;
			time_b = true;
			time_f = 0;
		}

		if (GUILayout.Button("Mode2"))
		{
			Time.timeScale = 2;
			mode_flag = 2;
			time_b = true;
			time_f = 0;
		}

		if (GUILayout.Button("Mode3"))
		{
			Time.timeScale = 3;
			mode_flag = 3;
			time_b = true;
			time_f = 0;
		}
        if (gameState == Game.lost)
        {
			GUILayout.Label("You Lost!", guiStyle);
			if (GUILayout.Button("Try again"))
            {
                Application.LoadLevel(Application.loadedLevel);
            }
        }
        else if (gameState == Game.won)
        {
			GUILayout.Label("You won!", guiStyle);
			if (GUILayout.Button("Play again"))
            {
                Application.LoadLevel(Application.loadedLevel);
            }
        }
    }

	IEnumerator down()
	{
		countdownText = "  3";
		yield return new WaitForSeconds(1);
		countdownText = "  2";
		yield return new WaitForSeconds(1);
		countdownText = "  1";
		yield return new WaitForSeconds(1);
		countdownText = "Go!";
		yield return new WaitForSeconds(1);
		countdownText = "";
		time_b = true;
	}


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Picker")
        {
            MyPickerGame.SP.FoundGem();
            Destroy(other.gameObject);
        }
        else if (other.tag == "GameOver")
        {
            MyPickerGame.SP.SetGameOver();
        }
        else
        {
            //Other collider.. See other.tag and other.name
        }
    }




    public void FoundGem()
    {
        foundGems++;
        if (foundGems >= totalGems)
        {
            WonGame();
        }
    }

    public void WonGame()
    {
        Time.timeScale = 0.0f; //Pause game
        gameState = Game.won;
    }

    public void SetGameOver()
    {
        Time.timeScale = 0.0f; //Pause game
        gameState = Game.lost;
    }
}
