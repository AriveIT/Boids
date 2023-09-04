using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour {
    Vector3 direction;
    public float speed;
    public float wraparoundBuffer;

    // Start is called before the first frame update
    void Start() {
        // set random direction
        direction = new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), 0);
        direction = Vector3.Normalize(direction);
    }

    // Update is called once per frame
    void Update()
    {
        // move boid
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
        wraparound(wraparoundBuffer, wraparoundBuffer);

        // rotate boid
        transform.rotation = getRotationFromDirection(direction);

    }

    // returns a rotation such that boid points in given direction
    Quaternion getRotationFromDirection(Vector3 direction) {

        // don't divide by 0
        if (direction.y == 0) {
            if (direction.x >= 0) {
                return Quaternion.Euler(0,0,0);
            } else {
                return Quaternion.Euler(0,0,180);
            }
        }

        float rad = -Mathf.Atan(direction.x / direction.y);
        float deg = rad * Mathf.Rad2Deg;

        if(direction.y < 0) {
            deg += 180;
        }

        return Quaternion.Euler(0,0,deg);
    }

    Vector3 getMouseDirection() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return Vector3.Normalize(mousePos - transform.position);
    }

    // if boid goes offscreen, move to other side of screen (preserving velocity)
    void wraparound(float xBuffer, float yBuffer) {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

        // Wraparound on x-axis
        if(screenPos.x > Screen.width + xBuffer) {
            screenPos.x = 0 - xBuffer;
        } else if (screenPos.x < 0 - xBuffer) {
            screenPos.x = Screen.width + xBuffer;
        }

        // Wraparound on y-axis
        if(screenPos.y > Screen.height + yBuffer) {
            screenPos.y = 0 - yBuffer;
        } else if (screenPos.y < 0 - yBuffer) {
            screenPos.y = Screen.height + yBuffer;
        }

        transform.position = Camera.main.ScreenToWorldPoint(screenPos);
    }

    public Vector3 getDirection() {
        return direction;
    }

    public void squawk(int x) {
        Debug.Log("Squawk " + x);
    }

    public void exertForce(Vector3 force) {
        direction += force * Time.deltaTime;

        if(direction == Vector3.zero) direction = force;

        direction = Vector3.Normalize(direction);
    }


}
