using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability
{
    protected Rigidbody _rb { get; set; }
    public string textName { get; set; }

    public abstract void Movement(float speed);
}
