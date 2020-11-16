using System.Collections;
using System.Collections.Generic;

using SansyHuman.Debugging;
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Object;

using UnityEngine;

namespace SansyHuman.Debugging
{
    [DisallowMultipleComponent]
    public class GLDraw : MonoBehaviour
    {
        public enum RenderMode
        {
            ENEMY,
            PLAYER,
            BULLET,
            ALL,
            ID,
            NONE
        }

        private enum EntityKind
        {
            ENEMY,
            PLAYER,
            BULLET,
            NONE
        }

        public enum DrawMode
        {
            QUADS = 7,
            LINES = 1
        }

        private struct ClassifiedCollider
        {
            public EntityKind kind;
            public Collider2D collider;
        }

        [SerializeField]
        private Material renderMaterial;

        [SerializeField]
        private DrawMode drawMode = DrawMode.QUADS;

        [SerializeField]
        private Color enemyColor;

        [SerializeField]
        private Color playerColor;

        [SerializeField]
        private Color bulletColor;

        [SerializeField]
        private Color defaultColor;

        private RenderMode renderMode;
        private List<ClassifiedCollider> idColliders;

        public DrawMode DrawingMode
        {
            get => drawMode;
            set => drawMode = value;
        }

        private void Awake()
        {
            renderMode = RenderMode.NONE;
            idColliders = new List<ClassifiedCollider>();
        }

