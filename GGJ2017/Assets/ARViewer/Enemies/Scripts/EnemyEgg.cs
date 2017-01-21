using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEgg : MonoBehaviour
{

    public int Maxhealth = 3;
    public int CurrentHealth = 3;
    public bool Dead = false;

    // Use this for initialization
    void Start ()
    {
        CurrentHealth = Maxhealth;
	}
	
	// Update is called once per frame
	void Update ()
    {
		
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
