using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObjectEventArg : EventArgs
{
    public int nIndex;

    public MovableObjectEventArg(int nIndex)
    {
        this.nIndex = nIndex;
    }
}

public class MovableObject : MonoBehaviour
{
    public delegate void MovableObjectSelectedEventHandler(object sender, MovableObjectEventArg e);
    public event MovableObjectSelectedEventHandler MovableObjectSelectedEvent;

    public delegate void MovableObjectReleasedEventHandler(object sender, MovableObjectEventArg e);
    public event MovableObjectReleasedEventHandler MovableObjectReleasedEvent;

    public delegate void MovableObjectShakedEventHandler(object sender, MovableObjectEventArg e);
    public event MovableObjectShakedEventHandler MovableObjectShakedEvent;

    public static int NEXT_INDEX = 0;

    //Interactable
    [SerializeField] private Sprite Sprite_1;
    [SerializeField] private Sprite Sprite_2;
    private float fTimePerSprite = 0.25f;
    private float fTimeCount = 0;
    private Sprite currentSprite;
    [SerializeField] private GameObject goHighlight;
    private Collider2D objectCollider;

    //Movable
    private SpriteRenderer sprtRenderer;
    private Rigidbody2D rgbd2D;
    [SerializeField] private float FollowSpeed = 8f;
    [SerializeField] private float GravityScale = 5f;

    private List<Vector3> lstMousePositions;

    private bool bIsMoving = false;
    private bool bGravityEnable = false;

    //Retourne l'objet automatiquement
    [SerializeField] private float ResetPositionX;
    private Coroutine coroutine_ResetPosition;

    //Shake
    private int nMoveCount = 0;
    private int bDirection = 1;
    private Vector3 vLastMousePosition;
    private float fTimeWithoutChanged = 0f;

    //
    private bool bMouseDetection = true;

    private int nIndex;

    void Start()
    {
        objectCollider = GetComponent<PolygonCollider2D>();
        sprtRenderer = GetComponent<SpriteRenderer>();
        rgbd2D = GetComponent<Rigidbody2D>();

        lstMousePositions = new List<Vector3>();
        rgbd2D.gravityScale = 0f;
    }

    void Update()
    {
        //Change de sprite toutes les X secondes
        if (fTimeCount > fTimePerSprite)
        {
            if (currentSprite == Sprite_1)
            {
                currentSprite = Sprite_2;
            }
            else
            {
                currentSprite = Sprite_1;
            }

            sprtRenderer.sprite = currentSprite;
            fTimeCount = 0;
        }

        fTimeCount += Time.deltaTime;

        if (bIsMoving)
        {
            UpdatePosition();

            lstMousePositions.Add(GetMousePosition());

            if (lstMousePositions.Count >= 20)
            {
                lstMousePositions.RemoveAt(0);
            }
        }

        if (bIsMoving)
        {
            Vector3 vCurrentPosition = GetMousePosition();

            float xDifference = vCurrentPosition.x - vLastMousePosition.x;

            if (xDifference != 0)//Si on a bougé la souris
            {
                fTimeWithoutChanged += 0;

                int nCurrentDirection = 0;

                //On regarde si la souris est déplacé vers la droite ou la gauche (différence de la position en X)
                if (xDifference > 0)
                    nCurrentDirection = 1;
                else if (xDifference <= 0)
                    nCurrentDirection = -1;

                //Si on a changé de direction
                if (nCurrentDirection != bDirection)
                {
                    nMoveCount++;
                    fTimeWithoutChanged = 0;
                }

                bDirection = nCurrentDirection;
            }

            fTimeWithoutChanged += Time.deltaTime;

            //Si on ne change pas de direction pendant 1 seconde, on reset le Shake
            if (fTimeWithoutChanged > 1f)
            {
                fTimeWithoutChanged = 0;
                nMoveCount = 0;
            }

            //Si on change 5 fois de direction avec que le timer ne soit à 1 seconde, on prend en compte le Shake
            if (nMoveCount == 7)
            {
                //shake OK
                fTimeWithoutChanged = 0;
                nMoveCount = 0;

                MovableObjectShakedEvent?.Invoke(this, new MovableObjectEventArg(nIndex));
            }

            vLastMousePosition = vCurrentPosition;
        }
        else
        {
            fTimeWithoutChanged = 0;
            nMoveCount = 0;
        }
    }

    public void InitMovableObject()
    {
        this.nIndex = NEXT_INDEX++;
    }

