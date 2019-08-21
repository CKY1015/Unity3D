using UnityEngine;
using System.Collections;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine.UI;

public class control : MonoBehaviour {
    public Text pitchText;              //顯示當前姿態角用
    public Text rollText;
    public Text yawText;
    public double pitch;
    public double roll;
    public double yaw;

	SerialPort sp;
	static int SID = 0;
	int r=0,init=0,t=0;
//	int UAr=0,FAr=0,UAl=0,FAl=0,CC=0,UTr=0,FTr=0,UTl=0,FTl=0,Hr=0,Hl=0,test=0;
	int Freq = 0, tmpcnt = 0;
	double t_tmp = 0.0;
	static double pi=3.14159265;
	byte[] bytearray=new byte[22];
	Queue<byte> dataQueue = new Queue<byte>();
	List<Byte> tempList = new List<Byte>();
//	List<Byte> tempBuf=new List<Byte>();
//	List<Byte> tempBuf2=new List<Byte>();
//	List<Byte> tempBuf3=new List<Byte>();
//	List<Byte> tempBuf4=new List<Byte>();
	double[] senR=new double[9];
	string receivedValue;
	float[] rotation1={0,0,0};
	float[] translate1={0,0,0};

	public GameObject CubeModule;
	//NCF sensor=new NCF();
	List<NCF> sensor=new List<NCF>();

	System.Text.Encoding iso_8859_1=System.Text.Encoding.GetEncoding("iso-8859-1");
    

	// Use this for initialization
	void Start () {
		sp = new SerialPort( "\\\\.\\COM4" , 921600, Parity.None, 8, StopBits.One);
		sp.Encoding=iso_8859_1;
		sp.NewLine = "E";
		sp.ReadTimeout=200;
		if (!sp.IsOpen){
			sp.Open();
		}

		for(int nu=0;nu<15;nu++){
			sensor.Add(new NCF());
		}

		///////////open file
		//		FileStream stream_9 = File.Open(@"C:\MRdata\data5.txt", FileMode.OpenOrCreate, FileAccess.Write);
		//		stream_9.Seek(0, SeekOrigin.Begin);
		//		stream_9.SetLength(0); 
		//		stream_9.Close();	
		//		//////////////////	
		//		FileStream st = File.Open(@"C:\MRdata\qdata5.txt", FileMode.OpenOrCreate, FileAccess.Write);
		//		st.Seek(0, SeekOrigin.Begin);
		//		st.SetLength(0); 
		//		st.Close();
		////////////////// 

		sensor[1].ReadCalibration(@"C:\Users\huangtingshieh\Desktop\inertial_demo\califile\L1.txt");
//		sensor[2].ReadCalibration(@"D:\Desktop_D\UnityProject\Unity_MT\califile\N8_s.txt");
//		sensor[3].ReadCalibration(@"D:\Desktop_D\UnityProject\Unity_MT\califile\L3.txt");
//		sensor[4].ReadCalibration(@"D:\Desktop_D\UnityProject\Unity_MT\califile\L4.txt");
//		sensor[5].ReadCalibration(@"D:\Desktop_D\UnityProject\Unity_MT\califile\L5.txt");
//		sensor[6].ReadCalibration(@"D:\Desktop_D\UnityProject\Unity_MT\califile\N6_s.txt");
//		sensor[7].ReadCalibration(@"D:\Desktop_D\UnityProject\Unity_MT\califile\N7_s.txt");
//		sensor[9].ReadCalibration(@"D:\Desktop_D\UnityProject\Unity_MT\califile\N9_s.txt");
//		sensor[10].ReadCalibration(@"D:\Desktop_D\UnityProject\Unity_MT\califile\L10.txt");
//		sensor[11].ReadCalibration(@"D:\Desktop_D\UnityProject\Unity_MT\califile\L11.txt");
//		sensor[12].ReadCalibration(@"D:\Desktop_D\UnityProject\Unity_MT\califile\L12.txt");

	}
	
