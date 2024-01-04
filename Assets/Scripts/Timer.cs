using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    TMP_Text t;

    // Start is called before the first frame update
    void Start()
    {
        t = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        t.text = "Turn: " + FTTurnManager.Instance.currentTurn;
    }
}
