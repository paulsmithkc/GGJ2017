using System;
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
    private Origin _origin;

    //private GameObject _cameraParent;
    private string _deviceName;
    private WebCamTexture _cameraTexture;
    private const float _smoothingDuration = 0.5f;
    private LocationInfo? _lastLocation;
    private float? _roll;
    private float? _pitch;
    private float? _heading;
    private EnemySpawn _closestSpawn;
    private Vector3 _velocity;
    
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

        // Find the origin
        _origin = FindObjectOfType<Origin>();
        if (_origin != null)
        {
            _camera.transform.position = 
                _origin.LocationToWorld(_origin._latitude, _origin._longitude)
                - Vector3.forward * 20.0f;
        }

        // Reparent the camera
        //_cameraParent = new GameObject("CameraParent");
        //_cameraParent.transform.position = _camera.transform.position;
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
        //StartLocationService();

        _roll = null;
        _pitch = null;
        _heading = null;
    }

    // Update is called once per frame
    void Update()
    {
        float deltaTime = Time.deltaTime;

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

        //if (_origin != null && _lastLocation != null)
        //{
        //    float latitude = _lastLocation.Value.latitude;
        //    float longitude = _lastLocation.Value.longitude;
        //    _camera.transform.position = _origin.LocationToWorld(latitude, longitude);
        //}

        Quaternion cameraRotation = Quaternion.identity;
        Vector3 velocity = Vector3.zero;

        if (SystemInfo.supportsAccelerometer)
        {
            //Vector3 accel = new Vector3(0.0f, -1.0f, 0.0f);
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

            //float forwardMovement = -0.5f - Mathf.Clamp(accel.z, -1.0f, 0.0f);
            //if (forwardMovement > -0.25f && forwardMovement < 0.25f)
            //{
            //    forwardMovement = 0.0f;
            //}
            velocity = Vector3.right * Mathf.Clamp(accel.x, -0.5f, 0.5f) * 1.0f; //+ Vector3.forward * forwardMovement * 2.0f;
        }

        var spawns = GameObject.FindObjectsOfType<EnemySpawn>();
        EnemySpawn closestSpawn = null;
        float closestDistance = float.PositiveInfinity;
        Vector3 pos = _camera.transform.position;

        foreach (var s in spawns)
        {
            float d = (s.transform.position - pos).magnitude;
            if (d < closestDistance)
            {
                closestSpawn = s;
                closestDistance = d;
            }
        }
        _closestSpawn = closestSpawn;

        float heading = 0.0f;
        if (Input.compass.enabled)
        {
            heading = Input.compass.trueHeading;
        }
        else if (closestSpawn != null)
        {
            var v = closestSpawn.transform.position - pos;
            //heading = Mathf.LerpAngle(
            //    _heading ?? 0.0f,
            //    Mathf.Atan2(v.x, v.z) * Mathf.Rad2Deg,
            //    deltaTime / (_smoothingDuration + deltaTime)
            //);
            heading = Mathf.Atan2(v.x, v.z) * Mathf.Rad2Deg;
        }
        else
        {
            heading = (Time.time * 60.0f) % 360.0f;
        }
        _heading = heading;

        //Debug.LogFormat("Heading {0}", heading);
        var headingRotation = Quaternion.AngleAxis(heading, Vector3.up);
        cameraRotation = headingRotation * cameraRotation;
        
        _camera.transform.localRotation = Quaternion.Lerp(
            _camera.transform.localRotation,
            cameraRotation,
            deltaTime / (_smoothingDuration + deltaTime)
        );
        _camera.transform.position += headingRotation * velocity;
        _velocity = velocity;

        if (closestSpawn != null)
        {
            var p1 = _camera.transform.position;
            var p2 = closestSpawn.transform.position;
            var v = (p2 - p1);
            v *= (1 - 10 / v.magnitude);
            _camera.transform.position += v * deltaTime / (_smoothingDuration + deltaTime);
        }

        // Update the camera orientation based on the current orientation of the gyro
        //Quaternion r1 = Input.gyro.attitude;
        //Quaternion r2 = new Quaternion(r1.x, r1.y, r1.z, r1.w);
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

            _cameraTexture = new WebCamTexture(_deviceName, Screen.width / 8, Screen.height / 8, _cameraFPS);
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
        if (_closestSpawn != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(pos, _closestSpawn.transform.position);
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

    public Vector3 velocity
    {
        get { return _velocity; }
    }

    public LocationServiceStatus locationStatus
    {
        get { return Input.location.status; }
    }
}
