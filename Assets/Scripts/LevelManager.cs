using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; //  pour .All()

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Niveaux")]
    public List<LevelData> Levels;
    public int CurrentLevelIndex = 0;

    [Header("Prefabs")]
    public GameObject WallPrefab;
    public GameObject FloorPrefab;
    public GameObject PlayerPrefab;
    public GameObject DicePrefab;
    public GameObject TargetPrefab; 
    
    private GameObject levelContainer;
    private List<TargetZone> activeTargets = new List<TargetZone>();
    private bool isLevelLoading = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (Levels.Count > 0) LoadLevel(CurrentLevelIndex);
    }

    private void Update()
    {
        // Touches de debug pour test
        if (Input.GetKeyDown(KeyCode.R)) LoadLevel(CurrentLevelIndex);
        if (Input.GetKeyDown(KeyCode.N)) NextLevel();
        if (Input.GetKeyDown(KeyCode.P)) PreviousLevel();
    }

    

    // on apelle a chaque fin de mouvement
    public void RefreshGame()
    {
        if (isLevelLoading) return;

        
        foreach (var target in activeTargets)
        {
            CheckSingleTarget(target);
        }

        // vérifie si toutes les targets sont ok
        CheckGlobalVictory();
    }

    private void CheckSingleTarget(TargetZone target)
    {
       //collider du de
        Vector3 center = target.transform.position + Vector3.up * 0.5f;

        // overlap de detection
        Collider[] hits = Physics.OverlapBox(center, Vector3.one * 0.4f);
        
        bool isTargetCorrect = false;

        foreach (var hit in hits)
        {
            DiceBlock dice = hit.GetComponent<DiceBlock>();
            
            if (dice != null)
            {
                // de detecte verifie la face
                if (dice.CurrentState.TopFace == target.RequiredNumber)
                {
                    isTargetCorrect = true;
                    // joue le son ok
                }
                break;
            }
        }

        // actualisation du visuel
        target.SetSolved(isTargetCorrect);
    }

    private void CheckGlobalVictory()
    {
        if (activeTargets.Count == 0) return;

        
        if (activeTargets.All(t => t.IsSolved))
        {
            Debug.Log("VICTOIRE TOTALE !");
            StartCoroutine(VictorySequence());
        }
    }

    private IEnumerator VictorySequence()
    {
        isLevelLoading = true;
       
        yield return new WaitForSeconds(1.0f); 
        NextLevel();
        isLevelLoading = false;
    }
    

    public void NextLevel()
    {
        int nextIndex = (CurrentLevelIndex + 1) % Levels.Count;
        LoadLevel(nextIndex);
    }

    public void PreviousLevel()
    {
        int prevIndex = CurrentLevelIndex - 1;
        if (prevIndex < 0) prevIndex = Levels.Count - 1;
        LoadLevel(prevIndex);
    }

    

    public void LoadLevel(int index)
    {
        if (index < 0 || index >= Levels.Count) return;
        CurrentLevelIndex = index;

        
        if (levelContainer != null) Destroy(levelContainer);
        levelContainer = new GameObject("LevelContainer");
        activeTargets.Clear();

        LevelData data = Levels[CurrentLevelIndex];

        
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.InitScore(data.OptimalMoves);
        }

        string[] lines = data.LevelString.Trim().Split('\n');

        
        for (int y = 0; y < lines.Length; y++)
        {
            string line = lines[y].Trim();
            for (int x = 0; x < line.Length; x++)
            {
                char tileChar = line[x];
                //lines.Length - 1 - y permet de construire dans le bon sens 
                Vector3 pos = new Vector3(x, 0, lines.Length - 1 - y);
                SpawnTile(tileChar, pos);
            }
        }
        
        
        StartCoroutine(InitialCheckDelay());
    }

    private IEnumerator InitialCheckDelay()
    {
        yield return null; 
        RefreshGame();
    }

    private void SpawnTile(char type, Vector3 pos)
    {
        
        if (type != '#') Instantiate(FloorPrefab, pos, Quaternion.identity, levelContainer.transform);

        switch (type)
        {
            case '#': // Mur
                Instantiate(WallPrefab, pos + Vector3.up * 0.5f, Quaternion.identity, levelContainer.transform); 
                break;
            case 'P': // Player
                Instantiate(PlayerPrefab, pos + Vector3.up * 0.5f, Quaternion.identity, levelContainer.transform); 
                break;
            case 'D': // Dé
                Instantiate(DicePrefab, pos + Vector3.up * 0.5f, Quaternion.identity, levelContainer.transform); 
                break;

            // face du de
            case '1': case '2': case '3': case '4': case '5': case '6':
                
                GameObject tObj = Instantiate(TargetPrefab, pos + new Vector3(0, 0.01f, 0), Quaternion.identity, levelContainer.transform);
                TargetZone zone = tObj.GetComponent<TargetZone>();
                
                if (zone != null)
                {
                    int number = type - '0'; // Convertit le char '3' en int 3
                    zone.Init(number);
                    activeTargets.Add(zone);
                }
                else
                {
                    Debug.LogError("Le Prefab Target n'a pas de script TargetZone !");
                }
                break;
        }
    }
}