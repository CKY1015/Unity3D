using UnityEngine;
using System.Collections;

public class addBox : MonoBehaviour {
	public int max = 30;

	void Start () {

	}

	void Update () {
		
		RaycastHit hit ;

		if (Input.GetMouseButtonUp (0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast (ray , out hit , 1000) )
			{
				addBoxObj(hit.point);
				print(hit.point.y);
			}

		}
	}

	void addBoxObj(Vector3 pos)
	{
		pos.y += 5;
		Instantiate (Resources.Load ("myBox"), pos, Quaternion.identity);
		GameObject[] tempGameObject = GameObject.FindGameObjectsWithTag ("Box");
		int n = tempGameObject.Length;

		if (n == max) {
			for (int i=0; i<n; i++) {
				Destroy (tempGameObject[i]);
			}
		}
	}
}
