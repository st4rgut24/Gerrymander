using UnityEngine;
using System.Collections;
using TMPro;
public class Score : MonoBehaviour
{
	TextMeshProUGUI scoreText;

    private void OnEnable()
    {
        //PersonPrefab.DieEvent += OnDie;
    }

    // Use this for initialization
    void Start()
	{
        scoreText = GetComponent<TextMeshProUGUI>();
        SetScore(0, 0);
	}

    private void OnDisable()
    {
        //PersonPrefab.DieEvent -= OnDie;
    }

    public void SetScore(int democratDistricts, int republicanDistricts)
    {
        string scoreStr = democratDistricts.ToString() + " : " + republicanDistricts.ToString();
        scoreText.text = scoreStr;
    }
}

