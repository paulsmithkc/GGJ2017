using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public GameObject _fragmentPrefab;
    public Rigidbody _rigidBody;
    public float _oscillatePeriod = 4.0f;
    public float _oscillateAmplitude = 2.0f;
    public float _oscillateTheta;

    public int Maxhealth = 3;
    private int CurrentHealth = 3;
    public bool Dead = false;

    // Use this for initialization
    void Start () {
        if (_rigidBody == null)
        {
            _rigidBody = GetComponent<Rigidbody>();
        }

        float tau = Mathf.PI * 2.0f;
        _oscillateTheta = Random.Range(0.0f, tau);
        _rigidBody.position += Vector3.up * (Mathf.Sin(_oscillateTheta) * _oscillateAmplitude);
        _rigidBody.velocity = Vector3.zero;

        CurrentHealth = Maxhealth;
    }
	
	// Update is called once per frame
	void Update () {
        float deltaTime = Time.deltaTime;
        float tau = Mathf.PI * 2.0f;

        float prevTheta = _oscillateTheta;
        float curTheta = prevTheta + (deltaTime / _oscillatePeriod) * tau;
        if (curTheta >= tau)
        {
            curTheta -= tau;
        }

        float oscillateDelta = Mathf.Sin(curTheta) - Mathf.Sin(prevTheta); 
        _rigidBody.position += Vector3.up * (oscillateDelta * _oscillateAmplitude);
        _rigidBody.velocity = Vector3.zero;
        _oscillateTheta = curTheta;

        //AddHealth(-1);
        if (Dead == true)
        {
            //particle effect to destroy objects
            for (int i = 0; i < 4; ++i)
            {
                float fireTheta = Random.Range(0.0f, 360.0f);
                float firePhi = Random.Range(60.0f, 75.0f);
                var q = Quaternion.Euler(-firePhi, fireTheta, 0.0f);
                var fireVelocity = q * Vector3.forward * 5.0f;

                var fragment = GameObject.Instantiate<GameObject>(
                    _fragmentPrefab, transform.position + fireVelocity * Random.Range(0.5f, 1.0f), Quaternion.identity
                );
                var fragmentRigidBody = fragment.GetComponent<Rigidbody>();
                fragmentRigidBody.velocity = fireVelocity;

                Destroy(fragment, 3.0f);
            }

            Destroy(this.gameObject);
        }

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
