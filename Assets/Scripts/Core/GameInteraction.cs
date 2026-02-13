using UnityEngine;

[System.Serializable]
public class GameInteraction 
{
    public string interactionName;
    public string interactionDescription;
    public AudioClip interactionAudio;
    public BaseInteraction interaction;
    public bool targetValue;
    public Transform canvasTransform;
    public string target;
}