using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BreakerManager : MonoBehaviour
{
    public Transform breakerParent;
    IEnumerator Start()
    {
        if (breakerParent == null)
        {
            yield break;
        }
        yield return new WaitForSeconds(0.3f);
        var breakerInteraction = GetComponent<BreakerInteraction>();
        breakerInteraction.OnTrip += SetZero;
        breakerInteraction.OnClose += SetOne;
    }

    public void SetZero()
    {
        breakerParent.DOLocalMoveX(-0.11f, 0.5f);
    }

    public void SetOne()
    {
        breakerParent.DOLocalMoveX(0f, 0.5f);
    }

}
