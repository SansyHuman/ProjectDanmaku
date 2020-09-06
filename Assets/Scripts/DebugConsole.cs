using System;
using System.Collections;
using System.Collections.Generic;

using SansyHuman.UDE.Management;
using SansyHuman.UDE.Util;
using SansyHuman.UDE.Util.Math;

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

            StartCoroutine(TransitConsole(yEnabled, yDisabled, 0.5f, UDETransitionHelper.easeOutCubic, UDETime.Instance.ResumeGame));

            return;
        }

        if (!commandInput.enabled && Input.GetKeyDown(KeyCode.F2))
        {
            Debug.Log("Enable");

            Vector3 pos = rectTransform.anchoredPosition3D;
            pos.y = yEnabled;
            rectTransform.anchoredPosition3D = pos;

            UDETime.Instance.PauseGame();

            StartCoroutine(TransitConsole(yDisabled, yEnabled, 0.5f, UDETransitionHelper.easeOutCubic, () => {
                commandInput.enabled = true;
                commandInput.ActivateInputField();
            }));

            return;
        }
    }

    private IEnumerator TransitConsole(float yInit, float yLast, float time, UDEMath.TimeFunction ease, Action lastAction)
    {
        UDEMath.TimeFunction yFunc = t => yInit + t * (yLast - yInit);
        yFunc = yFunc.Composite(ease).Composite(t => t / time);

        float acct = 0;
        while (true)
        {
            acct += Time.deltaTime;

            Vector3 pos = rectTransform.anchoredPosition3D;
            pos.y = yFunc(acct);
            if (acct > time)
                pos.y = yLast;
            rectTransform.anchoredPosition3D = pos;
            if (acct > time)
                break;

            yield return null;
        }

        lastAction.Invoke();

        yield return null;
    }

    public void OnEndEdit()
    {
        if (!Input.GetKeyDown(KeyCode.Return))
            return;

        if (commandInput.text == string.Empty)
            return;

        Debug.Log(commandInput.text);

        string output = outputText.text;
        output += $"\n{commandInput.text}";
        outputText.text = output;

        commandInput.text = "";
        commandInput.ActivateInputField();
    }
}
