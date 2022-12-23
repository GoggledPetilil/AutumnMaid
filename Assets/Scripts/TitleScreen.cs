using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.TransferPlayer(2, Vector2.zero, true);
    }
}