	// Update is called once per frame
	void Update(){

		//rt++;
		//Debug.Log("Update time :" + Time.deltaTime);
		try
		{
			for(int se=0;se<10;se++){            //調整傳輸速度
				sp.NewLine="E";
				receivedValue = sp.ReadLine();
				bytearray = iso_8859_1.GetBytes(  receivedValue  );

				tempList.Clear(); 
				for(int d=0;d<bytearray.Length;d++){
					tempList.Add((Byte)bytearray[d]);
				}
				if(tempList.Count != 34){ 
					while(tempList.Count < 34){
						tempList.Add(69);
						t++;
						sp.NewLine="E";
						receivedValue = sp.ReadLine();
						bytearray = iso_8859_1.GetBytes(  receivedValue  );
						for(int d = 0; d < bytearray.Length; d++){
							tempList.Add((Byte)bytearray[d]);
						}
					}
				}

				if(tempList[0]==83 && tempList.Count==34){
					SID=tempList[2]; 
					sensor[SID].EKFAW_main(tempList);

//					if(SID == 1) UAr = UAr+1;
//					else if(SID == 2) FTr = FTr+1;
//					else if(SID == 3) FAr = FAr +1;
//					else if(SID == 4) UAl = UAl +1;
//					else if(SID == 5) FAl = FAl +1;
//					else if(SID == 6) CC = CC+1;
//					else if(SID == 7) UTr = UTr+1;
//					else if(SID == 8) test = test+1;
//					else if(SID == 9) UTl = UTl+1;
//					else if(SID == 10) FTl = FTl+1;
//					else if(SID == 11) Hr = Hr+1;
//					else if(SID == 12) Hl = Hl+1;

					r=r+1;

					if(Time.time - t_tmp >= 1.0){
						t_tmp = Time.time;
						Freq = r - tmpcnt;
						tmpcnt = r;
						print ("Freq="+Freq+" r="+r);
//						print ("Freq="+Freq+" r="+r+" CC"+CC+" UAr"+UAr+" FAr"+FAr+" UAl"+UAl+" FAl"+FAl+" UTr"+UTr+" FTr"+FTr+" UTl"+UTl+" FTl"+FTl+" Hr"+Hr+" Hl"+Hl+" test"+test);
//						CC = 0; UAr = 0; FAr = 0; UAl = 0; FAl = 0;UTr = 0;FTr = 0;UTl = 0;FTl = 0;Hr = 0;Hl = 0;test=0;
					}
				}

				if (Input.GetKey (KeyCode.C)){
					//InitialCalibration();
					//          print("SID: "+SID+" q_ini: "+sensor[SID].q_ini[0]+" "+sensor[SID].q_ini[1]+" "+sensor[SID].q_ini[2]+" "+sensor[SID].q_ini[3]);
					//          print("SID: "+SID+" q_rug: "+sensor[SID].q_rug[0]+" "+sensor[SID].q_rug[1]+" "+sensor[SID].q_rug[2]+" "+sensor[SID].q_rug[3]);
					//          print("SID: "+SID+" q_tes: "+sensor[SID].q_tes[0]+" "+sensor[SID].q_tes[1]+" "+sensor[SID].q_tes[2]+" "+sensor[SID].q_tes[3]);
					init=1;
					print("123");
				}

				if (init==1 ){
                    // CubeModule.transform.rotation = new Quaternion((float)sensor[SID].q[1], (float)sensor[SID].q[3], (float)sensor[SID].q[2], (float)sensor[SID].q[0]);
                    //print(SID+"  "+r);
                    switch (SID)
                    {
                        case 1:
                            CubeModule.transform.rotation = new Quaternion((float)sensor[1].q[1], -(float)sensor[1].q[3], (float)sensor[1].q[2], (float)sensor[1].q[0]);
                           // CubeModule.transform.rotation = new Quaternion((float)sensor[1].q[1], -(float)sensor[1].q[3] - (float)0.7, (float)sensor[1].q[2], (float)sensor[1].q[0] + (float)0.7);
                            break;
                        default:
                            break;
                    }

                }
			}   
		}

		catch (TimeoutException timeoutEx){
			print ("timeout");         // do nothing
		}
        catch (Exception ex)
        {
            sp.Close();
            if (!sp.IsOpen)
            {
                sp.Open();
            }
        }

        pitchText.text = "pitch_angle";
        rollText.text="roll_angle";
        yawText.text="yaw_angle";
        pitch=transform.localRotation.x;
        pitch = pitch / (0.7071067) * 90;
        pitchText.text = "" + pitch;
        roll = transform.localRotation.z;
        roll = roll / (-0.7071067) * 90;
        rollText.text = "" + roll;
        yaw = transform.localRotation.y;
        yaw = yaw / (0.7071067) * 90;
        yawText.text = "" + yaw;



    }



