using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Button trainingBtn, assessmentBtn, settingsBtn;
    [SerializeField]
    void Start()
    {
        trainingBtn = transform.GetChildWithName("TrainingBtn").GetComponent<Button>();
        assessmentBtn = transform.GetChildWithName("AssessmentBtn").GetComponent<Button>();
        settingsBtn = transform.GetChildWithName("SettingsBtn").GetComponent<Button>();
        AssignListeners();
    }

    private void AssignListeners()
    {
        trainingBtn.onClick.AddListener(OnTrainingButtonPressed);
        assessmentBtn.onClick.AddListener(OnAssessmentButtonPressed);
    }
    private void OnTrainingButtonPressed()
    {
        SceneManager.LoadScene("TrainingScene");
    }
    private void OnAssessmentButtonPressed()
    {
        SceneManager.LoadScene("AssessmentScene");
    }
}
