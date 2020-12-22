using System.Collections;
using System.Collections.Generic;

using SansyHuman.Management;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace SansyHuman.UI.Main
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public class MainMenu : MonoBehaviour
    {
        private const int START_GAME = 0;
        private const int SETTINGS = 1;
        private const int LICENSE = 2;
        private const int EXIT_GAME = 3;

        [SerializeField]
        private TextMeshProUGUI[] menuEntries;

        [SerializeField]
        private AudioClip menuMove;

        [SerializeField]
        private AudioClip menuSelect;

        private AudioSource audioSource;

        private static int selectedEntry = 0;
        private bool readInput = true;
        private bool menuAnimationing = false;

        public bool ReadInput
        {
            get => readInput;
            set => readInput = value;
        }

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            menuEntries[selectedEntry].GetComponent<Animator>().SetBool("Selected", true);
        }

        private void Update()
        {
            if (!readInput || menuAnimationing)
                return;

            Gamepad pad = Gamepad.current;

            if (Input.GetKeyDown(KeyCode.DownArrow) || (pad != null && pad.dpad.down.wasPressedThisFrame))
            {
                audioSource.clip = menuMove;
                audioSource.Play();

                menuEntries[selectedEntry].GetComponent<Animator>().SetBool("Selected", false);

                if (selectedEntry == menuEntries.Length - 1)
                    selectedEntry = 0;
                else
                    selectedEntry++;

                menuEntries[selectedEntry].GetComponent<Animator>().SetBool("Selected", true);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) || (pad != null && pad.dpad.up.wasPressedThisFrame))
            {
                audioSource.clip = menuMove;
                audioSource.Play();

                menuEntries[selectedEntry].GetComponent<Animator>().SetBool("Selected", false);

                if (selectedEntry == 0)
                    selectedEntry = menuEntries.Length - 1;
                else
                    selectedEntry--;

                menuEntries[selectedEntry].GetComponent<Animator>().SetBool("Selected", true);
            }
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || (pad != null && pad.buttonSouth.wasPressedThisFrame))
            {
                audioSource.clip = menuSelect;
                audioSource.Play();

                menuAnimationing = true;
                StartCoroutine(MenuSelecting());
            }
        }

        private IEnumerator MenuSelecting()
        {
            var animator = menuEntries[selectedEntry].GetComponent<Animator>();
            animator.SetTrigger("Select");

            AsyncOperation result = null;
            switch (selectedEntry)
            {
                case START_GAME:
                    break;
                case SETTINGS:
                    result = SceneManager.LoadSceneAsync("Settings", LoadSceneMode.Additive);
                    result.allowSceneActivation = false;
                    break;
                case LICENSE:
                    result = SceneManager.LoadSceneAsync("License", LoadSceneMode.Additive);
                    result.allowSceneActivation = false;
                    break;
                case EXIT_GAME:
                    break;
            }

            yield return null;
            yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Menu Selecting"));

            switch (selectedEntry)
            {
                case START_GAME:
                    LoadingSceneManager.LoadScene("Stage1");
                    break;
                case SETTINGS: 
                case LICENSE:
                    result.allowSceneActivation = true;
                    readInput = false;
                    yield return new WaitUntil(() => result.isDone);
                    break;
                case EXIT_GAME:
                    Application.Quit(0);
                    break;
            }

            menuAnimationing = false;

            Scene scene = new Scene();
            switch (selectedEntry)
            {
                case START_GAME:
                    break;
                case SETTINGS:
                    scene = SceneManager.GetSceneByName("Settings");
                    yield return new WaitWhile(() => scene.isLoaded || scene.IsValid());
                    readInput = true;
                    break;
                case LICENSE:
                    scene = SceneManager.GetSceneByName("License");
                    yield return new WaitWhile(() => scene.isLoaded || scene.IsValid());
                    readInput = true;
                    break;
            }
        }
    }
}