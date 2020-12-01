using System;
using System.Collections;
using System.Collections.Generic;

using SansyHuman.Debugging.Commands;
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Util;
using SansyHuman.UDE.Util.Math;
using SansyHuman.UI.Pause;

using UnityEngine;
using UnityEngine.UI;

namespace SansyHuman.Debugging
{
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

        [SerializeField]
        private int maxOutputLength = 4096;

        [SerializeField]
        private GameObject pauseMenu;

        private LinkedList<string> undo;
        private LinkedList<string> redo;

        private void Start()
        {
            DontDestroyOnLoad(this);

            undo = new LinkedList<string>();
            redo = new LinkedList<string>();

            Vector3 pos = rectTransform.anchoredPosition3D;
            pos.y = yDisabled;
            rectTransform.anchoredPosition3D = pos;

            commandInput.DeactivateInputField();
            commandInput.enabled = false;
        }

        private bool consoleStateChangable = true;
        private string undoLastOriginal = null;

        private void Update()
        {
            if (!pauseMenu.activeSelf && consoleStateChangable && commandInput.enabled && Input.GetKeyDown(KeyCode.F2))
            {
                UnityEngine.Debug.Log("Disable");

                Vector3 pos = rectTransform.anchoredPosition3D;
                pos.y = yDisabled;
                rectTransform.anchoredPosition3D = pos;

                commandInput.DeactivateInputField();
                commandInput.enabled = false;

                StartCoroutine(TransitConsole(yEnabled, yDisabled, 0.5f, UDETransitionHelper.easeOutQuart, UDETime.Instance.ResumeGame));

                return;
            }

            if (!pauseMenu.activeSelf && consoleStateChangable && !commandInput.enabled && Input.GetKeyDown(KeyCode.F2))
            {
                UnityEngine.Debug.Log("Enable");

                Vector3 pos = rectTransform.anchoredPosition3D;
                pos.y = yEnabled;
                rectTransform.anchoredPosition3D = pos;

                UDETime.Instance.PauseGame();

                StartCoroutine(TransitConsole(yDisabled, yEnabled, 0.5f, UDETransitionHelper.easeOutQuart, () =>
                {
                    commandInput.enabled = true;
                    commandInput.ActivateInputField();
                }));

                return;
            }

            if (commandInput.enabled && commandInput.isFocused)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow)) // Undo
                {
                    if (undo.Count == 0)
                        return;

                    if (redo.Count == 50)
                        redo.RemoveFirst();
                    redo.AddLast(commandInput.text);

                    undoLastOriginal = undo.Last.Value;
                    commandInput.text = undoLastOriginal;
                    undo.RemoveLast();

                    return;
                }

                if (Input.GetKeyDown(KeyCode.DownArrow)) // Redo
                {
                    if (redo.Count == 0)
                        return;

                    if (undo.Count == 50)
                        undo.RemoveFirst();
                    undo.AddLast(commandInput.text);

                    commandInput.text = redo.Last.Value;
                    redo.RemoveLast();
                    if (redo.Count == 0)
                        undoLastOriginal = null;
                    else
                        undoLastOriginal = commandInput.text;

                    return;
                }
            }
        }

        private IEnumerator TransitConsole(float yInit, float yLast, float time, UDEMath.TimeFunction ease, Action lastAction)
        {
            consoleStateChangable = false;

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

            consoleStateChangable = true;

            yield return null;
        }

        public void OnEndEdit()
        {
            if (!Input.GetKeyDown(KeyCode.Return))
                return;

            string command = commandInput.text;

            if (command == string.Empty)
                return;

            Debug.Log($"Command: {command}");

            string result = CommandList.ExecuteCommand(command);
            Debug.Log($"Result: {result}");

            string output = outputText.text;
            if (result != string.Empty)
                output += $"\n{result}";
            if (output.Length > maxOutputLength)
            {
                int index = output.IndexOf('\n', output.Length - maxOutputLength);
                output = output.Remove(0, index + 1);
            }
            outputText.text = output;

            if (undoLastOriginal != null)
            {
                if (undo.Count == 50)
                    undo.RemoveFirst();
                undo.AddLast(undoLastOriginal);
                undoLastOriginal = null;
            }

            int cnt = redo.Count - 1;
            for (int i = 0; i < cnt; i++)
            {
                if (undo.Count == 50)
                    undo.RemoveFirst();
                undo.AddLast(redo.Last.Value);
                redo.RemoveLast();
            }

            if (undo.Count == 50)
                undo.RemoveFirst();
            undo.AddLast(command);

            redo.Clear();

            commandInput.text = "";
            commandInput.ActivateInputField();
        }
    }
}