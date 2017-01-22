using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{

    public GameObject Cube1;
    public GameObject Egg1;
    public GameObject Pawn1;
    public int wave = 0;
    public bool lastWave = false;
    public bool win = false;

    public float _latitude;
    public float _longitude;

    // Use this for initialization
    void Start()
    {
        lastWave = false;
        win = false;

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
        if (!win && GameObject.FindGameObjectWithTag("Enemy") == null)
        {
            if (lastWave)
            {
                win = true;
                return;
            }

            ++wave;
            int green = Random.Range(3, 7);
            int blue = green + Random.Range(3, 7);
            int red = blue + Random.Range(3, 7);

            for (int i = 0; i < wave; ++i)
            {
                GameObject prefab = null;
                if (i < green)
                {
                    prefab = Egg1;
                }
                else if (i < blue)
                {
                    prefab = Cube1;
                }
                else if (i < red)
                {
                    prefab = Pawn1;
                }
                else
                {
                    lastWave = true;
                    break;
                }

                if (prefab != null)
                {
                    Vector2 pos = Random.insideUnitCircle * 10.0f;
                    GameObject.Instantiate<GameObject>(
                        prefab,
                        transform.position + new Vector3(pos.x, 0.0f, pos.y),
                        Quaternion.identity
                    );
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 1.0f);
    }
}
