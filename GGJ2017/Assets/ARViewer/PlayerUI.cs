﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

	public Player _player;
	public Text _healthField;
    public Text _energyField;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        _healthField.text = _player.CurrentHealth.ToString();
        _energyField.text = _player.CurrentEnergy.ToString();
    }
}
