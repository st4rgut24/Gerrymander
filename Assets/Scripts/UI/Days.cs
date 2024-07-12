using UnityEngine;
using TMPro;
public class Days : MonoBehaviour
{
    TextMeshProUGUI DaysText;

    private void OnEnable()
    {
        //PersonPrefab.DieEvent += OnDie;
    }

    // Use this for initialization
    void Start()
    {
        DaysText = GetComponent<TextMeshProUGUI>();
        SetDays(GameManager.Instance.DaysTilElection);
    }

    private void OnDisable()
    {
        //PersonPrefab.DieEvent -= OnDie;
    }

    public void SetDays(int days)
    {
        string daysStr = days.ToString() + " Days Left";
        DaysText.text = daysStr;
    }
}

