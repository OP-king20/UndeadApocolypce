using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Movement varibles
    public float Walkspeed = 15f;
    public float Runspeed = 25f;
    public float Jumpheight = 7f;
    public CharacterController controller;

    public static PlayerMovement instance;

    //Positions
    Vector3 prevPos;

    //Groundcheck
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance = 0.4f;
    public bool isGrounded;
    bool inair = false;

    //Gravity
    Vector3 velocity;
    public float gravity = -9.82f;

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical"); 
        Vector3 move = transform.right * x + transform.forward * z; //Current input from keyboard
        



        if (isGrounded == true)
        {

            controller.Move(move * Walkspeed * Time.deltaTime);

            if (Input.GetKey(KeyCode.LeftShift) && isGrounded == true && StaminaBar.instance.currentStamina >= 0.1f)
            {

                controller.Move(move * Runspeed * Time.deltaTime);
                StaminaBar.instance.UseStamina(.1f);
            }
            else if (Input.GetKey(KeyCode.LeftShift) && StaminaBar.instance.currentStamina <= 0f)
            {
                controller.Move(move * Walkspeed * Time.deltaTime);
            }


            if (Input.GetButtonDown("Jump") && isGrounded == true && StaminaBar.instance.currentStamina >= 25f)
            {
                velocity.y = Mathf.Sqrt(Jumpheight * -2f * gravity);
                StaminaBar.instance.UseStamina(25f);
                inair = true; //Enables in airmovement
                prevPos = move;

            }
            else if (Input.GetButtonDown("Jump") && isGrounded == true && StaminaBar.instance.currentStamina <= 24f)
            {
                Debug.Log("cant jump");
                inair = false;
                velocity.y = 0f;

            }
            else
            {
                inair = false;
            }

        }
        else if(!isGrounded && !inair) //In airmovement
        {
            
            //Takes the previous "move" input and makes it the deafult direction you're going in
            controller.Move(prevPos * Walkspeed * Time.deltaTime);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

    }
}
