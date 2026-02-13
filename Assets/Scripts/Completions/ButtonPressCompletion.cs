public class ButtonPressCompletion: CompletionCondition
{
    public override bool IsCompleted()
    {
        foreach (var gameInteraction in GameManager.Instance.currentStep.interactions)
        {
            if (gameInteraction.interaction is ButtonPressInteraction buttonPressInteraction)
            {
                if (buttonPressInteraction.buttonPressed)
                {
                    return true;
                }
            }
        }
        return false;
    }
}