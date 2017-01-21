using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	
	public int CurrentHealth = 100;
	public int MaxHealth = 100;
    public int CurrentEnergy = 100;
    public int MaxEnergy = 100;
    public bool Dead = false;
    
	
	// Use this for initialization
	void Start () {
		CurrentHealth = MaxHealth;
        CurrentEnergy = MaxEnergy;
		Dead = false; 
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void AddHealth(int value)
	{
		CurrentHealth += value;
		if(CurrentHealth > MaxHealth)
		{
			CurrentHealth = MaxHealth;
		}
		if(CurrentHealth <= 0)
		{
            CurrentHealth = 0;
			Dead = true;
		}
	}

    public void AddEnergy(int value)
    {
        CurrentEnergy += value;
        if(CurrentEnergy > MaxEnergy)
        {
            CurrentEnergy = MaxEnergy;
        }

        if(MaxEnergy <= 0)
        {
            MaxEnergy = 0;
        }
    }
}
