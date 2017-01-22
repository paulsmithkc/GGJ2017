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
		
		_googleMap._markers = new GoogleMapMarker[4];
		_googleMap._markers[0] = new GoogleMapMarker
		{
			size = GoogleMapMarkerSize.mid,
			color = GoogleMapColor.blue,
			label = "1",
			location = new GoogleMapLocation
			{
				latitude = 38.711320f, 
				longitude = -90.311533f
			}
		};
		_googleMap._markers[1] = new GoogleMapMarker
		{
			size = GoogleMapMarkerSize.mid,
			color = GoogleMapColor.blue,
			label = "2",
			location = new GoogleMapLocation
			{
				latitude = 38.711167f, 
				longitude = -90.311361f
			}
		};
		
		_googleMap._markers[2] = new GoogleMapMarker
		{
			size = GoogleMapMarkerSize.mid,
			color = GoogleMapColor.blue,
			label = "3",
			location = new GoogleMapLocation
			{
				latitude = 38.710472f, 
				longitude = -90.311194f
			}
		};
		
		_googleMap._markers[3] = new GoogleMapMarker
		{
			size = GoogleMapMarkerSize.mid,
			color = GoogleMapColor.blue,
			label = "4",
			location = new GoogleMapLocation
			{
				latitude = 38.709778f, 
				longitude = -90.311778f
			}
		};
    }

    // Update is called once per frame
    void Update () {
	}

    private void OnPinch(Vector2 center, float scaleFactor, Touch finger1, Touch finger2)
    {
        _googleMap.Zoom(center, scaleFactor);
    }
}
