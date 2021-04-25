using System;
using System.Collections;
using System.Collections.Generic;

using SansyHuman.Management;
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

        [Serializable]
        public struct KeyMapping
        {
            public TextMeshProUGUI moveUp;
            public TextMeshProUGUI moveDown;
            public TextMeshProUGUI moveLeft;
            public TextMeshProUGUI moveRight;
            public TextMeshProUGUI shoot;
            public TextMeshProUGUI slowMode;
        }

        [SerializeField]
        private KeyMapping keyMapping;

        public enum KeyName
        {
            MoveUp = 0,
            MoveDown = 1,
            MoveLeft = 2,
            MoveRight = 3,
            Shoot = 4,
            SlowMode = 5
        }

        // The order is row first.
        public const int RowNumber = 6;
        public const int ColumnNumber = 1;

        private int currentRow = 0;
        private int currentColumn = 0;

        private bool readingKeyInput = false;

        public bool ReadingKeyInput => readingKeyInput;

        private void OnEnable()
        {
            UDEPlayer.KeyMappingInfo keyMappingInfo = KeyMappingManager.Instance.KeyMapping;

            keyMapping.moveUp.text = keyMappingInfo.moveUp.ToString();
            keyMapping.moveDown.text = keyMappingInfo.moveDown.ToString();
            keyMapping.moveLeft.text = keyMappingInfo.moveLeft.ToString();
            keyMapping.moveRight.text = keyMappingInfo.moveRight.ToString();
            keyMapping.shoot.text = keyMappingInfo.shoot.ToString();
            keyMapping.slowMode.text = keyMappingInfo.slowMode.ToString();

            KeyName currKeySelected = (KeyName)(currentRow + RowNumber * currentColumn);

            if (currKeySelected == KeyName.MoveUp)
            {
                keyMapping.moveUp.color = selectColor;
            }
            else
            {
                keyMapping.moveUp.color = unselectColor;
            }

            if (currKeySelected == KeyName.MoveDown)
            {
                keyMapping.moveDown.color = selectColor;
            }
            else
            {
                keyMapping.moveDown.color = unselectColor;
            }

            if (currKeySelected == KeyName.MoveLeft)
            {
                keyMapping.moveLeft.color = selectColor;
            }
            else
            {
                keyMapping.moveLeft.color = unselectColor;
            }

            if (currKeySelected == KeyName.MoveRight)
            {
                keyMapping.moveRight.color = selectColor;
            }
            else
            {
                keyMapping.moveRight.color = unselectColor;
            }

            if (currKeySelected == KeyName.Shoot)
            {
                keyMapping.shoot.color = selectColor;
            }
            else
            {
                keyMapping.shoot.color = unselectColor;
            }

            if (currKeySelected == KeyName.SlowMode)
            {
                keyMapping.slowMode.color = selectColor;
            }
            else
            {
                keyMapping.slowMode.color = unselectColor;
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
                        KeyName currKeySelected = (KeyName)(currentRow + RowNumber * currentColumn);
                        switch (currKeySelected)
                        {
                            case KeyName.MoveUp:
                                keyMapping.moveUp.color = unselectColor;
                                break;
                            case KeyName.MoveDown:
                                keyMapping.moveDown.color = unselectColor;
                                break;
                            case KeyName.MoveLeft:
                                keyMapping.moveLeft.color = unselectColor;
                                break;
                            case KeyName.MoveRight:
                                keyMapping.moveRight.color = unselectColor;
                                break;
                            case KeyName.Shoot:
                                keyMapping.shoot.color = unselectColor;
                                break;
                            case KeyName.SlowMode:
                                keyMapping.slowMode.color = unselectColor;
                                break;
                        }
                        currentColumn--;

                        currKeySelected = (KeyName)(currentRow + RowNumber * currentColumn);
                        switch (currKeySelected)
                        {
                            case KeyName.MoveUp:
                                keyMapping.moveUp.color = selectColor;
                                break;
                            case KeyName.MoveDown:
                                keyMapping.moveDown.color = selectColor;
                                break;
                            case KeyName.MoveLeft:
                                keyMapping.moveLeft.color = selectColor;
                                break;
                            case KeyName.MoveRight:
                                keyMapping.moveRight.color = selectColor;
                                break;
                            case KeyName.Shoot:
                                keyMapping.shoot.color = selectColor;
                                break;
                            case KeyName.SlowMode:
                                keyMapping.slowMode.color = selectColor;
                                break;
                        }

                        settings.PlayMenuMove();
                    }
                    else
                    {
                        settings.PlayMenuError();
                    }
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow) || (pad != null && pad.dpad.right.wasPressedThisFrame))
                {
                    if (currentColumn < ColumnNumber - 1)
                    {
                        KeyName currKeySelected = (KeyName)(currentRow + RowNumber * currentColumn);
                        switch (currKeySelected)
                        {
                            case KeyName.MoveUp:
                                keyMapping.moveUp.color = unselectColor;
                                break;
                            case KeyName.MoveDown:
                                keyMapping.moveDown.color = unselectColor;
                                break;
                            case KeyName.MoveLeft:
                                keyMapping.moveLeft.color = unselectColor;
                                break;
                            case KeyName.MoveRight:
                                keyMapping.moveRight.color = unselectColor;
                                break;
                            case KeyName.Shoot:
                                keyMapping.shoot.color = unselectColor;
                                break;
                            case KeyName.SlowMode:
                                keyMapping.slowMode.color = unselectColor;
                                break;
                        }
                        currentColumn++;

                        currKeySelected = (KeyName)(currentRow + RowNumber * currentColumn);
                        switch (currKeySelected)
                        {
                            case KeyName.MoveUp:
                                keyMapping.moveUp.color = selectColor;
                                break;
                            case KeyName.MoveDown:
                                keyMapping.moveDown.color = selectColor;
                                break;
                            case KeyName.MoveLeft:
                                keyMapping.moveLeft.color = selectColor;
                                break;
                            case KeyName.MoveRight:
                                keyMapping.moveRight.color = selectColor;
                                break;
                            case KeyName.Shoot:
                                keyMapping.shoot.color = selectColor;
                                break;
                            case KeyName.SlowMode:
                                keyMapping.slowMode.color = selectColor;
                                break;
                        }

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
                        KeyName currKeySelected = (KeyName)(currentRow + RowNumber * currentColumn);
                        switch (currKeySelected)
                        {
                            case KeyName.MoveUp:
                                keyMapping.moveUp.color = unselectColor;
                                break;
                            case KeyName.MoveDown:
                                keyMapping.moveDown.color = unselectColor;
                                break;
                            case KeyName.MoveLeft:
                                keyMapping.moveLeft.color = unselectColor;
                                break;
                            case KeyName.MoveRight:
                                keyMapping.moveRight.color = unselectColor;
                                break;
                            case KeyName.Shoot:
                                keyMapping.shoot.color = unselectColor;
                                break;
                            case KeyName.SlowMode:
                                keyMapping.slowMode.color = unselectColor;
                                break;
                        }
                        currentRow--;

                        currKeySelected = (KeyName)(currentRow + RowNumber * currentColumn);
                        switch (currKeySelected)
                        {
                            case KeyName.MoveUp:
                                keyMapping.moveUp.color = selectColor;
                                break;
                            case KeyName.MoveDown:
                                keyMapping.moveDown.color = selectColor;
                                break;
                            case KeyName.MoveLeft:
                                keyMapping.moveLeft.color = selectColor;
                                break;
                            case KeyName.MoveRight:
                                keyMapping.moveRight.color = selectColor;
                                break;
                            case KeyName.Shoot:
                                keyMapping.shoot.color = selectColor;
                                break;
                            case KeyName.SlowMode:
                                keyMapping.slowMode.color = selectColor;
                                break;
                        }

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
                        KeyName currKeySelected = (KeyName)(currentRow + RowNumber * currentColumn);
                        switch (currKeySelected)
                        {
                            case KeyName.MoveUp:
                                keyMapping.moveUp.color = unselectColor;
                                break;
                            case KeyName.MoveDown:
                                keyMapping.moveDown.color = unselectColor;
                                break;
                            case KeyName.MoveLeft:
                                keyMapping.moveLeft.color = unselectColor;
                                break;
                            case KeyName.MoveRight:
                                keyMapping.moveRight.color = unselectColor;
                                break;
                            case KeyName.Shoot:
                                keyMapping.shoot.color = unselectColor;
                                break;
                            case KeyName.SlowMode:
                                keyMapping.slowMode.color = unselectColor;
                                break;
                        }
                        currentRow++;

                        currKeySelected = (KeyName)(currentRow + RowNumber * currentColumn);
                        switch (currKeySelected)
                        {
                            case KeyName.MoveUp:
                                keyMapping.moveUp.color = selectColor;
                                break;
                            case KeyName.MoveDown:
                                keyMapping.moveDown.color = selectColor;
                                break;
                            case KeyName.MoveLeft:
                                keyMapping.moveLeft.color = selectColor;
                                break;
                            case KeyName.MoveRight:
                                keyMapping.moveRight.color = selectColor;
                                break;
                            case KeyName.Shoot:
                                keyMapping.shoot.color = selectColor;
                                break;
                            case KeyName.SlowMode:
                                keyMapping.slowMode.color = selectColor;
                                break;
                        }

                        settings.PlayMenuMove();
                    }
                    else
                    {
                        settings.PlayMenuError();
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || (pad != null && pad.buttonSouth.wasPressedThisFrame))
                {
                    KeyName currKeySelected = (KeyName)(currentRow + RowNumber * currentColumn);
                    switch (currKeySelected)
                    {
                        case KeyName.MoveUp:
                            keyMapping.moveUp.text = I2.Loc.ScriptLocalization.Settings_Keyboard.ReadingKey;
                            break;
                        case KeyName.MoveDown:
                            keyMapping.moveDown.text = I2.Loc.ScriptLocalization.Settings_Keyboard.ReadingKey;
                            break;
                        case KeyName.MoveLeft:
                            keyMapping.moveLeft.text = I2.Loc.ScriptLocalization.Settings_Keyboard.ReadingKey;
                            break;
                        case KeyName.MoveRight:
                            keyMapping.moveRight.text = I2.Loc.ScriptLocalization.Settings_Keyboard.ReadingKey;
                            break;
                        case KeyName.Shoot:
                            keyMapping.shoot.text = I2.Loc.ScriptLocalization.Settings_Keyboard.ReadingKey;
                            break;
                        case KeyName.SlowMode:
                            keyMapping.slowMode.text = I2.Loc.ScriptLocalization.Settings_Keyboard.ReadingKey;
                            break;
                    }
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

                    switch (currKeySelected)
                    {
                        case KeyName.MoveUp:
                            keyMapping.moveUp.text = keyMappingInfo.moveUp.ToString();
                            break;
                        case KeyName.MoveDown:
                            keyMapping.moveDown.text = keyMappingInfo.moveDown.ToString();
                            break;
                        case KeyName.MoveLeft:
                            keyMapping.moveLeft.text = keyMappingInfo.moveLeft.ToString();
                            break;
                        case KeyName.MoveRight:
                            keyMapping.moveRight.text = keyMappingInfo.moveRight.ToString();
                            break;
                        case KeyName.Shoot:
                            keyMapping.shoot.text = keyMappingInfo.shoot.ToString();
                            break;
                        case KeyName.SlowMode:
                            keyMapping.slowMode.text = keyMappingInfo.slowMode.ToString();
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

                        switch (currKeySelected)
                        {
                            case KeyName.MoveUp:
                                keyMappingInfo.moveUp = key;
                                keyMapping.moveUp.text = keyMappingInfo.moveUp.ToString();
                                break;
                            case KeyName.MoveDown:
                                keyMappingInfo.moveDown = key;
                                keyMapping.moveDown.text = keyMappingInfo.moveDown.ToString();
                                break;
                            case KeyName.MoveLeft:
                                keyMappingInfo.moveLeft = key;
                                keyMapping.moveLeft.text = keyMappingInfo.moveLeft.ToString();
                                break;
                            case KeyName.MoveRight:
                                keyMappingInfo.moveRight = key;
                                keyMapping.moveRight.text = keyMappingInfo.moveRight.ToString();
                                break;
                            case KeyName.Shoot:
                                keyMappingInfo.shoot = key;
                                keyMapping.shoot.text = keyMappingInfo.shoot.ToString();
                                break;
                            case KeyName.SlowMode:
                                keyMappingInfo.slowMode = key;
                                keyMapping.slowMode.text = keyMappingInfo.slowMode.ToString();
                                break;
                        }
                        KeyMappingManager.Instance.KeyMapping = keyMappingInfo;

                        readingKeyInput = false;
                    }
                }
            }
        }
    }
}