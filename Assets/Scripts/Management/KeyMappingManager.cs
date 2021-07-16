using System;
using System.Collections;
using System.Collections.Generic;

using SansyHuman.Player;
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Object;

using UnityEngine;

namespace SansyHuman.Management
{
    [DisallowMultipleComponent]
    public class KeyMappingManager : UDESingleton<KeyMappingManager>
    {
        private UDEPlayer.KeyMappingInfo keyMapping;
        private PlayerBase.AdditionalKeyMappingInfo additionalKeyMapping;

        /// <summary>
        /// Gets the key code of the key that is pressed this frame.
        /// </summary>
        /// <returns>Key code. If there is no key pressed, return <see cref="KeyCode.None"/></returns>
        public static KeyCode GetKeyCodeDown()
        {
            if (!Input.anyKeyDown)
                return KeyCode.None;

            KeyCode[] codes = Enum.GetValues(typeof(KeyCode)) as KeyCode[];
            for (int i = 0; i < codes.Length; i++)
            {
                if (Input.GetKeyDown(codes[i]))
                    return codes[i];
            }

            return KeyCode.None;
        }

        protected override void Awake()
        {
            base.Awake();

            keyMapping = new UDEPlayer.KeyMappingInfo()
            {
                moveUp = KeyCode.UpArrow,
                moveDown = KeyCode.DownArrow,
                moveLeft = KeyCode.LeftArrow,
                moveRight = KeyCode.RightArrow,
                shoot = KeyCode.Z,
                slowMode = KeyCode.LeftShift
            };

            additionalKeyMapping = new PlayerBase.AdditionalKeyMappingInfo()
            {
                skill1Key = KeyCode.A,
                skill2Key = KeyCode.S,
                skill3Key = KeyCode.X
            };
        }

        /// <summary>
        /// Gets and sets the key mapping.s
        /// </summary>
        public UDEPlayer.KeyMappingInfo KeyMapping
        {
            get => keyMapping;
            set
            {
                keyMapping = value;
                ApplyMapping();
            }
        }

        public PlayerBase.AdditionalKeyMappingInfo AdditionalKeyMapping
        {
            get => additionalKeyMapping;
            set
            {
                additionalKeyMapping = value;
                ApplyMapping();
            }
        }

        /// <summary>
        /// Applies key mapping to the player.
        /// </summary>
        public void ApplyMapping()
        {
            PlayerBase player = GameManager.player;

            if (player != null)
            {
                player.SetKeyMapping(keyMapping);
                player.SetAdditionalKeyMapping(additionalKeyMapping);
            }
        }
    }
}