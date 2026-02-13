using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using DG.Tweening;

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
    public GameObject welcomPanel, procedurePanel, finishedPanel;

    public Button startProcedureButton, NextButton, HomeButton, Home;

    public Action OnNextStepCalled;
    public GameStep currentStep;

    // Cached UI Components
    // private TextMeshProUGUI stepNumText, stepNameText, stepDescriptionText;

    public ModuleMode moduleMode;

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
        welcomPanel.SetActive(false);
        procedurePanel.SetActive(true);
        OnNextStepCalled?.Invoke();
        PlayNextInteraction();
    }

    private void OnNextButtonPressed()
    {
        if (currentStep != null && currentStep.completionCondition != null && !currentStep.completionCondition.IsCompleted())
        {
            Debug.Log("Current step not yet completed.");
            return;
        }
        // ToggleCanvasAnimation();
        PlayNextInteraction();
        NextButton.gameObject.SetActive(false);
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
        {
            if (interactionData.interaction is TeleportInteraction teleportInteraction)
            {
                string teleportTypeText = "";
                if (teleportInteraction.teleportType == TeleportType.Location)
                {
                    if (stepIndex == 0 && interactionIndex == 0)
                        teleportTypeText = "First, you have to go to <color=blue>" + interactionData.target + "</color>.";
                    else
                        teleportTypeText = "For the next step, you have to go to <color=blue>" + interactionData.target + "</color>.";
                }
                else
                {
                    teleportTypeText = "Now you have to go to <color=blue>" + interactionData.target + "</color>.";
                }
                MainCanvas.Instance.ShowSingleButtonPanel(interactionData.interactionName, teleportTypeText, $"Go to {interactionData.target}", () =>
                {
                    StartCoroutine(_activeInteraction.Process());
                    MainCanvas.Instance.option1.gameObject.SetActive(false);
                });
            }
            else
            {
                string instructionText = "You are at " + interactionData.target + ". " + interactionData.interactionDescription;
                MainCanvas.Instance.ShowSingleButtonPanel(interactionData.interactionName, instructionText, "Proceed to Interaction", () =>
                {
                    StartCoroutine(_activeInteraction.Process());
                    MainCanvas.Instance.option1.gameObject.SetActive(false);

                    ToggleCanvasAnimation();
                });
            }
        }
        else
        {
            if (interactionData.interaction is TeleportInteraction teleportInteraction)
            {
                string teleportTypeText = "";
                if (teleportInteraction.teleportType == TeleportType.Location)
                {
                    if (stepIndex == 0 && interactionIndex == 0)
                        teleportTypeText = "For the first step, where do you want to go?";
                    else
                        teleportTypeText = "For the next step, where do you want to go?";

                    if (locations.Contains(interactionData.target))
                    {
                        MainCanvas.Instance.ShowFourOptionPanel(
                            "",
                            teleportTypeText,
                            locations.Randomize(),
                            (selectedIndex) =>
                            {
                                if (locations[selectedIndex] == interactionData.target)
                                {
                                    //Correct option selected
                                    "Correct option selected".Print();
                                    MainCanvas.Instance.ResetPanel();
                                    StartCoroutine(_activeInteraction.Process());
                                }
                                else
                                {
                                    "Incorrect option selected".Print();
                                    //Incorrect option selected
                                }
                            });
                    }
                }
                else
                {
                    teleportTypeText = "Which panel do you want to go?";
                    List<string> options = new List<string>
                        {
                            interactionData.target,
                            GenerateBusString(),
                            GenerateBusString(),
                            GenerateBusString()
                        };
                    MainCanvas.Instance.ShowFourOptionPanel(
                        "",
                        teleportTypeText,
                        options.Randomize(),
                        (selectedIndex) =>
                        {
                            if (options[selectedIndex] == interactionData.target)
                            {
                                //Correct option selected
                                "Correct option selected".Print();
                                MainCanvas.Instance.ResetPanel();
                                StartCoroutine(_activeInteraction.Process());
                            }
                            else
                            {
                                "Incorrect option selected".Print();
                                //Incorrect option selected
                            }
                        });
                }


            }
            else
            {

            }


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
                interaction.target.Print();
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