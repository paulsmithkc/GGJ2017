﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

    public Player _player;
	public Text _healthField;
    public Text _energyField;
    public Text _rotationField;
    public Text _locationField;

    public Camera _camera;
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
        if (_player == null)
        {
            _player = GameObject.FindObjectOfType<Player>();
        }
        if (_camera == null)
        {
            _camera = Camera.main;
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
        if (_healthField != null && _player != null)
        {
            _healthField.text = _player.CurrentHealth.ToString();
        }
        if (_energyField != null && _player != null)
        {
            _energyField.text = _player.CurrentEnergy.ToString();
        }
        //if (_rotationField != null && _cameraGyro != null)
        //{
        //    _rotationField.text = string.Format(
        //        "{0,3:0} {1,3:0} {2,3:0}", 
        //        _cameraGyro.roll, 
        //        _cameraGyro.pitch, 
        //        _cameraGyro.heading
        //    );
        //}
        //if (_locationField != null && _cameraGyro != null)
        //{
        //    var pos = _cameraGyro.transform.position;
        //    var vel = _cameraGyro.velocity;
        //    _locationField.text = string.Format(
        //        "{0:F5} {1:F5} {3}\n{4,5:0.0} {5,5:0.0} {6,5:0.0}\n{7,5:0.0} {8,5:0.0} {9,5:0.0}",
        //        _cameraGyro.latitude,
        //        _cameraGyro.longitude,
        //        _cameraGyro.altitude,
        //        _cameraGyro.locationStatus,
        //        pos.x, pos.y, pos.z,
        //        vel.x, vel.y, vel.z
        //    );
        //}
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

            var camera = _camera;
            var cameraTransform = camera.transform;
            
            var dragEndPoint = camera.ScreenToWorldPoint(new Vector3(end.x, end.y, 1.0f));
            var dragStartPoint = camera.ScreenToWorldPoint(new Vector3(state.startPosition.x, state.startPosition.y, 1.0f));
            var spawnPoint = camera.ScreenToWorldPoint(new Vector3(state.startPosition.x, state.startPosition.y, 1.0f));

            var bullet = GameObject.Instantiate<GameObject>(
                _bulletPrefab, spawnPoint, Quaternion.identity
            );
            var bulletRigidBody = bullet.GetComponent<Rigidbody>();
            bulletRigidBody.velocity =
                cameraTransform.forward * 10.0f +
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

        var camera = _camera;
        var cameraTransform = camera.transform;
        
        var spawnPoint = camera.ScreenToWorldPoint(new Vector3(point.x, point.y, 1.0f));

        var bullet = GameObject.Instantiate<GameObject>(
            _bulletPrefab, spawnPoint, Quaternion.identity
        );
        var bulletRigidBody = bullet.GetComponent<Rigidbody>();
        
        bulletRigidBody.velocity = cameraTransform.forward * 10.0f;
    }
}
