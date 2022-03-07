using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controller permettant de g�rer les interactions quand la lampe est s�lection�e
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
