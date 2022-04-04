using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BodyDirection { Gauche = 0, Droite = 1 }
public enum PersoType { Perso = 0, Maxine = 1}

public class MovingBody : MonoBehaviour
{
    public static List<Sprite> lstSprites;

    [SerializeField] private SpeakingBody speakingCtrl;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject Arm_1_Target;
    [SerializeField] private GameObject Arm_1;
    [SerializeField] private float fSpeed = 0.5f;
    [SerializeField] private PersoType persoType = PersoType.Perso;

    private Vector3 vStartMovingTargetPosition;//Position de d�part de la cible des bras

    private IEnumerator currentStopCoroutine;
    private BodyDirection currentDirection = BodyDirection.Gauche;
    Vector3 vStartingScale;

    //Emotes : d�calage des �motes pour partir de la t�te
    private float fEmoteXSpawn = 1f;
    private float fEmoteYSpawn = 15.3f;
    private float fEmoteYSpawn_Maxine = 13.3f;

    //Etats
    private bool IsActive = true;
    private bool bIsMoving = false;
    private bool bIsStopping = false;
    private bool bIsGoingToPosition = false;

    private float fMovingTime = 0f;

    void Start()
    {
        vStartingScale = transform.localScale;
        vStartMovingTargetPosition = Arm_1_Target.transform.localPosition;

        fEmoteYSpawn = fEmoteYSpawn * (gameObject.transform.localScale.x);
        fEmoteYSpawn_Maxine = fEmoteYSpawn_Maxine * (gameObject.transform.localScale.x);
    }

    void Update()
    {
        if (!IsActive)
        {
            if (!bIsMoving)
                return;
            else
                bIsMoving = false;
        }

        //Si on est en mouvement
        if (bIsMoving) 
        {
            //Si on est en train d'arr�ter le mouvement, on stop la coroutine pour pouvoir rebouger imm�diatement
            if (currentStopCoroutine != null)
            {
                StopCoroutine(currentStopCoroutine);
                currentStopCoroutine = null;
            }

            //Calcul de la position de la souris
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(0);
            Vector3 vRes = new Vector3(rayPoint.x, rayPoint.y, -1);

            //On met la target du bras sur la souris
            Arm_1_Target.transform.position = new Vector3(vRes.x, vRes.y, Arm_1_Target.transform.position.z);

            //L'animation du joueur d�pend de la distance entre l'effector du bras et la target
            float fDistance = Vector3.Distance(Arm_1_Target.transform.position, Arm_1.transform.position);

            Vector3 vDirection = Arm_1_Target.transform.position - Arm_1.transform.position;
            //Gestion du sens vers la droite ou vers la gauche du perso
            if (currentDirection == BodyDirection.Gauche && vDirection.x < 0 && fDistance > 0.5f)
            {
                ChangeDirection(BodyDirection.Droite);
            }
            else if(currentDirection == BodyDirection.Droite && vDirection.x > 0 && fDistance > 0.5f)
            {
                ChangeDirection(BodyDirection.Gauche);
            }

            if (fDistance > 5)
                fDistance = 5;

            float fBlendValue = fDistance / 5;

            if (fBlendValue <= 0.1f)
                fBlendValue = 0;

            SetBlendValue(fBlendValue);

            //On modifie la position, seulement si le joueur n'est pas hore de l'�cran
            if (gameObject.transform.position.x < CommonController.LIMIT_X_ECRAN && gameObject.transform.position.x > -CommonController.LIMIT_X_ECRAN)
            {
                //La vitesse d�pend aussi de la distance pour que plus l'animation est proche de la course, plus il va vite
                if (vDirection.x > 0)//Vers la droite
                    transform.position += new Vector3(fSpeed * fBlendValue * Time.deltaTime, 0, 0);
                else//Vers la gauche
                    transform.position -= new Vector3(fSpeed * fBlendValue * Time.deltaTime, 0, 0);
            }

            fMovingTime += Time.deltaTime;
        }
        else
        {
            StopMoving();
        }
    }

    public void ChangeDirection(BodyDirection newDirection)
    {
        if(currentDirection != newDirection)
        {
            if (newDirection == BodyDirection.Droite)
            {
                currentDirection = BodyDirection.Droite;

                transform.localScale = new Vector3(-vStartingScale.x, vStartingScale.y, vStartingScale.z);
                transform.position = new Vector3(transform.position.x - 3f, transform.position.y, transform.position.z);
                fEmoteXSpawn -= 2f;
            }
            else
            {
                currentDirection = BodyDirection.Gauche;

                transform.localScale = new Vector3(vStartingScale.x, vStartingScale.y, vStartingScale.z);
                transform.position = new Vector3(transform.position.x + 3f, transform.position.y, transform.position.z);
                fEmoteXSpawn += 2f;
            }
        }
    }

