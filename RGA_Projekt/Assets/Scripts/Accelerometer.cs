using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Net;
using System.IO;
using UnityEngine.Networking;

public class Accelerometer : MonoBehaviour
{
    // Start is called before the first frame update
    Text accelerometerX;
    Text accelerometerY;
    Text accelerometerZ;
    Text bumpAt;
    Text latitudeText;
    Text longitudeText;
    double mAccelLast;
    double mAccelCurrent;
    int temp;
    double X, Y, Z;
    public GameObject Indicator;

    void Start()
    {
        accelerometerX = GameObject.Find("Canvas/accelerometerX").GetComponent<Text>();
        accelerometerY = GameObject.Find("Canvas/accelerometerY").GetComponent<Text>();
        accelerometerZ = GameObject.Find("Canvas/accelerometerZ").GetComponent<Text>();
        latitudeText = GameObject.Find("Canvas/latitude").GetComponent<Text>();
        longitudeText = GameObject.Find("Canvas/longitude").GetComponent<Text>();
        
        bumpAt = GameObject.Find("Canvas/bumpAt").GetComponent<Text>();
        //_ = StartCoroutine(GetSignData());
    }

    // Update is called once per frame
    void Update()
    {
        accelerometerX.text = "X: " + Input.acceleration.x.ToString();
        accelerometerY.text = "Y: " + Input.acceleration.y.ToString();
        accelerometerZ.text = "Z: " + Input.acceleration.z.ToString();

        X = Input.acceleration.x;
        Y = Input.acceleration.y;
        Z = Input.acceleration.z;
        //trenutno vrednost pospeškometra shranim v mAccelLast
        mAccelLast = mAccelCurrent;
        //izračunam trenutno vrednost pospeškometra
        mAccelCurrent = Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));
        temp = compare((int)X, (int)Y, (int)Z);
        if (temp == 0)
        {
            if ((mAccelLast - mAccelCurrent) > 0.55)
            {
                bumpAt.text = "Bump at: X";
                //Instantiate(Indicator, new Vector3(0,0,0), new Quaternion(0,0,0,0));
                PostData();
            }
        }
        else if (temp == 1)
        {
            if ((mAccelLast - mAccelCurrent) > 0.55)
            {
                bumpAt.text = "BumpAt: Y";
                //Instantiate(Indicator, new Vector3(0,0,0), new Quaternion(0,0,0,0));
                PostData();
            }
        }
        else if (temp == 2)
        {
            if ((mAccelLast - mAccelCurrent) > 0.55)
            {
                bumpAt.text = "BumpAt: Z";
                PostData();
                //Instantiate(Indicator, new Vector3(0,0,0), new Quaternion(0,0,0,0));
            }
        }
    }

    [System.Serializable]
    public class gpsData
    {
        public double longitude;
        public double latitude;
    }

    void PostData()
    {
        var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://10.244.116.84:3000/gpsData");
        httpWebRequest.ContentType = "application/json";
        httpWebRequest.Method = "POST";

        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        {
            string json = "{\"latitude\":\"" + Input.location.lastData.latitude.ToString().Replace(',', '.') + "\"," +
                            "\"longitude\":\"" + Input.location.lastData.longitude.ToString().Replace(',', '.') + "\"}";

            streamWriter.Write(json);
        }

        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        {
            var result = streamReader.ReadToEnd();
        }
    }
    int compare(int X, int Y, int Z)
    {
        X = Mathf.Abs(X);
        Y = Mathf.Abs(Y);
        Z = Mathf.Abs(Z);
        if (X > Y)
        {
            if (X > Z) return 0;
        }
        else if (Y > Z) return 1;
        else return 2;

        return -1;
    }
}