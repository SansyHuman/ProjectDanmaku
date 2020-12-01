using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SansyHuman.UDE.Pattern;
using SansyHuman.UDE.Object;
using SansyHuman.UDE.Management;

public class GameManager : UDESingleton<GameManager>
{
    public static GameObject player;
    public List<UDEBaseStagePattern> stages;
    
    private AudioSource bgm;

    protected override void Awake()
    {
        base.Awake();

        GameManager.player = GameObject.FindGameObjectWithTag("Player");
        GameManager.player.transform.position = new Vector2(-6, 0);
    }
    // Start is called before the first frame update
    void Start()
    {
        bgm = GetComponent<AudioSource>();
        StartCoroutine(StartCorou());
    }

    public void PauseBGM()
    {
        bgm.Pause();
    }

    public void ResumeBGM()
    {
        bgm.Play();
    }

    IEnumerator StartCorou()
    {
        bgm.Play();
        yield return new WaitForSeconds(3f);
        stages[0].StartStage();
    }
}
