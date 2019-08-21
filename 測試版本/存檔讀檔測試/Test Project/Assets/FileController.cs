using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

public class FileController : MonoBehaviour {
    //save_initial
    public int N = 1;//編號
    private int data_num = 0;//檔案名字編號
    private string data = "";
    private bool savedataflag = false;
    //load_initial
    private int t = 0;
    private bool loaddataflag = false;
    // Use this for initialization
    void Start () {
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

        //load_start
        /*
        Debug.Log (CSV.GetInstance().m_ArrayData.Count);
        Debug.Log("Name : " + CSV.GetInstance ().getString(0, 0));//Number
        Debug.Log("Name : " + CSV.GetInstance ().getString(0, 1));//Position_x
        Debug.Log("Name : " + CSV.GetInstance ().getString(0, 2));//Position_y
        Debug.Log("Name : " + CSV.GetInstance ().getString(0, 3));//Position_z
        Debug.Log("Name : " + CSV.GetInstance ().getString(0, 4));//Rotation_x
        Debug.Log("Name : " + CSV.GetInstance ().getString(0, 5));//Rotation_y
        Debug.Log("Name : " + CSV.GetInstance ().getString(0, 6));//Rotation_z
        */
    }
    
	void FixedUpdate(){
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

    void SaveRawData(int num)
    {

        //設定csv檔案位置
        string strPath = "C:/Users/CILS/Desktop/SensorData_" + num.ToString() + ".csv";
        Debug.Log("savefile:" + "SensorData_" + num.ToString() + ".csv");

        //開啟CSV檔案
        FileStream fs = new FileStream(strPath, FileMode.Create);
        StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);

        //修改檔案為非唯讀屬性(Normal)
        System.IO.FileInfo FileAttribute = new FileInfo(strPath);
        FileAttribute.Attributes = FileAttributes.Normal;

        //加入資料，注意！如果原本有資料的csv，會被覆蓋。
        //若是需要寫入其中幾個欄位，可先讀取csv存入string，
        //在對其中幾個欄位修改，並Write全部已修改資料
        sw.Write(data);
        sw.Close();
    }

    void LoadRawData(int t,int num)
    {
        CSV.GetInstance().loadFile("C:/Users/CILS/Desktop/SensorData_" + num.ToString() + ".csv");
        Debug.Log("loadfile:" + "SensorData_" + num.ToString() + ".csv");
        //判斷是否讀完值
        if (t<= CSV.GetInstance().m_ArrayData.Count - 1)
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
            transform.rotation = Quaternion.Euler ((float)Rotation_x, (float)Rotation_y, (float)Rotation_z);

            //transform.Rotate (Gx*Time.deltaTime, 0, 0);//加速度
        }
        else
        {
            Debug.Log("finish!");
            loaddataflag = false;
        }
    }

    

    private void OnGUI()
    {
        if (GUILayout.Button("save"))
        {
            savedataflag = true;
            Debug.Log("save data!!");
        }

        if (GUILayout.Button("load"))
        {
            loaddataflag = true;
            Debug.Log("load data!!");
        }
    }

}
