using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class Accelerometer : MonoBehaviour
{
    // Start is called before the first frame update
    Text accelerometerX;
    Text accelerometerY;
    Text accelerometerZ;
    Text bumpAt;
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
        bumpAt = GameObject.Find("Canvas/bumpAt").GetComponent<Text>();
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
            if ((mAccelLast - mAccelCurrent) > 0.33)
            {
                bumpAt.text = "Bump at: X";
                Instantiate(Indicator, new Vector3(0,0,0), new Quaternion(0,0,0,0));
            }
        }
        else if (temp == 1)
        {
            if ((mAccelLast - mAccelCurrent) > 0.33)
            {
                bumpAt.text = "BumpAt: Y";
                Instantiate(Indicator, new Vector3(0,0,0), new Quaternion(0,0,0,0));
            }
        }
        else if (temp == 2)
        {
            if ((mAccelLast - mAccelCurrent) > 0.33)
            {
                bumpAt.text = "BumpAt: Z";
                Instantiate(Indicator, new Vector3(0,0,0), new Quaternion(0,0,0,0));
            }
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