using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointFollower : MonoBehaviour
{
    [SerializeField] private GameObject[] waypoints;
    private int currentWaypoint = 0;
    private int direction = 1;

    [SerializeField] private float speed = 4f;

    private void Update()
    {
        // Updates waypoint destination if the currentWaypoint is reached, flips direction if necessary
        if (Vector2.Distance(waypoints[currentWaypoint].transform.position, transform.position) < .1f)
        {
            currentWaypoint += direction;
            if (currentWaypoint == 0 || currentWaypoint == waypoints.Length - 1) direction *= -1;
        }
        transform.position = Vector2.MoveTowards(transform.position, waypoints[currentWaypoint].transform.position, Time.deltaTime * speed);
    }
}
