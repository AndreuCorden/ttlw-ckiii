using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterCreatorUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField firstNameInput;
    public TMP_InputField lastNameInput;

    public TMP_InputField kingdomNameInput;
    public GameObject creatorPanel;

    public void OnClickContinue()
    {
        // 1. Send data to PlayerManager
        string fName = firstNameInput.text;
        string lName = lastNameInput.text;
        string kName = kingdomNameInput.text;

        if (string.IsNullOrEmpty(fName) || string.IsNullOrEmpty(lName) || string.IsNullOrEmpty(kName))
        {
            Debug.LogWarning("Please enter a name, family name, and kingdom name!");
            return;
        }

        // 1. Store data in the persistent bridge
        GameDataBridge.Instance.playerFirstName = fName;
        GameDataBridge.Instance.playerLastName = lName;
        GameDataBridge.Instance.playerKingdomName = kName;

        // 2. Change scene (Make sure "PoliticsScene" is in your Build Settings!)
        SceneManager.LoadScene("Politics");
    }
}