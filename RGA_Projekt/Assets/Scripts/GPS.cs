using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using System;
using System.Net;
using System.IO;

public class GPS : MonoBehaviour
{
    public static GPS Instance{set; get;}

    private string APIKey = "AIzaSyArILspLBDe-Ku4kJ8oGiM51nK7sSZkg_s";
 
    RawImage img;
 
    string url;
    string readData;
    string markers;

    List<string>lat = new List<string>();
    List<string>lon = new List<string>();

    Text latitude;
    Text longitude;
    Text txtPozor;
    RawImage excMark;
    GameObject dialog = null;
    
    public int zoom = 14;
    public int mapWidth = 1440;
    public int mapHeight = 1500;

    IEnumerator coroutine;
    void Start()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            dialog = new GameObject();
            while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation)){}
            StartCoroutine(StartLocationService());
        }
        Instance = this;
        coroutine = UpdateGPS();
        DontDestroyOnLoad(gameObject);
        StartCoroutine(StartLocationService());
        latitude = GameObject.Find("Canvas/latitude").GetComponent<Text>();
        longitude = GameObject.Find("Canvas/longitude").GetComponent<Text>();
        img = GameObject.Find("Canvas/RawImage").GetComponent<RawImage>();
        excMark = GameObject.Find("Canvas/ExcMark").GetComponent<RawImage>();
        txtPozor = GameObject.Find("Canvas/txtPozor").GetComponent<Text>();
        excMark.enabled = false;
        txtPozor.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (lat.Count != 0) {
            if (checkProximity())
            {
                excMark.enabled = true;
                txtPozor.enabled = true;
            }
            else
            {
                excMark.enabled = false;
                txtPozor.enabled = false;
            }
        }
    }

    private IEnumerator StartLocationService()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("GPS not enabled");
            yield break;
        }
        Input.location.Start();
        int maxWait = 20;
        while(Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if(maxWait <= 0)
        {
            Debug.Log("Timed out!");
            yield break;
        }

        if(Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Location initialization failed");
            yield break;
        }
        latitude.text = "Latitude: " + Input.location.lastData.latitude.ToString();
        longitude.text = "Longitude: " + Input.location.lastData.longitude.ToString();
        StartCoroutine(coroutine);
    }

    IEnumerator UpdateGPS()
    {
        float UPDATE_TIME = 5f; //Every 30 seconds
        WaitForSeconds updateTime = new WaitForSeconds(UPDATE_TIME);

        while (true)
        {
            GetData();
            latitude.text = "Latitude: " +  Input.location.lastData.latitude.ToString();
            longitude.text = "Longitude: " +  Input.location.lastData.longitude.ToString();
            url = "https://maps.googleapis.com/maps/api/staticmap?center=" + Input.location.lastData.latitude + "," + Input.location.lastData.longitude +
                  "&zoom=" + zoom + "&size=1440x2560&scale=1"
                  + "&maptype="+ "roadmap" + markers + "&key=" + APIKey;
            Debug.Log(url); 
            WWW www = new WWW(url);
            yield return www;
            img.texture = www.texture;
            img.SetNativeSize();
            Debug.Log("GPS");
            //Debug.Log(checkProximity());
            yield return updateTime;
        }
    }

    [System.Serializable]
    public class GPSData
    {
        public string id;
        public double longitude;
        public double latitude;
    }

    void GetData() {
        var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://10.244.116.84:3000/gpsdata");
        httpWebRequest.ContentType = "application/json";
        httpWebRequest.Method = "GET";

        lat.Clear();
        lon.Clear();

        using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
        using (Stream stream = response.GetResponseStream())
        using (StreamReader reader = new StreamReader(stream))
        {
            readData = reader.ReadToEnd();
        }
        String[] strs = readData.Split(new char[] { ':', ',' });
        for (int i = 0; i < strs.Length; i++)
        {
            if (strs[i] == "\"latitude\"")
            {
                lat.Add(strs[i + 1]);
            }else if(strs[i] == "\"longitude\"")
            {
                lon.Add(strs[i + 1]);
            }
        }
        //GPSData[] gpsData = JsonUtility.FromJson<GPSData[]>(readData);
        markers = "";
        for (int i = 0; i < lat.Count; i++) {
            markers += "&markers=color:blue%7Clabel:S%7C" + lat[i] +","+ lon[i] + "";
        }
        //markers = "color:blue%7Clabel:S%7C40.702147,-74.015794&markers=color:green%7Clabel:G%7C40.711614,-74.012318&markers=color:red%7Clabel:C%7C40.718217,-73.998284";
    }
    double degreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180f;
    }

    bool DistanceBetweenPointsInMetres(double lat1, double lon1, double lat2, double lon2)
    {
        float earthRadiusKm = 6375f;

        var dLat = degreesToRadians(lat2 - lat1);
        var dLon = degreesToRadians(lon2 - lon1);

        lat1 = degreesToRadians(lat1);
        lat2 = degreesToRadians(lat2);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        //Debug.Log((float)(earthRadiusKm * c) * 1000f);
        if (((float)(earthRadiusKm * c) * 1000f) > 100) {
            return false;
        }
        else
        {
            return true;
        }
    }

    bool checkProximity() {
        for (int i = 0; i < lat.Count; i++) {
            //Debug.Log(Convert.ToDouble(lat[i].Replace('.', ',')) + "   " + Convert.ToDouble(lon[i].Replace('.', ',')) + "   " + Input.location.lastData.latitude + "   " + Input.location.lastData.longitude);
            if (DistanceBetweenPointsInMetres(Convert.ToDouble(lat[i].Replace('.', ',')), Convert.ToDouble(lon[i].Replace('.', ',')), Input.location.lastData.latitude, Input.location.lastData.longitude)) {
                return true;
            }
        }
        return false;
    }

}