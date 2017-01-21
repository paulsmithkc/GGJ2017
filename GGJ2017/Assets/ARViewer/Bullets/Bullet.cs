using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float _lifetime = 5.0f;
    public int _damage = 1;

    // Use this for initialization
    void Start()
    {
        GameObject.DestroyObject(this.gameObject, _lifetime);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnCollisionEnter(Collision col)
    {
        //Debug.Log("OnCollisionEnter");
        OnCollideImpl(col.gameObject);
    }

    void OnTriggerEnter(Collider col)
    {
        //Debug.Log("OnTriggerEnter");
        OnCollideImpl(col.gameObject);
    }

    private void OnCollideImpl(GameObject obj)
    {
        string bulletTag = this.tag;
        string colTag = obj.tag;
        //Debug.LogFormat("OnCollideImpl {0}, {1}", bulletTag, colTag);

        if (string.Equals(bulletTag, "EnemyBullet") && 
            (string.Equals(colTag, "Player") || string.Equals(colTag, "MainCamera")))
        {
            var player = obj.GetComponent<Player>();
            if (player != null)
            {
                player.AddHealth(-_damage);
            }
        }
        if (string.Equals(bulletTag, "PlayerBullet") && string.Equals(colTag, "Enemy"))
        {
            var enemy = obj.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.AddHealth(-_damage);
            }
        }
    }
}
