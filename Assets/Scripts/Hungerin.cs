using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hungerin : MonoBehaviour
{
    EssencialProperties m_EssencialProperties;
    [SerializeField] private Rigidbody m_RigidBody;
    
    [Space]
    private Ability m_CurrentAbility;
    [SerializeField] private string currentAbilityName;
    [SerializeField] private float speed = 300f;


    private void Awake()
    {
        m_CurrentAbility = new Normal(m_RigidBody);
    }

    // Here we control the player
    private void Update()
    {
        currentAbilityName = m_CurrentAbility.textName;
    }

    // Here we make physics
    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        m_CurrentAbility.Movement(speed);
    }
}
