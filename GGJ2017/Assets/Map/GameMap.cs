using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMap : MonoBehaviour {

    public GoogleMap _googleMap;
    public GestureDetector _gestureDetector;

    // Use this for initialization
    void Start () {

        // Find missing links
        if (_googleMap == null)
        {
            _googleMap = GameObject.FindObjectOfType<GoogleMap>();
        }
        if (_gestureDetector == null)
        {
            _gestureDetector = GameObject.FindObjectOfType<GestureDetector>();
        }

        // Hook up the event listener
        if (_gestureDetector != null && _googleMap != null)
        {
            _gestureDetector.pinch += OnPinch;
        }
    }
	
	// Update is called once per frame
	void Update () {
	}

    private void OnPinch(Vector2 center, float scaleFactor, Touch finger1, Touch finger2)
    {
        _googleMap.Zoom(center, scaleFactor);
    }
}
