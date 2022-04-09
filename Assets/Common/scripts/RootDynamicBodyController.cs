using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootDynamicBodyController : MonoBehaviour
{
    public delegate void CollisionEventHandler(object sender, EventArgs e);
    public event CollisionEventHandler CollisionEvent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Floor" || collision.transform.tag == "ObjetSol")
            CollisionEvent?.Invoke(this, EventArgs.Empty);
    }
}
