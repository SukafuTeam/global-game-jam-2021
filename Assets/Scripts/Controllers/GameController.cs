using System.Collections;
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
    }
    
    public void CreateRune()
    {
        if (LastRune == Data.Options.Length)
        {
            StartCoroutine(FinishSequence());
            return;
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
        MainController.Instance.CompletedLevel++;
        _changeScene.LoadScene("congrats_scene");
    }
}
