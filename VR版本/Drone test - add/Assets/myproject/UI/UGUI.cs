using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

public enum GameStateUI { playing, won, lost };

public class UGUI : MonoBehaviour
{
    Transform mytransform;

    ///////////////////////////////連線初始化///////////////////////////////
    private float time_f = 0;
    private Thread myThread;
    private bool open = false;
    private bool stop = false;
    private bool sys_state = false;
    private Rect windowCon = new Rect(Screen.width / 2 - 60, Screen.height / 2 - 50, 170, 50);
    private int Roll_num = 0;
    private int Pitch_num = 0;
    private int Yaw_num = 0;
    private GUIStyle guiStyle = new GUIStyle(); //create a new variable
    private string sensornumber = "";
    private float Yawoffset = 0;

    ///////////////////////////////讀存檔初始化///////////////////////////////
    //save_initial
    private int N = 1;//編號
    private int data_num = 0;//檔案名字編號
    private string data = "";
    private int savedataflag = 0;
    //load_initial
    private int t = 0;
    private bool loaddataflag = false;
    private string datastate = "";

    ///////////////////////////////遊戲模式///////////////////////////////
    public static UGUI SP;
    private float movementSpeed = 6.0f;
    private int totalGems;
    private int foundGems;
    private GameStateUI gameState;
    private bool time_b = false;
    private int mode_flag = 1;
    private string countdownText = "";
    private GUIStyle guiNumber = new GUIStyle();
    private Rect windowWon = new Rect(Screen.width / 2 - 60, Screen.height / 2 - 50, 120, 50);
    private Rect windowLost = new Rect(Screen.width / 2 - 60, Screen.height / 2 - 50, 120, 50);

    ///////////////////////////////UI///////////////////////////////
    public Text TIME;
    public Text PITCH;
    public Text ROLL;
    public Text YAW;
    public Text SENSORSTEXT;
    public Text NUMBERSTEXT;
    public Text DATASTATE;
    public Text FOUNDGEMSTEXT;

    public GameObject LEFT_ATTENTION;
    public GameObject RIGHT_ATTENTION;
    public GameObject UP_ATTENTION;
    public GameObject WIN_ATTENTION;
    public GameObject LOST_ATTENTION;

    void Awake()
    {
        Application.targetFrameRate = 120;//固定fps

        SP = this;
        foundGems = 0;
        gameState = GameStateUI.playing;
        totalGems = GameObject.FindGameObjectsWithTag("Picker").Length;
        Time.timeScale = 1.0f;
    }

