using System.Collections;
using System.Collections.Generic;

using SansyHuman.UDE.Management;
using SansyHuman.UDE.Object;
using UnityEngine;
using UnityEngine.UI;

namespace SansyHuman.Debugging
{
    [DisallowMultipleComponent]
    public class TransformDebug : MonoBehaviour
    {
        public enum ShowMode
        {
            ENEMY,
            PLAYER,
            BULLET,
            ALL,
            ID,
            NONE
        }

        [SerializeField]
        private Text sampleText;

        private ShowMode mode;
        private List<Transform> transforms;
        private List<Text> texts;
        private Stack<Text> textPool;

        private void Awake()
        {
            mode = ShowMode.NONE;
            transforms = new List<Transform>();
            texts = new List<Text>();
            textPool = new Stack<Text>(3000);
        }

        private void Update()
        {
            if (mode == ShowMode.NONE)
                return;

            switch (mode)
            {
                case ShowMode.ENEMY:
                    DrawEnemies();
                    break;
                case ShowMode.PLAYER:
                    DrawPlayer();
                    break;
                case ShowMode.BULLET:
                    DrawBullets();
                    break;
                case ShowMode.ALL:
                    DrawAll();
                    break;
                case ShowMode.ID:
                    DrawInstanceIDs();
                    break;
                case ShowMode.NONE:
                default:
                    break;
            }
        }

        private void DrawEnemies()
        {
            var enemies = UDEObjectManager.Instance.GetAllEnemies();

            if (texts.Count < enemies.Count)
            {
                int cnt = enemies.Count - texts.Count;

                for (int i = 0; i < cnt; i++)
                    texts.Add(GetTextFromPool());
            }
            else if (texts.Count > enemies.Count)
            {
                int cnt = texts.Count - enemies.Count;
                
                for (int i = enemies.Count + cnt - 1; i >= enemies.Count; i--)
                {
                    Text text = texts[i];
                    text.gameObject.SetActive(false);
                    textPool.Push(text);
                    texts.RemoveAt(i);
                }
            }

            for (int i = 0; i < enemies.Count; i++)
            {
                Text text = texts[i];
                text.text = ((Vector2)enemies[i].transform.position).ToString();
                text.transform.position = enemies[i].transform.position;
            }
        }

        private void DrawPlayer()
        {
            var player = FindObjectOfType<UDEPlayer>();
            if (player.gameObject.activeInHierarchy && player.enabled && texts.Count == 0)
            {
                texts.Add(GetTextFromPool());
            }

            if ((!player.gameObject.activeInHierarchy || !player.enabled) && texts.Count == 1)
            {
                Text text = texts[0];
                text.gameObject.SetActive(false);
                textPool.Push(text);
                texts.RemoveAt(0);
                return;
            }

            Text txt = texts[0];
            txt.text = ((Vector2)player.transform.position).ToString();
            txt.transform.position = player.transform.position;
        }

        private void DrawBullets()
        {
            var bullets = UDEObjectManager.Instance.GetAllBullets();

            if (texts.Count < bullets.Count)
            {
                int cnt = bullets.Count - texts.Count;

                for (int i = 0; i < cnt; i++)
                    texts.Add(GetTextFromPool());
            }
            else if (texts.Count > bullets.Count)
            {
                int cnt = texts.Count - bullets.Count;

                for (int i = bullets.Count + cnt - 1; i >= bullets.Count; i--)
                {
                    Text text = texts[i];
                    text.gameObject.SetActive(false);
                    textPool.Push(text);
                    texts.RemoveAt(i);
                }
            }

            for (int i = 0; i < bullets.Count; i++)
            {
                Text text = texts[i];
                text.text = ((Vector2)bullets[i].transform.position).ToString();
                text.transform.position = bullets[i].transform.position;
            }
        }

        private void DrawInstanceIDs()
        {
            for (int i = 0; i < transforms.Count; i++)
            {
                Text text = texts[i];
                text.text = ((Vector2)transforms[i].position).ToString();
                text.transform.position = transforms[i].position;
            }
        }

