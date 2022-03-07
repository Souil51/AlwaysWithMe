using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controller permettant de gérer les interactions quand la lampe est sélectionée
public class LampController : MonoBehaviour
{
    [SerializeField] private GameObject innerObject;

    // Start is called before the first frame update
    void Start()
    {
        DisableInnerObject();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableInnerObject()
    {
        innerObject.SetActive(true);
    }

    public void DisableInnerObject()
    {
        innerObject.SetActive(false);
    }
}
