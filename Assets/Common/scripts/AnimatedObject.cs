using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedObject : MonoBehaviour
{
    public void DisableObject()
    {
        this.gameObject.SetActive(false);
    }

    public void EnableObject()
    {
        this.gameObject.SetActive(true);
    }
}
