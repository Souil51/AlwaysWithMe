using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadePanelController : MonoBehaviour
{
    [SerializeField] private CommonController controller;

    public void AnimationFinished()
    {
        controller.SetFadeEnded();
    }
}
