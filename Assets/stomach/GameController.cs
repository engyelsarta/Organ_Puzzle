using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic; // Add this to access List<T>
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject completeOrgan;
    public GameObject[] puzzlePieces;
    public GameObject startButton;
    public TextMeshProUGUI winText;
    public TextMeshProUGUI loseText;

    // Audio clips for win and lose sounds
    public AudioClip winSound;
    public AudioClip loseSound;
    private AudioSource audioSource;

    private Camera mainCamera;

    private List<Vector3> occupiedPositions = new List<Vector3>();
    private float minDistanceBetweenPieces = 0.2f; 

    void Start()
    {
        // Initialize audio source component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Get the main camera
        mainCamera = Camera.main;
    }

    // Check if all pieces are snapped
    public bool AreAllPiecesSnapped()
    {
        foreach (GameObject piece in puzzlePieces)
        {
            DragAndDrop dragComponent = piece.GetComponent<DragAndDrop>();
            if (dragComponent != null && !dragComponent.IsSnapped())
            {
                return false;
            }
        }
        return true;
    }

    public void StartGame()
    {
        if (completeOrgan != null)
        {
            completeOrgan.SetActive(false);
        }

        foreach (GameObject piece in puzzlePieces)
        {
            if (piece != null)
            {
                // Set each piece active and randomize its position within the specified range
                piece.SetActive(true);
                RandomizePiecePosition(piece);
                RandomizePieceRotation(piece);
            }
        }

        DragAndDrop.gameStarted = true;

        if (startButton != null)
        {
            startButton.SetActive(false);
        }

        if (winText != null) winText.gameObject.SetActive(false);
        if (loseText != null) loseText.gameObject.SetActive(false);
    }

    private void RandomizePiecePosition(GameObject piece)
    {
        // Define the specified x and y ranges for randomization
        float minX = 0.30f;
        float maxX = 1.0f;
        float minY = -0.1f;
        float maxY = 0.4f;

        Vector3 newPosition;
        bool isPositionValid;

        do
        {
            // Generate a random position within these bounds
            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);

            // Set the new position
            newPosition = new Vector3(randomX, randomY, piece.transform.position.z);

            // Check if the position is far enough from all occupied positions
            isPositionValid = true;
            foreach (Vector3 occupied in occupiedPositions)
            {
                if (Vector3.Distance(newPosition, occupied) < minDistanceBetweenPieces)
                {
                    isPositionValid = false;
                    break;
                }
            }
        } 
        while (!isPositionValid);

        // Add the new position to the occupied list
        occupiedPositions.Add(newPosition);

        // Set the position of the puzzle piece
        piece.transform.position = newPosition;
    }
    
    private void RandomizePieceRotation(GameObject piece)
    {
        // Define the allowed rotation angles
        int[] allowedAngles = { 0, 90, 180, 270 };

        // Pick a random angle from the allowed list
        int randomIndex = Random.Range(0, allowedAngles.Length);
        float randomAngle = allowedAngles[randomIndex];

        // Apply the random rotation around the Y-axis
        piece.transform.rotation = Quaternion.Euler(0, randomAngle, 0);
    }

    public void ShowWinMessage()
    {
        if (winText != null)
        {
            winText.gameObject.SetActive(true);
            Debug.Log("You Win!");
        }
        PlaySound(winSound);
    }

    public void ShowLoseMessage()
    {
        if (loseText != null)
        {
            loseText.gameObject.SetActive(true);
            Debug.Log("You Lose!");
        }
        PlaySound(loseSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