    // Use this for initialization
    void Start()
    {
        mytransform = transform;
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

        Yawoffset = this.mytransform.localEulerAngles.y >180? this.mytransform.localEulerAngles.y-360:this.mytransform.localEulerAngles.y;
        Debug.Log(Yawoffset);

        LEFT_ATTENTION.SetActive(false);
        RIGHT_ATTENTION.SetActive(false);
        UP_ATTENTION.SetActive(false);
        WIN_ATTENTION.SetActive(false);
        LOST_ATTENTION.SetActive(false);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        ///////////////////////////////連線部分///////////////////////////////
        if (sys_state == true)
        {
            if (time_b == true)
            {
                time_f = time_f + Time.deltaTime;
            }
            //Debug.Log("" + TCP_Handler.pitch);
            //Debug.Log("" + TCP_Handler.roll_status);
            this.mytransform.rotation = Quaternion.Euler(-(float)TCP_Handler.pitch, Yawoffset - (float)TCP_Handler.yaw, -(float)TCP_Handler.roll);
            //Thread.Sleep(3); //Delay
        }

        Roll_num = TCP_Handler.roll_status;
        Pitch_num = TCP_Handler.pitch_status;
        Yaw_num = TCP_Handler.yaw_status;
        ///////////////////////////////讀存檔部分///////////////////////////////
        //save_update
        if (savedataflag == 1)
        {
            //編號
            data = data + N.ToString();
            data = data + ",";
            //x位置
            data = data + this.mytransform.localPosition.x.ToString();
            data = data + ",";
            //y位置
            data = data + this.mytransform.localPosition.y.ToString();
            data = data + ",";
            //z位置
            data = data + this.mytransform.localPosition.z.ToString();
            data = data + ",";
            //x角度
            data = data + this.mytransform.localEulerAngles.x.ToString();
            data = data + ",";
            //y角度
            data = data + this.mytransform.localEulerAngles.y.ToString();
            data = data + ",";
            //z角度
            data = data + this.mytransform.localEulerAngles.z.ToString();
            data = data + "\n";

            N++;
        }
        else if (savedataflag == 2)
        {
            savedataflag = 0;
            data_num++;
            SaveRawData(data_num);
            //歸零用
            data = "";
            N = 1;
        }
        //load_update
        if (loaddataflag == true)
        {
            t++;
            LoadRawData(t, data_num);
        }
        
        ///////////////////////////////////////DATA///////////////////////////////////////

        ///////////////////////////////系統部分///////////////////////////////
        switch (mode_flag)
        {
            case 1:
                TIME.text = "Times: " + (int)time_f / 60 + "m" + (int)time_f % 60 + "s";
                break;
            case 2:
                TIME.text = "Times: " + (int)time_f / 2 / 60 + "m" + (int)time_f / 2 % 60 + "s";
                break;
            case 3:
                TIME.text = "Times: " + (int)time_f / 3 / 60 + "m" + (int)time_f / 3 % 60 + "s";
                break;
        }
        PITCH.text = "Pitch: " + TCP_Handler.pitch.ToString("0.000");
        ROLL.text = "Roll: " + TCP_Handler.roll.ToString("0.000");
        YAW.text = "Yaw: " + TCP_Handler.yaw.ToString("0.000");

        ///////////////////////////////連線部分///////////////////////////////
        if (TCP_Handler.SID_List.Count > 0)
        {
            foreach (int ID in TCP_Handler.SID_List)
            {
                //Debug.Log("sensor ID: " + ID);
                sensornumber += ID.ToString();
                sensornumber += " ";
            }
            /*for (int ID = 0; ID <= 2; ID++)
            {
                sensornumber += TCP_Handler.SID_List[ID].ToString();
                sensornumber += " ";
            }*/
            TCP_Handler.SID_List.Clear();
        }
        SENSORSTEXT.text = sensornumber;
        NUMBERSTEXT.text = SocketApp.TCP_clientList.Count.ToString();

        ///////////////////////////////讀存檔部分///////////////////////////////
        DATASTATE.text = datastate;

        ///////////////////////////////遊戲部分///////////////////////////////
        FOUNDGEMSTEXT.text = foundGems + "/" + totalGems;

        ///////////////////////////////初始化///////////////////////////////
        if (Input.GetKey("c"))//校正
        {
            Debug.Log("C");
            TCP_Handler.Gimbaloffset_flag_P = true;
            TCP_Handler.Gimbaloffset_flag_R = true;
            TCP_Handler.Gimbaloffset_flag_Y = true;
        }
        if (Input.GetKey("r"))//reset
        {
            Debug.Log("R");
            //TCP_Handler.Gimbaloffset_flag_P = true;
            //TCP_Handler.Gimbaloffset_flag_R = true;
            //TCP_Handler.Gimbaloffset_flag_Y = true;

            this.mytransform.position = new Vector3(-468.2f, 23f, -492.7f);
        }
    }
    
