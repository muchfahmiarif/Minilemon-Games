using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; set; }

    public UnityEvent OnDayPass = new UnityEvent(); // Day Passed Event


    private void Awake() 
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public int dayInGame = 1;
    public TextMeshProUGUI dayUi;

    private void Start() 
    {
        dayUi.text = $"Day: {dayInGame}";
    }

    public void TiggerNextDay()
    {
        dayInGame += 1;
        dayUi.text = $"Day: {dayInGame}";

        OnDayPass.Invoke();
    }
}
