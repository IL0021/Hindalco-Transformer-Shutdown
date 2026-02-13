
public class BreakerCompletion : CompletionCondition
{
    public override bool IsCompleted()
    {
        foreach (var gameInteraction in GameManager.Instance.currentStep.interactions)
        {
            if (gameInteraction.interaction is BreakerInteraction breakerInteraction)
            {
                if (breakerInteraction.lever != null && breakerInteraction.gameObject.activeInHierarchy)
                {
                    if (breakerInteraction.lever.value == breakerInteraction.targetValue)
                    {
                        return false;
                    }

                }
            }
        }
        GameManager.Instance.ToggleCanvasAnimation();
        return true;
    }
}