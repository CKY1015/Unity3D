using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

public class SaveData : MonoBehaviour
{
    public bool savedataflag = false;
    public int i = 1;
    public int data_num = 1;
    private string data = "";

    // Use this for initialization
    void Start()
    {
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
    void Update()
    {
        //Debug.Log(transform.localEulerAngles.x);
        if (Input.GetKey("x"))
        {
            transform.Rotate(10 * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey("y"))
        {
            transform.Rotate(-10 * Time.deltaTime, 0, 0);
        }

        if (savedataflag == false)
        {
            //編號
            data = data + i.ToString();
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

            i++;
        }
        else if (savedataflag == true)
        {
            SaveRawData(data_num);
            data_num++;
            savedataflag = false;
            data = "";
            i = 1;
        }
    }

    void SaveRawData(int num)
    {

        //設定csv檔案位置
        string strPath = "C:/Users/CILS/Desktop/SensorData_" + num.ToString() + ".csv";

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

    private void OnGUI()
    {
        if (GUILayout.Button("save"))
        {
            savedataflag = true;
            Debug.Log("save data!!");
        }
    }
}