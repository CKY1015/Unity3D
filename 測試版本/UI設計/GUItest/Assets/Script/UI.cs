using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System;

public enum GameState { playing, won, lost };

public class UI : MonoBehaviour {

    ///////////////////////////////連線初始化///////////////////////////////
    public float time_f = 0;
    public int state = 100;
    private Thread myThread;
    private bool open = false;
    private bool s = false;
    private Rect windowCon = new Rect(Screen.width / 2 - 60, Screen.height / 2 - 50, 120, 50);
    public int Roll_num = 0;
    public int Pitch_num = 0;
    public int Yaw_num = 0;
    private GUIStyle guiStyle = new GUIStyle(); //create a new variable
    public string sensornumber = "";

    ///////////////////////////////讀存檔初始化///////////////////////////////
    //save_initial
    public int N = 1;//編號
    private int data_num = 0;//檔案名字編號
    private string data = "";
    private bool savedataflag = false;
    //load_initial
    private int t = 0;
    private bool loaddataflag = false;
    private string datastate = "";

    ///////////////////////////////遊戲模式///////////////////////////////
    public static UI SP;
    public float movementSpeed = 6.0f;
    private int totalGems;
    private int foundGems;
    private GameState gameState;
    private bool time_b = false;
    public int mode_flag = 1;
    public string countdownText = "";
    private GUIStyle guiNumber = new GUIStyle();
    private Rect windowWon = new Rect(Screen.width / 2 - 60, Screen.height / 2 - 50, 120, 50);
    private Rect windowLost = new Rect(Screen.width / 2 - 60, Screen.height / 2 - 50, 120, 50);

    void Awake()
    {
        Application.targetFrameRate = -1;//固定fps

        SP = this;
        foundGems = 0;
        gameState = GameState.playing;
        totalGems = GameObject.FindGameObjectsWithTag("Picker").Length;
        Time.timeScale = 1.0f;
    }

    // Use this for initialization
    void Start()
    {
        //save_start
        data = data + "Number";
        data = data + ",";
        data = data + "Position_x";
        data = data + ",";
        data = data + "Position_y";
        data = data + ",";
        data = data + "Position_z";
        data = data + ",";
        data = data + "Rotation_x";
        data = data + ",";
        data = data + "Rotation_y";
        data = data + ",";
        data = data + "Rotation_z";
        data = data + "\n";
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        ///////////////////////////////連線部分///////////////////////////////
        if (s == true)
        {
            if (time_b == true)
            {
                time_f = time_f + Time.deltaTime;
            }
            //Debug.Log("" + TCP_Handler.pitch);
            //Debug.Log("" + TCP_Handler.roll_status);
            transform.rotation = Quaternion.Euler(-(float)TCP_Handler.pitch, -(float)TCP_Handler.yaw, -(float)TCP_Handler.roll);
            //Thread.Sleep(3); //Delay
        }

        //state = TCP_Handler.status;
        Roll_num = TCP_Handler.roll_status;
        Pitch_num = TCP_Handler.pitch_status;
        Yaw_num = TCP_Handler.yaw_status;

        ///////////////////////////////讀存檔部分///////////////////////////////
        //save_update
        if (savedataflag == false)
        {
            //編號
            data = data + N.ToString();
            data = data + ",";
            //x位置
            data = data + transform.localPosition.x.ToString();
            data = data + ",";
            //y位置
            data = data + transform.localPosition.y.ToString();
            data = data + ",";
            //z位置
            data = data + transform.localPosition.z.ToString();
            data = data + ",";
            //x角度
            data = data + transform.localEulerAngles.x.ToString();
            data = data + ",";
            //y角度
            data = data + transform.localEulerAngles.y.ToString();
            data = data + ",";
            //z角度
            data = data + transform.localEulerAngles.z.ToString();
            data = data + "\n";

            N++;
        }
        else if (savedataflag == true)
        {
            data_num++;
            SaveRawData(data_num);
            //歸零用
            savedataflag = false;
            data = "";
            N = 1;
        }
        //load_update
        if (loaddataflag == true)
        {
            t++;
            LoadRawData(t, data_num);
        }
    }
  
