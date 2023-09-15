using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasController : MonoBehaviour
{
    [SerializeField] GameObject startPanel, finishPanel;

    [Header("Task")]
    [SerializeField] GameObject bluePanel, yellowPanel, redPanel, greenPanel;
    [SerializeField] int blueValue, yellowValue, redValue, greenValue;
    public TMP_Text blueText, yellowText, redText, greenText;

    [Header("Level Size")]
    [SerializeField] int width, height;

    [HideInInspector] public Board board;

    private void Awake()
    {
        board = FindObjectOfType<Board>();
        board.width = width;
        board.height = height;
    }

    private void Start()
    {
        startPanel.SetActive(true);
        finishPanel.SetActive(false);

        if (blueValue <= 0)
            bluePanel.SetActive(false);

        else
            blueText.text = blueValue.ToString();

        if (yellowValue <= 0)
            yellowPanel.SetActive(false);

        else
            yellowText.text = yellowValue.ToString();

        if (redValue <= 0)
            redPanel.SetActive(false);

        else
            redText.text = redValue.ToString();

        if (greenValue <= 0)
            greenPanel.SetActive(false);

        else
            greenText.text = greenValue.ToString();

    }

    public void StartGame()
    {
        startPanel.SetActive(false);
        board.Setup();
    }

    public void UpdateTask(int taskID)
    {
        if (taskID == 0 && blueValue > 0)
        {
            blueValue--;
            blueText.text = blueValue.ToString();
        }
        else if (taskID == 1 && greenValue > 0)
        {
            greenValue--;
            greenText.text = greenValue.ToString();
        }
        else if (taskID == 2 && redValue > 0)
        {
            redValue--;
            redText.text = redValue.ToString();
        }
        else if (taskID == 3 && yellowValue > 0)
        {
            yellowValue--;
            yellowText.text = yellowValue.ToString();
        }

        if (blueValue == 0 && greenValue == 0 && redValue == 0 && yellowValue == 0)
        {
            finishPanel.SetActive(true);
            board.gameStatus = GameStatus.over;
        }
    }

    public void NextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }
}
