using UnityEngine;
using System.Collections;

public class DiceBlock : MonoBehaviour
{
    [Header("État Logique")]
    public DiceState CurrentState; 
    
    [Header("Debug")]
    public Vector2Int GridPosition; 
    
    private bool isMoving = false;

    private void Start()
    {
        if (CurrentState == null || CurrentState.TopFace == 0) 
        {
            CurrentState = new DiceState(1, 2); 
        }
        SnapToGrid();
    }

    
    public bool Push(Vector3 direction)
    {
        if (isMoving) return false;

        
        if (Physics.Raycast(transform.position, direction, 1f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            return false;
        }

        
        StartCoroutine(Roll(direction));
        return true;
    }

    
    public void Pull(Vector3 direction)
    {
        if (isMoving) return;
        
        
        StartCoroutine(Slide(direction));
    }

    
    private IEnumerator Roll(Vector3 direction)
    {
        isMoving = true;

        float remainingAngle = 90f;
        float speed = 300f; 

        Vector3 rotationCenter = transform.position + (direction * 0.5f) + (Vector3.down * 0.5f);
        Vector3 rotationAxis = Vector3.Cross(Vector3.up, direction);

        while (remainingAngle > 0)
        {
            float rotationAmount = Mathf.Min(Time.deltaTime * speed, remainingAngle);
            transform.RotateAround(rotationCenter, rotationAxis, rotationAmount);
            remainingAngle -= rotationAmount;
            yield return null;
        }

        // Fin mouvement
        SnapToGrid();
        SnapRotation(); 

        
        UpdateDiceLogic(direction);

        FinishMove();
    }

    
    private IEnumerator Slide(Vector3 direction)
    {
        isMoving = true;

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + direction;
        float elapsedTime = 0;
        float slideSpeed = 5f; 

        while (elapsedTime < (1f / slideSpeed))
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime * slideSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        
        transform.position = targetPos;
        SnapToGrid();

        

        FinishMove();
    }

    
    private void FinishMove()
    {
        
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.RefreshGame();
        }
        isMoving = false;
    }

    

    private void SnapToGrid()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Round(pos.x);
        pos.z = Mathf.Round(pos.z);
        pos.y = 0.5f; 
        transform.position = pos;
        GridPosition = new Vector2Int((int)pos.x, (int)pos.z);
    }

    private void SnapRotation()
    {
        Vector3 euler = transform.rotation.eulerAngles;
        euler.x = Mathf.Round(euler.x / 90) * 90;
        euler.y = Mathf.Round(euler.y / 90) * 90;
        euler.z = Mathf.Round(euler.z / 90) * 90;
        transform.rotation = Quaternion.Euler(euler);
    }

    private void UpdateDiceLogic(Vector3 dir3D)
    {
        Vector2Int logicDir = Vector2Int.zero;
        if (dir3D == Vector3.forward) logicDir = Vector2Int.up;
        else if (dir3D == Vector3.back) logicDir = Vector2Int.down;
        else if (dir3D == Vector3.right) logicDir = Vector2Int.right;
        else if (dir3D == Vector3.left) logicDir = Vector2Int.left;

        CurrentState.Roll(logicDir);
    }
}