using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Turtle : MonoBehaviour
{
    public float minTakeOverTime;
    public float maxTakeOverTime;
    public float breakOutDifficulty;
    public float pauseTime;
    public Animator animator;
    public GameObject deathTurtle;
    public Transform questionMarkSpawn;
    public GameObject questionMark;
    public GameObject keysImage;
    public Sprite[] keyImages;
    public GameObject progressBar;
    public Sprite[] progressBarImages;
    public GameObject footSteps;

    NavMeshAgent navMeshAgent;
    float breakOutProgress;
    float takeOverTime;
    bool AITakeOver = false;
    string breakOutKey = "a";
    Vector3 previousLocation;
    GameObject[] DangerZones;
    float pause;
    bool spawnQuestionMark = true;
    bool stopMovement = false;
    Vector3 previousStepLocation;
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = this.GetComponent<NavMeshAgent>();
        DangerZones = GameObject.FindGameObjectsWithTag("DangerZone");
        takeOverTime = Random.Range(minTakeOverTime, maxTakeOverTime);
        previousLocation = transform.position;
        previousStepLocation = transform.position;
        pause = pauseTime;

    }

    // Update is called once per frame
    void Update()
    {
        if (breakOutKey == "a")
        {
            keysImage.GetComponent<Image>().sprite = keyImages[0];
        }
        else
        {
            keysImage.GetComponent<Image>().sprite = keyImages[1];
        }
        UpdateGraphics();
        MovementHandler();
        PlaceFootPrints();
        TakeOver();
        BreakOut();
    }

    void PlaceFootPrints()
    {
        Vector3 differenceStepLocation = (transform.position - previousStepLocation) / Time.deltaTime;
        Debug.Log(differenceStepLocation);
        if (differenceStepLocation.z >= 150)
        {
            Instantiate(footSteps, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
            previousStepLocation = transform.position;
        }
        else if (differenceStepLocation.z <= -150)
        {
            Instantiate(footSteps, transform.position, Quaternion.Euler(new Vector3(0, 180, 0)));
            previousStepLocation = transform.position;
        }
        else if (differenceStepLocation.x >= 150)
        {
            Instantiate(footSteps, transform.position, Quaternion.Euler(new Vector3(0, 90, 0)));
            previousStepLocation = transform.position;
        }
        else if (differenceStepLocation.x <= -150)
        {
            Instantiate(footSteps, transform.position, Quaternion.Euler(new Vector3(0, -90, 0)));
            previousStepLocation = transform.position;
        }
    }

    void ResetAnimatorBools()
    {
        animator.SetBool("FacingForward", false);
        animator.SetBool("FacingBackward", false);
        animator.SetBool("FacingRight", false);
        animator.SetBool("FacingLeft", false);
        animator.SetBool("Idle", false);
        animator.SetBool("Walking", false);

    }

    void UpdateGraphics()
    {
        Vector3 currentVelocity = (transform.position - previousLocation);
        if (currentVelocity.z > 0)
        {
            ResetAnimatorBools();
            //Moving Up
            animator.SetBool("FacingForward", true);
            animator.SetBool("Walking", true);
        }
        else if (currentVelocity.z < 0)
        {
            //Moving down
            ResetAnimatorBools();
            animator.SetBool("FacingBackward", true);
            animator.SetBool("Walking", true);
        }
        else if (currentVelocity.x > 0)
        {
            //Moving right
            ResetAnimatorBools();
            animator.SetBool("FacingRight", true);
            animator.SetBool("Walking", true);
        }
        else if (currentVelocity.x < 0)
        {
            //Moving left
            ResetAnimatorBools();
            animator.SetBool("FacingLeft", true);
            animator.SetBool("Walking", true);
        }
        else
        {
            //Not Moving
            animator.SetBool("Idle", true);
            animator.SetBool("Walking", false);
        }
        previousLocation = transform.position;
    }

    void MovementHandler()
    {
        //If the AI is currently taking over, do use WASD movement
        if (stopMovement)
        {
            return;
        }

        //WASD Movement
        Vector3 Movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        this.transform.position += Movement * navMeshAgent.speed * Time.deltaTime;
    }

    void TakeOver()
    {
        takeOverTime -= Time.deltaTime;
        if (takeOverTime > 0)
        {
            return;
        }
        if (spawnQuestionMark)
        {
            Instantiate(questionMark, questionMarkSpawn.position, Quaternion.identity, this.transform);
            spawnQuestionMark = false;
        }
        stopMovement = true;
        pause -= Time.deltaTime;
        if (pause > 0)
        {
            return;
        }
        else
        {
            GameObject target = GetNearestTarget();
            if (target == null)
            {
                Debug.LogError("No danger zones found!");
                return;
            }
            navMeshAgent.SetDestination(target.transform.position);
            AITakeOver = true;
            takeOverTime = Mathf.Infinity;
        }
    }

    GameObject GetNearestTarget()
    {
        GameObject closest = null;
        float distance = Mathf.Infinity;
        foreach(GameObject DZ in DangerZones)
        {
            Vector3 diff = DZ.transform.position - this.transform.position;
            float curDistance = diff.sqrMagnitude;
            if(curDistance < distance)
            {
                closest = DZ;
                distance = curDistance;
            }
        }
        return closest;
    }

    void BreakOut()
    {
        if (!AITakeOver)
        {
            return;
        }
        keysImage.SetActive(true);
        progressBar.SetActive(true);
        if (Input.GetKeyUp(breakOutKey))
        {
            
            breakOutProgress += breakOutDifficulty;
            if(breakOutKey == "a")
            {
                breakOutKey = "d";
            }
            else
            {
                breakOutKey = "a";
            }
        }
        Debug.Log(breakOutProgress);
        breakOutProgress -= Time.deltaTime;
        progressBar.GetComponent<Image>().sprite = progressBarImages[(int)breakOutProgress / 10];
        if (breakOutProgress >= 100)
        {
            navMeshAgent.ResetPath();
            AITakeOver = false;
            stopMovement = false;
            breakOutProgress = 0;
            takeOverTime = Random.Range(minTakeOverTime, maxTakeOverTime);
            spawnQuestionMark = true;
            pause = pauseTime;
            keysImage.SetActive(false);
            progressBar.SetActive(false);
        }
    }

    public void Die()
    {
        //Instantiate Death Turtle
        Instantiate(deathTurtle, transform.position, Quaternion.identity);
        //Destroy Self
        Destroy(this.gameObject);
    }
}
