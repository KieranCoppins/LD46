using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinZone : MonoBehaviour
{

    public GameObject UIObject;
    public float time = 0;
    public Text timeText;

    // Start is called before the first frame update
    void Start()
    {
        time = 0; 
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        Time.timeScale = 0;
        UIObject.SetActive(true);
        timeText.text = $"Completed in {time} seconds";
    }
}
