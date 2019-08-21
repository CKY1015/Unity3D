using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TowerHealth : MonoBehaviour
{
	public int numberOfLives = 3;	//設定塔的血有多少
	public Image damageImage;		//死掉的畫面

    int currentLives;				//目前血量
	AudioSource damageAudio;		//音效
	bool alive = true;				//生或死

    void Awake()
	{
        currentLives = numberOfLives;
		damageAudio = GetComponent<AudioSource>();
    }

	void OnTriggerEnter(Collider other)
	{
		//當敵人碰到塔就受傷
		if (other.tag != "Enemy" || !alive)
			return;

		Destroy(other.gameObject);
        currentLives -= 1;
		damageAudio.Play();

		//如果沒血了
		if(currentLives <= 0)
		{
			//死亡重生3秒
			alive = false;
            if (damageImage)
            {
                Color col = damageImage.color;
                col.a = 1f;
                damageImage.color = col;
            }

			//重新開始
			Invoke("Restart", 3f);
		}
	}

	void Restart()
	{
		//重置畫面
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemies.Length; i++)
            Destroy(enemies[i]);

        currentLives = numberOfLives;
        alive = true;

        if (damageImage)
        {
            Color col = damageImage.color;
            col.a = 0f;
            damageImage.color = col;
		}
    }
}
