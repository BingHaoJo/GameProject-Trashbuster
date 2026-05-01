using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

public class WinScreenAnimated : MonoBehaviour
{
    [SerializeField] private TMP_Text wellDoneText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text scoreAmountText;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private TMP_Text creditsText;
    [SerializeField] private int finalScore = CompletionCheck.trashScore;
    float timer = 0.01f;
    private int score = 0;

    private void Update()
    {
        if (score < finalScore && scoreAmountText.gameObject.activeSelf)// Score count up
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                score++;
                scoreAmountText.text = score.ToString();
                gameObject.GetComponent<AudioSource>().Play();
                timer = 0.01f;
            }
        }
        else if (score >= finalScore && scoreAmountText.gameObject.activeSelf)
        {
            ButtonActive();
        }
    }

    private void WellDoneText()
    {
        wellDoneText.gameObject.SetActive(true);
    }

    private void ScoreText()
    {
        scoreText.gameObject.SetActive(true);
        scoreAmountText.gameObject.SetActive(true);
    }

    private void ButtonActive()
    {
        mainMenuButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
        creditsText.gameObject.SetActive(true);
    }
}
