using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Game Configuration")]
    [SerializeField] private float gameTimer = 300f;
    [SerializeField] private int totalObjectsToFind;
    
    [Header("Audio")]
    [SerializeField] private AudioSource voicemailAudio;
    [SerializeField] private float initialDelay = 2f;
    
    [Header("Time Audio")]
    [SerializeField] private AudioClip[] whisperClips;
    [SerializeField] private float whisperMinDelay = 5f;
    [SerializeField] private float whisperMaxDelay = 15f;
    [SerializeField] private float minVolume = 0.2f;
    [SerializeField] private float maxVolume = 1f;
    [SerializeField] private Transform player;
    private float nextWhisperTime;
    private float volumeCurve = 5f;
    private bool gameStarted = false;

    [Header("Game Over Effects")]
    [SerializeField] private Image blackScreen;
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private int numberOfFlashes = 4;
    [SerializeField] private float finalPauseTime = 1f;

    [Header("Level Materials")]
    [SerializeField] private MeshRenderer levelMeshRenderer;
    [SerializeField] private Material occultLevelMaterial;

    [Header("Win State")]
    [SerializeField] private AudioClip winAudio;

    [Header("References")]
    [SerializeField] private List<TransformableObject> allTransformableObjects = new List<TransformableObject>();
    [SerializeField] private MatProgressionHandler matHandler;
    
    private float currentTime;
    private int objectsFound = 0;
    private bool gameOver = false;
    private Material originalLevelMaterial;

    private static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        if (levelMeshRenderer != null)
        {
            originalLevelMaterial = levelMeshRenderer.material;
        }

        currentTime = gameTimer;
        if (allTransformableObjects.Count == 0)
        {
            allTransformableObjects.AddRange(FindObjectsOfType<TransformableObject>());
        }
        totalObjectsToFind = allTransformableObjects.Count;

        PlayerMovement.setCanMove(false);
        Invoke(nameof(PlayVoicemail), initialDelay);
    }

    private void PlayVoicemail()
    {
        if (voicemailAudio != null)
        {
            voicemailAudio.Play();
            Invoke(nameof(StartGame), voicemailAudio.clip.length);
        }
        else
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        EnableMovement();
        gameStarted = true;
        nextWhisperTime = Time.time + Random.Range(whisperMinDelay, whisperMaxDelay);
    }

    private void EnableMovement()
    {
        PlayerMovement.setCanMove(true);
    }

    private void Update()
    {
        if (!gameOver && gameStarted)
        {
            HandleTimer();
            CheckForObjectClick();
            HandleWhispers();
        }
    }

    private void HandleTimer()
    {
        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            GameLost();
        }
    }

    private void HandleWhispers()
    {
        if (Time.time >= nextWhisperTime && whisperClips != null && whisperClips.Length > 0)
        {
            float timeProgress = 1 - (currentTime / gameTimer);
            float delayMultiplier = 1 - (timeProgress * 0.7f);
            nextWhisperTime = Time.time + Random.Range(whisperMinDelay, whisperMaxDelay) * delayMultiplier;

            float volumeProgress = Mathf.Pow(timeProgress, volumeCurve);

            AudioSource.PlayClipAtPoint(
                whisperClips[Random.Range(0, whisperClips.Length)],
                player.position,
                Mathf.Lerp(minVolume * 0.1f, maxVolume, volumeProgress)
            );
        }
    }

    public void StartWinSequence()
{
    gameOver = true; 
    StartCoroutine(WinSequence());
}

private void CheckForObjectClick()
{
    if (Input.GetMouseButtonDown(0))
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            MatProgressionHandler mat = hit.collider.GetComponent<MatProgressionHandler>();
            if (mat != null)
            {
                mat.OnMatClicked();
                return;
            }
                TransformableObject transformableObject = hit.collider.GetComponent<TransformableObject>();
                
                if (transformableObject == null)
                {
                    var handler = hit.collider.GetComponent<TransformableObjectClickHandler>();
                    if (handler != null)
                    {
                        transformableObject = handler.GetMainObject();
                    }
                }

                if (transformableObject != null && !transformableObject.IsFound())
                {
                    ObjectFound(transformableObject);
                }
            }
        }
    }

    private void ObjectFound(TransformableObject obj)
    {
        if (!obj.IsTransformed())
        {
            obj.CycleToNextState();
        }
        
        obj.MarkAsFound();
        objectsFound++;

        if (matHandler != null)
        {
            matHandler.OnObjectFound();
        }

    }

    private IEnumerator WinSequence()
    {
        gameOver = true;

        blackScreen.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);

        if (winAudio != null)
        {
            AudioSource.PlayClipAtPoint(winAudio, player.position);
            yield return new WaitForSeconds(winAudio.length);
        }

        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    private IEnumerator LoseSequence()
    {
        blackScreen.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);

        if (levelMeshRenderer != null && occultLevelMaterial != null)
        {
            levelMeshRenderer.material = occultLevelMaterial;
        }

        for(int i = 0; i < numberOfFlashes; i++)
        {
            blackScreen.gameObject.SetActive(false);
            yield return new WaitForSeconds(flashDuration);
            blackScreen.gameObject.SetActive(true);
            yield return new WaitForSeconds(flashDuration);
        }

        yield return new WaitForSeconds(finalPauseTime);
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    private void GameWon()
    {
        StartCoroutine(WinSequence());
    }

    private void GameLost()
    {
        gameOver = true;
        StartCoroutine(LoseSequence());
    }

    public void RegisterTransformableObject(TransformableObject obj)
    {
        if (!allTransformableObjects.Contains(obj))
        {
            allTransformableObjects.Add(obj);
            totalObjectsToFind = allTransformableObjects.Count;
        }
    }
}