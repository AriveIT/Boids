using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Flock : MonoBehaviour
{

    public float nearbyRadius;
    public int flockSize;
    public GameObject boidPrefab;

    public float separationMultiplier;
    public float cohesionMultiplier;
    public float alignmentMultiplier;
    
    Boid[] flock; // boid scripts
    int lastFlockSize;

    // Start is called before the first frame update
    void Start()
    {
        flock = new Boid[flockSize];
        lastFlockSize = flockSize;
        
        // create boids with random starting position
        for (int i = 0; i < flockSize; i++) {
            insertNewBoid(flock, i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (flockSize != lastFlockSize) {
            updateFlockSize();
        }

        for(int i = 0; i < flockSize; i++) {
            List<int> nearby = getNearby(i, nearbyRadius);

            // no force exerted if no nearby boids
            if(nearby.Count == 0) {
                continue;
            }
            
            Vector3 separationForce = separation(i, nearby) * separationMultiplier;
            Vector3 cohesionForce = cohesion(i, nearby) * cohesionMultiplier;
            Vector3 alignmentForce = alignment(nearby) * alignmentMultiplier;

            flock[i].exertForce(separationForce + cohesionForce + alignmentForce);

        }
    }

    void updateFlockSize() {

        // if flock size has increased
        if(flockSize > lastFlockSize) {
            // check if have to make new array
            if(flockSize > flock.Length) {
                // make new array to make room for new boids
                Boid[] newFlock = new Boid[flockSize];
                Array.Copy(flock, newFlock, lastFlockSize);

                // create new Boids
                for(int i = lastFlockSize; i < flockSize; i++) {
                    insertNewBoid(newFlock, i);
                }

                flock = newFlock;
            } else {
                // create new Boids
                for(int i = lastFlockSize; i < flockSize; i++) {
                    insertNewBoid(flock, i);
                }
            }
        } else {
            // if flock size has decreased, destroy excess Boids
            for(int i = flockSize; i < lastFlockSize; i++) {
                flock[i].destroyBoid();
            }
        }

        lastFlockSize = flockSize;
    }

    void insertNewBoid(Boid[] curFlock, int i) {
        Vector3 randomPos = new Vector3(UnityEngine.Random.Range(-10.0f, 10.0f), UnityEngine.Random.Range(-5.0f, 5.0f), 0);
        GameObject newBoid = Instantiate(boidPrefab, randomPos, Quaternion.identity, transform);
        curFlock[i] = newBoid.GetComponent<Boid>();
    }

    // push boids to move in same direction
    Vector3 alignment(List<int> nearby) {
        Vector3 sum = Vector3.zero;

        foreach (int index in nearby) {
            sum += flock[index].getDirection();
        }

        return sum / nearby.Count;

    }

    // push boids to not run into each other
    // force = sum(v / ||v||^2)
    Vector3 separation(int curBoid, List<int> nearby) {
        Vector3 force = Vector3.zero;

        foreach (int index in nearby) {
            Vector3 difference = flock[curBoid].transform.position - flock[index].transform.position;
            float sqrDist = difference.sqrMagnitude;

            force += difference / sqrDist;
        }

        return force;
    }

    // push boids to move to centre of flock
    Vector3 cohesion(int curBoid, List<int> nearby) {
        Vector3 sum = Vector3.zero;

        foreach (int index in nearby) {
            sum += flock[index].transform.position;
        }

        Vector3 avgPos = sum / nearby.Count;

        return avgPos - flock[curBoid].transform.position;
    }


    // returns list of indexes of boids within radius r;
    List<int> getNearby(int curBoid, float r) {
        List<int> nearby = new List<int>();
        float rSquared = r * r;

        for (int i = 0; i < flockSize; i++) {
            if(curBoid == i) continue;

           float sqrDist = getSqrDist(curBoid, i);

            if(sqrDist < rSquared) {
                nearby.Add(i);
            }
        }

        return nearby;
    }

    // returns square distance between the boids at indices i and j
    float getSqrDist(int i, int j) {
        Vector3 difference = flock[i].transform.position - flock[j].transform.position;
        return difference.sqrMagnitude;
    }

    void OnValidate() {
        if (flockSize < 0) flockSize = 0;
    }
}

