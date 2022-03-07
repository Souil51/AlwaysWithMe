using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    [SerializeField] private GameObject goInnerTarget;

    [SerializeField] private float fScale = 3f;//Taille de la boucle
    [SerializeField] private float fTimeFactor = 2f;//Vitesse de mouvement
    [SerializeField] private float fShapeFactor = 2.5f;//Forme de la boucle (2 = ~un signe infini)

    [SerializeField] private float fRotationScale = 10f;

    [SerializeField] private float FollowSpeed = 8f;

    private float fLastTimeCos = 0;
    private float fRotationSign = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Mouvement automatique
        float fSin = (Mathf.Sin(fShapeFactor * Time.time * fTimeFactor) / 2) * fScale;
        float fCos = Mathf.Cos(Time.time * fTimeFactor) * fScale;

        goInnerTarget.transform.localPosition = new Vector3(fCos, fSin, 1);

        fCos = Mathf.Cos(Time.time);

        if (fCos * fLastTimeCos < 0) 
        {
            fRotationSign *= -1;
        }

        fLastTimeCos = fCos;

        float fRotation = transform.eulerAngles.z + (fRotationScale * fRotationSign * Time.deltaTime);
        
        transform.eulerAngles = new Vector3(0, 0, fRotation);
        goInnerTarget.transform.localEulerAngles = new Vector3(0, 0, 360 - fRotation);

        //Mouvement de la souris
        Vector3 vMouse = GetMousePosition();

        float fFinalX = vMouse.x;
        float fFinalY = vMouse.y;

        if (fFinalX < -7.8f) fFinalX = -7.8f;
        if (fFinalX > 8.4f) fFinalX = 8.4f;

        if (fFinalY < -0.3f) fFinalY = -0.3f;
        if (fFinalY > 7.7f) fFinalY = 7.7f;

        Vector3 vMouseFinalPos = new Vector3(fFinalX, fFinalY, vMouse.z);

        Vector3 vResLerp = Vector3.Lerp(transform.position, vMouseFinalPos, FollowSpeed * Time.deltaTime);

        transform.position = new Vector3(vResLerp.x, vResLerp.y, transform.position.z);
    }

    private Vector3 GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 rayPoint = ray.GetPoint(1);
        Vector3 vRes = new Vector3(rayPoint.x, rayPoint.y, transform.position.z);

        return vRes;
    }
}
