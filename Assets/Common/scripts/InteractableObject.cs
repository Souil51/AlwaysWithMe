using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Objet dans la scene surlequel l'utilisateur peut cliquer pour "zoomer" ou effectuer une action
public class InteractableObject : MonoBehaviour
{
    public delegate void OnStartInteractionDelegate(object sender, EventArgs e);
    public event OnStartInteractionDelegate startInteractionDelegate;

    public delegate void OnStartTooFarInteractionDelegate(object sender, EventArgs e);
    public event OnStartTooFarInteractionDelegate startTooFarInteractionDelegate;

    //La taille que la caméra doit avoir quand on zoom sur l'objet
    [SerializeField] private float SizeForObject = 3.3f;
    [SerializeField] private bool ChangeSpriteOnZoom;

    [SerializeField] private Sprite Sprite_1;
    [SerializeField] private Sprite Sprite_2;

    [SerializeField] private bool ChangeOpacityOnChangeSprite = false;
    [SerializeField] private float Sprite_1_Opacity = 255f;
    [SerializeField] private float Sprite_2_Opacity = 255f;

    [SerializeField] private GameObject goSprite;
    [SerializeField] private GameObject goHighlight;
    [SerializeField] private GameObject goZoomedSprite;
    [SerializeField] private SpriteRenderer highlightSprtRenderer;

    [SerializeField] private InteractionType interactionType;
    [SerializeField] private ObjectType objectType;
    [SerializeField] private bool IsInnerObject = false;//Est-ce que l'objet est déjà dans un Interactable objet ?
    [SerializeField] private bool CanBeTooFar = false;

    //CHangement de sprite tous les fTimePerSprite secondes
    private SpriteRenderer sprtRenderer;
    private float fTimePerSprite = 0.25f;
    private float fTimeCount = 0;
    private Sprite currentSprite;
    private float fCurrentOpacity;

    public bool IsClicked { get; private set; } = false;
    private bool bIsZoomed = false;
    private bool bIsTooFar = false;//Permet de ne pas autoriser l'interaction selon le dernier appel de SetIsTooFar

    void Start()
    {
        sprtRenderer = goSprite.GetComponent<SpriteRenderer>();
        currentSprite = Sprite_1;
        fCurrentOpacity = -1;
    }

    void Update()
    {
        //CHange le sprite de l'objet pour le mettre en évidence -> interaction possible
        if (fTimeCount > fTimePerSprite && !bIsZoomed)
        {
            if (ChangeOpacityOnChangeSprite)
            {
                if (fCurrentOpacity < 0 || fCurrentOpacity == Sprite_1_Opacity)
                {
                    fCurrentOpacity = Sprite_2_Opacity;
                }
                else
                {
                    fCurrentOpacity = Sprite_1_Opacity;
                }

                sprtRenderer.color = new Color(1f, 1f, 1f, fCurrentOpacity / 255f);
            }

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
    }

    //Affiche le sprite "contour" autour de l'objet pour mettre en évidence qu'on peut cliquer desssus
    private void OnMouseEnter()
    {
        if (bIsZoomed) return;

        goHighlight.SetActive(true);
    }

    //Cache le sprite "contour"
    private void OnMouseExit()
    {
        if (bIsZoomed) return;

        goHighlight.SetActive(false);
    }

    //Interaction avec l'objet (click gauche)
    void OnMouseDown()
    {
        if (CanBeTooFar && bIsTooFar)
        {
            startTooFarInteractionDelegate?.Invoke(this, EventArgs.Empty);
            return;
        }

        if (IsClicked)
        {
            IsClicked = false;
        }
        else
        {
            IsClicked = true;
        }

        startInteractionDelegate?.Invoke(this, EventArgs.Empty);
    }

    private void OnMouseUp()
    {

    }

    public void DisableHilight()
    {
        OnMouseExit();
    }

    public float GetSizeCamera()
    {
        return SizeForObject;
    }

    public InteractionType GetInteractionType()
    {
        return interactionType;
    }

    public ObjectType GetObjectType()
    {
        return objectType;
    }

    public void EnterZoom()
    {
        bIsZoomed = true;

        goHighlight.SetActive(false);

        if (ChangeSpriteOnZoom)
        {
            StartCoroutine(coroutine_FadeInZoomedSprite());
        }
    }

    public void LeaveZoom()
    {
        bIsZoomed = false;

        if (ChangeSpriteOnZoom)
        {
            StartCoroutine(coroutine_FadeOutZoomedSprite());
        }
    }

    public void SetIsTooFar(bool bValue)
    {
        if (!CanBeTooFar) return;

        if(bValue && !bIsTooFar)
        {
            highlightSprtRenderer.color = new Color(1, 0, 0);
        }
        else if(!bValue && bIsTooFar)
        {
            highlightSprtRenderer.color = new Color(1, 1, 1);
        }

        bIsTooFar = bValue;
    }

    public bool IsInner()
    {
        return IsInnerObject;
    }

    public void ChangeSprite_1(Sprite sprt)
    {
        Sprite_1 = sprt;
    }

    public void ChangeSprite_2(Sprite sprt)
    {
        Sprite_2 = sprt;
    }

    private IEnumerator coroutine_FadeInZoomedSprite(float duration = 0.25f)
    {
        goZoomedSprite.SetActive(true);
        goZoomedSprite.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);

        float elapsedTime = 0;

        float fCurrentOpacity = 0;
        float fCurrentOpacitySprite = 1f;
        
        while (elapsedTime < duration)
        {
            fCurrentOpacity = Mathf.Lerp(0, 1, (elapsedTime / duration));
            fCurrentOpacitySprite = Mathf.Lerp(1, 0, (elapsedTime / duration));

            goZoomedSprite.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, fCurrentOpacity);
            goSprite.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, fCurrentOpacitySprite);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        goZoomedSprite.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        goSprite.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
    }

    private IEnumerator coroutine_FadeOutZoomedSprite(float duration = 0.25f)
    {
        float elapsedTime = 0;

        float fCurrentOpacity = 0;
        float fCurrentOpacitySprite = 1f;

        while (elapsedTime < duration)
        {
            fCurrentOpacity = Mathf.Lerp(1, 0, (elapsedTime / duration));
            fCurrentOpacitySprite = Mathf.Lerp(0, 1, (elapsedTime / duration));

            goZoomedSprite.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, fCurrentOpacity);
            goSprite.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, fCurrentOpacitySprite);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        goZoomedSprite.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        goSprite.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        goZoomedSprite.SetActive(false);
    }
}
