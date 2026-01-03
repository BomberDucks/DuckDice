using UnityEngine;
using TMPro; 

public class ScoreManager : MonoBehaviour
{
    
    public static ScoreManager Instance;

    [Header("Interface (UI)")]
    public TextMeshProUGUI ScoreText; 

    private int currentMoves = 0;
    private int optimalMovesForThisLevel = 0;

    private void Awake()
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

    
    public void InitScore(int optimalMoves)
    {
        optimalMovesForThisLevel = optimalMoves;
        currentMoves = 0; 
        UpdateUI();
    }

    
    public void AddMove()
    {
        currentMoves++;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (ScoreText != null)
        {
            
            string colorHex = (currentMoves <= optimalMovesForThisLevel) ? "blue" : "red";
            
            
            ScoreText.text = $"Mouvements:<color={colorHex}> {currentMoves} / {optimalMovesForThisLevel}</color>";
        }
    }
}