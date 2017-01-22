using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Origin : MonoBehaviour {
    public float _latitude;
    public float _longitude;

    public Vector3 LocationToWorld(float latitude, float longitude)
    {
        // SEE https://en.wikipedia.org/wiki/Geographic_coordinate_system

        double latitudeToMeters =
            111132.92
            - 559.82 * Mathf.Cos(2 * _latitude)
            + 1.175 * Mathf.Cos(4 * _latitude)
            - 0.0023 * Mathf.Cos(6 * _latitude);
        double longitudeToMeters =
            111412.84 * Mathf.Cos(_latitude)
            - 93.5 * Mathf.Cos(3 * _latitude)
            + 0.118 * Mathf.Cos(5 * _latitude);

        return transform.position +
            new Vector3(
                (float)((longitude - _longitude) * longitudeToMeters),
                0.0f,
                (float)((latitude - _latitude) * latitudeToMeters)
            );
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 1.0f);
    }
}
