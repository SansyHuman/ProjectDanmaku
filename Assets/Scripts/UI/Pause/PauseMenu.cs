using System.Collections;
using System.Collections.Generic;

using SansyHuman.Debugging;
using SansyHuman.Management;
using SansyHuman.UDE.Management;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace SansyHuman.UI.Pause
{
    [RequireComponent(typeof(AudioSource))]
    [DisallowMultipleComponent]
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject pauseMenu;

        [SerializeField]
        private TextMeshProUGUI pause;

        [SerializeField]
        private RectTransform menuBox;

        [SerializeField]
        private TextMeshProUGUI[] menuEntries;

        [SerializeField]
        private WarningMessageBox warningBox;

        [SerializeField]
        private AudioClip pauseSound;

        [SerializeField]
        private AudioClip menuMove;

        [SerializeField]
        private AudioClip menuSelect;

        private AudioSource audioSource;
        private AudioSource pauseSource;

        private int selectedEntry = 0;
        private bool menuAnimationing = false;
        private bool readInput = true;

        public bool GamePaused => pauseMenu.activeSelf;

        [SerializeField]
        private DebugConsole debugConsole;

        private void Awake()
        {
            pauseMenu.SetActive(false);
            audioSource = GetComponent<AudioSource>();
            pauseSource = gameObject.AddComponent<AudioSource>();
            pauseSource.playOnAwake = false;
        }

        private const int RESUME = 0;
        private const int RETURN_TO_TITLE = 1;
        private const int RESTART_STAGE = 2;
        private const int SETTINGS = 3;
        private const int EXIT_GAME = 4;

        private void Update()
        {
            if (!readInput || menuAnimationing || debugConsole.DebugConsoleEnabled)
                return;

            Gamepad pad = Gamepad.current;

            if (pauseMenu.activeSelf)
            {
                Scene settings = SceneManager.GetSceneByName("Settings");
                if (settings.IsValid() && settings.isLoaded)
                    return;

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

            if (Input.GetKeyDown(KeyCode.Escape) || (pad != null && pad.startButton.wasPressedThisFrame))
            {

                if (!pauseMenu.activeSelf)
                {
                    GameManager.Instance.PauseBGM();
                    pauseSource.clip = pauseSound;
                    pauseSource.pitch = 1;
                    pauseSource.Play();
                    pauseMenu.SetActive(true);

                    pause.gameObject.SetActive(false);
                    for (int i = 0; i < menuEntries.Length; i++)
                        menuEntries[i].gameObject.SetActive(false);

                    menuBox.localScale = new Vector3(1, 0, 1);

                    StartCoroutine(PauseTransition());
                    menuAnimationing = true;
                }
                else
                {
                    pause.gameObject.SetActive(false);
                    StartCoroutine(UnpauseTransition());
                    menuAnimationing = true;
                }
            }
        }

        private IEnumerator MenuSelecting()
        {
            var animator = menuEntries[selectedEntry].GetComponent<Animator>();
            animator.SetTrigger("Select");

            AsyncOperation result = null;
            switch (selectedEntry)
            {
                case RESUME:
                    break;
                case RETURN_TO_TITLE:
                    break;
                case RESTART_STAGE:
                    break;
                case SETTINGS:
                    result = SceneManager.LoadSceneAsync("Settings", LoadSceneMode.Additive);
                    result.allowSceneActivation = false;
                    break;
                case EXIT_GAME:
                    break;
            }

            yield return null;
            yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Menu Selecting"));

            WarningMessageBox.WarningMessageBoxResult msg;

            switch (selectedEntry)
            {
                case RESUME:
                    pause.gameObject.SetActive(false);
                    yield return StartCoroutine(UnpauseTransition());
                    break;
                case RETURN_TO_TITLE:
                    msg = warningBox.ShowMessage(I2.Loc.ScriptTerms.Generic.WarningReturnToTitle);
                    readInput = false;
                    yield return new WaitUntil(() => msg.isDone);
                    if (msg.isYes)
                    {
                        GameManager.Instance.StopStage();
                        LoadingSceneManager.LoadScene("MainMenu");
                    }
                    break;
                case RESTART_STAGE:
                    msg = warningBox.ShowMessage(I2.Loc.ScriptTerms.Generic.WarningRestart);
                    readInput = false;
                    yield return new WaitUntil(() => msg.isDone);
                    if (msg.isYes)
                    {
                        GameManager.Instance.StopStage();
                        LoadingSceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    }
                    break;
                case SETTINGS:
                    result.allowSceneActivation = true;
                    readInput = false;
                    yield return new WaitUntil(() => result.isDone);
                    break;
                case EXIT_GAME:
                    msg = warningBox.ShowMessage(I2.Loc.ScriptTerms.Generic.WarningExitGame);
                    readInput = false;
                    yield return new WaitUntil(() => msg.isDone);
                    if (msg.isYes)
                        Application.Quit(0);
                    break;
            }

            menuAnimationing = false;

            Scene scene = new Scene();
            switch (selectedEntry)
            {
                case RESUME:
                    break;
                case RETURN_TO_TITLE:
                    readInput = true;
                    break;
                case RESTART_STAGE:
                    readInput = true;
                    break;
                case SETTINGS:
                    scene = SceneManager.GetSceneByName("Settings");
                    yield return new WaitWhile(() => scene.isLoaded || scene.IsValid());
                    readInput = true;
                    break;
                case EXIT_GAME:
                    readInput = true;
                    break;
            }
        }

        bool resumeOnUnpause; // Whether the debug console paused the game

        private IEnumerator PauseTransition()
        {
            if (UDETime.Instance.Paused)
            {
                resumeOnUnpause = false;
            }
            else
            {
                UDETime.Instance.PauseGame();
                resumeOnUnpause = true;
            }

            float accTime = 0;

            while (true)
            {
                accTime += Time.deltaTime;

                menuBox.localScale = new Vector3(1, 5 * accTime, 1);
                yield return null;

                if (accTime >= 0.2f)
                {
                    menuBox.localScale = new Vector3(1, 1, 1);
                    break;
                }
            }

            pause.gameObject.SetActive(true);
            for (int i = 0; i < menuEntries.Length; i++)
                menuEntries[i].gameObject.SetActive(true);

            selectedEntry = 0;
            menuEntries[0].GetComponent<Animator>().SetBool("Selected", true);

            menuAnimationing = false;
            yield return null;
        }

        private IEnumerator UnpauseTransition()
        {
            float accTime = 0;

            while (true)
            {
                accTime += Time.deltaTime;

                menuBox.localScale = new Vector3(1, 1 - 5 * accTime, 1);
                yield return null;

                if (accTime >= 0.2f)
                {
                    menuBox.localScale = new Vector3(1, 0, 1);
                    break;
                }
            }

            for (int i = 0; i < menuEntries.Length; i++)
                menuEntries[i].GetComponent<Animator>().SetBool("Selected", false);

            pauseMenu.SetActive(false);
            if (resumeOnUnpause)
            {
                UDETime.Instance.ResumeGame();
            }

            GameManager.Instance.ResumeBGM();
            menuAnimationing = false;

            yield return null;
        }
    }
}