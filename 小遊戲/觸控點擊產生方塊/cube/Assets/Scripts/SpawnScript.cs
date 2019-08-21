using UnityEngine;

public class SpawnScript : MonoBehaviour
{
	public GameObject enemy;			//敵人容器
	public Transform target;			//塔目標容器
	public float spawnInterval = 8f;	//重生時間

	float spawnVariance;				//亂數用

	void Start () 
	{
		//亂數被設定為速度的一半
		spawnVariance = spawnInterval * .5f;
		//重生敵人
		Invoke ("Spawn", spawnInterval + Random.Range(-spawnVariance, spawnVariance));
	}

	void Update()
	{
		//最小重生時間為一秒
		if (spawnInterval > 1f)
		{
			//讓重生越來越快(每50秒降低1秒)
			float timeReduction = Time.deltaTime / 50;

			//確保亂數時間不會過低
			spawnInterval = Mathf.Max(1f, spawnInterval - timeReduction);
			spawnVariance = spawnInterval * .5f;
		}
	}

	void Spawn()
	{
		//在重生點重生敵人
        GameObject enemyObj = Instantiate (enemy, transform.position, transform.rotation) as GameObject;
        enemyObj.transform.parent = transform;
		//設定目標
		enemyObj.GetComponent<EnemyNavigation> ().target = target;
		//繼續重生循環
		Invoke("Spawn", spawnInterval + Random.Range(-spawnVariance, spawnVariance));
	}
}