    public class NCF{
		public double[] q =new double[] {1,0,0,0};
		public int choose=0;
		double[] P={1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1};
		//double[] P={1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
		double[] Q=new double[9];  //GYRO
		double[] Ra=new double[9]; //ACC
		double[] Rm=new double[9];  //MAG
		public float[] qf=new float[4];
		public double[] qu=new double[4];
		public float[] quf=new float[4];
		public double[] inertial_cali=new double[9];
		//double r1=0.5,r2=130;
		double dT2=0.01;
		public double[] RUG={1,0,0,0,1,0,0,0,1};
		public double[] RGU={1,0,0,0,1,0,0,0,1};
		double[] initial_RM={1,0,0,0,1,0,0,0,1};
		double[] q_RM=new double[9];
		double[] euler=new double[3];
		double[] R=new double[9];
		double[] M={0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1};
		string line;
		public double[] sensor_cali=new double[63];  //calibration array
		double[] sum_iner ={0,0,0,0,0,0,0,0,0};
		public double[] eu=new double[3];
		public double[] eul=new double[3];
		public double[] eur=new double[3];

		double[] initial_RM_t0=new double[9];
		public double[] eu_t0=new double[3];
		public double[] eur_t0=new double[3];
		public double[] iner_for_q=new double[9]; 

		public double[] q_ini=new double[4];
		public double[] q_rug=new double[4];
		public double[] q_tes=new double[4];



		public int ReadCalibration(string caliaddress){
			StreamReader sr = new StreamReader(caliaddress);
			line=sr.ReadLine();
			sensor_cali=Array.ConvertAll(line.Split(' '),new Converter<string,double>(Double.Parse));
			for(int b=0;b<9;b++){
				Q[b]=sensor_cali[b+12];
				Ra[b]=sensor_cali[b+33];
				Rm[b]=sensor_cali[b+54];
			}
			return 1;
		}


		public int Cali(List<Byte> array, double[] inertial_data_c){

			double[] rawData = new double[9];
			//gyro
			inertial_data_c[0]=(short)(array[6]<<8|array[7]);
			inertial_data_c[1]=(short)(array[8]<<8|array[9]);
			inertial_data_c[2]=(short)(array[10]<<8|array[11]);
			inertial_data_c[0]=sensor_cali[0]*inertial_data_c[0]+sensor_cali[3];
			inertial_data_c[1]=sensor_cali[5]*inertial_data_c[1]+sensor_cali[7];
			inertial_data_c[2]=sensor_cali[10]*inertial_data_c[2]+sensor_cali[11];
			rawData[0]=(short)(array[6]<<8|array[7]);
			rawData[1]=(short)(array[8]<<8|array[9]);
			rawData[2]=(short)(array[10]<<8|array[11]);

			//acc
			inertial_data_c[3]=(short)(array[17]<<8|array[18]);
			inertial_data_c[4]=(short)(array[19]<<8|array[20]);
			inertial_data_c[5]=(short)(array[21]<<8|array[22]);
			inertial_data_c[3]=sensor_cali[21]*inertial_data_c[3]+sensor_cali[24];
			inertial_data_c[4]=sensor_cali[26]*inertial_data_c[4]+sensor_cali[28];
			inertial_data_c[5]=sensor_cali[31]*inertial_data_c[5]+sensor_cali[32];
			rawData[3]=(short)(array[17]<<8|array[18]);
			rawData[4]=(short)(array[19]<<8|array[20]);
			rawData[5]=(short)(array[21]<<8|array[22]);

			//mag
			inertial_data_c[6]=(short)(array[28]<<8|array[29]);
			inertial_data_c[7]=(short)(array[30]<<8|array[31]);
			inertial_data_c[8]=(short)(array[32]<<8|array[33]);		
			inertial_data_c[6]=sensor_cali[42]*inertial_data_c[6]+sensor_cali[43]*inertial_data_c[7]+sensor_cali[44]*inertial_data_c[8]+sensor_cali[45];
			inertial_data_c[7]=sensor_cali[47]*inertial_data_c[7]+sensor_cali[48]*inertial_data_c[8]+sensor_cali[49];
			inertial_data_c[8]=sensor_cali[52]*inertial_data_c[8]+sensor_cali[53];
			rawData[6]=(short)(array[28]<<8|array[29]);
			rawData[7]=(short)(array[30]<<8|array[31]);
			rawData[8]=(short)(array[32]<<8|array[33]);	
			//print ("GYRO: "+inertial_data_c[0]+"  "+inertial_data_c[1]+"  "+inertial_data_c[2]+" ACC: "+inertial_data_c[3]+"  "+inertial_data_c[4]+"  "+inertial_data_c[5]+" MAG: "+inertial_data_c[6]+"  "+inertial_data_c[7]+"  "+inertial_data_c[8]);

			//// write value to file////

			//			FileStream aFile9 = new FileStream(@"C:\MRdata\data5.txt", FileMode.Append);
			//			StreamWriter sw9 = new StreamWriter(aFile9);     
			//			//sw9.WriteLine(System.Convert.ToString(SID)); 
			//			sw9.WriteLine(System.Convert.ToString(SID)+"\t"+System.Convert.ToString(array[1])+"\t"+System.Convert.ToString(inertial_data_c[0])+"\t"+System.Convert.ToString(inertial_data_c[1])+"\t"+System.Convert.ToString(inertial_data_c[2])+"\t"+System.Convert.ToString(inertial_data_c[3])+"\t"+System.Convert.ToString(inertial_data_c[4])+"\t"+System.Convert.ToString(inertial_data_c[5])+"\t"+System.Convert.ToString(inertial_data_c[6])+"\t"+System.Convert.ToString(inertial_data_c[7])+"\t"+System.Convert.ToString(inertial_data_c[8]));        
			//			//sw9.WriteLine(System.Convert.ToString(SID)+"\t"+System.Convert.ToString(array[1])+"\t"+System.Convert.ToString(rawData[0])+"\t"+System.Convert.ToString(rawData[1])+"\t"+System.Convert.ToString(rawData[2])+"\t"+System.Convert.ToString(rawData[3])+"\t"+System.Convert.ToString(rawData[4])+"\t"+System.Convert.ToString(rawData[5])+"\t"+System.Convert.ToString(rawData[6])+"\t"+System.Convert.ToString(rawData[7])+"\t"+System.Convert.ToString(rawData[8]));
			//			sw9.Close();     

			//// write value to file////

			return 1;
		}

