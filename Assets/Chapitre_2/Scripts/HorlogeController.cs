using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorlogeController : MonoBehaviour
{
    [SerializeField] private GameObject aiguilleHeure;
    [SerializeField] private GameObject aiguilleMinutes;
    [SerializeField] private float vitesse = 10;//mise à jour par seconde

    // Update is called once per frame
    void Update()
    {
        Vector3 vMinutes = aiguilleMinutes.transform.eulerAngles;
        Vector3 vHeures = aiguilleHeure.transform.eulerAngles;

        aiguilleMinutes.transform.eulerAngles = new Vector3(0, 0, vMinutes.z - vitesse * Time.deltaTime);
        //Les heures vont 12 fois moins vite
        aiguilleHeure.transform.eulerAngles = new Vector3(0, 0, vHeures.z - vitesse / 12 * Time.deltaTime);
    }

    public void SetSpeed(float fVitesse)
    {
        vitesse = fVitesse;
    }

    public float GetSpeed()
    {
        return vitesse;
    }

    public float GetHeureEulerAngle()
    {
        return aiguilleHeure.transform.eulerAngles.z;
    }
}
