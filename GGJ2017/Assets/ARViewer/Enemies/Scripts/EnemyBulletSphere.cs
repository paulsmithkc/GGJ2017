using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletSphere : MonoBehaviour {

    public GameObject _bulletPrefab = null;
    public float _timePerShot = 0.3f;
    public int _bulletsPerShot = 1;

    private Enemy _enemy;
    private float _shotDelay;

    // Use this for initialization
    void Start ()
    {
        if (_enemy == null)
        {
            _enemy = GetComponent<Enemy>();
        }
        _shotDelay = 0;
    }
	
	// Update is called once per frame
	void Update ()
    {
        float deltaTime = Time.deltaTime;
        if (!_enemy.Dead &&
            _enemy._oscillateTheta >= Mathf.PI * 1.25 && 
            _enemy._oscillateTheta <= Mathf.PI * 1.75)
        {
            _shotDelay -= deltaTime;
            if (_shotDelay <= 0)
            {
                _shotDelay += _timePerShot;
                for (int i = 0; i < _bulletsPerShot; ++i)
                {
                    float fireTheta = Random.Range(0.0f, 360.0f);
                    float firePhi = Random.Range(45.0f, 75.0f);
                    var q = Quaternion.Euler(-firePhi, fireTheta, 0.0f);
                    var fireVelocity = q * Vector3.forward * 7.0f;

                    var bullet = GameObject.Instantiate<GameObject>(
                        _bulletPrefab, transform.position + fireVelocity * 0.5f, Quaternion.identity
                    );
                    var bulletRigidBody = bullet.GetComponent<Rigidbody>();
                    bulletRigidBody.velocity = fireVelocity;
                }
            }
        }
    }
}
