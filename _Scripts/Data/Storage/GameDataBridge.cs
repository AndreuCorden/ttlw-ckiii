using UnityEngine;

public class GameDataBridge : MonoBehaviour
{
    public static GameDataBridge Instance { get; private set; }

    // This is the memory that survives the scene change
    public string playerFirstName;
    public string playerLastName;
    public string playerKingdomName;

    private void Awake()
    {
        // The "Singleton" pattern: Ensure only one exists
        if (Instance == null)
        {
            Instance = this;
            // This is the magic line that prevents deletion on scene load
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}