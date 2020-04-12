using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.IO; 
using TMPro;

public class Mopac {
	public Mopac(float _ax,float _ay,float _az,float _rx,float _ry,float _rz,float _mx,float _my,float _mz,double _secs)
	{
		ax = _ax;
		ay = _ay;
		az = _az;
		rx = _rx;
		ry = _ry;
		rz = _rz;
		mx = _mx;
		my = _my;
		mz = _mz;
		secs = _secs;
		date = "";
	}
	public float ax{ set; get; }
	public float ay{ set; get; }
	public float az{ set; get; }
	public float rx{ set; get; }
	public float ry{ set; get; }
	public float rz{ set; get; }
	public float mx{ set; get; }
	public float my{ set; get; }
	public float mz{ set; get; }
	public double secs{ set; get; }
	public String date;

}
public class dataFileLoad : MonoBehaviour {
	Boolean dataLoaded = false;
    Boolean animateMooring = true;
	private double nextActionTime = 0.0;
	private double timeBegin = 0.0;
	private double previousSeconds = 0.0f;
	int mopacCounter = 0;
	List<Mopac> mopacsList = new List<Mopac>();

	public TextMeshProUGUI accelerationTextLocal;
	public TextMeshProUGUI rotationTextLocal;
	public TextMeshProUGUI magnetTextLocal;

	public Vector3 targetAngle = new Vector3(0f, 345f, 0f);
	private Vector3 currentAngle;

	private Vector3 currentAcceleration, initialAcceleration;
	public float rotationSpeed = 0.4f;
	public float sensitivity = 6;
	public float newRotation;
    public double timeMultiplier = 1.0;
	Vector3 velocity;
	Vector3 angularVelocity;
    public float speed = 10f;
    //Array SENSOR_SIGN = {1,1,1,-1,-1,-1,1,1,1};
    Mopac previousM = null;
    double dz = 0f;
    double vz = 0f;
    // Faces for 6 sides of the cube
    private GameObject[] quads = new GameObject[6];
    // Textures for each quad, should be +X, +Y etc
    // with appropriate colors, red, green, blue, etc
    public Texture[] labels;
    Vector3 curAC;
    Vector3 zeroAC;
    float sensH = 10f;
    float sensV = 10f;
    float smooth = 0.5f;
    float GetAxisH = 0f;
    float GetAxisV = 0f;
    float zIntegral = 0f;
    //public WMG_X_Plot_Overtime graphOverTime;

