using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLeaveController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySound(Sound sound)
    {
        MusicController.GetInstance().PlaySound(sound);
    }
}