    private void OnMouseEnter()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider == null) return;

        Collider2D k = GetComponent<Collider2D>();
        if (hit.collider != k) return;

        goHighlight.SetActive(true);
    }

    private void OnMouseExit()
    {
        if (bIsMoving) return;

        goHighlight.SetActive(false);
    }

    void OnMouseDown()
    {
        if (!bMouseDetection) return;

        MovableObjectSelectedEvent?.Invoke(this, new MovableObjectEventArg(nIndex));

        if (coroutine_ResetPosition != null)
        {
            StopCoroutine(coroutine_ResetPosition);
            coroutine_ResetPosition = null;
        }

        if (!bGravityEnable)
        {
            objectCollider.isTrigger = false;
            bGravityEnable = true;
            rgbd2D.gravityScale = GravityScale;
        }

        bIsMoving = true;
        rgbd2D.bodyType = RigidbodyType2D.Kinematic;
        rgbd2D.velocity = Vector2.zero;

        lstMousePositions = new List<Vector3>();
    }

    private void OnMouseUp()
    {
        if (!bMouseDetection) return;

        MovableObjectReleasedEvent?.Invoke(this, new MovableObjectEventArg(nIndex));

        bIsMoving = false;
        rgbd2D.bodyType = RigidbodyType2D.Dynamic;

        if (lstMousePositions.Count > 0)
        {
            Vector3 vDifference = lstMousePositions[lstMousePositions.Count - 1] - lstMousePositions[0];
            rgbd2D.AddForce(vDifference * 10, ForceMode2D.Impulse);
        }

        lstMousePositions = new List<Vector3>();

        OnMouseExit();
    }

    public int GetObjectIndex()
    {
        return nIndex;
    }

    private Vector3 UpdatePosition()
    {
        Vector3 vRes = GetMousePosition();

        Vector3 vResLerp = Vector3.Lerp(transform.position, vRes, FollowSpeed * Time.deltaTime);

        transform.position = vResLerp;

        return vRes;
    }
    private Vector3 GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 rayPoint = ray.GetPoint(1);
        Vector3 vRes = new Vector3(rayPoint.x, rayPoint.y, transform.position.z);

        return vRes;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!bGravityEnable)
        {
            bGravityEnable = true;
            rgbd2D.gravityScale = GravityScale;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        
    }

    public void Launch(float fForce)
    {
        OnMouseUp();
        bMouseDetection = false;

        rgbd2D.AddForce(new Vector2(1, 1) * fForce, ForceMode2D.Impulse);
    }

    public void ChangeOpacity(float nNewOpacity, float fDuration = 0.5f)
    {
        StartCoroutine(coroutine_ChangeOpacity(nNewOpacity, fDuration));
    }

    private IEnumerator coroutine_SmoothResetPosition(float fResetRotationDuration = 0.5f, float fResetPositionDuration = 0.5f)
    {
        Vector3 vCurrentRotation = gameObject.transform.eulerAngles;
        float fElapsedTime = 0;

        while (fElapsedTime < fResetRotationDuration)
        {
            Vector3 vNewRotation = Vector3.Lerp(vCurrentRotation, Vector3.zero, (fElapsedTime / fResetRotationDuration));
            gameObject.transform.eulerAngles = vNewRotation;

            fElapsedTime += Time.deltaTime;

            yield return null;
        }

        gameObject.transform.eulerAngles = Vector3.zero;

        Vector3 vCurrentPosition = gameObject.transform.position;
        Vector3 vDestination = new Vector3(ResetPositionX, vCurrentPosition.y, vCurrentPosition.z);
        fElapsedTime = 0;

        yield return new WaitForSeconds(0.5f);

        while(fElapsedTime < fResetPositionDuration)
        {
            Vector3 vNewPosition = Vector3.Lerp(vCurrentPosition, vDestination, (fElapsedTime / fResetPositionDuration));
            gameObject.transform.position = vNewPosition;

            fElapsedTime += Time.deltaTime;

            yield return null;
        }

        gameObject.transform.position = vDestination;

        coroutine_ResetPosition = null;
    }

    private IEnumerator coroutine_ChangeOpacity(float nNewOpacity, float fDuration)
    {
        Color c = sprtRenderer.color;
        float fCurrentOpacity = c.a;
        float fElapsedTime = 0;

        while (fElapsedTime < fDuration)
        {
            float fOpacity = Mathf.Lerp(fCurrentOpacity, nNewOpacity, (fElapsedTime / fDuration));
            sprtRenderer.color = new Color(c.r, c.g, c.b, fOpacity);

            fElapsedTime += Time.deltaTime;

            yield return null;
        }
    }
}
