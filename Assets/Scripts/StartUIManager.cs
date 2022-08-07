using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class StartUIManager : MonoBehaviour
{
    [SerializeField] TMP_InputField nameText;
    [SerializeField] Name nameObject;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        Debug.Log("Name: " + nameText.text);
        nameObject.playerName = nameText.text;
        SceneManager.LoadScene(1);
        
    }
}
