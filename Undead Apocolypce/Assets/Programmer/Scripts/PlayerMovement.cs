using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Samuel Pettersson
// * Unity Version: 2020.3.16
// * IDE: Visual Studio Community 2017
// * Date: October 2021
// * Instructions: This script will add Playermovement

public class PlayerMovement : MonoBehaviour
{
    //Movement varibles
    [Header("Movement Varibles")]
    public float Walkspeed = 4f;
    public float CrouchSpeed = 2f;
    public float SlideSpeed = 8f;
    public float Runspeed = 7f;
    public float Jumpheight = 3f;

    [Space]
    [Header("Charather Controller")]
    public CharacterController controller;

    public static PlayerMovement instance;

    //Positions
    Vector3 prevPos;

    //Groundcheck
    [Space]
    [Header("Groundchecks")]
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance = 0.4f;
    public bool isGrounded;
    bool inair = false;
    
    //Gravity
    [Space]
    [Header("Gravity")]
    Vector3 velocity;
    public float gravity = -9.82f;


    // Update is called once per frame
    void Update()
    {
        //Check if grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        //Keyboard Input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z; 


        if (isGrounded == true)
        {

            
            //Sprint
            if (Input.GetKey(KeyCode.LeftShift) && isGrounded == true && StaminaBar.instance.currentStamina >= 0.1f)
            {

                controller.Move(move * Runspeed * Time.deltaTime);
                StaminaBar.instance.UseStamina(.1f);

                //Sliding
                if(Input.GetKey(KeyCode.X) && isGrounded == true)
                {
                    controller.Move(move * SlideSpeed * Time.deltaTime);
                    controller.height = 0.5f;
                    StaminaBar.instance.UseStamina(.5f);
                    isGrounded = false;

                }
            } //Sprint - Out of stamina
            else if (Input.GetKey(KeyCode.LeftShift) && StaminaBar.instance.currentStamina <= 0f)
            {
                controller.Move(move * Walkspeed * Time.deltaTime);
            }
            //Crouching
            else if(Input.GetKey(KeyCode.LeftControl) && isGrounded == true)
            {
                controller.height = .75f;
                controller.Move(move * CrouchSpeed * Time.deltaTime);
            }
            //Walking
            else
            {
                controller.Move(move * Walkspeed * Time.deltaTime);
                controller.height = 2f;
            }

            //Jumping
            if (Input.GetButtonDown("Jump") && isGrounded == true && StaminaBar.instance.currentStamina >= 25f)
            {
                velocity.y = Mathf.Sqrt(Jumpheight * -2f * gravity);
                StaminaBar.instance.UseStamina(25f);
                inair = true; //Enables in airmovement
                prevPos = move;

            }//Jumping - Out of stamina
            else if (Input.GetButtonDown("Jump") && isGrounded == true && StaminaBar.instance.currentStamina <= 24f)
            {
                Debug.Log("cant jump");
                inair = false;
                velocity.y = 0f;

            }
            else //For Airmovement
            {
                inair = false;
            }

        }
        else if (!isGrounded && !inair) //In airmovement
        {

            //Takes the previous "move" input and makes it the deafult direction you're going in
            controller.Move(prevPos * Walkspeed * Time.deltaTime);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

    }
}
