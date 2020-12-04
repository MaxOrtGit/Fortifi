using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public CharacterController controller;

    public Transform playerBody;
    public Animator animator;

    public float velocity = 4f;
    public float turnSpeed = 20f;
    bool stop = false;

    float x;
    float y;
    

    float angle;

    Quaternion targetRotation;
    Transform cam;


    private void Start() {
        cam = Camera.main.transform;
        animator = transform.GetChild(0).GetComponent<Animator>();
        animator.SetFloat("Speed", velocity);
    }

    // Update is called once per frame
    void Update() {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        float speed = Mathf.Abs(x) + Mathf.Abs(y);

        if (stop) {
            animator.SetBool("Moving", false);
        }

        if (speed < 0.2f) {
            stop = true;
            return;
        }

        speed -= 0.2f;
        animator.SetBool("Moving", true);

        if (speed > 0.75f) {
            speed = 1.0f;
        }

        stop = false;
        animator.SetFloat("Speed", velocity * speed);

        angle = Mathf.Atan2(x, y);
        angle = Mathf.Atan2(x, y);
        angle = Mathf.Rad2Deg * angle;
        angle += cam.eulerAngles.y;

        targetRotation = Quaternion.Euler(0, angle, 0);
        playerBody.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        controller.Move(transform.forward * velocity * Time.deltaTime * speed);
    }
}