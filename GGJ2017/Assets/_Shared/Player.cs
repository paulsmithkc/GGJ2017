using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	
	public int CurrentHealth = 100;
	public int MaxHealth = 100;
	public bool Dead = false;
	
	// Use this for initialization
	void Start () {
		CurrentHealth = MaxHealth;
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
			Dead = true;
		}
	}
}
