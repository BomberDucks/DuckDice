using UnityEngine;

[CreateAssetMenu(fileName = "NewLevel", menuName = "Sokoban/LevelData")]
public class LevelData : ScriptableObject
{
    public string LevelName;
    
    [Header("Structure du Niveau")]
    [Tooltip("Utilisez : # pour Mur, . pour Sol, P pour Player, D pour Dé, T pour Target")]
    [TextArea(10, 20)] 
    public string LevelString;

    [Header("Paramètres")]
    public int Width;
    public int Height;
    public int OptimalMoves; 
}