    // gravity constant
    float g=9.8f;
	// Use this for initialization
	void Start () {
		initialAcceleration = Input.acceleration;
		currentAcceleration = Vector3.zero;
		//  20160726_030007.mopak.log
		//   /home/gi01sumo/D0003/cg_data/dcl11/mopak/20170109_070011.mopak.log     20170108_100008.mopak.log
		//  20170110_210009.mopak
		//  20170110_220009.mopak
		//  20160714_200009.mopak

		//  20160602_180010   should be STILL

        //possible leak
        //20150326_150007.mopak

        // 20150215_220006.mopak   LEAK!  D0001   20150215_230008.mopak
        // 20150216_020007.mopak
        //definite craziness
        //    second 580!!     string pathToOpen = "///Users/johnreine/Documents/whoiNoBackup/gi01sumo/D0003/cg_data/dcl11/mopak/"+"20161227_180010.mopak.log";
        //    second 580!!     string pathToOpen = "///Users/johnreine/Documents/whoiNoBackup/gi01sumo/D0003/cg_data/dcl11/mopak/"+"20161227_180010.mopak.log";

        //last 2017 irminger mopak: 20171012_090011.mopak.log  (hour before:  20171012_080010.mopak.log  
        //hour before
        //string pathToOpen = "///Users/johnreine/Documents/whoiNoBackup/gi01sumo/D0004/cg_data/dcl11/mopak/"+"20171012_080010.mopak.log";
        //LAST ONE FROM IRMINGER:
        //string pathToOpen = "///Users/johnreine/Documents/whoiNoBackup/gi01sumo/D0004/cg_data/dcl11/mopak/"+"20171012_090011.mopak.log";
        //hour before LAST one from irminger: 20171012_080010.mopak.log
        string pathToOpen = "///Users/johnreine/Documents/whoiNoBackup/gi01sumo/D0004/cg_data/dcl11/mopak/"+"20171012_080010.mopak.log";

        //of interest in irminger dissapearance: 20171009_190009.mopak.log
        //string pathToOpen = "///Users/johnreine/Documents/whoiNoBackup/gi01sumo/D0004/cg_data/dcl11/mopak/"+"20171009_190009.mopak.log";

        //string pathToOpen = "///Users/johnreine/Documents/whoiNoBackup/gi01sumo/D0003/cg_data/dcl11/mopak/"+"20161227_180010.mopak.log";

		//string pathToOpen = "///Users/johnreine/Documents/whoiNoBackup/gi01sumo/D0003/cg_data/dcl11/mopak/"+"20170110_220009.mopak.log";
        //string pathToOpen = "///Users/johnreine/Documents/whoiNoBackup/gi01sumo/D0003/cg_data/dcl11/mopak/"+"20161227_190010.mopak.log";

        //jim not opening this file:  20150730_100011.mopak
        //string pathToOpen = "///Users/johnreine/Documents/whoiNoBackup/gi01sumo/D0002/cg_data/dcl11/mopak/"+"20150730_090010.mopak.log";

        //string pathToOpen = "///Users/johnreine/Documents/whoiNoBackup/gi01sumo/"+"20170926_074229.3dmgx3.log";

        //  http://ooi-cgoms1.whoi.net/oms-bin/view_syslog?pid=gi01sumo&deploy=D0004&plot=idata&inst=mopak&dcl=dcl11&fileinfo=0&show_platform_inst=1&nrecs=1000
        // view mopak file:
        // http://ooi-cgoms1.whoi.net/oms-bin/view_syslog?pid=gi01sumo&deploy=D0004&view=idata&dcl=dcl11&inst=mopak&nrecs=1000&file=20170720_160009.mopak.log&show_platform_inst=1
            
        //25meter wave dec 9 2014 16:00   20141209_140005.mopak
        //string pathToOpen = "///Users/johnreine/Documents/whoiNoBackup/gi01sumo/D0001/cg_data/dcl11/mopak/"+"20141209_140005.mopak.log";

        Debug.Log  ("yep.. opening: " + pathToOpen);
		String notext = " ";

		 accelerationTextLocal.text = notext;
		 rotationTextLocal.text = notext;
		 magnetTextLocal.text = notext;
		Debug.Log ("beginning Load...");
		Load (pathToOpen);
		currentAngle = transform.eulerAngles;

        curAC =  Vector3.zero;
        zeroAC = new Vector3(0,3,0);

        Debug.Log  ("file loaded... ");
        //graphOverTime = GetComponent<WMG_X_Plot_Overtime>();
	}   
    /*
    public class GraphPointInteraction : MonoBehaviour {
        
         public WMG_Axis_Graph myGraph;

        void MyCustomFunction(WMG_Series series, WMG_Node node) {
            Debug.Log("Node: " + node.name + " on series: " + series.name + " was clicked!");
           }
                 void Start() {
           myGraph.WMG_Click += MyCustomFunction;
         }
        
        }
    */
	// Update is called once per frame
	void Update () {
		if (dataLoaded) {
			if (Time.time >=  nextActionTime) {
				Mopac m = mopacsList [mopacCounter];
                double seconds = m.secs*1.0;// * timeMultiplier;
				//Debug.Log (mopacCounter + " time=" + m.secs + " mx=" + m.mx);
				if (mopacCounter == 0) {
                    previousSeconds = seconds;
				}
                float tDelta = (float)(seconds - previousSeconds);
				//Vector3 rot = new Vector3 (((m.rx * tDelta) * Mathf.Rad2Deg), ((m.rz * tDelta) * Mathf.Rad2Deg), ((m.ry * tDelta) * Mathf.Rad2Deg)); 
				//transform.Rotate (rot);
				//NOTE: z and y are switched
				targetAngle = new Vector3 ((m.rx * Mathf.Rad2Deg), (m.rz * Mathf.Rad2Deg), (m.ry * Mathf.Rad2Deg)); 
				currentAngle = new Vector3(
					Mathf.LerpAngle(currentAngle.x, targetAngle.x, tDelta),
					Mathf.LerpAngle(currentAngle.z, targetAngle.z, tDelta),
					Mathf.LerpAngle(currentAngle.y, targetAngle.y, tDelta));
                if (animateMooring)
                {
                    transform.eulerAngles = currentAngle;
                }

                Vector2 animateV2 = new Vector2(mopacCounter, m.ax);
                //graphOverTime.animateAddPointFromEnd(animateV2, tDelta);
                //  animateAddPointFromEnd

				String accelText = string.Format("aX={0,5:0.00} aY={1,5:0.00} aZ={2,5:0.00}", m.ax, m.ay, m.az);
				accelerationTextLocal.text = accelText;
                String rotText = string.Format("rX={0,5:0.00} rY={1,5:0.00} rZ={2,5:0.00}", m.rx* Mathf.Rad2Deg, m.ry* Mathf.Rad2Deg, m.rz* Mathf.Rad2Deg);
				rotationTextLocal.text = rotText;
				String magText = string.Format("mX={0,5:0.00} mY={1,5:0.00} mZ={2,5:0.00}", m.mx, m.my, m.mz);
				
                magnetTextLocal.text = seconds + " seconds";

                //previousSeconds = mopacsList[mopacCounter].secs;
                previousSeconds = seconds;
				mopacCounter++;
				if (mopacCounter > mopacsList.Count) {
					mopacCounter = 0;
					initialAcceleration = Input.acceleration;
					currentAcceleration = Vector3.zero;
					dataLoaded = false;  //for now we stop at the end.. want to know when that is.
					Debug.Log("DONE DONE DONE DONE DONE DONE DONE DONE");
				}
                previousM = m;
                double s = mopacsList[mopacCounter].secs;
                nextActionTime = timeBegin + (s*1.0); //*timeMultiplier;//*timeMultiplier);
			}
		}
	} 

