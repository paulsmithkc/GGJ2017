using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplittingBullets : Bullet
{
    public int splitStages = 2;
    public int splitBullets = 2;
    public float splitLifetime = 1.0f;
    public float finalLifetime = 3.0f;

    // Use this for initialization
    public override void Start()
    {
        _lifetime = splitStages <= 0 ? finalLifetime : splitLifetime;
    }

    // Update is called once per frame
    public override void Update()
    {
        float deltaTime = Time.deltaTime;
        _lifetime -= deltaTime;

        if (_lifetime <= 0.0f)
        {
            Vector3 pos = transform.position;
            Vector3 velocity = GetComponent<Rigidbody>().velocity;
            if (splitStages > 0)
            {
                for (int i = 0; i < splitBullets; ++i)
                {
                    float fireTheta = Random.Range(0.0f, 360.0f);
                    float firePhi = Random.Range(45.0f, 75.0f);
                    var q = Quaternion.Euler(firePhi, fireTheta, 0.0f);
                    var fireVelocity = q * velocity + Vector3.up * 1f;

                    var bullet = GameObject.Instantiate<GameObject>(
                        this.gameObject, pos, Quaternion.identity
                    );
                    bullet.transform.localScale *= 0.7f;
                    var bulletRigidBody = bullet.GetComponent<Rigidbody>();
                    bulletRigidBody.velocity = fireVelocity;
                    var bulletSplit = bullet.GetComponent<SplittingBullets>();
                    bulletSplit.splitStages -= 1;
                }
            }
            Destroy(this.gameObject);
        }
    }
}
