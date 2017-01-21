﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public Rigidbody _rigidBody;
    public float _oscillatePeriod = 4.0f;
    public float _oscillateAmplitude = 2.0f;
    public float _oscillateTheta;

    public int Maxhealth = 3;
    public int CurrentHealth = 3;
    public bool Dead = false;

    // Use this for initialization
    void Start () {
        if (_rigidBody == null)
        {
            _rigidBody = GetComponent<Rigidbody>();
        }

        float tau = Mathf.PI * 2.0f;
        _oscillateTheta = Random.Range(0.0f, tau);
        _rigidBody.position += Vector3.up * (Mathf.Cos(_oscillateTheta) * _oscillateAmplitude);
        _rigidBody.velocity = Vector3.zero;

        CurrentHealth = Maxhealth;
    }
	
	// Update is called once per frame
	void Update () {
        float deltaTime = Time.deltaTime;
        float tau = Mathf.PI * 2.0f;

        float prevTheta = _oscillateTheta;
        float curTheta = prevTheta + (deltaTime / _oscillatePeriod) * tau;
        if (curTheta >= tau) { curTheta -= tau; }

        float oscillateDelta = Mathf.Cos(curTheta) - Mathf.Cos(prevTheta); 
        _rigidBody.position += Vector3.up * (oscillateDelta * _oscillateAmplitude);
        _rigidBody.velocity = Vector3.zero;
        _oscillateTheta = curTheta;
    }

    public void AddHealth(int value)
    {
        CurrentHealth += value;
        if (CurrentHealth <= 0)
        {
            Dead = true;
        }
    }
}
