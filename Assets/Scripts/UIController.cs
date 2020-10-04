using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    public void UpdateScore(int newScore)
    {
        scoreText.text = newScore.ToString("D8");
        scoreText.GetComponentInParent<Animator>().SetTrigger("Update");
    }
}
