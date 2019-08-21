using UnityEngine;
using System.Collections;

public enum MarbleGameState {playing, won,lost };

public class MarbleGameManager : MonoBehaviour
{
	public static MarbleGameManager SP;
	public float movementSpeed = 6.0f;

	
	private int totalGems;
	private int foundGems;
	private MarbleGameState gameState;
	
	
	void Awake()
	{
		SP = this; 
		foundGems = 0;
		gameState = MarbleGameState.playing;
		totalGems = GameObject.FindGameObjectsWithTag("Pickup").Length;
		Time.timeScale = 1.0f;
	}
	
	void Update () {
		Vector3 movement = (Input.GetAxis("Horizontal") * -Vector3.left * movementSpeed) + 
			(Input.GetAxis("Vertical") * Vector3.forward *movementSpeed);
		GetComponent<Rigidbody>().AddForce(movement, ForceMode.Force);
	}


	void OnGUI () {
		GUILayout.Label(" Found gems: "+foundGems+"/"+totalGems );
		
		if (gameState == MarbleGameState.lost)
		{
			GUILayout.Label("You Lost!");
			if(GUILayout.Button("Try again") ){
				Application.LoadLevel(Application.loadedLevel);
			}
		}
		else if (gameState == MarbleGameState.won)
		{
			GUILayout.Label("You won!");
			if(GUILayout.Button("Play again") ){
				Application.LoadLevel(Application.loadedLevel);
			}
		}
	}

	void OnTriggerEnter  (Collider other  ) {
		if (other.tag == "Pickup")
		{
			MarbleGameManager.SP.FoundGem();
			Destroy(other.gameObject);
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
		gameState = MarbleGameState.won;
	}
	
	public void SetGameOver()
	{
		Time.timeScale = 0.0f; //Pause game
		gameState = MarbleGameState.lost;
	}
}