    void OnGUI()
    {
        //(x位置, y位置, x長度, y長度)
        ///////////////////////////////////////BUTTON///////////////////////////////////////
        GUI.Box(new Rect(10, 10, 200, 500), "Button");

        ///////////////////////////////連線部分///////////////////////////////
        GUI.color = Color.green;
        GUI.Box(new Rect(20, 30, 180, 100), "連線:");
        GUI.color = Color.white;
        if (GUI.Button(new Rect(40, 60, 70, 50), "ON"))//開始連線(ON)
        {
            Debug.Log("TSB_Open");
            FileInfo.loadSettingFile();
            Debug.Log("TSB_Start");
            myThread = new Thread(new ThreadStart(ServerProcess));
            myThread.IsBackground = true;
            myThread.Start();
            open = true;
        }
        if (SocketApp.TCP_clientList.Count == 3 && open == true)//確定3顆都連線
        {
            windowCon = GUI.Window(0, windowCon, Constate, "All Connected!");
            //StartCoroutine("Activate");//註解比較穩定
        }
        if (GUI.Button(new Rect(115, 60, 70, 50), "OFF"))//關閉連線(OFF)
        {
            Debug.Log("TSB_Stop");
            string msg = "#stop$";
            SocketApp.SendAll(msg);// do things
            Debug.Log("send_stop");

            Debug.Log("TSB_Close");
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
                Debug.Log("Abort");
            }
            if (SocketApp.transMode == (int)SocketApp.Transfer_Mode.M_UDP)
                SocketApp.CloseAll();
            
            sensornumber = "";
            SocketApp.TCP_clientList.Clear();
        }

        ///////////////////////////////讀存檔部分///////////////////////////////
        GUI.color = Color.green;
        GUI.Box(new Rect(20, 140, 180, 70), "軌跡紀錄:");
        GUI.color = Color.white;
        if (GUI.Button(new Rect(55, 170, 50, 20), "Save"))//存檔
        {
            savedataflag = true;
            datastate = "Save";
            Debug.Log("save data!!");
        }
        if (GUI.Button(new Rect(110, 170, 50, 20), "Load"))//讀檔
        {
            loaddataflag = true;
            datastate = "Load";
            Debug.Log("load data!!");
        }

        ///////////////////////////////遊戲部分///////////////////////////////
        GUI.color = Color.green;
        GUI.Box(new Rect(20, 220, 180, 90), "遊戲模式:");
        GUI.color = Color.white;
        if (GUI.Button(new Rect(30, 250, 50, 20), "Mode1"))//模式1
        {
            Time.timeScale = 1;
            mode_flag = 1;
            time_b = true;
            time_f = 0;
        }
        if (GUI.Button(new Rect(85, 250, 50, 20), "Mode2"))//模式2
        {
            Time.timeScale = 2;
            mode_flag = 2;
            time_b = true;
            time_f = 0;
        }
        if (GUI.Button(new Rect(140, 250, 50, 20), "Mode3"))//模式3
        {
            Time.timeScale = 3;
            mode_flag = 3;
            time_b = true;
            time_f = 0;
        }
        if (gameState == GameState.lost)
        {
            GUI.color = Color.red;
            windowWon = GUI.Window(0, windowWon, Gamestate, "You Lost!");
            GUI.color = Color.white;
        }
        if (gameState == GameState.won)
        {
            GUI.color = Color.green;
            windowLost = GUI.Window(1, windowLost, Gamestate, "You Won!");
            GUI.color = Color.white;
        }

        ///////////////////////////////系統部分///////////////////////////////
        GUI.color = Color.green;
        GUI.Box(new Rect(20, 320, 180, 90), "系統相關:");
        GUI.color = Color.white;
        if (GUI.Button(new Rect(40, 350, 70, 20), "Play"))//開始
        {
            StartCoroutine(down());
        }
        if (GUI.Button(new Rect(115, 350, 70, 20), "Pause"))//暫停
        {
            Time.timeScale = 0;
        }
        if (GUI.Button(new Rect(40, 380, 70, 20), "Continue"))//繼續
        {
            Time.timeScale = 1;
        }
        if (GUI.Button(new Rect(115, 380, 70, 20), "Restart"))//重整
        {
            Debug.Log("Restart");
            SocketApp.TCP_clientList.Clear();
            Application.LoadLevel(Application.loadedLevel);
        }


