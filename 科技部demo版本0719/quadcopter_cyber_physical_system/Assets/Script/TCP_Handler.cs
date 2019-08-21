using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
//using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Net.NetworkInformation;
using UnityEngine;
//using System.Collections;


public delegate void UpdateLableHandler(string text);
    
public class TCP_Handler
    {
        public static double pitch=0,roll=0,yaw=0;
        public static IMU[] Sensors = new IMU[20];
        public static List<int> SID_List = new List<int>();
        public static List<int> sensor_num_ID = new List<int>();//add by cky
        public string SSID = "MR_network";
        public string Password = "12345678";
        public static ProcessStartInfo pStart = new ProcessStartInfo("netsh.exe");

        //public static DateTime StartTime = new DateTime();
        //public static DateTime StopTime = new DateTime();
        //public static TimeSpan ts;
        public static Socket Server;
        public static string LocalIP, showMsg;
        public static List<Socket> clientList = new List<Socket>();
        public static string foldername = FileInfo.Datafilepath_temp;
        public static bool isInitial = true;
        //public static bool isRun = true;
        public static StreamWriter sw;
        public static int PortNumber = 5000;
        public static ManualResetEvent acceptDone = new ManualResetEvent(false);
        //public static int client_count = 0;
        public static int count = 0;
        public static int count2 = 0;
        public static int count4 = 0;
        //public const int GYRO_S = 2000;
        //public const int ACC_S = 4;
        //public const int MAG_S = 8;
        public static int roll_status = 0;
        public static int pitch_status = 0;
        public static int yaw_status = 0;
        private static bool  Gimbaloffset_flag_Y = false;
        private static bool Gimbaloffset_flag_R = false;
        private static bool Gimbaloffset_flag_P = false;
        public static double yawoffset = 0, pitchoffset = 0, rolloffset = 0;
        public static int count_pitch = 0;
        public static int count_roll = 0;
        public static int count_yaw = 0;
        public static void Initial()
        {
            LocalIP = SocketApp.getLocalNetworkIP();
            if (LocalIP == "")
            {
                //MessageBox.Show("錯誤! 沒有取得任何IP!! 請檢查網路連線...");
                while (true) ;
            }
            //huangting
            //myForm.Invoke(new UpdateLableHandler(myForm.printIP), LocalIP);
           
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(LocalIP), PortNumber);
            Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            SocketApp._Server = Server;
            //       

            Server.Bind(localEndPoint);
            Server.Listen(10); //指定最大連接數
            //to test if the client is disconnected
            //Server.IOControl(IOControlCode.KeepAliveValues, KeepAlive(1, 1000, 1000), null);

            while (isInitial)
            {
                try
                {
                    //=============Asynchronous architecture===============================
                    while (SocketApp.isRun)
                    {
                        SocketApp.acceptDone.Reset();
                        showMsg = "Waiting for a connecting...\n";
                        //huangting
                        //myForm.Invoke(new UpdateLableHandler(myForm.printDialog), showMsg);
                        Server.BeginAccept(new AsyncCallback(AcceptCallback), Server);
                        SocketApp.acceptDone.WaitOne();
                    }
                }
                catch (ThreadAbortException e)
                {
                    //huangting
                    //MessageBox.Show(e.ToString());
                }
                finally
                {

                }
            }

        }

        private byte[] KeepAlive(int onOff, int keepAliveTime, int keepAliveInterval)
        {
            byte[] buffer = new byte[12];
            BitConverter.GetBytes(onOff).CopyTo(buffer, 0);
            BitConverter.GetBytes(keepAliveTime).CopyTo(buffer, 4);
            BitConverter.GetBytes(keepAliveInterval).CopyTo(buffer, 8);
            return buffer;
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            
            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;

            try
            {
                if (listener != null)
                {
                    Socket client = listener.EndAccept(ar);
                    client.ReceiveTimeout = 200;
                    SocketApp.addClient(client);
                    //clientList.Add(client);
                    showMsg = "CONNECTED!\n";
                    //huangting
                    //myForm.Invoke(new UpdateLableHandler(myForm.printDialog), showMsg);
                    showClientName();
                    // Signal the main thread to continue.
                    SocketApp.acceptDone.Set();

                    // Create the state object.
                    StateObject state = new StateObject();
                    state.workSocket = client;
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                       out state.errorcode, new AsyncCallback(ReceiveCallback), state);
                }
            }
            catch( ObjectDisposedException e)
            {
                //do nothing
            }
            catch (Exception e)
            {
                //huangting
                //MessageBox.Show(e.ToString());
            }             
        }

        public static void ReceiveCallback(IAsyncResult ar)
        {
            String Gimbal_content = String.Empty;
            String content = String.Empty;
            List<byte> data = new List<byte>();
            double[] data_f = new double[9];
            double[] data_9axis = new double[9];
            double[] acc = new double[3];
            double[] gyr = new double[3];
            double[] mag = new double[3];
            int deviceID, SN;
           
            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            
            Socket handler = state.workSocket;
            
            
            
            try
            {               
                // Read data from the client socket. 
                int bytesRead = handler.EndReceive(ar);

                
                if (bytesRead > 0)
                {
                    for (int i = 0; i <= bytesRead - 27; i++ )
                    {
                        if (state.buffer[i] == 35 && state.buffer[i + 1] == 222 && state.buffer[i + 2] == 233 &&
                                state.buffer[i + 24] == 233 && state.buffer[i + 25] == 222 && state.buffer[i + 26] == 36)
                        {
                            //Get the data from packet
                            for (int index = i + 3; index <= i + 23; index++)
                                data.Add(state.buffer[index]);

                            if (!state.IDchecked)
                            {
                                deviceID = data[0];
                                SID_List.Add(deviceID);
                                sensor_num_ID.Add(deviceID);
                                state.IDchecked = true;
                                Sensors[deviceID] = new IMU();
                                //System.Diagnostics.Debug.Print("idchecked");
                            }
                            else
                            {

                                count++;
                                //myForm.Invoke(new UpdateLableHandler(myForm.printRemsg), count.ToString());
                                //System.Diagnostics.Debug.Print("Thread ID: ");
                                //System.Diagnostics.Debug.Print(Thread.CurrentThread.ManagedThreadId.ToString());

                                deviceID = data[0];//packetData[3]
								
                                SN = (data[1] << 8 | data[2]);
                                //Gyro
                                gyr[0] = (short)(data[3] << 8 | data[4]);
                                gyr[1] = (short)(data[5] << 8 | data[6]);
                                gyr[2] = (short)(data[7] << 8 | data[8]);
                                //Acc
                                acc[0] = (short)(data[9] << 8 | data[10]);
                                acc[1] = (short)(data[11] << 8 | data[12]);
                                acc[2] = (short)(data[13] << 8 | data[14]);
                                //Mag
                                mag[0] = (short)(data[15] << 8 | data[16]);
                                mag[1] = (short)(data[17] << 8 | data[18]);
                                mag[2] = (short)(data[19] << 8 | data[20]);

                            // (""+ gyr[0]+" " +gyr[1]+" "+ gyr[2]);
                               // TCP_Handler.pitch = acc[0];
                               // TCP_Handler.roll = acc[1];
                               // TCP_Handler.yaw = acc[2];
                            double degree = 0;
                            double temp = 0;
                            int range = 0;
                            if (count == 1)
                            {
                                Gimbaloffset_flag_Y = true;
                                Gimbaloffset_flag_R = true;
                                Gimbaloffset_flag_P = true;
                            }
                            switch (deviceID)//看哪個sensor
                            {
                                case 11: //yaw
                                    temp = acc[0];//磁角值(原本是加速度值，但韌體封包改成是磁角值，因為磁角的值只有一個)
                                    range = 1006;//0`1006(理論上0~3.3V，值為0~1023，但可變電阻是人工的，無法精準，所以可能調到0~3.2V，值為0~1006，每顆都不一樣)
                                    count_yaw++;
                                    yaw_status = count_yaw;
                                    if (Gimbaloffset_flag_Y)
                                    {
                                        yawoffset = temp / range * 360;//初始值
                                        Gimbaloffset_flag_Y = false;//設立flag使他只讀一次
                                    }
                                    degree = temp / range * 360 - yawoffset;
                                    if (degree >= 180)//限制在-180~180度
                                    {
                                        degree -= 360;
                                    }
                                    else if (degree < -180)
                                    {
                                        degree += 360;
                                    }
                                    TCP_Handler.yaw = degree;
									
                                    break;
                                case 12:  //roll
                                    temp = acc[0];
                                    range = 1013;
                                    count_roll++;
                                    roll_status = count_roll;
                                    if (Gimbaloffset_flag_R)
                                    {
                                        rolloffset = temp / range * 360;
                                        Gimbaloffset_flag_R = false;
                                    }
                                    degree = temp / range * 360 - rolloffset;
                                    if (degree >= 180)
                                    {
                                        degree -= 360;
                                    }
                                    else if (degree < -180)
                                    {
                                        degree += 360;
                                    }
                                    TCP_Handler.roll = degree;
                                    break;
                                case 13:  //pitch
                                    temp = acc[0];
                                    range = 1005;
                                    count_pitch++;
                                    pitch_status = count_pitch;
                                    if (Gimbaloffset_flag_P)
                                    {
                                        pitchoffset = temp / range * 360;
                                        Gimbaloffset_flag_P = false;
                                    }
                                    degree = temp / range * 360 - pitchoffset;
                                    if (degree >= 180)
                                    {
                                        degree -= 360;
                                    }
                                    else if (degree < -180)
                                    {
                                        degree += 360;
                                    }
                                    TCP_Handler.pitch = degree;
                                    break;
                            }




                            //System.Diagnostics.Debug.Print(gyr[0].ToString() + " " + gyr[1].ToString() + " " + gyr[2].ToString());
                            //System.Diagnostics.Debug.Print(acc[0].ToString() + " " + acc[1].ToString() + " " + acc[2].ToString());
                            //System.Diagnostics.Debug.Print(mag[0].ToString() + " " + mag[1].ToString() + " " + mag[2].ToString());
                            //System.Diagnostics.Debug.Print("========");
                            //Raw data tranformation (calculate the ture value)

                            /*******************/
                            // n = mag.Length
                            // for (int i = 0 ;i< n;i++)
                            /*******************/

                            Sensors[deviceID].SN = SN;

                            //System.Diagnostics.Debug.Print("SN: " + SN.ToString());
                            // Write the data to file
                            //data_f = Sensors[deviceID].data_f;

                            //content += SN + "\t";
                            //content += deviceID + "\t";
                            Gimbal_content += SN + "\t";
                            Gimbal_content += deviceID + "\t";
                            Gimbal_content += degree + "\t";

                            //Debug.Print("my parameter G:{0},Q:{1},A:{2}", IMU.gyr_gf[0, 0], IMU.Q[0, 0], IMU.acc_gf[0, 0]);
                            string filename = foldername + "Sensor" + deviceID.ToString() + ".txt";
                                StreamWriter sw = new StreamWriter(filename, true);
                                sw.WriteLine(Gimbal_content);
                                sw.Close();
                            }
                        }
                        else
                        {
                            content = "";
                            Gimbal_content = "";
                            //WritetoFile(content, 1);
                            data.Clear();
                        }
                    }
#region original method to get the data
                    //for (int i = bytesRead - 1; i >= 26; i--)
                    //    {
                    //        //Decode the packet received from client
                    //        if (state.buffer[i] == 36 && state.buffer[i - 1] == 222 && state.buffer[i - 2] == 233 &&
                    //            state.buffer[i - 24] == 233 && state.buffer[i - 25] == 222 && state.buffer[i - 26] == 35)
                    //        {
                    //            //Get the data from packet
                    //            for (int index = i - 23; index <= i - 3; index++)
                    //                data.Add(state.buffer[index]);

                    //            if (!state.IDchecked)
                    //            {
                    //                deviceID = data[0];
                    //                MR_Term.SID_List.Add(deviceID);
                    //                state.IDchecked = true;
                    //                MR_Term.Sensors[deviceID] = new IMU();
                    //                System.Diagnostics.Debug.Print("idchecked");
                    //            }
                    //            else
                    //            {

                    //                count++;
                    //                //myForm.Invoke(new UpdateLableHandler(myForm.printRemsg), count.ToString());
                    //                //System.Diagnostics.Debug.Print("Thread ID: ");
                    //                //System.Diagnostics.Debug.Print(Thread.CurrentThread.ManagedThreadId.ToString());

                    //                deviceID = data[0];
                    //                SN = (data[1] << 8 | data[2]);
                    //                //Gyro
                    //                gyr[0] = (short)(data[3] << 8 | data[4]);
                    //                gyr[1] = (short)(data[5] << 8 | data[6]);
                    //                gyr[2] = (short)(data[7] << 8 | data[8]);
                    //                //Acc
                    //                acc[0] = (short)(data[9] << 8 | data[10]);
                    //                acc[1] = (short)(data[11] << 8 | data[12]);
                    //                acc[2] = (short)(data[13] << 8 | data[14]);
                    //                //Mag
                    //                mag[0] = (short)(data[15] << 8 | data[16]);
                    //                mag[1] = (short)(data[17] << 8 | data[18]);
                    //                mag[2] = (short)(data[19] << 8 | data[20]);

                    //                //Raw data tranformation (calculate the ture value)
                    //                for (int n = 0; n < 3; n++)
                    //                {
                    //                    acc[n] = MR_Term.Sensors[deviceID].calcAcc(acc[n]);
                    //                    gyr[n] = MR_Term.Sensors[deviceID].calcGyro(gyr[n]);
                    //                    mag[n] = MR_Term.Sensors[deviceID].calcMag(mag[n]);
                    //                }

                    //                for (int n = 0; n < 3; n++)
                    //                {
                    //                    MR_Term.Sensors[deviceID].data_9axis[n] = acc[n];
                    //                    MR_Term.Sensors[deviceID].data_9axis[n + 3] = gyr[n];
                    //                    MR_Term.Sensors[deviceID].data_9axis[n + 6] = mag[n];
                    //                }


                    //                ++MR_Term.Sensors[deviceID].indexInChart;
                    //                MR_Term.Sensors[deviceID].SN = SN;
                    //                MR_Term.Sensors[deviceID].data_f = MR_Term.Sensors[deviceID].MovingAverage_life(acc, gyr, mag);

                    //                switch (MR_Term.Mode)
                    //                {
                    //                    case MR_Term.Operation_Mode.M_Demo:
                    //                        //====================Demo Mode==========================
                    //                        #region Demo mode
                    //                        if (MR_Term.Sensors[deviceID].indexInChart < 10)
                    //                        {
                    //                            for (int t = 0; t < 9; t++)
                    //                                MR_Term.Sensors[deviceID].data4IniAtt[t].Add(MR_Term.Sensors[deviceID].data_f[t]);
                    //                            System.Diagnostics.Debug.Print("Att1");
                    //                        }
                    //                        else if (MR_Term.Sensors[deviceID].indexInChart == 10)
                    //                        {
                    //                            for (int t = 0; t < 9; t++)
                    //                                MR_Term.Sensors[deviceID].DataAve[t] = IMU.mean(MR_Term.Sensors[deviceID].data4IniAtt[t]);

                    //                            MR_Term.Sensors[deviceID].q = MR_Term.Sensors[deviceID].initial_attitude(MR_Term.Sensors[deviceID].DataAve);
                    //                            MR_Term.Sensors[deviceID].mag_ref[0] = MR_Term.Sensors[deviceID].DataAve[6];
                    //                            MR_Term.Sensors[deviceID].mag_ref[1] = MR_Term.Sensors[deviceID].DataAve[7];
                    //                            MR_Term.Sensors[deviceID].mag_ref[2] = MR_Term.Sensors[deviceID].DataAve[8];
                    //                            //System.Diagnostics.Debug.Print(MR_Term.Sensors[deviceID].q[0].ToString() + " " + MR_Term.Sensors[deviceID].q[1].ToString() + " " + MR_Term.Sensors[deviceID].q[2].ToString() + " " + MR_Term.Sensors[deviceID].q[3].ToString());
                    //                            //System.Diagnostics.Debug.Print("Att2");


                    //                        }
                    //                        else
                    //                        {
                    //                            MR_Term.Sensors[deviceID].q = MR_Term.Sensors[deviceID].EKF_cal(MR_Term.Sensors[deviceID].data_f, 0.02);
                    //                            MR_Term.Sensors[deviceID].eul = MR_Term.Sensors[deviceID].getEular(MR_Term.Sensors[deviceID].q);
                    //                            MR_Term.Sensors[deviceID].q_out = MR_Term.Sensors[deviceID].getQout(MR_Term.Sensors[deviceID].eul);

                    //                            //MR_Term.Sensors[deviceID].eul = MR_Term.Sensors[deviceID].qua2eul(MR_Term.Sensors[deviceID].q);
                    //                            //System.Diagnostics.Debug.Print(deviceID.ToString() + ": " + MR_Term.Sensors[deviceID].eul[0].ToString() + " " + MR_Term.Sensors[deviceID].eul[1].ToString() + " " + MR_Term.Sensors[deviceID].eul[2].ToString());
                    //                            //System.Diagnostics.Debug.Print("Att3");
                    //                        }
                    //                        #endregion

                    //                        break;
                    //                    case MR_Term.Operation_Mode.M_Record:
                    //                        //====================Record Mode==========================
                    //                        #region Record Mode

                    //                        #endregion
                    //                        break;
                    //                    case MR_Term.Operation_Mode.M_Calibartion:
                    //                        //====================Calibration Mode==========================
                    //                        #region

                    //                        #endregion
                    //                        break;
                    //                }


                    //                //StopTime = DateTime.Now;
                    //                //ts = StopTime - StartTime;
                    //                //myForm.Invoke(new UpdateLableHandler(myForm.printTime), ts.ToString());
                    //                if (deviceID == MR_Term.n_SID)
                    //                    MR_Term.isGet = true;

                    //                // Write the data to file
                    //                data_f = MR_Term.Sensors[deviceID].data_f;
                    //                content += SN + "\t";
                    //                content += deviceID + "\t";

                    //                foreach (double d in data_f)
                    //                {
                    //                    content += Math.Round(d, 4) + "\t";
                    //                }
                    //                foreach (double e in MR_Term.Sensors[deviceID].eul)
                    //                {
                    //                    content += Math.Round(e, 2) + "\t";
                    //                }

                    //                string filename = foldername + "tdata" + deviceID.ToString() + ".txt";
                    //                StreamWriter sw = new StreamWriter(filename, true);
                    //                sw.WriteLine(content);

                    //                sw.Close();
                    //            }
                    //        }
                    //        else
                    //        {
                    //            content = "";
                    //            //WritetoFile(content, 1);
                    //            data.Clear();
                    //        }
                    //    }
#endregion
#region another receive method
                    //for (int i = 0; i < bytesRead; i++)
                    //{
                    //    switch(state.buffer[i])
                    //    {
                    //        case 35:    //'#'
                    //            start_flag = true;
                    //            break;
                    //        case 36:    //'$'
                    //            count++;
                    //            if (start_flag == true && data.Count == 21)
                    //            {
                    //                deviceID = data[0];
                    //                SN = (short)(data[1] << 8 | data[2]);
                    //                Acc_x = (short)(data[3] << 8 | data[4]);
                    //                Acc_y = (short)(data[5] << 8 | data[6]);
                    //                Acc_z = (short)(data[7] << 8 | data[8]);
                    //                Gyro_x = (short)(data[9] << 8 | data[10]);
                    //                Gyro_y = (short)(data[11] << 8 | data[12]);
                    //                Gyro_z = (short)(data[13] << 8 | data[14]);
                    //                Mag_x = (short)(data[15] << 8 | data[16]);
                    //                Mag_y = (short)(data[17] << 8 | data[18]);
                    //                Mag_z = (short)(data[19] << 8 | data[20]);
                    //                content = deviceID + "\t" + SN + "\t" + Acc_x + "\t" + Acc_y + "\t" + Acc_z + "\t" + Gyro_x + "\t" +
                    //                          Gyro_y + "\t" + Gyro_z + "\t" + Mag_x + "\t" + Mag_y + "\t" + Mag_z;
                   
                    //                //WritetoFile(content, deviceID);
                    //                string filename = foldername + "tdata" + deviceID.ToString() + ".txt";
                    //                StreamWriter sww = new StreamWriter(filename, true);
                    //                sww.WriteLine(content);
                    //                //sww.Flush();
                    //                sww.Close();
                    //            }
                    //            else
                    //            {
                    //                start_flag = false;
                    //                content = "";
                    //                //WritetoFile(content);
                    //                data.Clear();
                    //            }
                    //            myForm.Invoke( new UpdateLableHandler(myForm.printRemsg), count.ToString() );
                    //            break;
                    //        default:
                    //            if (start_flag == true)
                    //                data.Add(state.buffer[i]);
                    //            break;
                    //    }
                    //}  
#endregion
                }
                //continue to receive next data
                if (SocketApp.isRun)
                {
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    out state.errorcode, new AsyncCallback(ReceiveCallback), state);
                }
            }
            catch (ObjectDisposedException e)
            {
                //MessageBox.Show(e.ToString());
                //do nothing
            }
            catch (SocketException e)
            {
                if(SocketApp.isRun)
                {
                    SocketApp.removeClient(handler.RemoteEndPoint);
                    showClientName();
                } 
                handler.Disconnect(true);
                //handler.Shutdown(SocketShutdown.Both);
                //handler.Close();
            }
            finally
            {

            }
        
        }

        public static void removeClient(EndPoint removeEndPoint)
        {

            for(int i = 0; i<clientList.Count;i++)
            {
                if (clientList[i].RemoteEndPoint == removeEndPoint)
                    clientList.Remove(clientList[i]);              
            }
        }

        private static void showClientName()
        {
            string showClient = "";
            //huangting
            //myForm.Invoke(new UpdateLableHandler(myForm.printClient), "");
            foreach (Socket s in SocketApp.TCP_clientList)
            {
                showClient = s.RemoteEndPoint.ToString();
				
                //showClient += "\n";
                //huangting
                //myForm.Invoke(new UpdateLableHandler(myForm.printClient), showClient);
            }
        }

        public static string getLocalNetworkIP()    //move
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();   //取得所有網路介面類別(封裝本機網路資料)
            string ip = "";
            foreach (NetworkInterface adapter in nics)
            {
                
                if (adapter.NetworkInterfaceType.ToString() == "Wireless80211")
                {
                    //取得IPInterfaceProperties(可提供網路介面相關資訊)
                    IPInterfaceProperties ipProperties = adapter.GetIPProperties();
                    // Try to get the IPv4 interface properties.
                    
                    if (ipProperties.UnicastAddresses.Count > 0)
                    {
                        PhysicalAddress mac = adapter.GetPhysicalAddress();                     //取得Mac Address
                        string name = adapter.Name;                                             //網路介面名稱
                        string description = adapter.Description;                               //網路介面描述
                        
                        if(description.Contains("Microsoft Hosted Network Virtual Adapter"))
                        {
                            foreach (UnicastIPAddressInformation addressinfo in ipProperties.UnicastAddresses)
                            {
                                if (addressinfo.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) //只取得IPv4
                                {
                                    ip = addressinfo.Address.ToString();                //取得IP
                                    string netmask = addressinfo.IPv4Mask.ToString();  //取得遮罩
                                }
                            }
                        }
                    }
                }
            }
            return ip;
        }

        public static void tcpClose()
        {
            Server.Close();
            //huangting
            //Server.Dispose();
            //SocketApp.clientList.Clear();
            foreach(Socket s in SocketApp.TCP_clientList)
            {
                s.Disconnect(true);
                //huangting
                //s.Dispose();
                s.Close();
            }
            SocketApp.TCP_clientList.Clear();
            clientList.Clear();
            //huangting
            //myForm.Invoke(new UpdateLableHandler(myForm.printall), "no Local IP");
        }

        public static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.UTF8.GetBytes(data);

            handler.Send(byteData, 0, byteData.Length, SocketFlags.None);
            // Begin sending the data to the remote device.
            //handler.BeginSend(byteData, 0, byteData.Length, 0,
            //    new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                // Console.WriteLine("Sent {0} bytes to client.", bytesSent);         

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }

        public void WritetoFile(String s, int fileID)
        {
            string filename = foldername + "tdata" + fileID.ToString() + ".txt";
            sw = new StreamWriter(filename, true);           
            sw.WriteLine(s);
            //sw.Flush();
            sw.Close();
            //fs.Close();
        }


    }

