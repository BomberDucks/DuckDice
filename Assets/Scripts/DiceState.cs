using UnityEngine;

[System.Serializable]
public class DiceState
{
    public int TopFace = 1;      // La face du dessus (celle qui compte pour la victoire)
    public int ForwardFace = 2;  // La face qui regarde vers le "Nord" (Haut de l'écran)

    // Constructeur
    public DiceState(int top, int forward)
    {
        TopFace = top;
        ForwardFace = forward;
    }

    // Fonction principale appelée quand on pousse le dé
    public void Roll(Vector2Int direction)
    {
        int oldTop = TopFace;
        int oldForward = ForwardFace;
        int oldRight = GetRightFace(oldTop, oldForward); // On calcule la face droite actuelle

        // --- LOGIQUE DE ROTATION ---
        if (direction == Vector2Int.up) // Vers le NORD
        {
            TopFace = 7 - oldForward; // Le Sud devient le dessus
            ForwardFace = oldTop;     // L'ancien dessus devient le Nord
        }
        else if (direction == Vector2Int.down) // Vers le SUD
        {
            TopFace = oldForward;     // Le Nord devient le dessus
            ForwardFace = 7 - oldTop; // L'ancien dessus devient le Sud (donc le Nord devient le dessous)
        }
        else if (direction == Vector2Int.right) // Vers l'EST
        {
            TopFace = 7 - oldRight;   // La gauche devient le dessus
            // Forward ne change pas quand on roule latéralement
        }
        else if (direction == Vector2Int.left) // Vers l'OUEST
        {
            TopFace = oldRight;       // La droite devient le dessus
            // Forward ne change pas
        }

        // --- DEBUG LOG ---
        // C'est ici que la console vous parlera !
        Debug.Log($"<color=cyan>DICE LOG :</color> Direction {direction} | Nouvelle TOP : <b>{TopFace}</b> (Front: {ForwardFace})");
    }

    // Helper : Calcule la face de droite sur un dé standard
    // Basé sur la règle : Top, Front et Right suivent un ordre cyclique précis
    private int GetRightFace(int top, int front)
    {
        // Table de correspondance pour un dé standard
        // (Combinaisons possibles de Top/Front pour trouver Right)
        if (top == 1) {
            if (front == 2) return 3;
            if (front == 3) return 5;
            if (front == 5) return 4;
            if (front == 4) return 2;
        }
        else if (top == 2) {
            if (front == 1) return 4;
            if (front == 4) return 6;
            if (front == 6) return 3;
            if (front == 3) return 1;
        }
        else if (top == 3) {
            if (front == 1) return 2;
            if (front == 2) return 6;
            if (front == 6) return 5;
            if (front == 5) return 1;
        }
        // ... On peut faire toutes les faces, mais utilisons l'astuce des faces opposées :
        // Si on ne trouve pas direct, on inverse le Top (le dé est symétrique)
        
        // Astuce générique simplifiée pour éviter 24 conditions :
        // Sur un dé standard : faces opposées = 7.
        // On peut déduire les autres cas.
        // Voici une version "Hardcodée" simplifiée qui couvre les cas principaux :
        
        return HardcodedRightFace(top, front);
    }

    private int HardcodedRightFace(int top, int front)
    {
        // Liste brute des combinaisons valides (Top, Front) -> Right
        if (top == 1 && front == 2) return 3;
        if (top == 1 && front == 3) return 5;
        if (top == 1 && front == 4) return 2;
        if (top == 1 && front == 5) return 4;

        if (top == 2 && front == 1) return 4;
        if (top == 2 && front == 3) return 1;
        if (top == 2 && front == 4) return 6;
        if (top == 2 && front == 6) return 3;

        if (top == 3 && front == 1) return 2;
        if (top == 3 && front == 2) return 6;
        if (top == 3 && front == 5) return 1;
        if (top == 3 && front == 6) return 5;

        if (top == 4 && front == 1) return 5;
        if (top == 4 && front == 2) return 1;
        if (top == 4 && front == 5) return 6;
        if (top == 4 && front == 6) return 2;

        if (top == 5 && front == 1) return 3;
        if (top == 5 && front == 3) return 6;
        if (top == 5 && front == 4) return 1;
        if (top == 5 && front == 6) return 4;

        if (top == 6 && front == 2) return 4;
        if (top == 6 && front == 3) return 2;
        if (top == 6 && front == 4) return 5;
        if (top == 6 && front == 5) return 3;

        return 0; // Erreur (configuration impossible)
    }
}