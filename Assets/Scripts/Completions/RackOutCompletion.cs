
public class RackOutCompletion : CompletionCondition
{
    public override bool IsCompleted()
    {
        foreach (var gameInteraction in GameManager.Instance.currentStep.interactions)
        {
            if (gameInteraction.interaction is RackOutInteraction rackOutInteraction)
            {
                if(rackOutInteraction.turns <= -25)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
