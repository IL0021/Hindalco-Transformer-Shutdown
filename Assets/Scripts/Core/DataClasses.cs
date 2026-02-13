using System;
using System.Collections.Generic;

[Serializable]
public class SubStep
{
    // public string subStepName;
    // public bool isCompleted;
}

[Serializable]
public class QuestionSubStep : SubStep
{
    public string questionText;
    public string[] options = new string[4];
    public int correctOptionIndex;
    public int selectedOptionIndex = -1;
}

[Serializable]
public class InteractionSubStep : SubStep
{
    public float timeTaken;
    public float targetTime;

}

[Serializable]
public class AssessmentStep
{
    public string stepName;
    // This list can now hold both QuestionSubSteps and InteractionSubSteps
    // [SerializeReference]
    public List<string> substeps = new List<string>();
}