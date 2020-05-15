using UnityEngine;
using System.Collections;

public class FPS : MonoBehaviour
{
    float deltaTime = 0.0f;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.03f;
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(w * 0.05f, h * 0.03f, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 5 / 100;
        style.normal.textColor = Color.white;
        float msec = deltaTime * 1000.0f;
        float averageFps = 1.0f / deltaTime;
        string text = string.Format("{0,-4:0.0} ms avg ({1,-5:0.} fps avg)\n({2,-5:0.} fps)", msec, averageFps, 1.0f / Time.deltaTime);
        GUI.Label(rect, text, style);

        GUIStyle style2 = GUI.skin.label;
        style2.alignment = TextAnchor.UpperRight;
        style2.fontSize = h * 3 / 100;
        style2.normal.textColor = Color.white;
        string version = Application.version;
        GUI.Label(new Rect(Screen.width * 0.98f - style2.fontSize * version.Length, Screen.height * 0.025f, style2.fontSize * version.Length, 100), version);
    }
}