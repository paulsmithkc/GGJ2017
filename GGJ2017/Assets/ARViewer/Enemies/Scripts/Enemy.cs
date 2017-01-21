using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public Rigidbody _rigidBody;
    public float _oscillatePeriod = 2.0f;
    public float _oscillateAmplitude = 2.0f;
    public float _oscillateTheta;
    
    // Use this for initialization
    void Start () {
        if (_rigidBody == null)
        {
            _rigidBody = GetComponent<Rigidbody>();
        }

        //float tau = Mathf.PI * 2.0f;
        _oscillateTheta = Random.Range(0.0f, 0.01f);
    }
	
	// Update is called once per frame
	void Update () {
        float deltaTime = Time.deltaTime;
        float tau = Mathf.PI * 2.0f;
        _oscillateTheta += (deltaTime / _oscillatePeriod) * tau;
        if (_oscillateTheta >= tau)
        {
            _oscillateTheta -= tau;
        }

        _rigidBody.velocity = Vector3.up * Mathf.Cos(_oscillateTheta) * _oscillateAmplitude * deltaTime;
    }
}
