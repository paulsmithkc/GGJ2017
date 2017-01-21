using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

    public Player _player;
	public Text _healthField;
    public Text _energyField;

    public CameraGyro _cameraGyro;
    public GestureDetector _gestureDetector;
    public GameObject _bulletPrefab;
    private int _dragFingerId;
    private Vector2 _dragStart;

    // Use this for initialization
    void Start () {
        if (_gestureDetector == null)
        {
            _cameraGyro = GameObject.FindObjectOfType<CameraGyro>();
        }
        if (_gestureDetector == null)
        {
            _gestureDetector = GameObject.FindObjectOfType<GestureDetector>();
        }

        _dragFingerId = -1;
        _gestureDetector.drag += OnDrag;
        _gestureDetector.dragEnd += OnDragEnd;
        _gestureDetector.tap += OnTap;
    }

    // Update is called once per frame
    void Update () {
        _healthField.text = _player.CurrentHealth.ToString();
        _energyField.text = _player.CurrentEnergy.ToString();
    }

    private void OnDrag(Vector2 start, Vector2 end, Touch finger)
    {
        if (_dragFingerId < 0)
        {
            _dragFingerId = finger.fingerId;
            _dragStart = start;
        }
    }

    private void OnDragEnd(Vector2 start, Vector2 end, Touch finger)
    {
        if (_dragFingerId >= 0)
        {
            _dragFingerId = -1;
            var camera = _cameraGyro._camera;
            var cameraTransform = camera.transform;
            
            var dragEndPoint = camera.ScreenToWorldPoint(new Vector3(end.x, end.y, 5.0f));
            var dragStartPoint = camera.ScreenToWorldPoint(new Vector3(_dragStart.x, _dragStart.y, 5.0f));
            var spawnPoint = camera.ScreenToWorldPoint(new Vector3(_dragStart.x, _dragStart.y, 1.0f));

            var bullet = GameObject.Instantiate<GameObject>(
                _bulletPrefab, spawnPoint, Quaternion.identity
            );
            var bulletRigidBody = bullet.GetComponent<Rigidbody>();
            bulletRigidBody.velocity =
                cameraTransform.forward * 5.0f +
                (dragEndPoint - dragStartPoint);
        }
    }

    private void OnTap(Vector2 point, Touch finger)
    {
        var camera = _cameraGyro._camera;
        var cameraTransform = camera.transform;
        
        var spawnPoint = camera.ScreenToWorldPoint(new Vector3(point.x, point.y, 1.0f));

        var bullet = GameObject.Instantiate<GameObject>(
            _bulletPrefab, spawnPoint, Quaternion.identity
        );
        var bulletRigidBody = bullet.GetComponent<Rigidbody>();
        
        bulletRigidBody.velocity = cameraTransform.forward * 5.0f;
    }
}
