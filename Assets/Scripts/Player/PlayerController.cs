using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private DefaultInput input;
    private CharacterController controller;
    
    [Header("Camera")] 
    public Camera cam;

    [Header("Movement")] 
    public float movementSpeed;
    private Vector2 inputMovement;
    private Vector3 newMovementSpeed;
    
    [Header("Gravity")]
    public float gravityAmount;
    public float gravityMin;
    private float playerGravity;
    
    [Header("Jumping")]
    public float jumpHeight;
    public float jumpFalloff;
    public float fallingSmoothing;

    [Header("References")] 
    public Transform heldObject;
    public Transform dropObjectTransform;
    private GameObject heldObjectRef;

    private bool isJumping;
    private bool isHeldObject = false;
    private Vector3 jumpingForce;
    private Vector3 jumpingForceVelocity;
    private float turnSmoothVelocity;
    
    private void Awake()
    {
        input = new DefaultInput();
        input.Player.Movement.performed += e => inputMovement = e.ReadValue<Vector2>();
        input.Player.Jump.performed += e => Jump();
        input.Player.Pickup.performed += e => PickupItem();

        controller = GetComponent<CharacterController>();
        
        input.Enable();
    }
    
    // Update is called once per frame
    void Update()
    {
        CalculatePlayerMovement();
        CalculateJump();
    }

    void CalculatePlayerMovement()
    {
        var vertSpeed = inputMovement.y;
        var horSpeed = inputMovement.x;
        Vector3 direction = new Vector3(horSpeed, 0f, vertSpeed);
        
        //take into account the current gravity affecting the player
        if (playerGravity > gravityMin)
        {
            playerGravity -= gravityAmount * Time.deltaTime;
        }

        if (playerGravity < -0.1f && controller.isGrounded)
        {
            playerGravity = -0.1f;
        }
        
        Vector3 movementDir = new Vector3();
            // rotate towards current target
        
        
        if (direction.magnitude >= 0.01f)
        {
            float targetRot = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            float rot = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRot, ref turnSmoothVelocity, 0.1f);
            transform.rotation = Quaternion.Euler(0f, rot, 0f);
            movementDir = Quaternion.Euler(0f, targetRot, 0f) * Vector3.forward;
        }
        
        //add constant gravity to the players up position
        movementDir.y += playerGravity;
        movementDir += jumpingForce;

        //call our controller to move towards our new position
        controller.Move(movementDir * movementSpeed * Time.deltaTime);
    }
    
    private void Jump()
    {    
        // return from function if we are currently in the air.
        if (!controller.isGrounded)
        {
            return;
        }
        
        jumpingForce = Vector3.up * jumpHeight;
        playerGravity = 0;
    }

    private void CalculateJump()
    {
        jumpingForce = Vector3.SmoothDamp(jumpingForce, Vector3.zero, ref jumpingForceVelocity,
            jumpFalloff);
    }

    private void PickupItem()
    {
        if (isHeldObject)
        {
            PlaceItem();
        }
  
        // trace infront of the player and if it hits something with an interactable tag
        // then we need to instantiate that object ontop of our held position
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 1))
        {
            var pickUp = hit.transform.gameObject.GetComponent<CubePickup>();
            if (pickUp)
            {
                heldObjectRef = Instantiate(hit.transform.gameObject, heldObject.transform.position, heldObject.transform.rotation);
                heldObjectRef.transform.SetParent(heldObject);
                
                Destroy(hit.transform.gameObject);
                isHeldObject = true;
                Debug.Log("Pick up item");
            }
        }
    }

    private void PlaceItem()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, 2))
        {
            var newTransform = new Vector3(dropObjectTransform.position.x, dropObjectTransform.position.y + 0.5f, dropObjectTransform.position.z);
            var tempObject = Instantiate(heldObjectRef, newTransform, new Quaternion(0,0,0,0));
            tempObject.transform.localScale = new Vector3(1,1,1);
            tempObject.transform.SetParent(null);
                            
            Destroy(heldObjectRef);
            Debug.Log("Drop Item");
            isHeldObject = false;
                            
        }
    }
}
