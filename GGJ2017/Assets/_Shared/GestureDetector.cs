using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * Based on "Unity and Touch Controls"
 * http://pixelnest.io/tutorials/unity-touch-controls/
 */
public class GestureDetector : MonoBehaviour
{
    public delegate void TapEventListener(Vector2 point, Touch finger);
    public delegate void DragEventListener(Vector2 start, Vector2 end, Touch finger);
    public delegate void PinchEventListener(Vector2 center, float scaleFactor, Touch finger1, Touch finger2);
    
    public event TapEventListener tap;
    public event DragEventListener drag;
    public event PinchEventListener pinch;
    public event PinchEventListener pinchStart;
    public event PinchEventListener pinchEnd;

    private bool _pinching = false;
    //private Touch _pinchFinger1;
    //private Touch _pinchFinger2;
    private float _pinchPrevDistance;

    private Dictionary<int, Touch> _prevFingerStates = new Dictionary<int, Touch>();
    
    // Use this for initialization
    void Start()
    {
        _pinching = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Pinching only works with two fingers
        if (_pinching || Input.touchCount == 2)
        {
            var finger1 = Input.GetTouch(0);
            var finger2 = Input.GetTouch(1);
            Vector2 finger1position = finger1.position;
            Vector2 finger2position = finger2.position;

            float distance = Vector2.Distance(finger1.position, finger2.position);
            Vector2 center = Vector2.Lerp(finger1position, finger2position, 0.5f);

            if (finger1.phase == TouchPhase.Began || finger2.phase == TouchPhase.Began)
            {
                _pinching = true;
                //_pinchFinger1 = finger1;
                //_pinchFinger2 = finger2;
                _pinchPrevDistance = distance;
                _prevFingerStates.Clear();
                
                if (pinchStart != null)
                {
                    pinchStart(center, 1.0f, finger1, finger2);
                }
            }
            else if (finger1.phase == TouchPhase.Ended || finger2.phase == TouchPhase.Ended)
            {
                _pinching = false;
                _pinchPrevDistance = distance;

                if (pinchEnd != null)
                {
                    pinchEnd(center, 1.0f, finger1, finger2);
                }
            }
            else if (finger1.phase == TouchPhase.Moved || finger2.phase == TouchPhase.Moved)
            {
                float scaleFactor = distance / _pinchPrevDistance;
                _pinchPrevDistance = distance;

                if (pinch != null)
                {
                    pinch(center, scaleFactor, finger1, finger2);
                }
            }
        }
        
        if (!_pinching)
        {
            // Look for all fingers
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                int fingerId = touch.fingerId;

                if (touch.phase == TouchPhase.Began)
                {
                    Touch prevState;
                    if (_prevFingerStates.TryGetValue(fingerId, out prevState))
                    {
                        _prevFingerStates[fingerId] = touch;
                    }
                    else
                    {
                        _prevFingerStates.Add(fingerId, touch);
                    }
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    Touch prevState;
                    if (_prevFingerStates.TryGetValue(fingerId, out prevState))
                    {
                        _prevFingerStates[fingerId] = touch;
                        if (drag != null)
                        {
                            drag(prevState.position, touch.position, touch);
                        }
                    }
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    Touch prevState;
                    if (_prevFingerStates.TryGetValue(fingerId, out prevState))
                    {
                        _prevFingerStates.Remove(fingerId);
                        if (prevState.phase == TouchPhase.Began && touch.tapCount == 1)
                        {
                            if (tap != null)
                            {
                                tap(touch.position, touch);
                            }
                        }
                        else
                        {
                            if (drag != null)
                            {
                                drag(prevState.position, touch.position, touch);
                            }
                        }
                    }
                }
            }
        }
    }
}
