using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    [Header("Mouvement")]
    public float MoveSpeed = 5f;

    [Header("Visuel")]
    public Transform ModelToRotate;
    public float RotationSpeed = 15f;

    [Header("Audio - Clips")]
    public AudioClip MoveSound;
    public AudioClip PushSound;
    public AudioClip PullSound;
    public AudioClip BumpSound;

    [Header("Audio - Volume")]
    [Range(0f, 1f)] public float MoveVolume = 0.4f; 
    [Range(0f, 1f)] public float PushVolume = 0.8f;
    [Range(0f, 1f)] public float PullVolume = 0.8f;
    [Range(0f, 1f)] public float BumpVolume = 0.6f;
    
    private AudioSource audioSource;

    [Header("Détection")]
    public LayerMask ObstacleLayer;
    public float DetectionDistance = 1.0f;
    public float DetectionHeightOffset = 0.0f;

    private bool isMoving = false;
    private Quaternion targetRotation;

    // Gizmos utile pour verifier les hitboxs
    private Vector3 debugBoxCenter;
    private bool hasDebugInfo = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (ModelToRotate != null) targetRotation = ModelToRotate.rotation;
    }

    private void Update()
    //code du mouvement du canard
    {
        if (ModelToRotate != null)
        {
            ModelToRotate.rotation = Quaternion.Slerp(ModelToRotate.rotation, targetRotation, Time.deltaTime * RotationSpeed);
        }

        if (isMoving) return;

        Vector3 direction = Vector3.zero;
        

        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");

        if (vertical > 0) direction = Vector3.forward;      
        else if (vertical < 0) direction = Vector3.back;    
        else if (horizontal < 0) direction = Vector3.left; 
        else if (horizontal > 0) direction = Vector3.right; 

        if (direction != Vector3.zero)
        {
            //incremente mon scoremanager
            ScoreManager.Instance.AddMove();
            
            if (ModelToRotate != null) targetRotation = Quaternion.LookRotation(direction);

            bool isPulling = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            if (isPulling) AttemptPull(direction);
            else AttemptPushOrMove(direction);
        }
    }

    private void AttemptPull(Vector3 moveDirection) //code pour tirer le de avec glissement
    {
        Vector3 checkPosBehind = transform.position - (moveDirection * DetectionDistance);
        checkPosBehind.y += DetectionHeightOffset;
        
        Collider[] hitsBehind = Physics.OverlapBox(checkPosBehind, Vector3.one * 0.4f, Quaternion.identity, ObstacleLayer);
        DiceBlock diceToPull = (hitsBehind.Length > 0) ? hitsBehind[0].GetComponent<DiceBlock>() : null;

        if (diceToPull != null)
        {
            Vector3 checkPosFront = transform.position + (moveDirection * DetectionDistance);
            checkPosFront.y += DetectionHeightOffset;
            Collider[] hitsFront = Physics.OverlapBox(checkPosFront, Vector3.one * 0.4f, Quaternion.identity, ObstacleLayer);

            if (hitsFront.Length == 0)
            {
                
                PlaySoundWithPitch(PullSound, PullVolume); 
                diceToPull.Pull(moveDirection); 
                StartCoroutine(MovePlayerSmoothly(transform.position + moveDirection));
            }
            else
            {
               
                PlaySoundWithPitch(BumpSound, BumpVolume);
            }
        }
        else
        {
            AttemptPushOrMove(moveDirection);
        }
    }

    private void AttemptPushOrMove(Vector3 direction)
    {
        Vector3 targetPos = transform.position + direction;
        Vector3 checkPos = transform.position + (direction * DetectionDistance);
        checkPos.y += DetectionHeightOffset;

        debugBoxCenter = checkPos;
        hasDebugInfo = true;
        

        Collider[] hitColliders = Physics.OverlapBox(checkPos, Vector3.one * 0.4f, Quaternion.identity, ObstacleLayer);

        if (hitColliders.Length > 0)
        {
            DiceBlock dice = hitColliders[0].GetComponent<DiceBlock>();

            if (dice != null)
            {
                if (dice.Push(direction))
                {
                    
                    PlaySoundWithPitch(PushSound, PushVolume);
                    StartCoroutine(MovePlayerSmoothly(targetPos));
                }
                else
                {
                    
                    PlaySoundWithPitch(BumpSound, BumpVolume);
                }
            }
            else
            {
                
                PlaySoundWithPitch(BumpSound, BumpVolume);
            }
        }
        else
        {
            
            PlaySoundWithPitch(MoveSound, MoveVolume);
            StartCoroutine(MovePlayerSmoothly(targetPos));
        }
    }

    private IEnumerator MovePlayerSmoothly(Vector3 targetPosition)
    {
        isMoving = true;
        float elapsedTime = 0;
        Vector3 startPosition = transform.position;

        while (elapsedTime < (1f / MoveSpeed))
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, (elapsedTime * MoveSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
    }

    
    private void PlaySoundWithPitch(AudioClip clip, float volume)
    {
        if (clip == null) return;

        audioSource.pitch = Random.Range(0.9f, 1.1f);
        
        audioSource.PlayOneShot(clip, volume);
    }

    private void OnDrawGizmos()
    {
        if (hasDebugInfo)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(debugBoxCenter, Vector3.one * 0.8f);
        }
    }
}