		public int EKFAW_main(List<Byte> array){
			double[] Omega=new double[16];
			double[] Jaco_A=new double[16];
			double[] Jaco_A_t=new double[16];
			double[] Jaco_B=new double[12];
			double[] Jaco_B_t=new double[12];
			double[] halfomega=new double[16];
			double[] theda=new double[16];
			double omega_d,q_rms;
			double[] I4={1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1};
			double[] qa=new double[4];
			double[] temp_A=new double[16];double[] temp_A2=new double[16];
			double[] temp_B=new double[12];double[] temp_B2=new double[12];
			double[] temp_C=new double[9];double[] temp_C2=new double[9];
			double[] temp_D=new double[4];double[] temp_D2=new double[4];
			double[] DCM_A=new double[9];double[] DCM_A_t=new double[9];
			Cali(array,inertial_cali);

			Omega[0]=0;Omega[1]=-inertial_cali[0];Omega[2]=-inertial_cali[1];Omega[3]=-inertial_cali[2];
			Omega[4]=inertial_cali[0];Omega[5]=0;Omega[6]=inertial_cali[2];Omega[7]=-inertial_cali[1];
			Omega[8]=inertial_cali[1];Omega[9]=-inertial_cali[2];Omega[10]=0;Omega[11]=inertial_cali[0];
			Omega[12]=inertial_cali[2];Omega[13]=inertial_cali[1];Omega[14]=-inertial_cali[0];Omega[15]=0;

			for(int an=0;an<16;an++){
				Jaco_A[an]=I4[an]+dT2*Omega[an];}

			Jaco_B[0]=-dT2*q[1];Jaco_B[1]=-dT2*q[2];Jaco_B[2]=-dT2*q[3];
			Jaco_B[3]=dT2*q[0];Jaco_B[4]=-dT2*q[3];Jaco_B[5]=dT2*q[2];
			Jaco_B[6]=dT2*q[3];Jaco_B[7]=dT2*q[0];Jaco_B[8]=-dT2*q[1];
			Jaco_B[9]=-dT2*q[2];Jaco_B[10]=dT2*q[1];Jaco_B[11]=dT2*q[0];

			MatrixTran(Jaco_A_t,Jaco_A,4,4);
			MatrixTran(Jaco_B_t,Jaco_B,3,4);
			//omega_d=Math.Sqrt(inertial_cali[0]*inertial_cali[0]+inertial_cali[1]*inertial_cali[1]+inertial_cali[2]*inertial_cali[2]);
			//for(int an=0;an<16;an++){
			//	theda[an]=Math.Cos(omega_d*dT2)*I4[an]+Math.Sin(omega_d*dT2)/omega_d*Omega[an];}
			//MatrixMul(qa,theda,q,4,4,1);
			//Array.Copy(qa,q,4);

			MatrixMul(qa,Jaco_A,q,4,4,1);
			q_rms=Math.Sqrt(qa[0]*qa[0]+qa[1]*qa[1]+qa[2]*qa[2]+qa[3]*qa[3]);
			for(int i=0;i<4;i++){
				q[i]=qa[i]/q_rms;
			}

			MatrixMul(temp_A,Jaco_A,P,4,4,4);
			MatrixMul(temp_A2,temp_A,Jaco_A_t,4,4,4);
			MatrixMul(temp_B,Jaco_B,Q,4,3,3);
			MatrixMul(temp_A,temp_B,Jaco_B_t,4,3,4);
			for(int an=0;an<16;an++){
				P[an]=temp_A2[an]+temp_A[an];}

			//q_rms=Math.Sqrt(q[0]*q[0]+q[1]*q[1]+q[2]*q[2]+q[3]*q[3]);
			//for(int i=0;i<4;i++){
			//	q[i]=q[i]/q_rms;
			//}


			Q_RM(DCM_A);MatrixTran(DCM_A_t,DCM_A,3,3);

			//AW_ACCELEROMETER not used
			double[] gravity={0,0,1};
			double gravity_rms=Math.Sqrt(gravity[0]*gravity[0]+gravity[1]*gravity[1]+gravity[2]*gravity[2]);
			double acc_rms=Math.Sqrt(inertial_cali[3]*inertial_cali[3]+inertial_cali[4]*inertial_cali[4]+inertial_cali[5]*inertial_cali[5]);
			double E_m=Math.Max(gravity_rms/acc_rms,acc_rms/gravity_rms);
			double[] gravity_d=new double[3];double[] acc_d=new double[3];
			for(int an=0;an<3;an++){
				gravity_d[an]=gravity[an]/gravity_rms;
				acc_d[an]=inertial_cali[an+3]/acc_rms;}

			//			double[] globel_g=new double[3];
			//			MatrixMul(globel_g,DCM_A,acc_d,3,3,1);

			double[] gravity_to_body=new double[3];
			MatrixMul(gravity_to_body,DCM_A_t,gravity_d,3,3,1);
			double[] E_dm=new double[3];
			for(int an=0;an<3;an++){
				E_dm[an]=gravity_to_body[an]-acc_d[an];}
			double E_d=Math.Sqrt(E_dm[0]*E_dm[0]+E_dm[1]*E_dm[1]+E_dm[2]*E_dm[2]);
			double EE_A=E_m*E_d;
			double EA_cov=Math.Exp(10*EE_A);
			if(EA_cov>=1e+10)
				EA_cov=1e+10;
			//acceleration update 
			double[] H={-2*q[2],2*q[3],-2*q[0],2*q[1],2*q[1],2*q[0],2*q[3],2*q[2],2*q[0],-2*q[1],-2*q[2],2*q[3]};
			double[] H_t=new double[12];
			MatrixTran(H_t,H,4,3);
			double[] Z_predict={2*(q[1]*q[3]-q[0]*q[2]),2*(q[2]*q[3]+q[0]*q[1]),q[0]*q[0]-q[1]*q[1]-q[2]*q[2]+q[3]*q[3]};
			double[] S=new double[9];
			double[] S_inv=new double[9];
			double[] KG=new double[12];
			//Kalman gain 
			//S=H*P*H' + Ra
			MatrixMul(temp_B,P,H_t,4,4,3);
			MatrixMul(temp_C,H,temp_B,3,4,3);
			for(int an=0;an<9;an++){
				S[an]=temp_C[an]+Ra[an];}
			//K = P*H'*inv(S)
			MatrixInv3(S_inv,S);
			MatrixMul(KG,temp_B,S_inv,4,3,3);
			//state updata acc
			//q=q+K*(z-H*q)
			double[] Z_error=new double[3];
			for(int an=0;an<3;an++){	//dz = z-H*q
				Z_error[an]=acc_d[an]-Z_predict[an];}
			MatrixMul(temp_D,KG,Z_error,4,3,1);
			for(int an=0;an<4;an++){	//q_new = q+K*dz
				qa[an]=q[an]+temp_D[an];}
			Array.Copy(qa,q,4);
			q_rms=Math.Sqrt(q[0]*q[0]+q[1]*q[1]+q[2]*q[2]+q[3]*q[3]);
			for(int i=0;i<4;i++){
				q[i]=q[i]/q_rms;}
			//P = (I-K*H)*P
			MatrixMul(temp_A,KG,H,4,3,4);
			for(int an=0;an<16;an++){
				temp_A[an]=I4[an]-temp_A[an];}
			MatrixMul(temp_A2,temp_A,P,4,4,4);
			Array.Copy(temp_A2,P,16);	//P_new

			Q_RM(DCM_A);MatrixTran(DCM_A_t,DCM_A,3,3);

			//AW_MAG not used
			double[] zv={2*q[1]*q[3]-2*q[0]*q[2],2*q[2]*q[3]+2*q[0]*q[1],q[0]*q[0]-q[1]*q[1]-q[2]*q[2]+q[3]*q[3]};
			double mz=inertial_cali[6]*zv[0]+inertial_cali[7]*zv[1]+inertial_cali[8]*zv[2];
			double[] z=new double[3];
			for(int an=0;an<3;an++){
				z[an]=inertial_cali[6+an]-mz*zv[an];}
			double mag_rms=Math.Sqrt(z[0]*z[0]+z[1]*z[1]+z[2]*z[2]);
			double Rmag_rms=Math.Sqrt(sum_iner[7]*sum_iner[7]); //
			E_m=Math.Max(mag_rms/Rmag_rms,Rmag_rms/mag_rms);
			double[] Rmag_d=new double[3];double[] mag_d=new double[3];
			Rmag_d[0]=0;Rmag_d[1]=sum_iner[7]/Rmag_rms;Rmag_d[2]=0;
			for(int an=0;an<3;an++){
				//Rmag_d[an]=sum_iner[an+6]/Rmag_rms;//
				mag_d[an]=z[an]/mag_rms;}
			double[] Rmag_to_body=new double[3];
			MatrixMul(Rmag_to_body,DCM_A_t,Rmag_d,3,3,1);//
			for(int an=0;an<3;an++){
				E_dm[an]=Rmag_to_body[an]-mag_d[an];}
			E_d=Math.Sqrt(E_dm[0]*E_dm[0]+E_dm[1]*E_dm[1]+E_dm[2]*E_dm[2]);
			double EE_M=E_m*E_d;
			double EM_cov=Math.Exp(30*EE_M);
			if(EM_cov>=1e+7)
				EM_cov=1e+7;
			//MAG UPDATE
			double[] H_M={2*q[3],2*q[2],2*q[1],2*q[0],2*q[0],-2*q[1],2*q[2],-2*q[3],-2*q[1],-2*q[0],2*q[3],2*q[2]};
			MatrixTran(H_t,H_M,4,3);
			double[] Z_M_predict={2*(q[1]*q[2]+q[0]*q[3]),q[0]*q[0]-q[1]*q[1]+q[2]*q[2]-q[3]*q[3],2*(q[2]*q[3]-q[0]*q[1])};
			//Kalman gain mag
			//S=H*P*H' + Rm
			MatrixMul(temp_B,P,H_t,4,4,3);
			MatrixMul(temp_C,H_M,temp_B,3,4,3);
			for(int an=0;an<9;an++){
				S[an]=temp_C[an]+Rm[an];}
			//K = P*H'*inv(S)
			MatrixInv3(S_inv,S);
			MatrixMul(KG,temp_B,S_inv,4,3,3);
			//state update mag
			//
			for(int an=0;an<3;an++){
				Z_error[an]=mag_d[an]-Z_M_predict[an];}
			MatrixMul(temp_D,KG,Z_error,4,3,1);
			for(int an=0;an<4;an++){
				qa[an]=q[an]+temp_D[an];}
			Array.Copy(qa,q,4);
			q_rms=Math.Sqrt(q[0]*q[0]+q[1]*q[1]+q[2]*q[2]+q[3]*q[3]);
			for(int i=0;i<4;i++){
				q[i]=q[i]/q_rms;}
			MatrixMul(temp_A,KG,H_M,4,3,4);
			for(int an=0;an<16;an++){
				temp_A[an]=I4[an]-temp_A[an];}
			MatrixMul(temp_A2,temp_A,P,4,4,4);
			Array.Copy(temp_A2,P,16);
			//double[] eulerangle=new double[3];
			//q_to_euler(eulerangle,q);
			//print(q[1]+" "+q[2]+" "+q[3]+" "+q[0]);
			//print(P[0]+" "+P[5]+" "+P[10]+" "+P[15]);
			//print(eulerangle[0]+" "+eulerangle[1]+" "+eulerangle[2]);
			//print(Z_predict[0]+" "+Z_predict[1]+" "+Z_predict[2]+"\n"+acc_d[0]+" "+acc_d[1]+" "+acc_d[2]);
			//print(Z_M_predict[0]+" "+Z_M_predict[1]+" "+Z_M_predict[2]+"\n"+mag_d[0]+" "+mag_d[1]+" "+mag_d[2]);
			//print(globel_g[0]+" "+globel_g[1]+" "+globel_g[2]);
			//			for(int a1=0;a1<16;a1++){
			//				print("B : "+a1+"  "+Jaco_B[a1]);
			//			    print("BT : "+a1+"  "+Jaco_B_t[a1]);}
			//print(Jaco_A[0]+" "+Jaco_A[5]+" "+Jaco_A[10]+" "+Jaco_A[15]+" "+ac);
			//ac=ac+1;
			//print(q_rms);
			//print(Z_error[0]+" "+Z_error[1]+" "+Z_error[2]);
			//print(Rm[0]+" "+Rm[1]+" "+Rm[2]+" "+Rm[3]+" "+Rm[4]+" "+Rm[5]+" "+Rm[6]+" "+Rm[7]+" "+Rm[8]);
			return 1;
		}


