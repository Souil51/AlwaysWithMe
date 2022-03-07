using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicBodyController : MonoBehaviour
{
    [SerializeField] private GameObject rootBone;
    [SerializeField] private List<GameObject> legs;
    [SerializeField] private bool CalculerDifferenceY;
    [SerializeField] private float GetUpVelocityPrecision = 0f;
    [SerializeField] private float GetUpAngularVelocityPrecision = 0f;

    public bool RagdollActive { get; private set; }

    public bool IsInit { get { return bIsInit; } private set { bIsInit = value; } }
    private bool bIsInit = false;

    private HingeJoint2D[] joints;//Joints du ragdoll
    private Rigidbody2D[] rbs;//Tous les rigidbody de l'objet
    private CapsuleCollider2D[] capsuleColliders;//Tous les CapsuleCollider de l'objet
    private BoxCollider2D boxCollider;//Collider utilisé quand le corps est dans sa position "UP"
    private CapsuleCollider2D capsuleCollider;//Collider du corps quand il est en mode dynamic
    private Rigidbody2D rootRigidBody;//Rigidbody de la racine du rig

    private Dictionary<Rigidbody2D, Vector3> initialPos = new Dictionary<Rigidbody2D, Vector3>();//Position initiale des rigidbody
    private Dictionary<Rigidbody2D, Quaternion> initialRot = new Dictionary<Rigidbody2D, Quaternion>();//Rotation initiale des rigidbody

    private bool IsDynamicModeEnabled = false;
    private bool IsGettingUp = false;//En train de se lever ?
    private bool IsUp = true;//Est levé ?

    private bool bFirstFrame = false;//Première frame ? (utilisé pour ne pas considéré qu'il faille se lever dès l'activation du mode dynamic

    private void Start()
    {
        IsInit = false;
    }

    private void Update()
    {
        if (!bFirstFrame && IsDynamicModeEnabled && !IsGettingUp && rootRigidBody.position != (Vector2)initialPos[rootRigidBody] 
            && (rootRigidBody.velocity.x > -GetUpVelocityPrecision && rootRigidBody.velocity.x < GetUpVelocityPrecision)
            && (rootRigidBody.velocity.y > -GetUpVelocityPrecision && rootRigidBody.velocity.y < GetUpVelocityPrecision)
            && (rootRigidBody.angularVelocity > -GetUpAngularVelocityPrecision && rootRigidBody.angularVelocity < GetUpAngularVelocityPrecision))
        {
            GetUp();
        }

        if (bFirstFrame)
            bFirstFrame = false;
    }

    // Start is called before the first frame update
    void Awake()
    {
        joints = GetComponentsInChildren<HingeJoint2D>();
        rbs = GetComponentsInChildren<Rigidbody2D>();
        capsuleColliders = GetComponentsInChildren<CapsuleCollider2D>();
        boxCollider = rootBone.transform.GetComponent<BoxCollider2D>();

        if(rootBone.transform.Find("rootCollider") != null)
            capsuleCollider = rootBone.transform.Find("rootCollider").GetComponent<CapsuleCollider2D>();
        else
            capsuleCollider = rootBone.transform.GetComponent<CapsuleCollider2D>();

        rootRigidBody = rootBone.transform.GetComponent<Rigidbody2D>();

        rootRigidBody.bodyType = RigidbodyType2D.Static;
    }

    //Initialisation du mode dynamic
    public void InitBody()
    {
        rootRigidBody.bodyType = RigidbodyType2D.Dynamic;

        foreach (var collider in capsuleColliders)
        {
            collider.enabled = false;
        }

        foreach (var rb in rbs)
        {
            initialPos.Add(rb, rb.transform.position);
            initialRot.Add(rb, rb.transform.localRotation);
        }

        foreach (var joint in joints)
        {
            joint.enabled = false;
        }

        foreach (var leg in legs)
        {
            leg.SetActive(true);
        }

        SetIdle();
        RecordTransform();

        IsInit = true;
    }


    //Modifie le bodyType du RigidBody pour Dynamic
    public void SetDynamic()
    {
        this.rootRigidBody.bodyType = RigidbodyType2D.Dynamic;
    }

    //Enregistre les positions et localRotation des RigidBody
    void RecordTransform()
    {
        foreach (var rb in rbs)
        {
            initialPos[rb] = rb.transform.position;
            initialRot[rb] = rb.transform.localRotation;
        }
    }

    //Déplace toutes les cibles des pattes
    public void MoveAllTargets(float difference, float differenceX)
    {
        foreach (var leg in legs)
        {
            leg.transform.GetChild(0).position = new Vector3(leg.transform.GetChild(0).position.x, leg.transform.GetChild(0).position.y + difference, leg.transform.GetChild(0).position.z);
        }
    }

    #region Dynamic

    //Mode idle, l'araignee ne bouge pas
    public void SetIdle(bool bResetPosition = true)
    {
        if (bResetPosition)
        {
            foreach (var rb in rbs)
            {
                rb.transform.position = initialPos[rb];
                rb.transform.localRotation = initialRot[rb];
            }
        }

        boxCollider.enabled = true;
        rootRigidBody.bodyType = RigidbodyType2D.Dynamic;
        IsDynamicModeEnabled = false;
    }

    //CHange le bodyType pour Static, le corps arrête de bouger
    public void StopDynamicBody()
    {
        rootRigidBody.bodyType = RigidbodyType2D.Static;
    }

    public void GetUp()
    {
        StartCoroutine(coroutine_SmoothGetUp());
    }

    public bool IsBodyUp()
    {
        return IsUp;
    }

    public bool IsBodyGettingUp()
    {
        return IsGettingUp;
    }

    //Propulse le corps en mode Dynamic
    public void SetDynamicMode(Vector2 force)
    {
        bFirstFrame = true;

        if (IsDynamicModeEnabled) return;

        SetIdle();
        boxCollider.enabled = false;
        capsuleCollider.enabled = true;
        IsDynamicModeEnabled = true;

        RecordTransform();
        foreach (var rb in rbs)
        {
            rb.transform.position = initialPos[rb];
            rb.transform.localRotation = initialRot[rb];
        }

        rootRigidBody.AddForce(force, ForceMode2D.Force);
        IsUp = false;
    }

    //Le corps se relève tout seul : les pattes se remettent en place et le corps se lève
    IEnumerator coroutine_SmoothGetUp(float duration = 0.25f)
    {
        IsGettingUp = true;
        float elapsedTime = 0;

        Dictionary<Rigidbody2D, Vector3> dicCurrentPos = new Dictionary<Rigidbody2D, Vector3>();
        Dictionary<Rigidbody2D, Quaternion> dicCurrentRot = new Dictionary<Rigidbody2D, Quaternion>();

        Dictionary<Rigidbody2D, Vector3> dicNewPos = new Dictionary<Rigidbody2D, Vector3>();
        float xDifference = rootRigidBody.transform.position.x - initialPos[rootRigidBody].x;
        float yDifference = 0;

        if (CalculerDifferenceY)
        {
            yDifference = rootRigidBody.transform.position.y - initialPos[rootRigidBody].y;
            yDifference += initialPos[rootRigidBody].y - legs[0].transform.GetChild(0).position.y;
        }

        foreach (var rb in rbs)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;

            dicCurrentPos.Add(rb, rb.transform.position);
            dicCurrentRot.Add(rb, rb.transform.localRotation);

            dicNewPos.Add(rb, new Vector3(initialPos[rb].x + xDifference, initialPos[rb].y + yDifference, initialPos[rb].z));
        }

        Dictionary<GameObject, Vector3> dicPosInitialeIK = new Dictionary<GameObject, Vector3>();
        Dictionary<GameObject, Vector3> dicPosNewIK = new Dictionary<GameObject, Vector3>();
        foreach (var leg in legs)
        {
            dicPosInitialeIK.Add(leg, leg.transform.GetChild(0).position);
            dicPosNewIK.Add(leg, leg.transform.GetChild(0).position + new Vector3(xDifference, yDifference, 0));
        }

        if (duration != 0)
        {
            while (elapsedTime < duration)
            {
                foreach (var rb in rbs)
                {
                    rb.transform.position = Vector3.Lerp(dicCurrentPos[rb], dicNewPos[rb], (elapsedTime / duration));
                    rb.transform.localRotation = Quaternion.Lerp(dicCurrentRot[rb], initialRot[rb], (elapsedTime / duration));
                }

                foreach (var leg in legs)
                {
                    leg.transform.GetChild(0).position = Vector3.Lerp(dicPosInitialeIK[leg], dicPosNewIK[leg], (elapsedTime / duration));
                }

                elapsedTime += Time.deltaTime;

                yield return null;
            }
        }

        foreach (var rb in rbs)
        {
            rb.transform.position = dicNewPos[rb];
            rb.transform.localRotation = initialRot[rb];
        }

        foreach (var leg in legs)
        {
            leg.transform.GetChild(0).position = dicPosNewIK[leg];
            leg.SetActive(true);
        }

        //animator.enabled = true;
        SetIdle(false);
        IsGettingUp = false;
        RecordTransform();

        IsUp = true;

        yield return null;
    }

    #endregion

}