        ///////////////////////////////////////DATA///////////////////////////////////////
        GUI.Box(new Rect(Screen.width - 110, 10, 100, 500), "Data");

        ///////////////////////////////系統部分///////////////////////////////
        GUI.color = Color.green;
        GUI.Label(new Rect(Screen.width - 100, 30, 50, 20), "時間:");
        GUI.color = Color.white;
        switch (mode_flag)
        {
            case 1:
                GUI.TextField(new Rect(Screen.width - 100, 50, 85, 20), "Times: " + (int)time_f / 60 + "m" + (int)time_f % 60 + "s");
                break;
            case 2:
                GUI.TextField(new Rect(Screen.width - 100, 50, 85, 20), "Times: " + (int)time_f / 2 / 60 + "m" + (int)time_f / 2 % 60 + "s");
                break;
            case 3:
                GUI.TextField(new Rect(Screen.width - 100, 50, 85, 20), "Times: " + (int)time_f / 3 / 60 + "m" + (int)time_f / 3 % 60 + "s");
                break;
        }
        GUI.color = Color.green;
        GUI.Label(new Rect(Screen.width - 100, 70, 50, 20), "角度:");
        GUI.color = Color.white;
        GUI.TextField(new Rect(Screen.width - 100, 90, 85, 20), "Pitch: " + TCP_Handler.pitch.ToString("0.000"));
        GUI.TextField(new Rect(Screen.width - 100, 110, 85, 20), "Roll: " + TCP_Handler.roll.ToString("0.000"));
        GUI.TextField(new Rect(Screen.width - 100, 130, 85, 20), "Yaw: " + TCP_Handler.yaw.ToString("0.000"));

        ///////////////////////////////連線部分///////////////////////////////
        GUI.color = Color.green;
        GUI.Box(new Rect(Screen.width - 105, 160, 90, 110), "連線狀況:");
        GUI.color = Color.white;
        GUI.Label(new Rect(Screen.width - 95, 180, 85, 20), "Sensors:");
        if (TCP_Handler.SID_List.Count > 0)
        {
            foreach (int ID in TCP_Handler.SID_List)
            {
                //Debug.Log("sensor ID: " + ID);
                sensornumber += ID.ToString();
                sensornumber += " ";
            }
            TCP_Handler.SID_List.Clear();
        }
        GUI.TextField(new Rect(Screen.width - 102, 200, 85, 20), sensornumber);
        GUI.Label(new Rect(Screen.width - 95, 220, 85, 20), "Numbers:");
        GUI.TextField(new Rect(Screen.width - 102, 240, 85, 20), SocketApp.TCP_clientList.Count.ToString());
        if (TCP_Handler.sensor_num_ID.Count > 0 && s == true)//activate後傳值的數量
        {
            foreach (int ID in TCP_Handler.sensor_num_ID)
            {
                Debug.Log("sensor ID: " + ID);
            }
        }

        ///////////////////////////////讀存檔部分///////////////////////////////
        GUI.color = Color.green;
        GUI.Box(new Rect(Screen.width - 105, 280, 90, 60), "檔案結果:");
        GUI.color = Color.white;
        GUI.TextField(new Rect(Screen.width - 102, 310, 85, 20), datastate);

