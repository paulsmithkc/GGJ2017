using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour {

    public GameObject Cube1;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (GameObject.FindGameObjectWithTag("Enemy") == null)
        {
            GameObject.Instantiate<GameObject>(
                Cube1, 
                transform.position, 
                Quaternion.identity
            );
        }
    }
}
