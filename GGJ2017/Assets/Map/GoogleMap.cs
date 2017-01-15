using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GoogleMap : MonoBehaviour
{
    public bool _autoUpdate = true;
    public GoogleMapMarker _centerMarker;
    public int _zoom = 13;
    //public bool _doubleResolution = false;
    public GoogleMapType _mapType;
    public GoogleMapMarker[] _markers;
    public GoogleMapPath[] _paths;
    public Image _image;
    public float _refreshDelay = 10.0f;

    private Texture2D _texture;
    private WWW _request;
    private float _refreshDelayRemaining = 10.0f;
    private const int _imageSize = 640;

    // Use this for initialization
    void Start()
    {
        _texture = null;
        _request = null;
        _refreshDelayRemaining = 0;

        if (_image == null)
        {
            _image = GetComponent<Image>();
        }

        UpdateImageScale();

        if (Input.location != null &&
            Input.location.isEnabledByUser &&
            Input.location.status != LocationServiceStatus.Running)
        {
            try
            {
                Input.location.Start();
                Debug.Log("GoogleMap started location service");
            }
            catch (Exception ex)
            {
                Debug.LogError("GoogleMap failed to start location service: " + ex.Message);
            }
        }

        if (_centerMarker == null)
        {
            Debug.LogError("GoogleMap center marker not initialized");
            _centerMarker = new GoogleMapMarker()
            {
                size = GoogleMapMarkerSize.mid,
                color = GoogleMapColor.red,
                label = "you"
            };
        }

        //var loc = Input.location.lastData;
        //_centerMarker.location.address = null;
        //_centerMarker.location.latitude = loc.latitude;
        //_centerMarker.location.longitude = loc.longitude;

        Refresh();
    }

    private void UpdateImageScale()
    {
        var imageSize = _imageSize;
        if (_image != null)
        {
            var imageScale = Mathf.Min((float)Screen.width / imageSize, (float)Screen.height / imageSize);
            var rect = _image.rectTransform;
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, imageSize * imageScale);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, imageSize * imageScale);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateImageScale();

        // If there is an ongoing request?
        if (_request != null)
        {
            if (_request.isDone)
            {
                if (_request.error == null)
                {
                    var imageSize = _imageSize;
                    if (_texture == null ||
                        imageSize != _texture.width ||
                        imageSize != _texture.height)
                    {
                        var tex = new Texture2D(imageSize, imageSize);
                        tex.LoadImage(_request.bytes);
                        _image.sprite = Sprite.Create(
                            tex,
                            new Rect(0, 0, imageSize, imageSize),
                            new Vector2(0.5f, 0.5f)
                        );
                        _texture = tex;
                    }
                    else
                    {
                        _texture.LoadImage(_request.bytes);
                    }

                    Debug.Log("GoogleMap Refresh Done");
                }
                else
                {
                    Debug.LogError("GoogleMap: " + _request.error);
                }

                _request = null;
                _refreshDelayRemaining = _refreshDelay;
            }
        }
        // Is auto update disabled?
        else if (!_autoUpdate || 
                 Input.location == null ||
                 !Input.location.isEnabledByUser ||
                 Input.location.status != LocationServiceStatus.Running)
        {
            // Do nothing
        }
        else
        {
            var deltaTime = Time.deltaTime;
            if (deltaTime > _refreshDelayRemaining)
            {
                _refreshDelayRemaining -= deltaTime;
                return;
            }
            else
            {
                _refreshDelayRemaining = 0;
            }

            // If the location has changed, then refresh the map
            var loc = Input.location.lastData;
            if (loc.latitude != _centerMarker.location.latitude ||
                loc.longitude != _centerMarker.location.longitude)
            {
                Debug.Log(string.Format("you: {0},{1}", loc.latitude, loc.longitude));

                _centerMarker.location.address = null;
                _centerMarker.location.latitude = loc.latitude;
                _centerMarker.location.longitude = loc.longitude;
                Refresh();
            }
        }
    }

    public void Refresh()
    {
        _refreshDelayRemaining = 0;
        if (_request != null) { return; }
        if (_image == null) { return; }

        Debug.Log("GoogleMap Refresh Started");

        var url = "http://maps.googleapis.com/maps/api/staticmap";
        var qs = new StringBuilder();

        qs.Append("size=").Append(WWW.EscapeURL(string.Format("{0}x{0}", _imageSize)));
        //qs.Append("&scale=").Append(_doubleResolution ? "2" : "1");
        qs.Append("&maptype=").Append(_mapType.ToString().ToLower());

        if (_centerMarker != null)
        {
            qs.Append("&center=").Append(_centerMarker.location.ToString());
            qs.Append("&zoom=").Append(_zoom);
            qs.Append("&markers=").Append(_centerMarker.ToString());
        }

        var usingSensor = false;
        usingSensor =
            Input.location != null &&
            Input.location.isEnabledByUser &&
            Input.location.status == LocationServiceStatus.Running;
        qs.Append("&sensor=").Append(usingSensor ? "true" : "false");

        foreach (var i in _markers)
        {
            qs.Append("&markers=").Append(i.ToString());
        }

        foreach (var i in _paths)
        {
            qs.Append("&path=").Append(i.ToString());
        }

        _request = new WWW(url + "?" + qs.ToString());
    }

    public void ZoomIn()
    {
        ++_zoom;
        Refresh();
    }

    public void ZoomOut()
    {
        --_zoom;
        Refresh();
    }
}


public enum GoogleMapType
{
    RoadMap,
    Satellite,
    Terrain,
    Hybrid
}

public enum GoogleMapColor
{
    black,
    brown,
    green,
    purple,
    yellow,
    blue,
    gray,
    orange,
    red,
    white
}

public enum GoogleMapMarkerSize
{
    tiny,
    small,
    mid
}

[System.Serializable]
public struct GoogleMapLocation
{
    public string address;
    public float latitude;
    public float longitude;

    public override string ToString()
    {
        if (!string.IsNullOrEmpty(address))
        {
            return WWW.EscapeURL(address);
        }
        else
        {
            return WWW.EscapeURL(string.Format("{0},{1}", latitude, longitude));
        }
    }
}

[System.Serializable]
public class GoogleMapMarker
{
    public GoogleMapMarkerSize size;
    public GoogleMapColor color;
    public string label;
    public GoogleMapLocation location;

    public override string ToString()
    {
        return string.Format(
            "size:{0}|color:{1}|label:{2}|", 
            size.ToString().ToLower(), 
            color, 
            label, 
            location.ToString()
        );
    }
}

[System.Serializable]
public class GoogleMapPath
{
    public int weight = 5;
    public GoogleMapColor color;
    public bool fill = false;
    public GoogleMapColor fillColor;
    public GoogleMapLocation[] locations;

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendFormat("weight:{0}|color:{1}", weight, color);
        if (fill)
        {
            sb.Append("|fillcolor:").Append(fillColor.ToString().ToLower());
        }
        foreach (var loc in locations)
        {
            sb.Append("|").Append(loc.ToString());
        }

        return sb.ToString();
    }
}