public class IMU
{
    //inertial data
    public List<List<Double>> data4IniAtt = new List<List<Double>>();
    public List<Queue<Double>> data4Filter = new List<Queue<Double>>();
    public List<Queue<Double>> data4ChartScale = new List<Queue<Double>>();
    public double[] DataAve = new double[9];
    public int SN;
    public int indexInChart = 0;
    public double[] data_9axis = new double[9];     //the data before filtering
    public double[] data_f = new double[9];         //the data after filtering
    public double[] heading = new double[3];

    public int GYRO_S = 2000;  //245 500 2000 dps
    public int ACC_S = 16;      //2 4 8 16 g
    public int MAG_S = 4;       //4 8 12 16 gauss

    private double gRes, aRes, mRes;
    private double[] magSensitivity = { 0.00014, 0.00029, 0.00043, 0.00058 };


    //Calibration parameter
    //for LSM9DS1
    /*public static double[,] gyr_gf = new double[3, 4]{{1.0269487, -0.00062204723, -0.00116137170000000, -4.5728},
                                               {0, 1.0058793, -0.0039886963, 0.5525},
                                               {0, 0, 1.0042415, -0.511}};
                                               */
    public static double[,] gyr_gf = new double[3, 4] { { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
    /*
    public static double[,] acc_gf = new double[3, 4]{{1.0092454, -0.00029065399, 0.0031483538, 0.013141958},
                                                {0, 1.0135213, 0.00013901916, 0.015510642},
                                                {0, 0, 1.0015474, -0.0005644593}};
                                                */
    public static double[,] acc_gf = new double[3, 4] { { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
    //private double[,] mag_gf = new double[3, 4]{{1, 0, 0, 0},
    //                                            {0, 1, 0, 0},
    //                                            {0, 0, 1, 0}};
    /*
    public static double[,] mag_gf = new double[3, 4]{{2.1948192, 0.54550192, -0.12066915, 0.36811137},
                                                {0, 2.6755256, 0.079239344, -0.089641922},
                                                {0, 0, 2.5857721, -0.70593839}};
                                                */
    public static double[,] mag_gf = new double[3, 4] { { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
    //private double[,] gyr_gf = new double[3, 4]{{0.07* Math.PI / 180, 0, 0, 0},
    //                                           {0, 0.07* Math.PI / 180, 0, 0},
    //                                           {0, 0, 0.07* Math.PI / 180 , 0}};
    //private double[,] acc_gf = new double[3, 4]{{0.000732, 0, 0, 0},
    //                                           {0, 0.000732, 0, 0},
    //                                           {0, 0, 0.000732 , 0}};
    //private double[,] mag_gf = new double[3, 4]{{0.00014, 0, 0, 0},
    //                                           {0, 0.00014, 0, 0},
    //                                           {0, 0, 0.00014 , 0}};

    //EKF output
    public double[] q = new double[4];
    public double[] eul = new double[3];
    public double[] q_out = new double[4];
    public double[] mag_ref = new double[3] { 0, 0.5, 0 };

    //EKF covarance
    //for LSM9DS1
    private double[,] P = new double[4, 4] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } };
    public static double[,] Q = new double[3, 3] { { 1.5649398e-06, 0, 0 }, { 0, 1.2812551e-06, 0 }, { 0, 0, 1.4609595e-06 } };
    public static double[,] Ra = new double[3, 3] { { 4.7710718e-07, 0, 0 }, { 0, 2.0976752e-07, 0 }, { 0, 0, 6.5281553e-07 } };
    public static double[,] Rm = new double[3, 3] { { 3.2890209e-05, 0, 0 }, { 0, 4.1557457e-05, 0 }, { 0, 0, 0.00010077412 } };

    //private double[,] P = new double[4, 4] { { 1, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
    //private double[,] Q = new double[3, 3] { { 0.00054046971, 0, 0 }, { 0, 0.00023082297, 0 }, { 0, 0, 0.0003105731 } };
    //private double[,] Ra = new double[3, 3] { { 9.8045558E-006, 0, 0 }, { 0, 9.2416212e-006, 0 }, { 0, 0, 1.3880015e-005 } };
    //private double[,] Rm = new double[3, 3] { { 0.001473115, 0, 0 }, { 0, 0.0018639672, 0 }, { 0, 0, 0.0027275205 } };

    public IMU()
    {
        init();

    }
    public void init()
    {
        indexInChart = 0;
        heading = new double[3] { 0, 0, 0 };
        data4Filter.Clear();
        data4IniAtt.Clear();
        data4ChartScale.Clear();
        for (int i = 0; i < 9; i++)
        {
            data4Filter.Add(new Queue<Double>());
            data4IniAtt.Add(new List<Double>());
        }
        for (int i = 0; i < 4; i++)
        {
            data4ChartScale.Add(new Queue<Double>());
        }
    }
}

