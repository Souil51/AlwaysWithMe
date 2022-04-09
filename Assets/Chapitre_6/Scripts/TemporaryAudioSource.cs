using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryAudioSource : MonoBehaviour
{
    float fTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        fTime += Time.deltaTime;

        if (fTime > 0.5f)
            Destroy(gameObject);
    }
}
