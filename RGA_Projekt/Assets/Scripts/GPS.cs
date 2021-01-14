using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using System;

public class GPS : MonoBehaviour
{
    public static GPS Instance{set; get;}

    private string APIKey = "AIzaSyArILspLBDe-Ku4kJ8oGiM51nK7sSZkg_s";
 
    RawImage img;
 
    string url;
    
    Text latitude;
    Text longitude;
    GameObject dialog = null;
    
    public int zoom = 14;
    public int mapWidth = 300;
    public int mapHeight = 300;

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
    }

    // Update is called once per frame
    void Update()
    {
        
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
        latitude.text = "latitude: " + Input.location.lastData.latitude.ToString();
        longitude.text = "longitude: " + Input.location.lastData.longitude.ToString();
        StartCoroutine(coroutine);
    }

    IEnumerator UpdateGPS()
    {
        float UPDATE_TIME = 3f; //Every  3 seconds
        WaitForSeconds updateTime = new WaitForSeconds(UPDATE_TIME);

        while (true)
        {
            latitude.text = "latitude: " + Input.location.lastData.latitude.ToString();
            longitude.text = "longitude: " + Input.location.lastData.longitude.ToString();
            url = "https://maps.googleapis.com/maps/api/staticmap?center=" + Input.location.lastData.latitude + "," + Input.location.lastData.longitude +
                  "&zoom=" + zoom + "&size=" + mapWidth + "x" + mapHeight + "&scale=" + "1"
                  + "&maptype="+ "roadmap" + "&markers=color:blue%7Clabel:S%7C40.702147,-74.015794&markers=color:green%7Clabel:G%7C40.711614,-74.012318&markers=color:red%7Clabel:C%7C40.718217,-73.998284&key=" + APIKey;
            WWW www = new WWW(url);
            yield return www;
            img.texture = www.texture;
            img.SetNativeSize();
            yield return updateTime;
        }
    }

}