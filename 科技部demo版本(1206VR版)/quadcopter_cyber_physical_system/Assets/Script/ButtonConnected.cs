using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;


public class ButtonConnected : MonoBehaviour {
    //public bool open_flag = false;
    public int state = 100;
    private Thread myThread;
    public bool s = false;
    public int Roll_num = 0;
    public int Pitch_num = 0;
    public int Yaw_num = 0;
	public float time_f = 0;
	private GUIStyle guiStyle = new GUIStyle(); //create a new variable
	public string sensornumber = "";

    void Awake()
    {
        Application.targetFrameRate = -1;//固定fps
    }

    void Start()
    {
        
    }
    // Use this for initialization
    void Update()
    {
		
		//Debug.Log("" + TCP_Handler.roll_status);
        if (s == true)
        {
			time_f = time_f + Time.deltaTime;
            //Debug.Log("" + TCP_Handler.pitch);
			transform.rotation = Quaternion.Euler (-(float)TCP_Handler.pitch, -(float)TCP_Handler.yaw, -(float)TCP_Handler.roll);

            //Thread.Sleep(3); //Delay
        }
        
        //state = TCP_Handler.status;
        Roll_num = TCP_Handler.roll_status;
        Pitch_num = TCP_Handler.pitch_status;
        Yaw_num = TCP_Handler.yaw_status;


    }
    void OnGUI()
    {
        
        GUILayout.Label("pitch: "+ TCP_Handler.pitch.ToString("0.00"));
		GUILayout.Label("roll: " + TCP_Handler.roll.ToString("0.00"));
		GUILayout.Label("yaw: " + TCP_Handler.yaw.ToString("0.00"));
		GUILayout.Label ("Times: " + (int)time_f/60 + "m" + (int)time_f%60 + "s");
		GUILayout.Label("connected number: " + SocketApp.TCP_clientList.Count);

        /*if (GUILayout.Button(TSB_Open, GUILayout.Height(50), GUILayout.Width(50)))//測試按鍵改成圖片用
		{
			Debug.Log("TSB_Open");
            FileInfo.loadSettingFile();
        }*/

        if (GUILayout.Button("TSB_Open"))//開啟
        {
            Debug.Log("TSB_Open");
            FileInfo.loadSettingFile();
        }

        if (GUILayout.Button("TSB_Start"))//開始
        {
			Debug.Log("TSB_Start");
            myThread = new Thread(new ThreadStart(ServerProcess));
            myThread.IsBackground = true;
            myThread.Start();
        }

        GUI.color = Color.gray;
        if (GUILayout.Button("TSB_CLose"))//關閉
        {
            s = false;
            //isGet = false;
            if (myThread != null)
			{
				SocketApp.isRun = false;
				if (myThread.IsAlive)
				{
					SocketApp.CloseAll();
					TCP_Handler.tcpClose();
					//printall("no Loacal IP");
					SocketApp.acceptDone.Set();
				}
				Thread.Sleep(100);          
				myThread.Abort();

			}
			if (SocketApp.transMode == (int)SocketApp.Transfer_Mode.M_UDP)
				SocketApp.CloseAll();
		}
        GUI.color = Color.white;

        if (GUILayout.Button("TSB_Activate"))//啟動連線
        {
            s = true;
			Debug.Log("TSB_Activate");
            string msg = "#send$";
            SocketApp.SendAll(msg);                                       // do things
            Debug.Log("send_activate");
            //StartCoroutine("Activate");//註解比較穩定
        }

        GUI.color = Color.gray;
        if (GUILayout.Button("TSB_Stop"))//暫停
        {
            s = false;
            Debug.Log("TSB_Stop");
            string msg = "#stop$";
            SocketApp.SendAll(msg);                                       // do things
            Debug.Log("send_stop");
        }

		if (GUILayout.Button("Restart"))//整個專案重新開始
        {
			Debug.Log("Restart");
			SocketApp.TCP_clientList.Clear ();
			Application.LoadLevel(Application.loadedLevel);

		}
        GUI.color = Color.white;

        GUILayout.Label ("");
		GUILayout.Label("connected sensors: ");
		if (TCP_Handler.SID_List.Count > 0) {
			foreach (int ID in TCP_Handler.SID_List) {
                //Debug.Log("sensor ID: " + ID);
                sensornumber += ID.ToString();
				sensornumber += " ";
			}
			TCP_Handler.SID_List.Clear();
		}
        GUILayout.Label(sensornumber);//目前連線的sensor
        if (TCP_Handler.sensor_num_ID.Count > 0 && s == true)//activate後傳值的數量
        {
            foreach (int ID in TCP_Handler.sensor_num_ID)
            {
                Debug.Log("sensor ID: " + ID);
            }
            
           
        }
    }

    private void ServerProcess()
    {
        TCP_Handler.Initial();
    }
/*
    private IEnumerator Activate()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f); // wait half a second
            string msg = "#send$";
            SocketApp.SendAll(msg);                                       // do things
            Debug.Log("send_activate");
        }
    }

    private IEnumerator Stop()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f); // wait half a second
            string msg = "#stop$";
            SocketApp.SendAll(msg);                                       // do things
            Debug.Log("send_stop");
        }
    }
 */

}




