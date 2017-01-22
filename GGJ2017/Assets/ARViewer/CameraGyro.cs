﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Based on "Mobile Dev from Scratch: Markerless Augmented Reality with Unity"
 * https://www.youtube.com/watch?v=gNwduUQrlJs
 */
public class CameraGyro : MonoBehaviour
{
    public Camera _camera;
    public MeshRenderer _cameraTarget;
    public int _cameraFPS = 30;
    public Origin _origin;

    private GameObject _cameraParent;
    private string _deviceName;
    private WebCamTexture _cameraTexture;
    private const float _smoothingDuration = 0.2f;
    private LocationInfo? _lastLocation;
    private float? _roll;
    private float? _pitch;
    private float? _heading;

    [Serializable]
    public class Origin
    {
        public float latitude;
        public float longitude;
        public Transform transform;
    }

    private void StartLocationService()
    {
        if (SystemInfo.supportsLocationService && Input.location.isEnabledByUser)
        {
            Input.location.Start(1.0f, 1.0f);
        }
    }

    // Use this for initialization
    void Start()
    {
        Application.RequestUserAuthorization(UserAuthorization.WebCam);

        // Find the camera
        if (_camera == null)
        {
            _camera = Camera.main;
        }

        // Reparent the camera
        _cameraParent = new GameObject("CameraParent");
        _cameraParent.transform.position = _camera.transform.position;
        //_cameraParent.transform.Rotate(Vector3.right, 90.0f);

        // Enable the gyro
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = false;
        }

        // Enable the compass
        Input.compass.enabled = true;

        // Start the location services
        _lastLocation = null;
        StartLocationService();

