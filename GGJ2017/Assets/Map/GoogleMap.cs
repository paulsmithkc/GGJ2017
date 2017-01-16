using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GoogleMap : MonoBehaviour
{
    public GoogleMapMarker _centerMarker;
    public GoogleMapType _mapType;
    public GoogleMapStyle[] _styles;
    public GoogleMapMarker[] _markers;
    public GoogleMapPath[] _paths;
    public Image _image;
    public float _refreshDelay = 2.0f;

    private Texture2D _texture;
    private string _url;
    private WWW _request;
    private float _refreshDelayRemaining = 0.0f;
    private bool _dirty = false;
    private int _zoom = 13;

    private const int _imageSize = 1024;
    private const int _imageScale = 2;
    private const int _minZoom = 10;
    private const int _maxZoom = 20;
    private const string _apiKey = "AIzaSyAC1BuMdumR_gvb7nrTkGVvyJlIgp8lgvI";

    // Use this for initialization
    void Start()
    {
        _texture = null;
        _url = null;
        _request = null;
        _refreshDelayRemaining = 0;
        _dirty = true;

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
                label = ""
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

                    //Debug.Log("GoogleMap Refresh Done");
                }
                else
                {
                    Debug.LogError("GoogleMap: " + _request.error);
                }

                _request = null;
                _refreshDelayRemaining = _refreshDelay;
            }
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

            if (Input.location != null &&
                Input.location.isEnabledByUser &&
                Input.location.status == LocationServiceStatus.Running)
            {
                var loc = Input.location.lastData;
                if (loc.latitude != _centerMarker.location.latitude ||
                    loc.longitude != _centerMarker.location.longitude)
                {
                    Debug.Log(string.Format("you: {0},{1}", loc.latitude, loc.longitude));

                    _dirty = true;
                    _centerMarker.location.address = null;
                    _centerMarker.location.latitude = loc.latitude;
                    _centerMarker.location.longitude = loc.longitude;
                }
            }

            if (_dirty)
            {
                Refresh();
            }
        }
    }

    public void Refresh()
    {
        _dirty = true;
        _refreshDelayRemaining = 0;

        if (_request != null) { return; }
        if (_image == null) { return; }

        //Debug.Log("GoogleMap Refresh Started");

        var url = "https://maps.googleapis.com/maps/api/staticmap";
        var qs = new StringBuilder();

        qs.Append("size=").Append(WWW.EscapeURL(string.Format("{0}x{0}", _imageSize / _imageScale)));
        qs.Append("&scale=").Append(_imageScale);
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

        if (!string.IsNullOrEmpty(_apiKey))
        {
            qs.Append("&key=").Append(WWW.EscapeURL(_apiKey));
        }

        foreach (var i in _styles)
        {
            qs.Append("&style=").Append(i.ToString());
        }
        foreach (var i in _markers)
        {
            qs.Append("&markers=").Append(i.ToString());
        }
        foreach (var i in _paths)
        {
            qs.Append("&path=").Append(i.ToString());
        }

        url += "?" + qs.ToString();
        _url = url;
        _request = new WWW(url);
        _dirty = false;

        Debug.Log(url);
    }

    public void ZoomIn()
    {
        if (_zoom < _maxZoom)
        {
            ++_zoom;
            Refresh();
        }
    }

    public void ZoomOut()
    {
        if (_zoom > _minZoom)
        {
            --_zoom;
            Refresh();
        }
    }

    public string CurrentUrl
    {
        get { return _url; }
    }

    public float RefreshDelayRemaining
    {
        get { return _refreshDelayRemaining; }
    }

    public bool IsRefreshing
    {
        get { return _request != null; }
    }
}


public enum GoogleMapType
{
    roadmap,
    satellite,
    terrain,
    hybrid
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
    public string label;  // labels must be a single alphanumeric character
    public GoogleMapLocation location;

    public override string ToString()
    {
        return string.Format(
            "size:{0}|color:{1}|label:{2}|{3}", 
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

[System.Serializable]
public class GoogleMapStyle
{
    public string feature;
    public string element;
    public GoogleMapStyleRule[] rules;

    public override string ToString()
    {
        var sb = new StringBuilder();
        
        sb.AppendFormat("feature:{0}", string.IsNullOrEmpty(feature) ? "all" : feature);

        if (!string.IsNullOrEmpty(element))
        {
            sb.AppendFormat("|element:{0}", element);
        }
        foreach (var r in rules)
        {
            sb.AppendFormat("|{0}:{1}", r.key, r.value);
        }

        return sb.ToString();
    }
}

[System.Serializable]
public class GoogleMapStyleRule
{
    public string key;
    public string value;
}
