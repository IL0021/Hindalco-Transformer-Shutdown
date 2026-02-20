using System.Collections;

public class EmptyInteraction : BaseInteraction
{

    public override IEnumerator Process()
    {
        FinishInteraction();
        yield break;
    }
}