        _roll = null;
        _pitch = null;
        _heading = null;
    }

    // Update is called once per frame
    void Update()
    {
        float deltaTime = Time.deltaTime;

        Quaternion cameraRotation = Quaternion.identity;

        if (SystemInfo.supportsAccelerometer)
        {
            Vector3 accel = Input.acceleration;
            float roll = Mathf.Atan2(-accel.x, -accel.y) * Mathf.Rad2Deg;
            float pitch = -Mathf.Atan2(accel.z, new Vector2(accel.x, accel.y).magnitude) * Mathf.Rad2Deg;
            //Debug.LogFormat("Accl {0} {1} {2}", accel.x, accel.y, accel.z);
            //Debug.LogFormat("Roll Pitch {0} {1}", roll, pitch);

            _roll = roll;
            _pitch = pitch;

            cameraRotation =
                Quaternion.AngleAxis(pitch, Vector3.right) *
                Quaternion.AngleAxis(roll, Vector3.forward);
        }

        if (Input.compass.enabled)
        {
            float heading = Input.compass.trueHeading;
            //Debug.LogFormat("Heading {0}", heading);
            cameraRotation = Quaternion.AngleAxis(heading, Vector3.up) * cameraRotation;
            _heading = heading;
        }

        _camera.transform.localRotation = Quaternion.Lerp(
            _camera.transform.localRotation,
            cameraRotation,
            deltaTime / (_smoothingDuration + deltaTime)
        );

        if (Input.location.status != LocationServiceStatus.Initializing &&
            Input.location.status != LocationServiceStatus.Running)
        {
            StartLocationService();
        }
        else if (Input.location.status == LocationServiceStatus.Running)
        {
            var loc = Input.location.lastData;
            if (_lastLocation == null && (loc.latitude != 0.0f || loc.longitude != 0.0f))
            {
                _lastLocation = loc;
            }
            else if (_lastLocation != null)
            {
                _lastLocation = loc;
            }
        }

        if (_origin != null && _origin.transform != null)
        {
            // SEE https://en.wikipedia.org/wiki/Geographic_coordinate_system

            double latitudeToMeters =
                111132.92
                - 559.82 * Mathf.Cos(2 * _origin.latitude)
                + 1.175 * Mathf.Cos(4 * _origin.latitude)
                - 0.0023 * Mathf.Cos(6 * _origin.latitude);
            double longitudeToMeters =
                111412.84 * Mathf.Cos(_origin.latitude)
                - 93.5 * Mathf.Cos(3 * _origin.latitude)
                + 0.118 * Mathf.Cos(5 * _origin.latitude);

            double latitude;
            double longitude;
            if (_lastLocation != null)
            {
                latitude = _lastLocation.Value.latitude;
                longitude = _lastLocation.Value.longitude;
            }
            else
            {
                latitude = _origin.latitude - 10.0 / latitudeToMeters;
                longitude = _origin.longitude;
            }

            _camera.transform.position =
                _origin.transform.position +
                new Vector3(
                    (float)((longitude - _origin.longitude) * longitudeToMeters),
                    0.0f,
                    (float)((latitude - _origin.latitude) * latitudeToMeters)
                );
        }

        // Rotate the camera at a constant speed
        // _camera.transform.localRotation = Quaternion.AngleAxis(Time.time * 36.0f, Vector3.up);

        // Update the camera orientation based on the current orientation of the gyro
        //Quaternion r1 = Input.gyro.attitude;
        //Quaternion r2 = new Quaternion(r1.x, r1.y, r1.z, r1.w);
        //_camera.transform.localRotation = r2;

        // Update the camera orientation based on the current orientation of the compass
        //Quaternion r2 = Quaternion.AngleAxis(Input.compass.trueHeading, Vector3.up);
        //_camera.transform.localRotation = r2;

        if (_cameraTexture == null &&
            _cameraTarget != null &&
            Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            Debug.Log("web cams available: " + WebCamTexture.devices.Length);

            _deviceName = null;
            foreach (var d in WebCamTexture.devices)
            {
                _deviceName = d.name;
                if (!d.isFrontFacing) { break; }
            }

            Debug.Log("using web cam: " + (_deviceName ?? "null"));

            _cameraTexture = new WebCamTexture(_deviceName, Screen.width / 2, Screen.height / 2, _cameraFPS);
            _cameraTarget.material.mainTexture = _cameraTexture;
            if (!string.IsNullOrEmpty(_deviceName))
            {
                _cameraTexture.Play();
                _cameraTarget.gameObject.SetActive(true);
            }
        }

        // Resize the web cam texture if the screen orientation changes
        //if (!string.IsNullOrEmpty(_deviceName) &&
        //    _cameraTexture != null &&
        //    (_cameraTexture.requestedWidth != Screen.width || _cameraTexture.requestedHeight != Screen.height))
        //{
        //    Destroy(_cameraTexture);

        //    _cameraTexture = new WebCamTexture(_deviceName, Screen.width, Screen.height, _cameraFPS);
        //    _cameraTarget.material.mainTexture = _cameraTexture;
        //    _cameraTexture.Play();
        //}
    }

    void OnDrawGizmos()
    {
        var pos =
            _camera != null ?
            _camera.transform.position :
            transform.position;

        // Gyro
        if (SystemInfo.supportsGyroscope && Input.gyro.enabled)
        {
            Quaternion r1 = Input.gyro.attitude;
            Quaternion r2 = new Quaternion(r1.x, r1.y, r1.z, r1.w);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(pos, r2 * Vector3.down);
        }
        // Compass
        if (Input.compass.enabled)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(pos, Quaternion.Euler(0.0f, Input.compass.trueHeading, 0.0f) * Vector3.forward);
        }
        // Accelerometer
        if (SystemInfo.supportsAccelerometer)
        {
            //Gizmos.color = Color.black;
            //foreach (var a in Input.accelerationEvents)
            //{
            //    Gizmos.DrawRay(pos, a.acceleration);
            //}

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(pos, Input.acceleration);
        }
    }

    void OnDestroy()
    {
        if (_cameraTexture != null)
        {
            _cameraTexture.Stop();
            Destroy(_cameraTexture);
            _cameraTexture = null;
        }
    }

    //public Vector3 CameraForward
    //{
    //    get
    //    {
    //        return _camera.transform.forward;
    //    }
    //}

    public float? latitude
    {
        get { return _lastLocation != null ? _lastLocation.Value.latitude : (float?)null; }
    }

    public float? longitude
    {
        get { return _lastLocation != null ? _lastLocation.Value.longitude : (float?)null; }
    }

    public float? altitude
    {
        get { return _lastLocation != null ? _lastLocation.Value.altitude : (float?)null; }
    }

    public float? roll
    {
        get { return _roll; }
    }

    public float? pitch
    {
        get { return _pitch; }
    }

    public float? heading
    {
        get { return _heading; }
    }

    public LocationServiceStatus locationStatus
    {
        get { return Input.location.status; }
    }
}
