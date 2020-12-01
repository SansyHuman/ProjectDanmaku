using System.Collections;
using System.Collections.Generic;

using SansyHuman.UDE.Management;

using TMPro;

using UnityEngine;
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
        private AudioClip pauseSound;

        [SerializeField]
        private AudioClip menuMove;

        [SerializeField]
        private AudioClip menuSelect;

        private AudioSource audioSource;
        private AudioSource pauseSource;

        private int selectedEntry = 0;
        private bool menuTransitioning = false;

        public bool GamePaused => pauseMenu.activeSelf;

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
            if (menuTransitioning)
                return;

            if (pauseMenu.activeSelf)
            {
                Scene settings = SceneManager.GetSceneByName("Settings");
                if (settings.IsValid() && settings.isLoaded)
                    return;

                if (Input.GetKeyDown(KeyCode.DownArrow))
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
                else if (Input.GetKeyDown(KeyCode.UpArrow))
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
                else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    audioSource.clip = menuSelect;
                    audioSource.Play();

                    switch (selectedEntry)
                    {
                        case RESUME:
                            pause.gameObject.SetActive(false);
                            StartCoroutine(UnpauseTransition());
                            menuTransitioning = true;
                            break;
                        case SETTINGS:
                            SceneManager.LoadSceneAsync("Settings", LoadSceneMode.Additive);
                            break;
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
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
                    menuTransitioning = true;
                }
                else
                {
                    pause.gameObject.SetActive(false);
                    StartCoroutine(UnpauseTransition());
                    menuTransitioning = true;
                }
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

            menuTransitioning = false;
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
            menuTransitioning = false;

            yield return null;
        }
    }
}