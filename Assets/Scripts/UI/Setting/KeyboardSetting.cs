using System;
using System.Collections;
using System.Collections.Generic;

using SansyHuman.Management;
using SansyHuman.Player;
using SansyHuman.UDE.Object;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;

namespace SansyHuman.UI.Setting
{
    [DisallowMultipleComponent]
    public class KeyboardSetting : MonoBehaviour
    {
        [SerializeField]
        private Color selectColor;

        [SerializeField]
        private Color unselectColor;

        [SerializeField]
        private SansyHuman.UI.Setting.Settings settings;

        [SerializeField]
        private List<TextMeshProUGUI> keyMapping;

        public enum KeyName
        {
            MoveUp = 0,
            MoveDown = 1,
            MoveLeft = 2,
            MoveRight = 3,
            Shoot = 4,
            SlowMode = 5,
            Skill1 = 6,
            Skill2 = 7,
            Skill3 = 8
        }

        // The order is row first.
        public const int RowNumber = 6;
        public const int ColumnNumber = 2;

        private int currentRow = 0;
        private int currentColumn = 0;

        private bool readingKeyInput = false;

        public bool ReadingKeyInput => readingKeyInput;

        private void OnEnable()
        {
            UDEPlayer.KeyMappingInfo keyMappingInfo = KeyMappingManager.Instance.KeyMapping;

            keyMapping[(int)KeyName.MoveUp].text = keyMappingInfo.moveUp.ToString();
            keyMapping[(int)KeyName.MoveDown].text = keyMappingInfo.moveDown.ToString();
            keyMapping[(int)KeyName.MoveLeft].text = keyMappingInfo.moveLeft.ToString();
            keyMapping[(int)KeyName.MoveRight].text = keyMappingInfo.moveRight.ToString();
            keyMapping[(int)KeyName.Shoot].text = keyMappingInfo.shoot.ToString();
            keyMapping[(int)KeyName.SlowMode].text = keyMappingInfo.slowMode.ToString();

            PlayerBase.AdditionalKeyMappingInfo additionalKeyMappingInfo = KeyMappingManager.Instance.AdditionalKeyMapping;

            keyMapping[(int)KeyName.Skill1].text = additionalKeyMappingInfo.skill1Key.ToString();
            keyMapping[(int)KeyName.Skill2].text = additionalKeyMappingInfo.skill2Key.ToString();
            keyMapping[(int)KeyName.Skill3].text = additionalKeyMappingInfo.skill3Key.ToString();

            int currKeySelected = currentRow + RowNumber * currentColumn;

            for (int i = 0; i < keyMapping.Count; i++)
            {
                if (i == currKeySelected)
                    keyMapping[i].color = selectColor;
                else
                    keyMapping[i].color = unselectColor;
            }

            readingKeyInput = false;
        }

