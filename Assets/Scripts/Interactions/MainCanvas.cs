using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MainCanvas : MonoBehaviour
{
    public static MainCanvas Instance;
    public Transform panel, option1, option2, option3, option4;
    public TextMeshProUGUI panelTitle, panelDescription;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        panel = transform.GetChildWithName("Panel");
        panelTitle = panel.transform.GetChildWithName("PanelTitle").GetComponent<TextMeshProUGUI>();
        panelDescription = panel.transform.GetChildWithName("PanelDescription").GetComponent<TextMeshProUGUI>();
        option1 = panel.transform.GetChildWithName("Option_1");
        option2 = panel.transform.GetChildWithName("Option_2");
        option3 = panel.transform.GetChildWithName("Option_3");
        option4 = panel.transform.GetChildWithName("Option_4");
    }
    public void ShowSingleButtonPanel(string title, string description, string buttonText, Action onClickAction)
    {
        panel.gameObject.SetActive(true);
        if (!string.IsNullOrEmpty(title))
            panelTitle.text = title;
        panelDescription.text = description;

        "Activating button 1".Print();
        option1.gameObject.SetActive(true);
        "Active hogya BC".Print();

        option2.gameObject.SetActive(false);
        option3.gameObject.SetActive(false);
        option4.gameObject.SetActive(false);

        option1.GetComponentInChildren<TextMeshProUGUI>().text = buttonText;

        var button1 = option1.GetComponent<Button>();
        button1.onClick.RemoveAllListeners();
        button1.onClick.AddListener(() => onClickAction?.Invoke());
    }

    public void ShowFourOptionPanel(string title, string description, List<string> options, Action<int> onOptionSelected)
    {
        panel.gameObject.SetActive(true);
        panelTitle.text = title;
        panelDescription.text = description;

        Transform[] optionTransforms = { option1, option2, option3, option4 };

        for (int i = 0; i < optionTransforms.Length; i++)
        {
            if (options != null && i < options.Count)
            {
                optionTransforms[i].gameObject.SetActive(true);
                optionTransforms[i].GetComponentInChildren<TextMeshProUGUI>().text = options[i];

                Button btn = optionTransforms[i].GetComponent<Button>();
                btn.onClick.RemoveAllListeners();
                int index = i;
                btn.onClick.AddListener(() => onOptionSelected?.Invoke(index));
            }
            else
            {
                optionTransforms[i].gameObject.SetActive(false);
            }
        }
    }

    public void ShowTitleAndDescription(string title, string description)
    {
        panel.gameObject.SetActive(true);
        if (!string.IsNullOrEmpty(title))
            panelTitle.text = title;
        panelDescription.text = description;
    }

    [ContextMenu("Test Single Button Panel")]
    public void TestSingleButtonPanel()
    {
        ShowSingleButtonPanel("Test Panel", "This is a test panel.", "Close", () => Debug.Log("Close button clicked"));
    }

    [ContextMenu("Test Four Option Panel")]
    public void TestFourOptionPanel()
    {
        ShowFourOptionPanel("Test Panel", "This is a test panel.", new List<string> { "Option 1", "Option 2", "Option 3", "Option 4" }, (optionIndex) => Debug.Log($"Option {optionIndex} selected"));
    }

    public void ResetPanel()
    {
        option1.gameObject.SetActive(false);
        "Setting button off".Print();
        option2.gameObject.SetActive(false);
        option3.gameObject.SetActive(false);
        option4.gameObject.SetActive(false);
    }
}
