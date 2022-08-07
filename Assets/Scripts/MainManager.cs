using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text HighScoreText;
    public GameObject GameOverText;
    
    private bool m_Started = false;
    private int m_Points;
    
    private bool m_GameOver = false;
    private string m_Name;

    [SerializeField] private HighScoreData m_ScoreData;

    [System.Serializable]
    class HighScoreData
    {
        public string name;
        public int highScore;
    }

    private void Awake()
    {
        GameObject nameObj = GameObject.Find("Name");
        m_Name = nameObj.GetComponent<Name>().playerName;
        LoadData();
    }

    void LoadData()
    {

        string savePath = Application.persistentDataPath + "/saveData.json";
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            m_ScoreData = JsonUtility.FromJson<HighScoreData>(json);
        }
        else
        {
            m_ScoreData = new HighScoreData();
            m_ScoreData.name = m_Name;
            m_ScoreData.highScore = 0;
        }

    }

    void SaveData()
    {
        string savePath = Application.persistentDataPath + "/saveData.json";
        string json = JsonUtility.ToJson(m_ScoreData);
        File.WriteAllText(savePath, json);
    }

    void StartGame()
    {
        DisplayScore();

        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    // Start is called before the first frame update
    void Start()
    {
        StartGame();
    }

    void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_GameOver = false;
                m_Started = false;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                QuitGame();
            }
        }
    }

    void DisplayScore()
    {
        if (m_Points > m_ScoreData.highScore)
        {
            m_ScoreData.highScore = m_Points;
            m_ScoreData.name = m_Name;
        }
        HighScoreText.text = $"Best Score : {m_ScoreData.name}: {m_ScoreData.highScore}";
        ScoreText.text = $"{m_Name}'s Score: {m_Points}";
    }

    void AddPoint(int point)
    {
        m_Points += point;
        DisplayScore();
    }

    public void GameOver()
    {
        SaveData();
        m_GameOver = true;
        GameOverText.SetActive(true);
    }
}
