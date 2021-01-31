using System.Collections;
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
    public Transform BackgroundStartPoint;
    public Transform BackgroundEndPoint;

    public AudioClip[] ErrorSounds;
    public int LastError = -1;
    
    public Transform EnemyTransform;
    public ParticleSystem EnemyHitParticle;
    
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
        Data = MainController.Instance.StageDatas[MainController.Instance.CompletedLevel];
        CreateRune();
        _changeScene = GetComponent<ChangeScene>();

        Background.transform.position = BackgroundStartPoint.position;
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
        }
        
        var runeOption = Data.Options[LastRune];
        LastRune++;

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

    public IEnumerator FinishSequence()
    {
        yield return new WaitForSeconds(1f);
//        MainController.Instance.CompletedLevel++;

        EnemyTransform.DOKill();
        
        PlayerPentagram.gameObject.SetActive(false);
        EnemyPentagram.gameObject.SetActive(false);

        var seq = DOTween.Sequence();
        var pos = new Vector3(0, 1, 0);
        seq.Append(EnemyTransform.DOMove(pos * 2f, 2f).SetEase(Ease.OutCubic));
        seq.Append(EnemyTransform.DOShakePosition(6f, Vector3.one * 0.3f));
        seq.Play();
        
        yield return new WaitForSeconds(3f);
        
        // final multiple particle play
        
        yield return new WaitForSeconds(3f);
        
        // final multiple particle stop
        // final explosion particle play
        EnemyTransform.DOKill();
        EnemyTransform.gameObject.SetActive(false);
        
        yield return new WaitForSeconds(3f);

        _changeScene.LoadScene("congrats_scene");
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
}
