using UnityEngine;
using System.Collections;

public enum GameState {playing, won,lost };

public class MyPickerGame1 : MonoBehaviour
{
	public static MyPickerGame1 SP;
	public float movementSpeed = 6.0f;
	
	private int totalGems;
	private int foundGems;
	private GameState gameState;
	
	
	void Awake()
	{
		SP = this; 
		foundGems = 0;
		gameState = GameState.playing;
		totalGems = GameObject.FindGameObjectsWithTag("Picker").Length;
		Time.timeScale = 1.0f;
	}
	
	void Update () {
		Vector3 movement = (Input.GetAxis("Horizontal") * -Vector3.left * movementSpeed) + 
			(Input.GetAxis("Vertical") * Vector3.forward *movementSpeed);
		GetComponent<Rigidbody>().AddForce(movement, ForceMode.Force);
	}


	void OnGUI () {
		GUILayout.Label(" Found gems: "+foundGems+"/"+totalGems );
		
		if (gameState == GameState.lost)
		{
			GUILayout.Label("You Lost!");
			if(GUILayout.Button("Try again") ){
				Application.LoadLevel(Application.loadedLevel);
			}
		}
		else if (gameState == GameState.won)
		{
			GUILayout.Label("You won!");
			if(GUILayout.Button("Play again") ){
				Application.LoadLevel(Application.loadedLevel);
			}
		}
	}

	void OnTriggerEnter  (Collider other  ) {
		if (other.tag == "Picker") {
			MyPickerGame1.SP.FoundGem ();
			Destroy (other.gameObject);
		} else if (other.tag == "GameOver") {
			MyPickerGame1.SP.SetGameOver ();
		} else 		{
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
		gameState = GameState.won;
	}
	
	public void SetGameOver()
	{
		Time.timeScale = 0.0f; //Pause game
		gameState = GameState.lost;
	}
}