        private void Update()
        {
            Gamepad pad = Gamepad.current;

            if (!readingKeyInput)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow) || (pad != null && pad.dpad.left.wasPressedThisFrame))
                {
                    if (currentColumn > 0)
                    {
                        int currKeySelected = currentRow + RowNumber * currentColumn;
                        keyMapping[currKeySelected].color = unselectColor;
                        currentColumn--;

                        currKeySelected = currentRow + RowNumber * currentColumn;
                        keyMapping[currKeySelected].color = selectColor;

                        settings.PlayMenuMove();
                    }
                    else
                    {
                        settings.PlayMenuError();
                    }
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow) || (pad != null && pad.dpad.right.wasPressedThisFrame))
                {
                    if (currentColumn < ColumnNumber - 1 && (currentRow + RowNumber * (currentColumn + 1)) < keyMapping.Count)
                    {
                        int currKeySelected = currentRow + RowNumber * currentColumn;
                        keyMapping[currKeySelected].color = unselectColor;
                        currentColumn++;

                        currKeySelected = currentRow + RowNumber * currentColumn;
                        keyMapping[currKeySelected].color = selectColor;

                        settings.PlayMenuMove();
                    }
                    else
                    {
                        settings.PlayMenuError();
                    }
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow) || (pad != null && pad.dpad.up.wasPressedThisFrame))
                {
                    if (currentRow > 0)
                    {
                        int currKeySelected = currentRow + RowNumber * currentColumn;
                        keyMapping[currKeySelected].color = unselectColor;
                        currentRow--;

                        currKeySelected = currentRow + RowNumber * currentColumn;
                        keyMapping[currKeySelected].color = selectColor;

                        settings.PlayMenuMove();
                    }
                    else
                    {
                        settings.PlayMenuError();
                    }
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow) || (pad != null && pad.dpad.down.wasPressedThisFrame))
                {
                    if (currentRow < RowNumber - 1)
                    {
                        int currKeySelected = currentRow + RowNumber * currentColumn;
                        keyMapping[currKeySelected].color = unselectColor;
                        currentRow++;

                        currKeySelected = currentRow + RowNumber * currentColumn;
                        if (currKeySelected >= keyMapping.Count)
                        {
                            currKeySelected -= RowNumber;
                            currentColumn--;
                        }
                        keyMapping[currKeySelected].color = selectColor;

                        settings.PlayMenuMove();
                    }
                    else
                    {
                        settings.PlayMenuError();
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || (pad != null && pad.buttonSouth.wasPressedThisFrame))
                {
                    int currKeySelected = currentRow + RowNumber * currentColumn;
                    keyMapping[currKeySelected].text = I2.Loc.ScriptLocalization.Settings_Keyboard.ReadingKey;
                    readingKeyInput = true;
                }
            }
            else
            {
                if (!Input.anyKeyDown)
                    return;

                KeyName currKeySelected = (KeyName)(currentRow + RowNumber * currentColumn);
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    UDEPlayer.KeyMappingInfo keyMappingInfo = KeyMappingManager.Instance.KeyMapping;
                    PlayerBase.AdditionalKeyMappingInfo additionalKeyMappingInfo = KeyMappingManager.Instance.AdditionalKeyMapping;

                    switch (currKeySelected)
                    {
                        case KeyName.MoveUp:
                            keyMapping[(int)KeyName.MoveUp].text = keyMappingInfo.moveUp.ToString();
                            break;
                        case KeyName.MoveDown:
                            keyMapping[(int)KeyName.MoveDown].text = keyMappingInfo.moveDown.ToString();
                            break;
                        case KeyName.MoveLeft:
                            keyMapping[(int)KeyName.MoveLeft].text = keyMappingInfo.moveLeft.ToString();
                            break;
                        case KeyName.MoveRight:
                            keyMapping[(int)KeyName.MoveRight].text = keyMappingInfo.moveRight.ToString();
                            break;
                        case KeyName.Shoot:
                            keyMapping[(int)KeyName.Shoot].text = keyMappingInfo.shoot.ToString();
                            break;
                        case KeyName.SlowMode:
                            keyMapping[(int)KeyName.SlowMode].text = keyMappingInfo.slowMode.ToString();
                            break;
                        case KeyName.Skill1:
                            keyMapping[(int)KeyName.Skill1].text = additionalKeyMappingInfo.skill1Key.ToString();
                            break;
                        case KeyName.Skill2:
                            keyMapping[(int)KeyName.Skill2].text = additionalKeyMappingInfo.skill2Key.ToString();
                            break;
                        case KeyName.Skill3:
                            keyMapping[(int)KeyName.Skill3].text = additionalKeyMappingInfo.skill3Key.ToString();
                            break;
                    }
                    readingKeyInput = false;
                }
                else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    return;
                }
                else
                {
                    KeyCode key = KeyMappingManager.GetKeyCodeDown();
                    if (key.ToString().Contains("Joystick")) // Do not allow joystick input.
                        return;

                    if (key != KeyCode.None)
                    {
                        UDEPlayer.KeyMappingInfo keyMappingInfo = KeyMappingManager.Instance.KeyMapping;
                        PlayerBase.AdditionalKeyMappingInfo additionalKeyMappingInfo = KeyMappingManager.Instance.AdditionalKeyMapping;

                        switch (currKeySelected)
                        {
                            case KeyName.MoveUp:
                                keyMappingInfo.moveUp = key;
                                keyMapping[(int)KeyName.MoveUp].text = keyMappingInfo.moveUp.ToString();
                                break;
                            case KeyName.MoveDown:
                                keyMappingInfo.moveDown = key;
                                keyMapping[(int)KeyName.MoveDown].text = keyMappingInfo.moveDown.ToString();
                                break;
                            case KeyName.MoveLeft:
                                keyMappingInfo.moveLeft = key;
                                keyMapping[(int)KeyName.MoveLeft].text = keyMappingInfo.moveLeft.ToString();
                                break;
                            case KeyName.MoveRight:
                                keyMappingInfo.moveRight = key;
                                keyMapping[(int)KeyName.MoveRight].text = keyMappingInfo.moveRight.ToString();
                                break;
                            case KeyName.Shoot:
                                keyMappingInfo.shoot = key;
                                keyMapping[(int)KeyName.Shoot].text = keyMappingInfo.shoot.ToString();
                                break;
                            case KeyName.SlowMode:
                                keyMappingInfo.slowMode = key;
                                keyMapping[(int)KeyName.SlowMode].text = keyMappingInfo.slowMode.ToString();
                                break;
                            case KeyName.Skill1:
                                additionalKeyMappingInfo.skill1Key = key;
                                keyMapping[(int)KeyName.Skill1].text = additionalKeyMappingInfo.skill1Key.ToString();
                                break;
                            case KeyName.Skill2:
                                additionalKeyMappingInfo.skill2Key = key;
                                keyMapping[(int)KeyName.Skill2].text = additionalKeyMappingInfo.skill2Key.ToString();
                                break;
                            case KeyName.Skill3:
                                additionalKeyMappingInfo.skill3Key = key;
                                keyMapping[(int)KeyName.Skill3].text = additionalKeyMappingInfo.skill3Key.ToString();
                                break;
                        }

                        KeyMappingManager.Instance.KeyMapping = keyMappingInfo;
                        KeyMappingManager.Instance.AdditionalKeyMapping = additionalKeyMappingInfo;

                        readingKeyInput = false;
                    }
                }
            }
        }
    }
}