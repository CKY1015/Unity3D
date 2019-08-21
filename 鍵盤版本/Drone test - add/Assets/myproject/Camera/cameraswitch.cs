using UnityEngine;
using System.Collections;

public class cameraswitch: MonoBehaviour
{



    public GameObject cam1, cam2, cam3, cam4; //兩個不同的攝影機
    public GameObject obj1, obj2, obj3; //兩個不同的GameObject

    /* 放在Awake內，在物件執行之前就先封鎖住，避免物件的出現，直到使用者切換攝影機後再創造該物件，若是放在Start內則會造成全部的物件都已經產生且初始化後再封鎖住。 */
    void Awake()
    {
        //預設先開啟第一部攝影機

        //一定要先暫停不使用的攝影機後，再開啟要使用的攝影機！
        cam4.SetActive(false);
        cam3.SetActive(false);
        cam2.SetActive(false);
        cam1.SetActive(true);

        //沒有要顯示出來的物件則先暫時關閉，同時開啟要顯示的物件，避免背景執行浪費效能。若多部攝影機都拍著同個物件，則不需要關閉該物件，只需關閉攝影機即可
        //被關閉的物件和其子物件都會被隱藏(其身上的script都會一起被暫停)
      
        obj3.SetActive(false);      //z text
        obj2.SetActive(false);      //y text
        obj1.SetActive(false);       //x text

       /* cub1.SetActive(true);
        cub2.SetActive(true);
        cub3.SetActive(true);*/
    }

    void Update()
    {
        if (Input.GetKey("x") == true)
        {
            //若是按下鍵盤的x則切換成第二部攝影機
            cam1.SetActive(true);
            obj1.SetActive(true);
            cam2.SetActive(true);
            obj2.SetActive(false);
            cam3.SetActive(false);
            obj3.SetActive(false);
            cam4.SetActive(false);
          
       
        }
        else if (Input.GetKey("y") == true)
        {
            //若是按下鍵盤的y則切換成第三部攝影機
            cam1.SetActive(true);
            obj1.SetActive(false);
            cam2.SetActive(false);
            obj2.SetActive(true);
            cam3.SetActive(true);
            obj3.SetActive(false);
            cam4.SetActive(false);
          
         
        }
        else if (Input.GetKey("z") == true)
        {
            //若是按下鍵盤的z則切換成第四部攝影機
            cam1.SetActive(true);
            obj1.SetActive(false);
            cam2.SetActive(false);
            obj2.SetActive(false);
            cam3.SetActive(false);
            obj3.SetActive(true);
            cam4.SetActive(true);
           
           
        }
        else if (Input.GetKey("o") == true)
        {
            //若是按下鍵盤的o則切換成第一部攝影機
            cam1.SetActive(true);
            obj1.SetActive(false);
            cam2.SetActive(false);
            obj2.SetActive(false);
            cam3.SetActive(false);
            obj3.SetActive(false);
            cam4.SetActive(false);
          
          
        }
    }


}
