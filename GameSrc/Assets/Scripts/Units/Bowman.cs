using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Bowman : Unit
{
    //public Transform target;
    public float speed = 1;
    public float speed_a = 1;

    void Update()
    {
        //if (target)
        //{
        //    Quaternion r = Quaternion.LookRotation(target.position - transform.position);
        //    transform.rotation = Quaternion.Slerp(transform.rotation, r, Time.deltaTime * speed_a);
        //    transform.Translate(Vector3.forward * Time.deltaTime * speed);
        //    target = null;
        //}
    }
}

