using UnityEngine;

[System.Serializable]
public class DiceState
{
    public int TopFace = 1;      
    public int ForwardFace = 2;  

    // Constructeur
    public DiceState(int top, int forward)
    {
        TopFace = top;
        ForwardFace = forward;
    }

    
    public void Roll(Vector2Int direction)
    {
        int oldTop = TopFace;
        int oldForward = ForwardFace;
        int oldRight = GetRightFace(oldTop, oldForward); 

        
        if (direction == Vector2Int.up) 
        {
            TopFace = 7 - oldForward; 
            ForwardFace = oldTop;     
        }
        else if (direction == Vector2Int.down) 
        {
            TopFace = oldForward;     
            ForwardFace = 7 - oldTop; 
        }
        else if (direction == Vector2Int.right) 
        {
            TopFace = 7 - oldRight;   
            
        }
        else if (direction == Vector2Int.left) 
        {
            TopFace = oldRight;       
            
        }

        
        
        Debug.Log($"<color=cyan>DICE LOG :</color> Direction {direction} | Nouvelle TOP : <b>{TopFace}</b> (Front: {ForwardFace})");
    }

    
    
    private int GetRightFace(int top, int front)
    {
        // Table de correspondance pour un dé standard
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

        return 0; 
    }
}