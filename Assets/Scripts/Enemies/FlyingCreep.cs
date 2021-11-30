using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VladsUsefulScripts;

public class FlyingCreep : FlyingEnemy
{
    Animator anim;
    internal override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
    }
    internal override void FixedUpdate()
    {
        base.FixedUpdate();
        anim.SetBool("PlayerSpotted", playerSeen);

    }
    internal override void FollowPlayer()
    {
        //base.FixedUpdate();
        if (path == null) return;
        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else reachedEndOfPath = false;
        Vector2 force;
        if (reachedEndOfPath) force = ((Vector2)target.position - rb.position).normalized * speed;
        else force = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized * speed;

        rb.velocity = Clampers.ClampedDrag(force, drag, -speed * maxSpeedMult, speed * maxSpeedMult);

        if (Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance) currentWaypoint++;

    }
}
