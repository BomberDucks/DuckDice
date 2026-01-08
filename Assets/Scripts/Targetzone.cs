using UnityEngine;
using TMPro;

public class TargetZone : MonoBehaviour
{
    public int RequiredNumber;
    
    // Cette variable est lue par le LevelManager pour savoir si c'est gagné
    public bool IsSolved = false; 

    [Header("Visuel")]
    public TextMeshPro NumberLabel;
    public Renderer ZoneRenderer;

    public void Init(int number)
    {
        RequiredNumber = number;
        IsSolved = false; // Reset au départ

        if (NumberLabel != null) 
            NumberLabel.text = RequiredNumber.ToString();
        
        UpdateColor();
    }

    // Appelée par le LevelManager lors du scan
    public void SetSolved(bool state)
    {
        
        if (IsSolved != state)
        {
            IsSolved = state;
            UpdateColor();

            
            if (IsSolved)
            {
                Debug.Log($"<color=green>Cible {RequiredNumber} : C'EST BON ! (Validée)</color>");
            }
            else
            {
                Debug.Log($"<color=red>Cible {RequiredNumber} : PAS BON... (Invalide)</color>");
            }
        }
    }

    private void UpdateColor()
    {
        if (ZoneRenderer != null)
        {
            ZoneRenderer.material.color = IsSolved ? Color.green : Color.red;
        }
    }
}