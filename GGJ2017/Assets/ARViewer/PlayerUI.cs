using System;
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
    private const int _flingCost = 2;

    private Dictionary<int, FlingState> _flingStates = new Dictionary<int, FlingState>();

    [Serializable]
    private class FlingState
    {
        public int fingerId;
        public float startTime;
        public Vector2 startPosition;
    }

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

        _flingStates.Clear();
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
        int fingerId = finger.fingerId;
        if (!_flingStates.ContainsKey(fingerId))
        {
            _flingStates.Add(
                fingerId, 
                new FlingState { fingerId = fingerId, startTime = Time.time, startPosition = start }
            );
        }
    }

    private void OnDragEnd(Vector2 start, Vector2 end, Touch finger)
    {
        int fingerId = finger.fingerId;
        FlingState state;
        if (_flingStates.TryGetValue(fingerId, out state))
        {
            _flingStates.Remove(fingerId);

            if (_player.CurrentEnergy >= _flingCost)
            {
                _player.AddEnergy(-_flingCost);
            }
            else
            {
                return;
            }

            var camera = _cameraGyro._camera;
            var cameraTransform = camera.transform;
            
            var dragEndPoint = camera.ScreenToWorldPoint(new Vector3(end.x, end.y, 1.0f));
            var dragStartPoint = camera.ScreenToWorldPoint(new Vector3(state.startPosition.x, state.startPosition.y, 1.0f));
            var spawnPoint = camera.ScreenToWorldPoint(new Vector3(state.startPosition.x, state.startPosition.y, 1.0f));

            var bullet = GameObject.Instantiate<GameObject>(
                _bulletPrefab, spawnPoint, Quaternion.identity
            );
            var bulletRigidBody = bullet.GetComponent<Rigidbody>();
            bulletRigidBody.velocity =
                cameraTransform.forward * 5.0f +
                (dragEndPoint - dragStartPoint) / (Time.time - state.startTime);
        }
    }

    private void OnTap(Vector2 point, Touch finger)
    {
        _flingStates.Remove(finger.fingerId);

        if (_player.CurrentEnergy >= _flingCost)
        {
            _player.AddEnergy(-_flingCost);
        }
        else
        {
            return;
        }

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
