using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerZone : MonoBehaviour
{

    public Animator graphicAnimator;
    public GameObject UIObject;
    public float WaitTime;
    float wait;
    bool dead = false;

    // Start is called before the first frame update
    void Start()
    {
        wait = WaitTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (dead)
        {
            wait -= Time.deltaTime;

            if (wait <= 0)
            {
                UIObject.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("DIE TURTLE");
        //Kill Turtle
        other.GetComponent<Turtle>().Die();
        //Play Kill animation
        graphicAnimator.SetBool("Killing", true);
        dead = true;
    }
}
