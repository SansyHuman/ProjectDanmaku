using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SansyHuman.UDE.Pattern;
using SansyHuman.UDE.Object;
using SansyHuman.UDE.Management;
using SansyHuman.Player;
using SansyHuman.Management;
using SansyHuman.Enemy;

public class GameManager : UDESingleton<GameManager>
{
    public static PlayerBase player;
    public List<UDEBaseStagePattern> stages;

    private int currentStage;
    private bool stageRunning = false;
    
    private AudioSource bgm;

    protected override void Awake()
    {
        base.Awake();
        bgm = GetComponent<AudioSource>();
    }

    public void StartStage(int stageNumber)
    {
        if (stageRunning)
        {
            Debug.LogError("The stage is already running.");
            return;
        }

        GameManager.player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBase>();
        GameManager.player.transform.position = new Vector2(-6, 0);

        currentStage = stageNumber;
        stageRunning = true;
        StartCoroutine(RunStage(stageNumber));
    }

    public void PauseBGM()
    {
        bgm.Pause();
    }

    public void ResumeBGM()
    {
        bgm.Play();
    }

    public void StopStage()
    {
        var enemies = UDEObjectManager.Instance.GetAllEnemies();
        for (int i = 0; i < enemies.Count; i++)
        {
            Destroy(enemies[i].gameObject);
        }

        UDEObjectManager.Instance.DestroyAllBullets(true);
        ItemManager.Instance.DestroyAllItems();
        if (UDETime.Instance.Paused)
            UDETime.Instance.ResumeGame();
        UDETime.Instance.EnemyTimeScale = 1;
        UDETime.Instance.PlayerTimeScale = 1;

        bgm.Stop();

        StopAllCoroutines();
        stageRunning = false;
        stages[currentStage].PauseStage();
        stages[currentStage].ResetStage();
    }

    IEnumerator RunStage(int stageNumber)
    {
        yield return new WaitForSeconds(1f);
        bgm.time = 0;
        bgm.Play();
        stages[stageNumber].StartStage();
    }
}