		public int Q_RM(double[] q_RM){
			q_RM[0]=q[0]*q[0]+q[1]*q[1]-q[2]*q[2]-q[3]*q[3];
			q_RM[1]=2*(q[1]*q[2]-q[0]*q[3]);
			q_RM[2]=2*(q[1]*q[3]+q[0]*q[2]);
			q_RM[3]=2*(q[1]*q[2]+q[0]*q[3]);
			q_RM[4]=q[0]*q[0]-q[1]*q[1]+q[2]*q[2]-q[3]*q[3];
			q_RM[5]=2*(q[2]*q[3]-q[0]*q[1]);
			q_RM[6]=2*(q[1]*q[3]-q[0]*q[2]);
			q_RM[7]=2*(q[2]*q[3]+q[0]*q[1]);
			q_RM[8]=q[0]*q[0]-q[1]*q[1]-q[2]*q[2]+q[3]*q[3];
			return 1;
		}

		public int RM_Q(double[] R){
			qu[0]=(Math.Sqrt(R[0]+R[4]+R[8]+1))/2;
			qu[1]=(R[5]-R[7])/(4*qu[0]);
			qu[2]=(R[6]-R[2])/(4*qu[0]);
			qu[3]=(R[1]-R[3])/(4*qu[0]);

			//			for(int ie=0;ie<4;ie++){
			//				quf[ie]=(float)qu[ie];
			//				qf[ie]=(float)q[ie];
			//			}
			return 1;
		}

