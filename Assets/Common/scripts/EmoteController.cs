using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmoteController : MonoBehaviour
{
    [SerializeField] private GameObject goParent;

    public void DestroyObject()
    {
        Destroy(goParent);
    }
}
