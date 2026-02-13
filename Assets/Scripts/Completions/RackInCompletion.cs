
public class RackInCompletion : CompletionCondition
{
    public override bool IsCompleted()
    {
        foreach (var gameInteraction in GameManager.Instance.currentStep.interactions)
        {
            if (gameInteraction.interaction is RackInInteraction rackInInteraction)
            {
                if(rackInInteraction.turns >= 25)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
