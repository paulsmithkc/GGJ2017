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

    private GameObject _cameraParent;
    private string _deviceName;
    private WebCamTexture _cameraTexture;
    private const float _accelerationDuration = 0.5f;
    private Vector3 _accelerationOverDuration;

    // Use this for initialization
    void Start()
    {
        Application.RequestUserAuthorization(UserAuthorization.WebCam);
        
        _accelerationOverDuration = Vector3.down * 9.8f;

        // Find the camera
        if (_camera == null)
        {
            _camera = Camera.main;
        }

        // Reparent the camera
        _cameraParent = new GameObject("CameraParent");
        _cameraParent.transform.position = _camera.transform.position;
        _cameraParent.transform.Rotate(Vector3.right, 90);

        // Enable the gyro
        Input.gyro.enabled = true;
        // Enable the compass
        Input.compass.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {

        // Rotate the camera at a constant speed
        // _camera.transform.localRotation = Quaternion.AngleAxis(Time.time * 36.0f, Vector3.up);

        // Rotate the camera using the accelerometer
        // Debug.Log("accel: " + Input.accelerationEventCount);
        for (int i = 0, n = Input.accelerationEventCount; i < n; ++i)
        {
            var e = Input.GetAccelerationEvent(i);

            _accelerationOverDuration = Vector3.Lerp(
                _accelerationOverDuration,
                e.acceleration,
                e.deltaTime / (_accelerationDuration + e.deltaTime)
            );
        }

        Vector3 accel = _accelerationOverDuration;
        var r =
            Quaternion.AngleAxis(Mathf.Atan2(-accel.x, new Vector2(accel.y, accel.z).magnitude) * Mathf.Rad2Deg, Vector3.forward) *
            Quaternion.AngleAxis(-Mathf.Atan2(accel.y, accel.z) * Mathf.Rad2Deg - 90.0f, Vector3.right);
        _camera.transform.localRotation = r;

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
        if (Input.gyro.enabled)
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
        {
            //Gizmos.color = Color.black;
            //foreach (var a in Input.accelerationEvents)
            //{
            //    Gizmos.DrawRay(pos, a.acceleration);
            //}

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(pos, Input.acceleration);

            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(pos, _accelerationOverDuration);
        }
    }
}
