using UnityEngine;

public class ShootingScript : MonoBehaviour
{
	public ParticleSystem impactEffect;	//用來放置撞擊的容器
	//public float shootFrequency = 0.5f; //射擊頻率，自動射擊 = O

	AudioSource gunFireAudio;			//放置槍聲的容器
	RaycastHit rayHit;					//射線碰到的物件

	void Start()
	{
		//取得物件身上的音源
		gunFireAudio = GetComponent<AudioSource>();
		//InvokeRepeating("AutoShooting",1,shootFrequency); //重複射擊，自動射擊 = O
	}

	void Update() //自動射擊 = X
	{ //自動射擊 = X
		//當按滑鼠左鍵就射擊...
		if (Input.GetButtonDown("Fire1")) //自動射擊 = X
		//void AutoShooting() //自動射擊 = O
		{
			//...播放槍聲...
			gunFireAudio.Stop();
			gunFireAudio.Play();

			//...射出射線
			if (Physics.Raycast(transform.position, transform.forward, out rayHit, 100f))
			{
				//播放撞擊特效
				impactEffect.transform.position = rayHit.point;
				impactEffect.transform.rotation = Quaternion.Euler(270, 0, 0);
				impactEffect.Stop();
				impactEffect.Play();

				//如果是敵人就消滅物件
				if (rayHit.transform.tag == "Enemy")
					Destroy(rayHit.transform.gameObject);
			}
		}
	} //自動射擊 = X
}
