using System.Collections;
using System.Collections.Generic;

using SansyHuman.Save;

using UnityEngine;

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
}
