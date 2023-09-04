using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{

    public float nearbyRadius;
    public int flockSize;
    public GameObject boidPrefab;

    public float separationMultiplier;
    public float cohesionMultiplier;
    public float alignmentMultiplier;
    
    Boid[] flock; // boid scripts
    Transform[] boidTransforms;

    // Start is called before the first frame update
    void Start()
    {
        boidTransforms = new Transform[flockSize];
        flock = new Boid[flockSize];
        
        // create boids with random starting position
        for (int i = 0; i < flockSize; i++) {
            Vector3 randomPos = new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-5.0f, 5.0f), 0);
            GameObject newBoid = Instantiate(boidPrefab, randomPos, Quaternion.identity, transform);

            boidTransforms[i] = newBoid.transform;
            flock[i] = newBoid.GetComponent<Boid>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < flockSize; i++) {
            List<int> nearby = getNearby(i, nearbyRadius);
            
            Vector3 separationForce = separation(i, nearby) * separationMultiplier;
            Vector3 cohesionForce = cohesion(i, nearby) * cohesionMultiplier;
            Vector3 alignmentForce = alignment(i, nearby) * alignmentMultiplier;
            
            flock[i].exertForce(separationForce + cohesionForce + alignmentForce);

        }
    }

    // push boids to move in same direction
    Vector3 alignment(int boidIndex, List<int> nearby) {
        Vector3 sum = Vector3.zero;

        foreach (int index in nearby) {
            sum += flock[index].getDirection();
        }

        return sum / nearby.Count;

    }

    // push boids to not run into each other
    // force = sum(v / ||v||^2)
    Vector3 separation(int boidIndex, List<int> nearby) {
        Vector3 force = Vector3.zero;

        foreach (int index in nearby) {
            Vector3 difference = boidTransforms[boidIndex].position - boidTransforms[index].position;
            float sqrDist = difference.sqrMagnitude;

            force += difference / sqrDist;
        }

        return force;
    }

    // push boids to move to centre of flock
    Vector3 cohesion(int boidIndex, List<int> nearby) {
        Vector3 sum = Vector3.zero;

        foreach (int index in nearby) {
            sum += boidTransforms[index].position;
        }

        Vector3 avgPos = sum / nearby.Count;

        return avgPos - boidTransforms[boidIndex].position;
    }


    // returns list of indexes of boids within radius r;
    List<int> getNearby(int boidIndex, float r) {
        List<int> nearby = new List<int>();
        float rSquared = r * r;

        for (int i = 0; i < flockSize; i++) {
            if(boidIndex == i) continue;

           float sqrDist = getSqrDist(boidIndex, i);

            if(sqrDist < rSquared) {
                nearby.Add(i);
            }
        }

        return nearby;
    }

    // returns square distance between the boids at indices i and j
    float getSqrDist(int i, int j) {
        Vector3 difference = boidTransforms[i].position - boidTransforms[j].position;
        return difference.sqrMagnitude;
    }
}
