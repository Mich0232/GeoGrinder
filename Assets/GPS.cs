namespace Moitho.Utils
{
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GPS : MonoBehaviour {

    public Text _text;

    public bool FakeGPS = false;
    public Vector2 FakeLocation;

    public static GPS Instance;

    [Range(3f, 10f)]
    public float locationUpdateFrequency = 5f;

    [HideInInspector]
    public bool IsWorking = false;

    private float latitude;
    private float longitude;


    public Vector2 Location { private set { }
        get {
            if (!FakeGPS)
            { return new Vector2(latitude, longitude); }
            else
            {
                return new Vector2(FakeLocation.x, FakeLocation.y);
            }
        }
    }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (!FakeGPS)
            StartCoroutine(StartLocationService());
        else
            IsWorking = true;
    }

    private IEnumerator StartLocationService()
    {
        if(!Input.location.isEnabledByUser)
        {
            Debug.Log("GPS not enabled by user");
            yield break;
        }

        int maxWait = Settings.GPS_TIMEOUT_LIMIT;

        Input.location.Start();
        while(Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSecondsRealtime(1);
            maxWait--;
        }

        if(maxWait <= 0)
        {
            Debug.Log("Timeout!");
            yield break;
        }

        if(Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("GPS init faild");
            yield break;
        }

        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;
        //_text.text = Input.location.status.ToString();
        IsWorking = true;

        StartCoroutine(UpdateGPSLocation());
        yield break;
    }

    private IEnumerator UpdateGPSLocation()
    {
        int counter = 0;
        while(Input.location.status == LocationServiceStatus.Running)
        {
            yield return new WaitForSecondsRealtime(locationUpdateFrequency);
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;
            //_text.text = "try: " + counter+ " lat: " + latitude + " lon: " + longitude;
            counter++;
        }
    }

}

}
