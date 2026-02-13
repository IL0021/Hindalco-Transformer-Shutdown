using System.Collections.Generic;


[System.Serializable]
public class GameStep
{
    public string stepName; 
    // public UnityEvent OnStepStarted;    
    public List<GameInteraction> interactions = new List<GameInteraction>(); 
    public CompletionCondition completionCondition;

}