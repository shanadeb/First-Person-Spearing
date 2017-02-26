using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour {

    public float speed = 0.001f;
    public float rotationSpeed = 4.0f;
    public float minSpeed = 0.8f;
    public float maxSpeed = 2.0f;
    Vector3 averageHeading;
    Vector3 averagePosition;

    //How much fish swim off, increase to have little schooling
    public float neighborDistance = 3.0f;
    public Vector3 newGoalPos;

    bool turning = false;

	// Use this for initialization
	void Start () {
        speed = Random.Range(minSpeed, maxSpeed);
        //this.GetComponent<Animation>()["Motion"].speed = speed;
	}

    void OnTriggerEnter(Collider other)
    {
        if (!turning)
        {
            newGoalPos = this.transform.position - other.gameObject.transform.position;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        turning = false;
    }

    // Update is called once per frame
    void Update ()
    {
        if (turning)
        {
            Vector3 direction = newGoalPos = newGoalPos - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(direction),
                rotationSpeed * Time.deltaTime);

            speed = Random.Range(minSpeed, maxSpeed);
            //this.GetComponent<Animation>()["Motion"].speed = speed;
        }
        else
        {
            if (Random.Range(0, 10) < 1)
            {
                ApplyRules();
            }
        }
        transform.Translate(0, 0, Time.deltaTime * speed);
	}

    void ApplyRules()
    {
        GameObject[] gos;
        gos = GlobalFlock.allFish;

        Vector3 vcentre = Vector3.zero;
        Vector3 vavoid = Vector3.zero;
        float gSpeed = 0.1f;

        Vector3 goalPos = GlobalFlock.goalPos;

        float dist;

        int groupSize = 0;
        foreach (GameObject go in gos)
        {
            if (go != this.gameObject)
            {
                dist = Vector3.Distance(go.transform.position, this.transform.position);
                if (dist <= neighborDistance)
                {
                    vcentre += go.transform.position;
                    groupSize++;

                    if (dist < 2.0f)
                    {
                        vavoid = vavoid + (this.transform.position - go.transform.position);
                    }

                    Flock anotherFlock = go.GetComponent<Flock>();
                    gSpeed = gSpeed + anotherFlock.speed;
                }
            }
        }

        if (groupSize > 0)
        {
            vcentre = vcentre / groupSize + (goalPos - this.transform.position);
            speed = gSpeed / groupSize;
            //this.GetComponent<Animation>()["Motion"].speed = speed;

            Vector3 direction = (vcentre + vavoid) - transform.position;

            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(direction),
                    rotationSpeed * Time.deltaTime);
            }
        }
    }


}
