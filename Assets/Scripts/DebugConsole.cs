using System.Collections;
using System.Collections.Generic;

using SansyHuman.UDE.Management;

using UnityEngine;
using UnityEngine.UI;

public class DebugConsole : MonoBehaviour
{
    [SerializeField]
    private InputField commandInput;

    [SerializeField]
    private Text outputText;

    [SerializeField]
    private RectTransform rectTransform;

    [SerializeField]
    private float yEnabled = -0f;

    [SerializeField]
    private float yDisabled = -1080f;

    private void Start()
    {
        Vector3 pos = rectTransform.anchoredPosition3D;
        pos.y = yDisabled;
        rectTransform.anchoredPosition3D = pos;

        commandInput.DeactivateInputField();
        commandInput.enabled = false;
    }

    private void Update()
    {
        if (commandInput.enabled && Input.GetKeyDown(KeyCode.F2))
        {
            Debug.Log("Disable");

            Vector3 pos = rectTransform.anchoredPosition3D;
            pos.y = yDisabled;
            rectTransform.anchoredPosition3D = pos;

            commandInput.DeactivateInputField();
            commandInput.enabled = false;

            return;
        }

        if (!commandInput.enabled && Input.GetKeyDown(KeyCode.F2))
        {
            Debug.Log("Enable");

            Vector3 pos = rectTransform.anchoredPosition3D;
            pos.y = yEnabled;
            rectTransform.anchoredPosition3D = pos;

            commandInput.enabled = true;
            commandInput.ActivateInputField();

            return;
        }
    }

    public void OnEndEdit()
    {
        if (Input.GetKeyDown(KeyCode.F2))
            return;

        Debug.Log(commandInput.text);

        string output = outputText.text;
        output += $"\n{commandInput.text}";
        outputText.text = output;

        commandInput.text = "";
        commandInput.ActivateInputField();
    }
}
