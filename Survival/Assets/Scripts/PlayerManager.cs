﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;
    public float rotation;

    public int health;

    public bool attack = false;
    public GameObject sword;

    public bool spectate = false;

    private TextMeshPro usernameBox;
    private Transform spriteTransform;
    private Animator animator;

    private Vector2 defaultSwordPosition;

    public SpriteRenderer spriteRenderer;
    public Sprite deadSprite;


    void Start()
    {
        usernameBox = gameObject.GetComponentInChildren<TextMeshPro>();
        spriteTransform = gameObject.GetComponentInChildren<SpriteRenderer>().transform;
        animator = gameObject.GetComponent<Animator>();
        defaultSwordPosition = sword.transform.localPosition;
    }

    void Update()
    {
        usernameBox.text = username;

        spriteTransform.rotation = Quaternion.Euler(0f, 0f, rotation);
    }

    private void FixedUpdate()
    {
        if (attack)
        {
            animator.SetTrigger("Attack");
        }
        else
        {
            sword.transform.localPosition = Vector2.zero;
        }
    }

    public void setDeadSprite()
    {
        spriteRenderer.sprite = deadSprite;
    }
}
