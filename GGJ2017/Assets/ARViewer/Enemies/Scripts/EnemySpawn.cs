using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{

    public GameObject Cube1;

    public float _latitude;
    public float _longitude;

    // Use this for initialization
    void Start()
    {
        var cameraGyro = GameObject.FindObjectOfType<CameraGyro>();
        if (cameraGyro != null)
        {
            cameraGyro._origin = new CameraGyro.Origin
            {
                latitude = _latitude,
                longitude = _longitude,
                transform = this.transform
            };
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.FindGameObjectWithTag("Enemy") == null)
        {
            GameObject.Instantiate<GameObject>(
                Cube1,
                transform.position,
                Quaternion.identity
            );
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 1.0f);
    }
}
