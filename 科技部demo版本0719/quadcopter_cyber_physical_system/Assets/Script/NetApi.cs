using System;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Threading;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Linq;
using System.Diagnostics;
using System.Text;
//using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;


public class StateObject
{
    // Client  socket.
    public Socket workSocket = null;
    // Size of receive buffer.
    public const int BufferSize = 1024;
    // Receive buffer.
    public byte[] buffer = new byte[BufferSize];

    public SocketError errorcode;

    public bool IDchecked = false;



}

public class FileInfo
{
    public const string Datafilepath = @"C:\MR_Data\saveData\";
    public const string Datafilepath_temp = @"C:\MR_Data\saveData\temp\";
    public const string Califilepath = @"C:\MR_Data\caliData\";
    public const string Califilepath_temp = @"C:\MR_Data\caliData\temp\";
    public const string settinginfopath = @"C:\MR_Data\Setting_info.txt";
    //public static string CaliParameterPath = Directory.GetCurrentDirectory(); //0404
    public static string CaliParameterPath = @"C:\MR_Data\";

    public static void loadSettingFile()
    {
        string path = settinginfopath;
        StreamReader sr = new StreamReader(path);
        SocketApp.Port = Convert.ToInt32(sr.ReadLine());
        VirtualNetApp.SSID = sr.ReadLine();
        VirtualNetApp.Pwd = sr.ReadLine();
        sr.ReadLine();
        SocketApp.Freq = Convert.ToInt32(sr.ReadLine());

    }

}

public class VirtualNetApp
{

    public static string SSID = "Sensor_Network"; //default

    public static string Pwd = "12345678"; //default

    public static ProcessStartInfo pStart = new ProcessStartInfo("netsh.exe");

    public static string iniNetsh(String command)
    {
        pStart.Arguments = command;             // 定義指令
        pStart.CreateNoWindow = true;           // 不顯示執行視窗
        pStart.RedirectStandardOutput = true;   // 標準輸出啟用
        pStart.RedirectStandardInput = true;    // 標準輸入啟用
                                                //pstart.UserName = "administrator";
        pStart.UseShellExecute = false;
        // 執行 netsh
        Process p = Process.Start(pStart);
        // 串流讀取
        StreamReader sr = p.StandardOutput;
        // 顯示狀態
        string result = "";
        string temp;
        while ((temp = sr.ReadLine()) != null)
        {
            if (temp == "已啟動主控網路。 ")
                return "Success";
            else if (temp == "已停止主控網路。 ")
                return "Close";
            else
            {
                return "Default";
            }
            // 狀態顯示區自動往下捲動
            // StatusBox.Select(StatusBox.Text.Length, 0);
            // StatusBox.ScrollToCaret(); 
        }
        p.WaitForExit();                // 等待 netsh 關閉
        p.Close();                      // 關閉 netsh
        sr.Close();                 // 關閉串流

        return result;
    }

    public static string setLocalNetwork(string SSID, string PWD)
    {
        string result = "";
        // 定義無線網路
        //result = iniNetsh("wlan set hostednetwork mode=allow ssid=" + SSID + " key=" + PWD);
        // 啟動服務指令
        //result = iniNetsh("wlan start hostednetwork");
        return result;
    }

    public static string stopLocalNetwork()
    {
        string result = "";
        iniNetsh("wlan stop hostednetwork");
        return result;
    }


}

public static class SocketApp
{
    public static int Port = 5000;  //default
    public static int Freq = 50;    //default
    public static Socket _Server = null;
    public static bool isRun = true;
    public static List<Socket> TCP_clientList = new List<Socket>();
    public static List<IPEndPoint> UDP_clientList = new List<IPEndPoint>();
    public enum Transfer_Mode { M_TCP = 0, M_UDP };
    public static int transMode = (int)Transfer_Mode.M_TCP;     //Default
    public static ManualResetEvent acceptDone = new ManualResetEvent(false);

    public static string getLocalNetworkIP()
    {
        NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();   //取得所有網路介面類別(封裝本機網路資料)
        string ip = "192.168.137.1";
        return ip;
    }

    public static void removeClient(EndPoint removeEndPoint)
    {
        switch (transMode)
        {
            case (int)Transfer_Mode.M_TCP:
                for (int i = 0; i < TCP_clientList.Count; i++)
                {
                    if (TCP_clientList[i].RemoteEndPoint == removeEndPoint)
                        TCP_clientList.Remove(TCP_clientList[i]);
                }
                break;
            case (int)Transfer_Mode.M_UDP:
                for (int i = 0; i < UDP_clientList.Count; i++)
                {
                    if (UDP_clientList[i] == (IPEndPoint)removeEndPoint)
                        UDP_clientList.Remove(UDP_clientList[i]);
                }
                break;
        }

    }

    public static void addClient(Socket s)
    {
        TCP_clientList.Add(s);
    }

    public static void addClient(EndPoint ep)
    {
        IPEndPoint ipep = (IPEndPoint)ep;
        UDP_clientList.Add(ipep);
    }

    public static void CloseAll()
    {

        isRun = false;
        switch (transMode)
        {
            case (int)Transfer_Mode.M_TCP:
                foreach (Socket s in TCP_clientList)
                {
                    s.Disconnect(true);
                    //s.Dispose();
                    s.Close();
                }
                TCP_clientList.Clear();
                break;
            case (int)Transfer_Mode.M_UDP:
                string msg = "#close$";
                foreach (IPEndPoint clienEP in UDP_clientList)
                {
                    EndPoint ep = (EndPoint)clienEP;
                    Send(ep, msg);
                    Thread.Sleep(3);
                }
                UDP_clientList.Clear();
                break;
        }

        _Server.Close();
        //_Server.Dispose();

    }

    public static void SendAll(String msg)
    {
        switch (transMode)
        {
            case (int)Transfer_Mode.M_TCP:
                foreach (Socket handler in TCP_clientList)
                {
                    Send(handler, msg);
                    Thread.Sleep(3);
                }
                break;
            case (int)Transfer_Mode.M_UDP:
                foreach (IPEndPoint clienEP in UDP_clientList)
                {
                    EndPoint ep = (EndPoint)clienEP;
                    Send(ep, msg);
                    Thread.Sleep(3);
                }
                break;
        }

    }

    public static void Send(Socket handler, String data)
    {
        byte[] byteData = Encoding.UTF8.GetBytes(data);

        handler.Send(byteData, 0, byteData.Length, SocketFlags.None);
        // Begin sending the data to the remote device.
        //handler.BeginSend(byteData, 0, byteData.Length, 0,
        //    new AsyncCallback(SendCallback), handler);
    }

    public static void Send(EndPoint clientEP, String data)
    {
        // Convert the string data to byte data using ASCII encoding.
        byte[] byteData = Encoding.UTF8.GetBytes(data);

        // Begin sending the data to the remote device.
        _Server.SendTo(Encoding.UTF8.GetBytes(data), clientEP);
    }
}
