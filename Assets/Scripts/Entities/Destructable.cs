using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    public virtual void GetHit()
    {
        GameManager.instance.SpawnSparkles(this.gameObject.transform.position);
        this.gameObject.SetActive(false);
    }
}
