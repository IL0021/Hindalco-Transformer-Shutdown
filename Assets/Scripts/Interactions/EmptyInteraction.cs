using System.Collections;
using UnityEngine;

public class EmptyInteraction : BaseInteraction
{
    public float delayTime = 3f;
    public override IEnumerator Process()
    {
        yield return new WaitForSeconds(delayTime);
        FinishInteraction();
        yield break;
    }
}
