using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldHud : MonoBehaviour
{

    private void Update()
    {
        this.transform.forward = Camera.main.transform.forward;
    }
}
