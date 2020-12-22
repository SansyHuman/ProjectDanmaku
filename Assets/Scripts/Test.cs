using System.Collections;
using System.Collections.Generic;

using SansyHuman.Save;

using UnityEngine;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour
{
    [SerializeField]
    private StoryEntry[] entries;

    // Start is called before the first frame update
    void Start()
    {
        SaveBinaryCreator.SaveStoryEntries(entries);
        var loadedEntries = SaveBinaryCreator.LoadStoryEntries();

        foreach(var e in loadedEntries)
        {
            Debug.Log(e.ToString());
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            I2.Loc.LocalizationManager.CurrentLanguage = "English";
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            I2.Loc.LocalizationManager.CurrentLanguage = "Korean";
        }
    }
}