        ///////////////////////////////遊戲部分///////////////////////////////
        GUI.color = Color.green;
        GUI.Box(new Rect(Screen.width - 105, 350, 90, 80), "遊戲分數:");
        GUI.color = Color.white;
        GUI.Label(new Rect(Screen.width - 100, 370, 85, 20), "Found gems:");
        GUI.TextField(new Rect(Screen.width - 102, 390, 85, 20), foundGems + "/" + totalGems);
        guiNumber.fontSize = 200;
        guiNumber.normal.textColor = Color.red;
        if (countdownText != "")
            GUI.Label(new Rect((Screen.width - 100) / 2 - 100, (Screen.height - 30) / 2, 100, 30), countdownText, guiNumber);
    }
    
    ///////////////////////////////連線部分///////////////////////////////
    private void ServerProcess()
    {
        TCP_Handler.Initial();
    }

    ///////////////////////////////讀存檔部分///////////////////////////////
    private void SaveRawData(int num)
    {
        //設定csv檔案位置
        string strPath = "C:/Users/CILS/Desktop/SensorData_" + num.ToString() + ".csv";
        Debug.Log("savefile:" + "SensorData_" + num.ToString() + ".csv");

        //開啟CSV檔案
        FileStream fs = new FileStream(strPath, FileMode.Create);
        StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);

        //修改檔案為非唯讀屬性(Normal)
        File.SetAttributes(strPath, FileAttributes.Normal);

        //加入資料，注意！如果原本有資料的csv，會被覆蓋。
        //若是需要寫入其中幾個欄位，可先讀取csv存入string，
        //在對其中幾個欄位修改，並Write全部已修改資料
        sw.Write(data);
        sw.Close();
    }

    private void LoadRawData(int t, int num)
    {
        CSV.GetInstance().loadFile("C:/Users/CILS/Desktop/SensorData_" + num.ToString() + ".csv");
        Debug.Log("loadfile:" + "SensorData_" + num.ToString() + ".csv");
        //判斷是否讀完值
        if (t <= CSV.GetInstance().m_ArrayData.Count - 1)
        {
            //Debug.Log(t);
            //position
            double Position_x = CSV.GetInstance().getDouble(t, 1);
            double Position_y = CSV.GetInstance().getDouble(t, 2);
            double Position_z = CSV.GetInstance().getDouble(t, 3);
            //rotation
            double Rotation_x = CSV.GetInstance().getDouble(t, 4);
            double Rotation_y = CSV.GetInstance().getDouble(t, 5);
            double Rotation_z = CSV.GetInstance().getDouble(t, 6);
            //Debug.Log(Position_y);
            //position呈現
            transform.position = new Vector3((float)Position_x, (float)Position_y, (float)Position_z);
            //rotation呈現
            transform.rotation = Quaternion.Euler((float)Rotation_x, (float)Rotation_y, (float)Rotation_z);

            //transform.Rotate (Gx*Time.deltaTime, 0, 0);//加速度
        }
        else
        {
            Debug.Log("finish!");
            loaddataflag = false;
        }
    }

    ///////////////////////////////遊戲部分///////////////////////////////
    IEnumerator down()
    {
        countdownText = "  3";
        yield return new WaitForSeconds(1);
        countdownText = "  2";
        yield return new WaitForSeconds(1);
        countdownText = "  1";
        yield return new WaitForSeconds(1);
        countdownText = "Go!";
        yield return new WaitForSeconds(1);
        countdownText = "";
        time_b = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Picker")
        {
            UI.SP.FoundGem();
            Destroy(other.gameObject);
        }
        else if (other.tag == "GameOver")
        {
            UI.SP.SetGameOver();
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
        gameState = GameState.won;
    }

    public void SetGameOver()
    {
        Time.timeScale = 0.0f; //Pause game
        gameState = GameState.lost;
    }

    public void Gamestate(int windowID)
    {
        if(gameState == GameState.lost)
        {
            if (GUI.Button(new Rect(10, 20, 100, 25), "Try again"))
            {
                Application.LoadLevel(Application.loadedLevel);
            }
        }
        else if (gameState == GameState.won)
        {
            if (GUI.Button(new Rect(10, 20, 100, 25), "Play again"))
            {
                Application.LoadLevel(Application.loadedLevel);
            }
        }


        // Make the windows be draggable.
        GUI.DragWindow(new Rect(0, 0, 10000, 10000));
    }

    public void Constate(int windowID)
    {
        if (GUI.Button(new Rect(10, 20, 100, 25), "OK"))
        {
            s = true;
            Debug.Log("TSB_Activate");
            string msg = "#send$";
            SocketApp.SendAll(msg);                                       // do things
            Debug.Log("send_activate");
            open = false;
        }
        // Make the windows be draggable.
        GUI.DragWindow(new Rect(0, 0, 10000, 10000));
    }
    
}
