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

    protected override void Awake()
    {
        base.Awake();

        GameManager.player = GameObject.FindGameObjectWithTag("Player");
        GameManager.player.transform.position = new Vector2(0, -4);
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartCorou());
    }

    IEnumerator StartCorou()
    {
        AudioSource bgm = GetComponent<AudioSource>();
        bgm.Play();
        yield return new WaitForSeconds(3f);
        stages[0].StartStage();
    }
}
