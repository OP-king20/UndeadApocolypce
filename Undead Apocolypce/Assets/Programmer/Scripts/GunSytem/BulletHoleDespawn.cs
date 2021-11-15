using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHoleDespawn : MonoBehaviour
{
    public float TimeLeft = 5;
    void Update()
    {
        TimeLeft -= Time.deltaTime;
        if (TimeLeft < 0)
        {
            Destroy(gameObject);
        }
    }
}