    public void OnGUI()
    {
        //useGUILayout = false;
        if (SocketApp.TCP_clientList.Count >= 3 && open == true)//確定3顆都連線
        {
            GUI.color = Color.green;
            windowCon = GUI.Window(0, windowCon, Constate1, "System Connected!");
            GUI.color = Color.white;
            //StartCoroutine("Activate");//註解比較穩定
        }

        if (stop == true)//停止連線
        {
            GUI.color = Color.red;
            windowCon = GUI.Window(0, windowCon, Constate2, "System Disconnected?");
            GUI.color = Color.white;
        }
        
        if (gameState == GameStateUI.lost)//遊戲輸掉
        {
            GUI.color = Color.red;
            windowWon = GUI.Window(0, windowWon, Gamestate, "You Lost!");
            GUI.color = Color.white;
        }
        
        if (gameState == GameStateUI.won)//遊戲勝利
        {
            GUI.color = Color.green;
            windowLost = GUI.Window(1, windowLost, Gamestate, "You Won!");
            GUI.color = Color.white;
        }

        guiNumber.fontSize = 200;
        guiNumber.normal.textColor = Color.red;
        if (countdownText != "")//321倒數
            GUI.Label(new Rect((Screen.width - 100) / 2 - 100, (Screen.height - 30) / 2, 100, 30), countdownText, guiNumber);
    }

    //(x位置, y位置, x長度, y長度)
    ///////////////////////////////////////BUTTON///////////////////////////////////////
    ///////////////////////////////連線部分///////////////////////////////
    public void ON()//開始連線(ON)
    {
        Debug.Log("TSB_Open");
        FileInfo.loadSettingFile();
        Debug.Log("TSB_Start");
        myThread = new Thread(new ThreadStart(ServerProcess));
        myThread.IsBackground = true;
        myThread.Start();
        open = true;
    }

    public void OFF()//關閉連線(OFF)
    {
        Debug.Log("TSB_Stop");
        string msg = "#stop$";
        SocketApp.SendAll(msg);// do things
        Debug.Log("send_stop");
        stop = true;
    }

    ///////////////////////////////讀存檔部分///////////////////////////////
    public void SAVE()//存檔
    {
        savedataflag++;
        if (savedataflag == 1)
        {
            datastate = "Saving...";
        }
        else if (savedataflag == 2)
        {
            datastate = "Saved";
            Debug.Log("save data!!");
        }
    }
    public void LOAD()//讀檔
    {
        loaddataflag = true;
        datastate = "Loaded";
        Debug.Log("load data!!");
    }

    ///////////////////////////////遊戲部分///////////////////////////////
    public void MODE1()//模式1
    {
        Time.timeScale = 1;
        mode_flag = 1;
        time_b = true;
        time_f = 0;
    }
    public void MODE2()//模式2
    {
        Time.timeScale = 2;
        mode_flag = 2;
        time_b = true;
        time_f = 0;
    }
    public void MODE3()//模式3
    {
        Time.timeScale = 3;
        mode_flag = 3;
        time_b = true;
        time_f = 0;
    }