	void AddLinearAcceleration(Vector3 aLinearAcceleration)
	{
		velocity += aLinearAcceleration * Time.deltaTime;
	}
	void AddAngularAcceleration(Vector3 aAngularAcceleration)
	{
		angularVelocity += aAngularAcceleration * Time.deltaTime;
	}

	private bool Load(string fileName)
	{
		Debug.Log ("beginning Load...)");
		// Handle any problems that might arise when reading the text
		try
		{
			
			// Create a new StreamReader, tell it which file to read and what encoding the file
			// was saved as
			StreamReader theReader = new StreamReader(fileName, Encoding.Default);
			// Immediately clean up the reader after this block of code is done.
			// You generally use the "using" statement for potentially memory-intensive objects
			// instead of relying on garbage collection.
			// (Do not confuse this with the using directive for namespace at the 
			// beginning of a class!)
			using (theReader)
			{
				var bytes = default(byte[]);
				Debug.Log ("Opened!!!!!!  :)");
				using (var memstream = new MemoryStream())
				{
					var buffer = new byte[512];
					var bytesRead = default(int);
					while ((bytesRead = theReader.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
						memstream.Write(buffer, 0, bytesRead);
					bytes = memstream.ToArray();
				}

//				43 bytes defined as follows 
//				Byte 1 		Header = 0xCB 
//				Bytes 2-5 	AccelX 
//				Bytes 6-9 	AccelY 
//				Bytes 10-13 	AccelZ 
//				Bytes 14-17 	AngRateX 
//				Bytes 18-21 	AngRateY 
//				Bytes 22-25 	AngRateZ 
//				Bytes 26-29 	MagX 
//				Bytes 30-33 	MagY 
//				Bytes 34-37 	MagZ 
//				Bytes 38-41 	Timer 
//				Bytes 42-43 	Checksum 

				for(int i=0; i < bytes.Length; i = i + 43) 
				{
					byte[] axbytes = null;
					axbytes = new byte[4];
					axbytes[0] = bytes[i+4];
					axbytes[1] = bytes[i+3];
					axbytes[2] = bytes[i+2];
					axbytes[3] = bytes[i+1];

					byte[] aybytes = null;
					aybytes = new byte[4];
					aybytes[0] = bytes[i+8];
					aybytes[1] = bytes[i+7];
					aybytes[2] = bytes[i+6];
					aybytes[3] = bytes[i+5];

					byte[] azbytes = null;
					azbytes = new byte[4];
					azbytes[0] = bytes[i+12];
					azbytes[1] = bytes[i+11];
					azbytes[2] = bytes[i+10];
					azbytes[3] = bytes[i+9];

					byte[] rxbytes = null;
					rxbytes = new byte[4];
					rxbytes[0] = bytes[i+16];
					rxbytes[1] = bytes[i+15];
					rxbytes[2] = bytes[i+14];
					rxbytes[3] = bytes[i+13];

					byte[] rybytes = null;
					rybytes = new byte[4];
					rybytes[0] = bytes[i+20];
					rybytes[1] = bytes[i+19];
					rybytes[2] = bytes[i+18];
					rybytes[3] = bytes[i+17];

					byte[] rzbytes = null;
					rzbytes = new byte[4];
					rzbytes[0] = bytes[i+24];
					rzbytes[1] = bytes[i+23];
					rzbytes[2] = bytes[i+22];
					rzbytes[3] = bytes[i+21];

					byte[] mxbytes = null;
					mxbytes = new byte[4];
					mxbytes[0] = bytes[i+28];
					mxbytes[1] = bytes[i+27];
					mxbytes[2] = bytes[i+26];
					mxbytes[3] = bytes[i+25];

					byte[] mybytes = null;
					mybytes = new byte[4];
					mybytes[0] = bytes[i+32];
					mybytes[1] = bytes[i+31];
					mybytes[2] = bytes[i+30];
					mybytes[3] = bytes[i+29];

					byte[] mzbytes = null;
					mzbytes = new byte[4];
					mzbytes[0] = bytes[i+36];
					mzbytes[1] = bytes[i+35];
					mzbytes[2] = bytes[i+34];
					mzbytes[3] = bytes[i+33];

					byte[] tbytes = null;
					tbytes = new byte[4];
					tbytes[0] = bytes[i+40];
					tbytes[1] = bytes[i+39];
					tbytes[2] = bytes[i+38];
					tbytes[3] = bytes[i+37];


					float accelX = BitConverter.ToSingle(axbytes, 0);
					float accelY = BitConverter.ToSingle(aybytes, 0);
					float accelZ = BitConverter.ToSingle(azbytes, 0);
					float rotX = BitConverter.ToSingle(rxbytes, 0);
					float rotY = BitConverter.ToSingle(rybytes, 0);
					float rotZ = BitConverter.ToSingle(rzbytes, 0);
					float magX = BitConverter.ToSingle(mxbytes, 0);
					float magY = BitConverter.ToSingle(mybytes, 0);
					float magZ = BitConverter.ToSingle(mzbytes, 0);
					uint t = BitConverter.ToUInt32(tbytes, 0);
					double tseconds = t / 62500.0; 
					//Byte b = bytes[i+0];
                    if(i < 1000) {
					    Debug.Log(i + " " +" aX="+ accelX + " aY=" + accelY +" aZ=" + accelZ + " rx="+rotX+" ry="+rotY+" rz="+rotZ + " mx="+magX+" my="+magY+" mz="+magZ+" t="+tseconds);
                    }
					Mopac mpac = new Mopac(accelX,accelY,accelZ,rotX,rotY,rotZ,magX,magY,magZ,tseconds);
					mopacsList.Add(mpac);

				}
				// Done reading, close the reader and return true to broadcast success    
				theReader.Close();
				Debug.Log("total mopacs: "+mopacsList.Count);
				if(mopacsList.Count > 1) 
				{
					dataLoaded = true;
					timeBegin = Time.time;
					Debug.Log("dataLoaded = true.. ");
				}
				return true;
			}
		}
		// If anything broke in the try block, we throw an exception with information
		// on what didn't work
		catch (Exception e)
		{
			Debug.Log ("failed to open..");
			Console.WriteLine("{0}\n", e.Message);
			return false;
		}
	}

    // make a quad for one side of the cube
    GameObject createQuad(GameObject quad, Vector3 pos, Vector3 rot, string name, Color col, Texture t)
    {
        Quaternion quat = Quaternion.Euler(rot);
        GameObject GO = Instantiate(quad, pos, quat);
        GO.name = name;
        GO.GetComponent<Renderer>().material.color = col;
        GO.GetComponent<Renderer>().material.mainTexture = t;
        GO.transform.localScale += new Vector3(0.25f, 0.25f, 0.25f);
        return GO;
    }

}