        private void DrawAll()
        {
            var enemies = UDEObjectManager.Instance.GetAllEnemies();
            var player = FindObjectOfType<UDEPlayer>();
            var bullets = UDEObjectManager.Instance.GetAllBullets();
            int count = enemies.Count + bullets.Count + (player.gameObject.activeInHierarchy && player.enabled ? 1 : 0);

            if (texts.Count < count)
            {
                int cnt = count - texts.Count;

                for (int i = 0; i < cnt; i++)
                    texts.Add(GetTextFromPool());
            }
            else if (texts.Count > count)
            {
                int cnt = texts.Count - count;

                for (int i = count + cnt - 1; i >= count; i--)
                {
                    Text text = texts[i];
                    text.gameObject.SetActive(false);
                    textPool.Push(text);
                    texts.RemoveAt(i);
                }
            }

            int index = 0;
            for (int i = 0; i < enemies.Count; i++, index++)
            {
                Text text = texts[index];
                text.text = ((Vector2)enemies[i].transform.position).ToString();
                text.transform.position = enemies[i].transform.position;
            }

            if (player.gameObject.activeInHierarchy && player.enabled)
            {
                Text text = texts[index];
                text.text = ((Vector2)player.transform.position).ToString();
                text.transform.position = player.transform.position;
                index++;
            }

            for (int i = 0; i < bullets.Count; i++, index++)
            {
                Text text = texts[index];
                text.text = ((Vector2)bullets[i].transform.position).ToString();
                text.transform.position = bullets[i].transform.position;
            }
        }

        private void LateUpdate()
        {
            for (int i = 0; i < transforms.Count; i++)
            {
                if (transforms[i] == null || !transforms[i].gameObject.activeInHierarchy)
                {
                    transforms.RemoveAt(i);
                    Text text = texts[i];
                    text.gameObject.SetActive(false);
                    textPool.Push(text);
                    texts.RemoveAt(i);
                    i--;
                }
            }
        }

        private Text GetTextFromPool()
        {
            if (textPool.Count == 0)
            {
                Text text = Instantiate(sampleText);
                text.rectTransform.SetParent(transform);
                text.rectTransform.localScale = new Vector3(1, 1, 1);
                text.gameObject.SetActive(false);

                textPool.Push(text);
            }

            Text ret = textPool.Pop();
            ret.gameObject.SetActive(true);
            return ret;
        }

        private void ResetText()
        {
            int cnt = texts.Count;
            for (int i = 0; i < cnt; i++)
            {
                Text text = texts[texts.Count - 1];
                text.gameObject.SetActive(false);
                textPool.Push(text);
                texts.RemoveAt(texts.Count - 1);
            }
        }

        public bool[] ShowInstanceID(int[] ids)
        {
            transforms.Clear();
            ResetText();

            mode = ShowMode.ID;
            bool[] exists = new bool[ids.Length];

            for (int i = 0; i < ids.Length; i++)
            {
                GameObject instance = DebugUtil.FindObjectFromInstanceID<GameObject>(ids[i]);
                if (instance == null || !instance.activeInHierarchy)
                {
                    exists[i] = false;
                    continue;
                }

                Transform tr = instance.transform;
                if (tr == null)
                {
                    exists[i] = false;
                    continue;
                }

                transforms.Add(tr);
                texts.Add(GetTextFromPool());
                exists[i] = true;
            }

            return exists;
        }

        public void ShowEnemies()
        {
            transforms.Clear();
            ResetText();
            mode = ShowMode.ENEMY;

            var enemies = UDEObjectManager.Instance.GetAllEnemies();

            for (int i = 0; i < enemies.Count; i++)
            {
                texts.Add(GetTextFromPool());
            }
        }

        public void ShowPlayer()
        {
            transforms.Clear();
            ResetText();
            mode = ShowMode.PLAYER;

            var player = FindObjectOfType<UDEPlayer>();
            if (player.gameObject.activeInHierarchy && player.enabled)
                texts.Add(GetTextFromPool());
        }

        public void ShowBullets()
        {
            transforms.Clear();
            ResetText();
            mode = ShowMode.BULLET;

            var bullets = UDEObjectManager.Instance.GetAllBullets();

            for (int i = 0; i < bullets.Count; i++)
            {
                texts.Add(GetTextFromPool());
            }
        }

        public void ShowAll()
        {
            transforms.Clear();
            ResetText();
            mode = ShowMode.ALL;

            var enemies = UDEObjectManager.Instance.GetAllEnemies();

            for (int i = 0; i < enemies.Count; i++)
            {
                texts.Add(GetTextFromPool());
            }

            var player = FindObjectOfType<UDEPlayer>();
            if (player.gameObject.activeInHierarchy && player.enabled)
                texts.Add(GetTextFromPool());

            var bullets = UDEObjectManager.Instance.GetAllBullets();

            for (int i = 0; i < bullets.Count; i++)
            {
                texts.Add(GetTextFromPool());
            }
        }

        public void StopShowing()
        {
            transforms.Clear();
            ResetText();
            mode = ShowMode.NONE;
        }
    }
}