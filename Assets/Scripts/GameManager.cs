using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        if (moduleMode == ModuleMode.Assessment) assessmentManager = GetComponent<AssessmentManager>();
    }

    public AssessmentManager assessmentManager;
    [Header("Configuration")]
    [SerializeField] public List<GameStep> steps;

    [SerializeField] private int _currentStepIndex = 0;
    [SerializeField] private int _currentInteractionIndex = 0;
    private BaseInteraction _activeInteraction;
    [Header("UI References")]
    public Canvas mainCanvas;
    public GameObject welcomePanel, procedurePanel, finishedPanel;

    public Button startProcedureButton, NextButton, HomeButton, Home;
    public Transform stepIncomplete, movementButtons;
    public Action OnNextStepCalled;
    public GameStep currentStep;

    // Cached UI Components
    // private TextMeshProUGUI stepNumText, stepNameText, stepDescriptionText;

    public ModuleMode moduleMode;
    public bool teleportationEnabled = false;

    private IEnumerator Start()
    {
        // Wait a frame to ensure MainCanvas is initialized
        yield return null;

        InitializeReferences();
        InitializeListeners();

        _currentStepIndex = 0;
        _currentInteractionIndex = 0;

        if (steps.Count > 0)
            currentStep = steps[_currentStepIndex];
    }


    private void InitializeReferences()
    {
        if (MainCanvas.Instance != null)
            mainCanvas = MainCanvas.Instance.GetComponentInChildren<Canvas>();
        // else 
        //     mainCanvas = FindObjectOfType<MainCanvas>().GetComponent<Canvas>();

        // Buttons
        NextButton = mainCanvas.transform.GetChildWithName("NextButton").GetComponent<Button>();
        HomeButton = mainCanvas.transform.GetChildWithName("HomeButton").GetComponent<Button>();
        Home = mainCanvas.transform.GetChildWithName("Home").GetComponent<Button>();
        startProcedureButton = mainCanvas.transform.GetChildWithName("StartProcedureButton").GetComponent<Button>();

        if (moduleMode == ModuleMode.Training)
            stepIncomplete = mainCanvas.transform.GetChildWithName("StepIncomplete");

        // // Text
        // stepNumText = mainCanvas.transform.GetChildWithName("StepNum").GetComponent<TextMeshProUGUI>();
        // stepNameText = mainCanvas.transform.GetChildWithName("StepName").GetComponent<TextMeshProUGUI>();
        // stepDescriptionText = mainCanvas.transform.GetChildWithName("StepDesc").GetComponent<TextMeshProUGUI>();
    }

    private void InitializeListeners()
    {
        NextButton.onClick.AddListener(OnNextButtonPressed);
        HomeButton.onClick.AddListener(LoadMainMenu);
        Home.onClick.AddListener(LoadMainMenu);
        startProcedureButton.onClick.AddListener(StartProcedure);
    }

    private void LoadMainMenu() => SceneManager.LoadScene("MainMenu");

    private void StartProcedure()
    {
        welcomePanel.SetActive(false);
        procedurePanel.SetActive(true);
        OnNextStepCalled?.Invoke();
        PlayNextInteraction();
    }

    private void OnNextButtonPressed()
    {
        if (currentStep != null && currentStep.completionCondition != null && !currentStep.completionCondition.IsCompleted())
        {
            Debug.Log("Current step not yet completed.");
            if (moduleMode == ModuleMode.Training)
                StartCoroutine(ShowNotCompletedMessage());
            return;
        }

        // todo: disable xr interaction with the objects in current step
        if (currentStep != null)
        {
            foreach (var interactionData in currentStep.interactions)
            {
                if (interactionData.interaction != null)
                {
                    var interactables = interactionData.interaction.GetComponentsInChildren<XRBaseInteractable>(true);
                    foreach (var interactable in interactables)
                    {
                        interactable.enabled = false;
                    }
                }
            }
        }

        PlayNextInteraction();
        NextButton.gameObject.SetActive(false);
    }

    private IEnumerator ShowNotCompletedMessage()
    {
        stepIncomplete.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        stepIncomplete.gameObject.SetActive(false);
    }

    public void PlayNextInteraction()
    {
        if (_currentStepIndex >= steps.Count)
        {
            FinishProcedure();
            return;
        }

        currentStep = steps[_currentStepIndex];

        if (_currentInteractionIndex >= currentStep.interactions.Count)
        {
            PrepareNextStep();
            return;
        }

        // UpdateUI();
        ProcessInteraction();
    }

    private void PrepareNextStep()
    {
        Debug.Log("Last interaction in step");
        NextButton.gameObject.SetActive(true);

        _currentInteractionIndex = 0;
        _currentStepIndex++;

        OnNextStepCalled?.Invoke();
    }

    private void FinishProcedure()
    {
        Debug.Log("All Game Steps Completed.");
        procedurePanel.SetActive(false);
        finishedPanel.SetActive(true);
    }

    private void ProcessInteraction()
    {
        var interactionData = currentStep.interactions[_currentInteractionIndex];
        BaseInteraction nextInteraction = interactionData.interaction;

        if (nextInteraction == null)
        {
            Debug.LogWarning($"Skipping empty interaction at Step {_currentStepIndex}, Index {_currentInteractionIndex}");
            HandleInteractionComplete(null);
            return;
        }

        // Re-enable XR interactions for the current step's object
        if (nextInteraction != null)
        {
            var interactables = nextInteraction.GetComponentsInChildren<XRBaseInteractable>(true);
            foreach (var interactable in interactables)
            {
                interactable.enabled = true;
            }
        }

        ConfigureInteraction(nextInteraction, interactionData);

        if (_activeInteraction != null)
            _activeInteraction.OnComplete -= HandleInteractionComplete;

        _activeInteraction = nextInteraction;
        _activeInteraction.OnComplete += HandleInteractionComplete;

        Debug.Log($"Playing Step {_currentStepIndex} | Interaction {_currentInteractionIndex}");

        UpdateUI(interactionData, _currentStepIndex, _currentInteractionIndex);
    }
    List<string> locations = new List<string> { "Switch House #1", "Switch House #2", "Control Room #1", "Switch Yard" };
    private void UpdateUI(GameInteraction interactionData, int stepIndex, int interactionIndex)
    {
        if (interactionData.canvasTransform != null)
        {
            MainCanvas.Instance.transform.SetPositionAndRotation(interactionData.canvasTransform.position, interactionData.canvasTransform.rotation);
        }



        if (moduleMode == ModuleMode.Training)
            UpdateTrainingUI(interactionData, stepIndex, interactionIndex);
        else
            UpdateAssessmentUI(interactionData, stepIndex, interactionIndex);
    }

    private void UpdateTrainingUI(GameInteraction interactionData, int stepIndex, int interactionIndex)
    {
        if (interactionData.interaction is TeleportInteraction teleportInteraction)
        {
            // string text;
            // if (teleportInteraction.teleportType == TeleportType.Location)
            // {
            //     text = (stepIndex == 0 && interactionIndex == 0)
            //         ? $"First, you have to go to <color=blue>{interactionData.target}</color>."
            //         : $"For the next step, you have to go to <color=blue>{interactionData.target}</color>.";
            // }
            // else
            // {
            //     text = $"Now you have to go to <color=blue>{interactionData.target}</color>.";
            // }

            MainCanvas.Instance.ShowSingleButtonPanel(interactionData.interactionName, interactionData.interactionDescription, $"Go to {interactionData.target}", () =>
            {
                StartCoroutine(_activeInteraction.Process());
                MainCanvas.Instance.option1.gameObject.SetActive(false);
            });
        }
        else if (interactionData.interaction is PPEPickInteraction ppe)
        {
            string buttonText = "Pick all required items to proceed";
            "controll is here".Print();
            MainCanvas.Instance.ShowSingleButtonPanel(interactionData.interactionName, interactionData.interactionDescription, buttonText, () =>
            {
                StartCoroutine(_activeInteraction.Process());
                MainCanvas.Instance.option1.gameObject.SetActive(false);

                teleportationEnabled = true;
                // ToggleCanvasAnimation();
            });
        }
        else if (interactionData.interaction is AcknowledgeInteraction ack)
        {
            StartCoroutine(_activeInteraction.Process());
        }
        else if (interactionData.interaction is EmptyInteraction empty)
        {
            StartCoroutine(_activeInteraction.Process());
        }
        else if (interactionData.interaction is MapInteraction map)
        {
            string buttonText = "Open Map";
            if (map.buttonDict[interactionData.target] == null) buttonText = $"Go to {interactionData.target}";
            MainCanvas.Instance.ShowSingleButtonPanel(interactionData.interactionName, interactionData.interactionDescription, buttonText, () =>
            {
                StartCoroutine(_activeInteraction.Process());
                MainCanvas.Instance.option1.gameObject.SetActive(false);
                // ToggleCanvasAnimation();
            });
        }
        else
        {
            // string text = $"You are at <color=blue>{interactionData.target}</color>. {interactionData.interactionDescription}";
            MainCanvas.Instance.ShowSingleButtonPanel(interactionData.interactionName, interactionData.interactionDescription, "Proceed to Interaction", () =>
            {
                StartCoroutine(_activeInteraction.Process());
                MainCanvas.Instance.option1.gameObject.SetActive(false);
                // ToggleCanvasAnimation();
            });
        }
    }

    public Transform arrowTarget;
    [ContextMenu("Check Target")]
    public void CheckTarget()
    {
        // PathDirector.instance.SetTarget(arrowTarget);
    }
    private void UpdateAssessmentUI(GameInteraction interactionData, int stepIndex, int interactionIndex)
    {
        string question;
        List<string> options = new List<string>();
        string correctAnswer = interactionData.target;

        if (interactionData.interaction is TeleportInteraction teleportInteraction)
        {
            if (teleportInteraction.teleportType == TeleportType.Location)
            {
                question = (stepIndex == 0 && interactionIndex == 0)
                    ? "For the first step, where do you want to go?"
                    : "For the next step, where do you want to go?";

                if (locations.Contains(interactionData.target))
                    options = new List<string>(locations);
            }
            else
            {
                question = "Where do you want to continue the procedure?";
                options = new List<string> { interactionData.target, GenerateBusString(), GenerateBusString(), GenerateBusString() };
            }
        }
        else
        {
            question = $"You are at <color=blue>{interactionData.target}</color>.\nWhat action do you want to perform?";

            if (interactionData.interaction is BreakerInteraction || interactionData.interaction is RackInInteraction || interactionData.interaction is RackOutInteraction)
            {
                options = new List<string> { "Perform Rack In", "Perform Rack Out", "Set breaker to Trip position", "Set breaker to Close position" };
                if (interactionData.interaction is RackInInteraction) correctAnswer = "Perform Rack In";
                else if (interactionData.interaction is RackOutInteraction) correctAnswer = "Perform Rack Out";
            }
            else if (interactionData.interaction is PPEPickInteraction)
            {
                options = new List<string> { "Equip PPE items", "Reject PPE items" };
            }
            else if (interactionData.interaction is AcknowledgeInteraction)
            {
                if (interactionData.target.Contains("Alumina"))
                    options = new List<string> { "Alumina Feeder #1", "Alumina Feeder #2", "Alumina Feeder #3", "Alumina Feeder #4" };
                else if (interactionData.target.Contains("Check"))
                    options = new List<string> { interactionData.target, "Perform Rack In", "Set breaker to Trip position", "Set breaker to Close position" };
            }
        }

        if (options.Count > 0)
        {
            var randomizedOptions = options.Randomize();
            MainCanvas.Instance.ShowFourOptionPanel("", question, randomizedOptions, (selectedIndex) =>
            {
                if (randomizedOptions[selectedIndex] == correctAnswer)
                {
                    "Correct option selected".Print();
                    MainCanvas.Instance.ResetPanel();
                    StartCoroutine(_activeInteraction.Process());
                }
                else
                {
                    "Incorrect option selected".Print();
                }
            });
        }
    }
    private static readonly System.Random _random = new System.Random();

    public static string GenerateBusString()
    {
        // Define the variables
        string[] types = { "Tie", "Coupler" };
        string[] letters = { "A", "B", "C", "D" };

        // Pick random elements
        string typeChoice = types[_random.Next(types.Length)];
        string firstLetter = letters[_random.Next(letters.Length)];
        string secondLetter = letters[_random.Next(letters.Length)];

        // Format: BUS [Tie/Coupler] of T31 [A/B/C/D] to [A/B/C/D]
        return $"BUS {typeChoice} of T31 {firstLetter} to {secondLetter}";
    }

    [ContextMenu("Debugger")]
    public void Debugger()
    {
        foreach (var step in steps)
        {
            foreach (var interaction in step.interactions)
            {
                if (interaction.interaction is TeleportInteraction teleportInteraction)
                {
                    $"{interaction.target} : ".Print();
                }
                // if (interaction.target == "")
                // {
                //     $"{step.stepName} : {interaction.interactionName}".Print();
                // }
            }
        }
    }

    private void ConfigureInteraction(BaseInteraction interaction, GameInteraction data)
    {
        if (interaction is BreakerInteraction breaker)
        {
            breaker.targetValue = data.targetValue;
            breaker.UpdateHighlights();
        }
        else if (interaction is AcknowledgeInteraction ack)
        {
            ack.skipButton = _currentInteractionIndex == currentStep.interactions.Count - 1;
        }
        else if (interaction is MapInteraction map)
        {
            map.locationTarget = data.target;
        }
    }

    bool isCanvasInfront = true;
    [ContextMenu("Test Canvas Animation")]
    public void ToggleCanvasAnimation()
    {
        if (isCanvasInfront)
        {
            MainCanvas.Instance.transform.GetChild(0).DOLocalMove(new Vector3(-400f, 0, -200f), 0.5f).SetEase(Ease.InOutSine);
            MainCanvas.Instance.transform.GetChild(0).DOLocalRotate(new Vector3(0, -45, 0), 0.5f).SetEase(Ease.InOutSine);
        }
        else
        {
            MainCanvas.Instance.transform.GetChild(0).DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.InOutSine);
            MainCanvas.Instance.transform.GetChild(0).DOLocalRotate(Vector3.zero, 0.5f).SetEase(Ease.InOutSine);
        }
        isCanvasInfront = !isCanvasInfront;
    }

    private void HandleInteractionComplete(BaseInteraction interaction)
    {
        if (interaction != null)
        {
            interaction.OnComplete -= HandleInteractionComplete;
            if (interaction is BreakerInteraction breaker)
            {
                breaker.ClearHighlights();
            }
            else if (isCanvasInfront == false) ToggleCanvasAnimation();
        }


        _currentInteractionIndex++;
        PlayNextInteraction();
    }

    // public void UpdateUI()
    // {
    //     if (stepNumText == null || stepNameText == null || stepDescriptionText == null)
    //         return;

    //     currentStep = steps[_currentStepIndex];
    //     GameInteraction currentInteraction = currentStep.interactions[_currentInteractionIndex];

    //     stepNumText.text = currentStep.stepName;

    //     if (!string.IsNullOrEmpty(currentInteraction.interactionName))
    //         stepNameText.text = currentInteraction.interactionName;

    //     stepDescriptionText.text = currentInteraction.interactionDescription;


    // }
}