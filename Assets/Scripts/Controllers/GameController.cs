﻿using System.Collections;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    
    private ChangeScene _changeScene;
    
    public PentagramController PlayerPentagram;
    public PentagramController EnemyPentagram;
    
    public GameObject RunePrefab;
    public int LastRune;
    
    public StageData Data;

    public GameObject Background;
    public GameObject Midground;
    public GameObject Foreground;
    public Transform BackgroundStartPoint;
    public Transform BackgroundEndPoint;

    public AudioClip[] ErrorSounds;
    public int LastError = -1;
    
    public TutorialController Tutorial;

    public Camera cam;
    
    public Transform PlayerTransform;
    public Transform EnemyTransform;
    public ParticleSystem EnemyHitParticle;
    public ParticleSystem EnemyFinalParticle;
    public ParticleSystem EnemyOrbParticle;
    public GameObject Soul;
    public CinemachineDollyCart SoulGuider;
    public GameObject FinalSoulAnimation;

    public int RightNotes;
    public int Lives;
    public GameObject LiveParent;
    public SpriteRenderer[] LivesIndicators;
    
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void Start()
    {
        SoundController.Instance.BmgSource.Pause();
        
        Data = MainController.Instance.StageDatas[MainController.Instance.CompletedLevel];
//        CreateRune();
        _changeScene = GetComponent<ChangeScene>();

        var main = MainController.Instance;
        Instantiate(main.Enemies[main.CompletedLevel], EnemyTransform.position, Quaternion.identity, EnemyTransform);
        
        Background.transform.position = BackgroundStartPoint.position;
        Midground.transform.position = BackgroundStartPoint.position;
        Foreground.transform.position = BackgroundStartPoint.position;

        cam.transform.position = new Vector3(0, -18.5f, -10);
        cam.transform.DOMove(new Vector3(0, 0, -10), 3f)
            .SetEase(Ease.InOutCubic)
            .OnComplete(() => CreateRune());
        
        for (var i = 0; i < LivesIndicators.Length; i++)
        {
            var life = LivesIndicators[i];
            var active = Lives > i;

            var dance = life.GetComponent<MusicScaler>();
            dance.enabled = active;

            life.color = active ? Color.white : Color.grey;
        }
    }
    

    public void CreateRune()
    {
        if (LastRune == Data.Options.Length)
        {
            StartCoroutine(FinishSequence());
            return;
        }

        if (LastRune == 0)
        {
            var time = Data.Options.Length * (PlayerPentagram.NDivisions / 2 + 1) * Conductor.Instance.secPerBeat + (Data.Options.Length * 2.2f);
            Background.transform.DOMove(BackgroundEndPoint.position, time).SetEase(Ease.InOutQuad);
            Midground.transform.DOMove(BackgroundEndPoint.position, time * 0.95f).SetEase(Ease.InOutQuad);
            Foreground.transform.DOMove(BackgroundEndPoint.position, time * 0.9f).SetEase(Ease.InOutQuad);

        }
        
        if (MainController.Instance.CompletedLevel == 0)
            UpdateTutorial(LastRune);
        
        var runeOption = Data.Options[LastRune];
        LastRune++;
        
        if (LastRune == Data.Music3index && Conductor.Instance.ActiveMusic < 3)
        {
            Conductor.Instance.ActiveMusic = 3;
            StartCoroutine(Conductor.Instance.ChangeMusic(3));
        }
        
        if (LastRune == Data.Music2index && Conductor.Instance.ActiveMusic < 2)
        {
            Conductor.Instance.ActiveMusic = 2;
            StartCoroutine(Conductor.Instance.ChangeMusic(2));
        }

        var obj = Instantiate(RunePrefab, new Vector3(0, 6, 0), Quaternion.identity);
        var rune = obj.GetComponent<RuneController>();
        if (rune == null) {
            Debug.LogError("RunePrefab is invalid. It should have a 'RuneController' component");
            return;
        }

        rune.Pentagram = runeOption.Owner == PentagramOwner.Player ? PlayerPentagram : EnemyPentagram;
        rune.IsEnemy = runeOption.Owner != PentagramOwner.Player;
        rune.Data = runeOption.Data;

        var spriteRenderer = obj.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("RunePrefab is invalid. It should have a 'SpriteRenderer' component");
            return;
        }

        spriteRenderer.sprite = rune.Data.Image;
        
        if(runeOption.Owner != PentagramOwner.Both)
            return;
        
        obj = Instantiate(RunePrefab, new Vector3(0, 6, 0), Quaternion.identity);
        rune = obj.GetComponent<RuneController>();
        if (rune == null) {
            Debug.LogError("RunePrefab is invalid. It should have a 'RuneController' component");
            return;
        }

        rune.Pentagram = PlayerPentagram;
        rune.Data = runeOption.PlayerData == null ? runeOption.Data : runeOption.PlayerData;
        rune.PreventNext = true;
        
        spriteRenderer = obj.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("RunePrefab is invalid. It should have a 'SpriteRenderer' component");
            return;
        }

        spriteRenderer.sprite = rune.Data.Image;
    }

    public void UpdateTutorial(int rune)
    {
        switch (rune)
        {
            case 0:
                Tutorial.SetTutorial(1);
                break;
            case 1:
                Tutorial.SetTutorial(2);
                break;
            case 2:
                Tutorial.SetTutorial(1);
                break;
            case 3:
                Tutorial.SetTutorial(2);
                break;
            case 4:
                Tutorial.SetTutorial(3);
                break;
            case 5:
                Tutorial.SetTutorial(4);
                break;
            default:
                break;
        }
    }
    
    public IEnumerator FinishSequence()
    {
        yield return new WaitForSeconds(1f);
        MainController.Instance.CompletedLevel++;

        EnemyTransform.DOKill();
        
        PlayerPentagram.gameObject.SetActive(false);
        EnemyPentagram.gameObject.SetActive(false);
        LiveParent.SetActive(false);

        var seq = DOTween.Sequence();
        var pos = new Vector3(0, 1.5f, 0);
        seq.Append(EnemyTransform.DOMove(pos, 2f).SetEase(Ease.OutCubic));
        seq.Append(EnemyTransform.DOShakePosition(4f, Vector3.one * 0.3f));
        seq.Play();

        yield return new WaitForSeconds(1f);
        
        EnemyFinalParticle.Play();

        yield return new WaitForSeconds(3f);
        
        StartCoroutine(Conductor.Instance.AllMusic());
        EnemyFinalParticle.Stop();
        EnemyOrbParticle.transform.position = EnemyTransform.position;
        EnemyOrbParticle.Play();
        EnemyTransform.DOKill();

        EnemyTransform.gameObject.SetActive(false);
        
        Soul.SetActive(true);
        
        DOTween.To(() => SoulGuider.m_Position, x => SoulGuider.m_Position = x, 1, 15f)
            .SetEase(Ease.InOutCubic);
        
        yield return new WaitForSeconds(13f);
        
        var lastStage = MainController.Instance.CompletedLevel == MainController.Instance.StageDatas.Length;
        if (!lastStage)
        {
            Conductor.Instance.musicSource1.DOFade(0, 2f);
            Conductor.Instance.musicSource2.DOFade(0, 2f);
            Conductor.Instance.musicSource3.DOFade(0, 2f);
            
            yield return new WaitForSeconds(2f);
            
            SoundController.Instance.BmgSource.Play();
            _changeScene.LoadScene("waiting_room");
        }
        else
        {
            yield return new WaitForSeconds(2f);

            PlayerTransform.DOKill();
            
            var seq1 = DOTween.Sequence();
            seq1.Append(PlayerTransform.DOMove(pos, 2f).SetEase(Ease.OutCubic));
            seq1.Append(PlayerTransform.DOShakePosition(4f, Vector3.one * 0.3f));
            seq1.Play();

            yield return new WaitForSeconds(4f);
        
            EnemyOrbParticle.Play();
            
            PlayerTransform.gameObject.SetActive(false);

            Instantiate(FinalSoulAnimation, Vector3.zero, Quaternion.identity);
            
            yield return new WaitForSeconds(15f);
            
            Conductor.Instance.musicSource1.DOFade(0, 2f);
            Conductor.Instance.musicSource2.DOFade(0, 2f);
            Conductor.Instance.musicSource3.DOFade(0, 2f);
            
            yield return new WaitForSeconds(2f);

            MainController.Instance.CompletedLevel = 0;
            SoundController.Instance.BmgSource.Play();
            _changeScene.LoadScene("menu");
        }
        
        
    }

    public AudioClip GetErrorSound()
    {
        var index = Random.Range(0, ErrorSounds.Length);
        
        while (index == LastError) 
            index = Random.Range(0, ErrorSounds.Length);

        LastError = index;
        return ErrorSounds[index];
    }

    public void HitEnemy(Color color)
    {
        var main = EnemyHitParticle.main;
        main.startColor = color;

        EnemyTransform.DOShakePosition(0.5f, Vector3.one * 0.3f);
        
        EnemyHitParticle.Play();
    }

    public void UpdateLives(bool correct)
    {
        var shouldUpdate = false;

        if (correct)
        {
            if (Lives == 5)
            {
                RightNotes = 0;
                return;
            }

            RightNotes++;

            if (RightNotes == 6)
            {
                Lives++;
                RightNotes = 0;
                shouldUpdate = true;
            }
        }
        else
        {
            Lives--;
            shouldUpdate = true;

            cam.transform.DOShakePosition(0.2f, Vector3.one * 0.3f);

            if (Lives == 0)
            {
                SoundController.Instance.BmgSource.Play();
                _changeScene.LoadScene("waiting_room");
            }
        }
        
        if(!shouldUpdate)
            return;

        for (var i = 0; i < LivesIndicators.Length; i++)
        {
            var life = LivesIndicators[i];
            var active = Lives > i;

            var dance = life.GetComponent<MusicScaler>();
            dance.enabled = active;

            life.color = active ? Color.white : Color.grey;
        }
    }
}