    //Modifie la valeur de Blend de l'animation (0 = Idle, 1 = Run)
    private void SetBlendValue(float fValue)
    {
        animator.SetFloat("Blend", fValue);
    }

    public void StopMoving(float fDuration = 0.5f)
    {
        if (currentStopCoroutine != null || animator.GetFloat("Blend") == 0)
            return;

        currentStopCoroutine = coroutine_StopMoving(fDuration);
        StartCoroutine(currentStopCoroutine);
    }

    public void SetActive(bool bValue)
    {
        IsActive = bValue;
    }

    public void SetMoving(bool bValue)
    {
        bIsMoving = bValue;
    }

    public bool IsMoving()
    {
        return bIsMoving;
    }

    //Arr�te le mouvement en fDuration secondes et replace le bras le long du corp
    private IEnumerator coroutine_StopMoving(float fDuration = 0.5f)
    {
        bIsStopping = true;
        float fStartValue = animator.GetFloat("Blend");
        Vector3 vStartTargetPosition = Arm_1_Target.transform.localPosition;

        Vector3 vFromTragetPosition = vStartMovingTargetPosition;

        float fElapsedTime = 0;
        
        while (fElapsedTime < fDuration)
        {
            float fNewValue = Mathf.Lerp(fStartValue, 0, (fElapsedTime / fDuration));
            animator.SetFloat("Blend", fNewValue);

            Vector3 vNewPos = Vector3.Lerp(vStartTargetPosition, vFromTragetPosition, (fElapsedTime / fDuration));
            Arm_1_Target.transform.localPosition = vNewPos;

            fElapsedTime += Time.deltaTime;

            yield return null;
        }

        animator.SetFloat("Blend", 0);
        currentStopCoroutine = null;
        bIsStopping = false;
        fMovingTime = 0f;
    }

    public void GoToPosition(Vector3 vPos, float fDuration = 0.5f, bool IsActiveAfterMoving = false)
    {
        StartCoroutine(coroutine_GoToPosition(vPos, fDuration, IsActiveAfterMoving));
    }

    public IEnumerator coroutine_GoToPosition(Vector3 vPos, float fDuration, bool IsActiveAfterMoving)
    {
        bIsGoingToPosition = true;

        this.SetActive(false);
        float fElapsedTime = 0;
        animator.SetFloat("Blend", 0.5f);

        Vector3 vCurrent = transform.position;
        float fDirection = (vPos - vCurrent).x;

        if(fDirection < 0 && currentDirection == BodyDirection.Gauche)//Il faut regarder vers la droite
        {
            ChangeDirection(BodyDirection.Droite);
        }
        else if(currentDirection == BodyDirection.Droite && fDirection > 0)//Il faut regarder vers la gauche
        {
            ChangeDirection(BodyDirection.Gauche);
        }
        
        vCurrent = transform.position;//On reprend la position car elle a pu changer en cas de changement de direction

        if (currentDirection == BodyDirection.Droite)
        {
            vPos = new Vector3(vPos.x - 3f, vPos.y, vPos.z);
        }

        while (fElapsedTime < fDuration)
        {
            Vector3 vNewPos = Vector3.Lerp(vCurrent, vPos, (fElapsedTime / fDuration));
            transform.position = vNewPos;

            fElapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = vPos;

        StartCoroutine(coroutine_StopMoving());

        while (bIsStopping)
            yield return null;

        this.SetActive(IsActiveAfterMoving);

        bIsGoingToPosition = false;
    }

    public bool IsGoingToPosition()
    {
        return bIsGoingToPosition;
    }

    public void Speak(List<Emote> lstEmotes)
    {
        speakingCtrl.Speak(lstEmotes, fEmoteXSpawn, (persoType == PersoType.Perso ? fEmoteYSpawn : fEmoteYSpawn_Maxine), currentDirection);
    }

    public void SpeakRandom(int nbEmotes)
    {
        speakingCtrl.SpeakRandom(nbEmotes, fEmoteXSpawn, (persoType == PersoType.Perso ? fEmoteYSpawn : fEmoteYSpawn_Maxine), currentDirection);
    }

    public void StopSpeaking()
    {

        speakingCtrl.StopSpeaking();
    }

    public bool IsSpeaking()
    {
        return speakingCtrl.IsSpeaking();
    }

    public Vector3 GetRootPosition()
    {
        Transform tRoot = transform.Find("root");

        return tRoot.position;
    }

    public void ExtendArms()
    {
        Arm_1_Target.transform.position = new Vector3(Arm_1_Target.transform.position.x + 5f, Arm_1_Target.transform.position.y + 2f, Arm_1_Target.transform.position.z);
    }

    public void UnextendedArms()
    {
        Arm_1_Target.transform.localPosition = vStartMovingTargetPosition;
    }

    public float GetMovingTime()
    {
        return fMovingTime;
    }
}