		public int RM_E(double[] R){
			eu[0]=Math.Atan2(R[7],R[8]);
			eu[1]=Math.Asin(-1*R[6]);
			eu[2]=Math.Atan2(R[3],R[0]);

			eur[0]=180*eu[0]/pi;
			eur[1]=180*eu[1]/pi;
			eur[2]=180*eu[2]/pi;

			eul[0]=-180*eu[0]/pi;
			eul[1]=-180*eu[2]/pi;
			eul[2]=-180*eu[1]/pi;
			return 1;
		}

		public int Initial_RM(){			//initial rotation matrix
			q_ini[0]=q[0];q_ini[1]=q[1];q_ini[2]=q[2];q_ini[3]=q[3];
			initial_RM[0]=q[0]*q[0]+q[1]*q[1]-q[2]*q[2]-q[3]*q[3];
			initial_RM[3]=2*(q[1]*q[2]-q[0]*q[3]);
			initial_RM[6]=2*(q[1]*q[3]+q[0]*q[2]);
			initial_RM[1]=2*(q[1]*q[2]+q[0]*q[3]);
			initial_RM[4]=q[0]*q[0]-q[1]*q[1]+q[2]*q[2]-q[3]*q[3];
			initial_RM[7]=2*(q[2]*q[3]-q[0]*q[1]);
			initial_RM[2]=2*(q[1]*q[3]-q[0]*q[2]);
			initial_RM[5]=2*(q[2]*q[3]+q[0]*q[1]);
			initial_RM[8]=q[0]*q[0]-q[1]*q[1]-q[2]*q[2]+q[3]*q[3];
			return 1;
		}  

