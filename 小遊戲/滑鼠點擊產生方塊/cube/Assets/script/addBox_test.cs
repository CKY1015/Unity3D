using UnityEngine;
using System.Collections;

public class addBox_test : MonoBehaviour
{
    public int max = 30;

    void Start()
    {

    }

    void Update()
    {
        if (Input.touchCount <= 0)
            return;

        RaycastHit hit;
        if (Input.touchCount == 1)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
                if (Physics.Raycast(ray, out hit, 1000))
                {
                    addBoxObj(hit.point);
                    print(hit.point.y);
                }

            }
        }
    }

    void addBoxObj(Vector3 pos)
    {
        pos.y += 5;
        Instantiate(Resources.Load("myBox"), pos, Quaternion.identity);
        GameObject[] tempGameObject = GameObject.FindGameObjectsWithTag("Box");
        int n = tempGameObject.Length;

        if (n == max)
        {
            for (int i = 0; i < n; i++)
            {
                Destroy(tempGameObject[i]);
            }
        }
    }
}