        private void LateUpdate()
        {
            if (renderMode == RenderMode.ID)
            {
                for (int i = 0; i < idColliders.Count; i++)
                {
                    if (idColliders[i].collider == null || !idColliders[i].collider.gameObject.activeInHierarchy || !idColliders[i].collider.enabled)
                    {
                        idColliders.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        private void OnPostRender()
        {
            if (renderMode == RenderMode.NONE)
                return;

            renderMaterial.SetPass(0);

            GL.Begin((int)drawMode);

            switch (renderMode)
            {
                case RenderMode.ENEMY:
                    DrawEnemies();
                    break;
                case RenderMode.PLAYER:
                    DrawPlayer();
                    break;
                case RenderMode.BULLET:
                    DrawBullets();
                    break;
                case RenderMode.ALL:
                    DrawEnemies();
                    DrawPlayer();
                    DrawBullets();
                    break;
                case RenderMode.ID:
                    DrawInstanceIDs();
                    break;
                case RenderMode.NONE:
                default:
                    break;
            }

            GL.End();
        }

        private void DrawEnemies()
        {
            GL.Color(enemyColor);

            var enemies = UDEObjectManager.Instance.GetAllEnemies();

            for (int i = 0; i < enemies.Count; i++)
            {
                Collider2D collider = enemies[i].GetComponent<Collider2D>();
                if (collider == null || !collider.isActiveAndEnabled)
                    continue;

                Bounds colliderBounds = collider.bounds;
                Vector3 min = colliderBounds.min;
                Vector3 max = colliderBounds.max;

                GL.Vertex3(min.x, min.y, 0);
                GL.Vertex3(min.x, max.y, 0);
                if (drawMode == DrawMode.LINES)
                    GL.Vertex3(min.x, max.y, 0);
                GL.Vertex3(max.x, max.y, 0);
                if (drawMode == DrawMode.LINES)
                    GL.Vertex3(max.x, max.y, 0);
                GL.Vertex3(max.x, min.y, 0);
                if (drawMode == DrawMode.LINES)
                {
                    GL.Vertex3(max.x, min.y, 0);
                    GL.Vertex3(min.x, min.y, 0);
                }
            }
        }

        private void DrawPlayer()
        {
            GL.Color(playerColor);

            var player = FindObjectOfType<UDEPlayer>();
            if (player == null || !player.isActiveAndEnabled)
                return;

            Collider2D collider = player.gameObject.GetComponent<Collider2D>();
            if (collider == null || !collider.isActiveAndEnabled)
                return;

            Bounds colliderBounds = collider.bounds;
            Vector3 min = colliderBounds.min;
            Vector3 max = colliderBounds.max;

            GL.Vertex3(min.x, min.y, 0);
            GL.Vertex3(min.x, max.y, 0);
            if (drawMode == DrawMode.LINES)
                GL.Vertex3(min.x, max.y, 0);
            GL.Vertex3(max.x, max.y, 0);
            if (drawMode == DrawMode.LINES)
                GL.Vertex3(max.x, max.y, 0);
            GL.Vertex3(max.x, min.y, 0);
            if (drawMode == DrawMode.LINES)
            {
                GL.Vertex3(max.x, min.y, 0);
                GL.Vertex3(min.x, min.y, 0);
            }
        }

        private void DrawBullets()
        {
            GL.Color(bulletColor);

            var bullets = UDEObjectManager.Instance.GetAllBullets();

            for (int i = 0; i < bullets.Count; i++)
            {
                Collider2D collider = bullets[i].GetComponent<Collider2D>();
                if (collider == null || !collider.isActiveAndEnabled)
                    continue;

                Bounds colliderBounds = collider.bounds;
                Vector3 min = colliderBounds.min;
                Vector3 max = colliderBounds.max;

                GL.Vertex3(min.x, min.y, 0);
                GL.Vertex3(min.x, max.y, 0);
                if (drawMode == DrawMode.LINES)
                    GL.Vertex3(min.x, max.y, 0);
                GL.Vertex3(max.x, max.y, 0);
                if (drawMode == DrawMode.LINES)
                    GL.Vertex3(max.x, max.y, 0);
                GL.Vertex3(max.x, min.y, 0);
                if (drawMode == DrawMode.LINES)
                {
                    GL.Vertex3(max.x, min.y, 0);
                    GL.Vertex3(min.x, min.y, 0);
                }
            }
        }

        private void DrawInstanceIDs()
        {
            for (int i = 0; i < idColliders.Count; i++)
            {
                Color colliderColor = new Color();

                switch (idColliders[i].kind)
                {
                    case EntityKind.ENEMY:
                        colliderColor = enemyColor;
                        break;
                    case EntityKind.PLAYER:
                        colliderColor = playerColor;
                        break;
                    case EntityKind.BULLET:
                        colliderColor = bulletColor;
                        break;
                    case EntityKind.NONE:
                        colliderColor = defaultColor;
                        break;
                }

                GL.Color(colliderColor);

                Bounds colliderBounds = idColliders[i].collider.bounds;
                Vector3 min = colliderBounds.min;
                Vector3 max = colliderBounds.max;

                GL.Vertex3(min.x, min.y, 0);
                GL.Vertex3(min.x, max.y, 0);
                if (drawMode == DrawMode.LINES)
                    GL.Vertex3(min.x, max.y, 0);
                GL.Vertex3(max.x, max.y, 0);
                if (drawMode == DrawMode.LINES)
                    GL.Vertex3(max.x, max.y, 0);
                GL.Vertex3(max.x, min.y, 0);
                if (drawMode == DrawMode.LINES)
                {
                    GL.Vertex3(max.x, min.y, 0);
                    GL.Vertex3(min.x, min.y, 0);
                }
            }
        }

        public bool[] RenderInstanceID(int[] ids)
        {
            idColliders.Clear();

            renderMode = RenderMode.ID;
            bool[] exists = new bool[ids.Length];

            for (int i = 0; i < ids.Length; i++)
            {
                GameObject instance = DebugUtil.FindObjectFromInstanceID<GameObject>(ids[i]);
                if (instance == null || !instance.activeInHierarchy)
                {
                    exists[i] = false;
                    continue;
                }

                Collider2D collider = instance.GetComponent<Collider2D>();
                if (collider == null || !collider.enabled)
                {
                    exists[i] = false;
                    continue;
                }

                UDEEnemy enemy = instance.GetComponent<UDEEnemy>();
                UDEPlayer player = instance.GetComponent<UDEPlayer>();
                UDEAbstractBullet bullet = instance.GetComponent<UDEAbstractBullet>();

                ClassifiedCollider clsCollider = new ClassifiedCollider() { kind = EntityKind.NONE, collider = collider };
                if (enemy != null)
                    clsCollider.kind = EntityKind.ENEMY;
                if (player != null)
                    clsCollider.kind = EntityKind.PLAYER;
                if (bullet != null)
                    clsCollider.kind = EntityKind.BULLET;

                idColliders.Add(clsCollider);
                exists[i] = true;
            }

            return exists;
        }

        public void RenderEnemies()
        {
            idColliders.Clear();
            renderMode = RenderMode.ENEMY;
        }

        public void RenderPlayer()
        {
            idColliders.Clear();
            renderMode = RenderMode.PLAYER;
        }

        public void RenderBullets()
        {
            idColliders.Clear();
            renderMode = RenderMode.BULLET;
        }

        public void RenderAll()
        {
            idColliders.Clear();
            renderMode = RenderMode.ALL;
        }

        public void StopRendering()
        {
            idColliders.Clear();
            renderMode = RenderMode.NONE;
        }
    }
}