using UnityEngine;

public class EnemyNavigation : MonoBehaviour {

	public Transform target;	//目標
	UnityEngine.AI.NavMeshAgent agent;			//尋徑物件

	void Start()
	{
		//找到物體身上的尋徑元件
		agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
    }

	void Update()
	{
		//不斷的向目標前進
		if (target && !agent.hasPath)
		{
			agent.SetDestination(target.position);
		}
	}
}
