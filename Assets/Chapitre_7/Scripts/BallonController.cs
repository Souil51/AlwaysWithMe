using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallonController : MonoBehaviour
{
    [SerializeField] private float fLimitX_1 = -5f;
    [SerializeField] private float fLimitX_2 = 5f;
    [SerializeField] private float fLimitY_1 = -3f;
    [SerializeField] private float fLimitY_2 = 3f;

    [SerializeField] private float fXSpeed = 1f;
    [SerializeField] private float fYSpeed = 1f;

    private bool bIsTargeted = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float fNewX = transform.position.x + (fXSpeed * Time.deltaTime);
        float fNewY = transform.position.y + (fYSpeed * Time.deltaTime);

        transform.position = new Vector3(fNewX, fNewY, transform.position.z);

        if(transform.position.x < fLimitX_1 || transform.position.x > fLimitX_2)
        {
            fXSpeed *= -1;
        }

        if(transform.position.y < fLimitY_1 || transform.position.y > fLimitY_2)
        {
            fYSpeed *= -1;
        }
    }

    public bool IsTargeted()
    {
        return bIsTargeted;
    }

    public void SetEnabled(bool bValue)
    {
        this.gameObject.SetActive(bValue);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision != null && collision.CompareTag("TargetCollider"))
        {
            bIsTargeted = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null && collision.CompareTag("TargetCollider"))
        {
            bIsTargeted = false;
        }
    }
}