    ///////////////////////////////系統部分///////////////////////////////
    public void PLAY()//開始
    {
        time_b = true;
        //StartCoroutine(down());
    }
    public void PAUSE()//暫停
    {
        Time.timeScale = 0;
    }
    public void CONTINUE()//繼續
    {
        Time.timeScale = 1;
    }
    public void RESTART()//重整
    {
        Debug.Log("Restart");
        SocketApp.TCP_clientList.Clear();
        EditorSceneManager.LoadScene(EditorSceneManager.GetActiveScene().name);
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
        if(t == 1)
        {
            CSV.GetInstance().loadFile("C:/Users/CILS/Desktop/SensorData_" + num.ToString() + ".csv");
        }
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
            this.mytransform.position = new Vector3((float)Position_x, (float)Position_y, (float)Position_z);
            //rotation呈現
            this.mytransform.rotation = Quaternion.Euler((float)Rotation_x, (float)Rotation_y, (float)Rotation_z);

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
        if (other.CompareTag("Picker"))
        {
            UGUI.SP.FoundGem();
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("GameOver"))
        {
            UGUI.SP.SetGameOver();
        }
        else if (other.CompareTag("Hint_up"))
        {
            Debug.Log("up_test");
            InvokeRepeating("Up_attention", 0.1f, 0.5f);
        }
        else if (other.CompareTag("Hint_left"))
        {
            Debug.Log("left_test");
            InvokeRepeating("Left_attention", 0.1f, 0.5f);
        }
        else if (other.CompareTag("Hint_right"))
        {
            Debug.Log("right_test");
            InvokeRepeating("Right_attention", 0.1f, 0.5f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hint_up"))
        {
            Debug.Log("up_test");
            CancelInvoke("Up_attention");
            UP_ATTENTION.SetActive(false);
        }
        else if (other.CompareTag("Hint_left"))
        {
            Debug.Log("left_test");
            CancelInvoke("Left_attention");
            LEFT_ATTENTION.SetActive(false);
        }
        else if (other.CompareTag("Hint_right"))
        {
            Debug.Log("right_test");
            CancelInvoke("Right_attention");
            RIGHT_ATTENTION.SetActive(false);
        }
    }

    public void FoundGem()
    {
        foundGems++;
        if (foundGems >= totalGems)
        {
            gameState = GameStateUI.won;
            WIN_ATTENTION.SetActive(true);
            Invoke("WonGame", 0.1f);
        }
    }

    public void WonGame()
    {
        Time.timeScale = 0.0f; //Pause game
    }

    public void SetGameOver()
    {
        gameState = GameStateUI.lost;
        LOST_ATTENTION.SetActive(true);
        Time.timeScale = 0.0f; //Pause game
    }

    public void Up_attention()
    {
        if (UP_ATTENTION.activeSelf)
        {
            UP_ATTENTION.SetActive(false);
        }
        else
        {
            UP_ATTENTION.SetActive(true);
        }
    }

    public void Left_attention()
    {
        if (LEFT_ATTENTION.activeSelf)
        {
            LEFT_ATTENTION.SetActive(false);
        }
        else
        {
            LEFT_ATTENTION.SetActive(true);
        }
    }

    public void Right_attention()
    {
        if (RIGHT_ATTENTION.activeSelf)
        {
            RIGHT_ATTENTION.SetActive(false);
        }
        else
        {
            RIGHT_ATTENTION.SetActive(true);
        }
    }

    public void Gamestate(int windowID)
    {
        if (gameState == GameStateUI.lost)
        {
            if (GUI.Button(new Rect(10, 20, 100, 25), "Try again"))
            {
                EditorSceneManager.LoadScene(EditorSceneManager.GetActiveScene().name);
            }
        }
        else if (gameState == GameStateUI.won)
        {
            if (GUI.Button(new Rect(10, 20, 100, 25), "Play again"))
            {
                EditorSceneManager.LoadScene(EditorSceneManager.GetActiveScene().name);
            }
        }


        // Make the windows be draggable.
        GUI.DragWindow(new Rect(0, 0, 10000, 10000));
    }

    public void Constate1(int windowID)
    {
        if (GUI.Button(new Rect(10, 20, 150, 25), "OK"))
        {
            sys_state = true;
            Debug.Log("TSB_Activate");
            string msg = "#send$";
            SocketApp.SendAll(msg);                                       // do things
            Debug.Log("send_activate");
            open = false;

            //初始化
            TCP_Handler.Gimbaloffset_flag_P = true;
            TCP_Handler.Gimbaloffset_flag_R = true;
            TCP_Handler.Gimbaloffset_flag_Y = true;
        }
        // Make the windows be draggable.
        GUI.DragWindow(new Rect(0, 0, 10000, 10000));
    }

    public void Constate2(int windowID)
    {
        if (GUI.Button(new Rect(10, 20, 150, 25), "OK"))
        {
            Debug.Log("TSB_Close");
            sys_state = false;
            if (myThread != null)
            {
                SocketApp.isRun = false;
                if (myThread.IsAlive)
                {
                    SocketApp.CloseAll();
                    //TCP_Handler.tcpClose();
                    //printall("no Loacal IP");
                    //SocketApp.acceptDone.Set();
                }
                Thread.Sleep(100);
                myThread.Abort();
                Debug.Log("Abort");
            }

            stop = false;
            sensornumber = "";
            SocketApp.TCP_clientList.Clear();
        }
        // Make the windows be draggable.
        GUI.DragWindow(new Rect(0, 0, 10000, 10000));
    }
}
