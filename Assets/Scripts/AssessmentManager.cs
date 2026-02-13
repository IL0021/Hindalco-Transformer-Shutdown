using UnityEngine;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
public class AssessmentManager : MonoBehaviour
{
    // [SerializeReference] 
    public List<AssessmentStep> allSteps = new List<AssessmentStep>();
    public SerializedDictionary<string, QuestionSubStep> questionDict = new SerializedDictionary<string, QuestionSubStep>();
    public SerializedDictionary<string, InteractionSubStep> interactionDict = new SerializedDictionary<string, InteractionSubStep>();

    [ContextMenu("Export Json")]
    public void ExportJson()
    {
        JsonUtility.ToJson(questionDict).Print("questionDict.json ");
        JsonUtility.ToJson(interactionDict).Print("interactionDict.json ");
    }

    // [ContextMenu("Load Assessment Steps")]
    // public void LoadAssessmentSteps()
    // {
    //     int stepIndex = 0;
    //     foreach (var step in GameManager.Instance.steps)
    //     {
    //         AssessmentStep assessmentStep = new AssessmentStep();
    //         assessmentStep.stepName = step.stepName;

    //         int substepIndex = 0;
    //         foreach (var substep in step.interactions)
    //         {
    //             string substepName = stepIndex + "_" + substepIndex;

    //             if (substep.interaction is TeleportInteraction teleportInteraction)
    //             {
    //                 QuestionSubStep questionSubStep = new QuestionSubStep();
    //                 questionSubStep.subStepName = substep.interactionName;
    //                 substepName = "QUES_" + substepName;
    //                 questionDict.Add(substepName, questionSubStep);
    //             }
    //             else
    //             {
    //                 InteractionSubStep interactionSubStep = new InteractionSubStep();
    //                 interactionSubStep.subStepName = substep.interactionName;
    //                 substepName = "INTER_" + substepName;
    //                 interactionDict.Add(substepName, interactionSubStep);
    //             }

    //             assessmentStep.substeps.Add(substepName);
    //             substepIndex++;
    //         }

    //         allSteps.Add(assessmentStep);
    //         stepIndex++;
    //     }
    // }

    // [ContextMenu("get type")]

    // public void Iterate()
    // {
    //     foreach (var step in allSteps)
    //     {
    //         Debug.Log(step.stepName);
    //         foreach (var substep in step.substeps)
    //         {
    //             // if (substep is QuestionSubStep questionSubStep)
    //             // {
    //             //     Debug.Log("Question:" + questionSubStep.subStepName);
    //             // }
    //             // else
    //             // {
    //             //     Debug.Log("Interaction:" + substep.subStepName);
    //             // }
    //         }
    //     }
    // }
}