		public int Initial_RM_t0(){  		//initial rotation matrix & euler angle
			q_tes[0]=q[0];q_tes[1]=q[1];q_tes[2]=q[2];q_tes[3]=q[3];
			initial_RM_t0[0]=q[0]*q[0]+q[1]*q[1]-q[2]*q[2]-q[3]*q[3];
			initial_RM_t0[1]=2*(q[1]*q[2]-q[0]*q[3]);
			initial_RM_t0[2]=2*(q[1]*q[3]+q[0]*q[2]);
			initial_RM_t0[3]=2*(q[1]*q[2]+q[0]*q[3]);
			initial_RM_t0[4]=q[0]*q[0]-q[1]*q[1]+q[2]*q[2]-q[3]*q[3];
			initial_RM_t0[5]=2*(q[2]*q[3]-q[0]*q[1]);
			initial_RM_t0[6]=2*(q[1]*q[3]-q[0]*q[2]);
			initial_RM_t0[7]=2*(q[2]*q[3]+q[0]*q[1]);
			initial_RM_t0[8]=q[0]*q[0]-q[1]*q[1]-q[2]*q[2]+q[3]*q[3];

			eu_t0[0]=Math.Atan2(initial_RM_t0[7],initial_RM_t0[8]);
			eu_t0[1]=Math.Asin(-1*initial_RM_t0[6]);
			eu_t0[2]=Math.Atan2(initial_RM_t0[3],initial_RM_t0[0]);

			eur_t0[0]=180*eu_t0[0]/pi;		//raw 		x
			eur_t0[1]=180*eu_t0[1]/pi;     	//pitch 	y
			eur_t0[2]=180*eu_t0[2]/pi;		//yaw  		z
			return 1;
		}

