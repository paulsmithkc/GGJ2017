using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Based on "Mobile Dev from Scratch: Markerless Augmented Reality with Unity"
 * https://www.youtube.com/watch?v=gNwduUQrlJs
 */
public class CameraGyro : MonoBehaviour {

    public Camera _camera;
    public MeshRenderer _cameraTarget;
    public int _cameraFPS = 30;

    private GameObject _cameraParent;
    private string _deviceName;
    private WebCamTexture _cameraTexture;

    // Use this for initialization
    void Start () {

        // Find the camera
        if (_camera == null)
        {
            _camera = Camera.main;
        }

        // Reparent the camera
        _cameraParent = new GameObject("CameraParent");
        _cameraParent.transform.position = _camera.transform.position;
        _cameraParent.transform.Rotate(Vector3.right, 90);

        // Set the texture of the camera target to the web cam input
        if (_cameraTarget != null)
        {
            Debug.Log("web cams available: " + WebCamTexture.devices.Length);

            _deviceName = null;
            foreach (var d in WebCamTexture.devices)
            {
                _deviceName = d.name;
                if (!d.isFrontFacing) { break; }
            }

            Debug.Log("using web cam: " + (_deviceName ?? "null"));

            _cameraTexture = new WebCamTexture(_deviceName, Screen.width, Screen.height, _cameraFPS);
            _cameraTarget.material.mainTexture = _cameraTexture;
            if (!string.IsNullOrEmpty(_deviceName))
            {
                _cameraTexture.Play();
            }
        }

        // Enable the gyro
        Input.gyro.enabled = true;
        // Enable the compass
        Input.compass.enabled = true;
    }
	
	// Update is called once per frame
	void Update () {

        // Rotate the camera at a constant speed
        // _camera.transform.localRotation = Quaternion.AngleAxis(Time.time * 36.0f, Vector3.up);

        // Update the camera orientation based on the current orientation of the gyro
        Quaternion r1 = Input.gyro.attitude;
        Quaternion r2 = new Quaternion(r1.x, r1.y, r1.z, r1.w);
        _camera.transform.localRotation = r2;

        // Update the camera orientation based on the current orientation of the compass
        //Quaternion r2 = Quaternion.AngleAxis(Input.compass.trueHeading, Vector3.up);
        //_camera.transform.localRotation = r2;

        // Resize the web cam texture if the screen orientation changes
        if (!string.IsNullOrEmpty(_deviceName) &&
            _cameraTexture != null &&
            (_cameraTexture.requestedWidth != Screen.width || _cameraTexture.requestedHeight != Screen.height))
        {
            Destroy(_cameraTexture);

            _cameraTexture = new WebCamTexture(_deviceName, Screen.width, Screen.height, _cameraFPS);
            _cameraTarget.material.mainTexture = _cameraTexture;
            _cameraTexture.Play();
        }
    }
}
