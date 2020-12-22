using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;

namespace SansyHuman.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(AudioSource))]
    public class WarningMessageBox : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI warningText;

        [SerializeField]
        private TextMeshProUGUI yes;

        [SerializeField]
        private TextMeshProUGUI no;

        [SerializeField]
        private AudioClip entryMove;

        [SerializeField]
        private AudioClip entrySelect;

        private RectTransform rectTr;
        private AudioSource audioSource;
        private string scriptTerm;

        private void Awake()
        {
            rectTr = GetComponent<RectTransform>();
            audioSource = GetComponent<AudioSource>();

            warningText.gameObject.SetActive(false);
            yes.gameObject.SetActive(false);
            no.gameObject.SetActive(false);

            rectTr.localScale = new Vector3(1, 0, 1);
        }

        public class WarningMessageBoxResult
        {
            public bool isDone;
            public bool isYes;
        }

        public WarningMessageBoxResult ShowMessage(string scriptTerm)
        {
            this.scriptTerm = scriptTerm;
            warningText.text = I2.Loc.LocalizationManager.GetTranslation(scriptTerm);

            WarningMessageBoxResult result = new WarningMessageBoxResult();
            result.isDone = false;
            result.isYes = true;

            StartCoroutine(MessageBoxLoop(result));
            return result;
        }

        private IEnumerator MessageBoxLoop(WarningMessageBoxResult result)
        {
            float accTime = 0;

            while (true)
            {
                accTime += Time.deltaTime;
                Vector3 scale = rectTr.localScale;
                scale.y = accTime * 5f;
                if (scale.y > 1f)
                    scale.y = 1f;
                rectTr.localScale = scale;

                if (accTime >= 0.2f)
                {
                    scale.y = 1f;
                    rectTr.localScale = scale;
                    break;
                }

                yield return null;
            }

            warningText.gameObject.SetActive(true);
            yes.gameObject.SetActive(true);
            no.gameObject.SetActive(true);

            if (result.isYes)
            {
                yes.GetComponent<Animator>().SetBool("Selected", true);
                no.GetComponent<Animator>().SetBool("Selected", false);
            }
            else
            {
                yes.GetComponent<Animator>().SetBool("Selected", false);
                no.GetComponent<Animator>().SetBool("Selected", true);
            }

            Gamepad pad = Gamepad.current;

            while (true)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow) ||
                    Input.GetKeyDown(KeyCode.LeftArrow) ||
                    (pad != null && pad.dpad.right.wasPressedThisFrame) ||
                    (pad != null && pad.dpad.left.wasPressedThisFrame))
                {
                    audioSource.clip = entryMove;
                    audioSource.Play();

                    result.isYes = !result.isYes;
                    if (result.isYes)
                    {
                        yes.GetComponent<Animator>().SetBool("Selected", true);
                        no.GetComponent<Animator>().SetBool("Selected", false);
                    }
                    else
                    {
                        yes.GetComponent<Animator>().SetBool("Selected", false);
                        no.GetComponent<Animator>().SetBool("Selected", true);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Return) ||
                    Input.GetKeyDown(KeyCode.KeypadEnter) ||
                    (pad != null && pad.buttonSouth.wasPressedThisFrame))
                {
                    audioSource.clip = entrySelect;
                    audioSource.Play();

                    yield return StartCoroutine(EntrySelecting(result));

                    warningText.gameObject.SetActive(false);
                    yes.gameObject.SetActive(false);
                    no.gameObject.SetActive(false);

                    rectTr.localScale = new Vector3(1, 0, 1);

                    result.isDone = true;

                    break;
                }

                yield return null;
            }
        }

        private IEnumerator EntrySelecting(WarningMessageBoxResult result)
        {
            Animator animator = null;
            if (result.isYes)
                animator = yes.GetComponent<Animator>();
            else
                animator = no.GetComponent<Animator>();

            animator.SetTrigger("Select");

            yield return null;
            yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Menu Selecting"));
        }

        public void OnLocalization()
        {
            warningText.text = I2.Loc.LocalizationManager.GetTranslation(scriptTerm);
        }
    }
}