		public int MatrixMul(double[] C,double[] A, double[] B, int m, int n, int p)
		{
			for (int i = 0; i < m; i++)
				for (int j = 0; j < p; j++)
				{
					C[i*p+j] = 0;
					for (int k = 0; k < n; k++)
						C[i*p+j] += A[i*n+k] * B[k*p+j];
				}
			return 1;
		}
		public int MatrixTran(double[] AT,double[] A, int m, int n)
		{
			for (int i = 0; i < m; i++)
				for (int j = 0; j < n; j++)
					AT[i*n+j] = A[j*m+i];
			return 1; 
		}
		public int MatrixInv3(double[] AI,double[] A)
		{
			double det=A[0]*A[4]*A[8]+A[1]*A[5]*A[6]+A[2]*A[3]*A[7]-A[2]*A[4]*A[6]-A[1]*A[3]*A[8]-A[0]*A[5]*A[7];
			if(det==0)
				return 0;
			AI[0] = (A[4]*A[8]-A[5]*A[7])/det;	AI[1] = (A[2]*A[7]-A[1]*A[8])/det;	AI[2] = (A[1]*A[5]-A[2]*A[4])/det;	
			AI[3] = (A[5]*A[6]-A[3]*A[8])/det;	AI[4] = (A[0]*A[8]-A[2]*A[6])/det;	AI[5] = (A[2]*A[3]-A[0]*A[5])/det;	
			AI[6] = (A[3]*A[7]-A[4]*A[6])/det;	AI[7] = (A[1]*A[6]-A[0]*A[7])/det;	AI[8] = (A[0]*A[4]-A[1]*A[3])/det;	
			return 1;
		}
	}
}
