using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SansyHuman.UDE.Management;

public class BackgroundScroll : MonoBehaviour
{
    public enum ScrollDirection
    {
        X, Y
    }

    [SerializeField] private float scrollSpeed = 1.5f;
    [SerializeField] private ScrollDirection scrollDirection = ScrollDirection.Y;
    [SerializeField] private UDETime.TimeScale timeScale = UDETime.TimeScale.UNSCALED;

    private Material mat;

    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float scaledDeltaTime = Time.deltaTime * UDETime.Instance.GetTimeScale(timeScale);
        Vector2 newOffset = mat.mainTextureOffset;
        switch (scrollDirection)
        {
            case ScrollDirection.X:
                newOffset.x += scrollSpeed * scaledDeltaTime;
                break;
            case ScrollDirection.Y:
                newOffset.y += scrollSpeed * scaledDeltaTime;
                break;
        }

        mat.mainTextureOffset = newOffset;
    }
}
