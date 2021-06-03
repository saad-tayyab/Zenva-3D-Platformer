using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // access the HUD
    public HudManager hud;
    
    public float walkSpeed = 8f;
    public float jumpSpeed = 7f;
    public AudioSource coinAudioSource;
    
    //to keep our rigid body
    Rigidbody rb;
    
    //to keep the collider object
    Collider coll;
    
    //flag to keep track of whether a jump started
    bool pressedJump = false;
 
    // Use this for initialization
    void Start () {
        //get the rigid body component for later use
        rb = GetComponent<Rigidbody>();
        
        //get the player collider
        coll = GetComponent<Collider>();
        
        //refresh the HUD
        hud.Refresh();
    }

    // Update is called once per frame
    void Update()
    {
        // Handle player walking
        WalkHandler();
        
        //Handle player jumping
        JumpHandler();
    }
    
    // Make the player walk according to user input
    void WalkHandler()
    {
        // Set x and z velocities to zero
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
 
        //Distance ( speed = distance / time --> distance = speed * time)
        float distance = walkSpeed * Time.deltaTime;
 
        // Input on x ("Horizontal")
        float hAxis = Input.GetAxis("Horizontal");
 
        // Input on z ("Vertical")
        float vAxis = Input.GetAxis("Vertical");
 
        // Movement vector
        Vector3 movement = new Vector3(hAxis * distance, 0f, vAxis * distance);
 
        // Current position
        Vector3 currPosition = transform.position;
 
        // New position
        Vector3 newPosition = currPosition + movement;
 
        // Move the rigid body
        rb.MovePosition(newPosition);
    }
    
    // Check whether the player can jump and make it jump
    void JumpHandler()
    {
        // Jump axis
        float jAxis = Input.GetAxis("Jump");
        
        // Is grounded
        bool isGrounded = CheckGrounded();
 
        // Check if the player is pressing the jump key
        if (jAxis > 0f)
        {
            // Make sure we've not already jumped on this key press
            if(!pressedJump && isGrounded)
            {
                // We are jumping on the current key press
                pressedJump = true;
 
                // Jumping vector
                Vector3 jumpVector = new Vector3(0f, jumpSpeed, 0f);
 
                // Make the player jump by adding velocity
                rb.velocity = rb.velocity + jumpVector;
            }            
        }
        else
        {
            // Update flag so it can jump again if we press the jump key
            pressedJump = false;
        }
    }

    bool CheckGrounded()
    {
        // Object size in x
        var bounds = coll.bounds;
        var sizeX = bounds.size.x;
        var sizeZ = bounds.size.z;
        var sizeY = bounds.size.y;
 
        // Position of the 4 bottom corners of the game object
        // We add 0.01 in Y so that there is some distance between the point and the floor
        var position = transform.position;
        var corner1 = position + new Vector3(sizeX/2, -sizeY / 2 + 0.01f, sizeZ / 2);
        var corner2 = position + new Vector3(-sizeX / 2, -sizeY / 2 + 0.01f, sizeZ / 2);
        var corner3 = position + new Vector3(sizeX / 2, -sizeY / 2 + 0.01f, -sizeZ / 2);
        var corner4 = position + new Vector3(-sizeX / 2, -sizeY / 2 + 0.01f, -sizeZ / 2);
 
        // Send a short ray down the cube on all 4 corners to detect ground
        var grounded1 = Physics.Raycast(corner1, new Vector3(0, -1, 0), 0.01f);
        var grounded2 = Physics.Raycast(corner2, new Vector3(0, -1, 0), 0.01f);
        var grounded3 = Physics.Raycast(corner3, new Vector3(0, -1, 0), 0.01f);
        var grounded4 = Physics.Raycast(corner4, new Vector3(0, -1, 0), 0.01f);
 
        // If any corner is grounded, the object is grounded
        return (grounded1 || grounded2 || grounded3 || grounded4);
    }
    
    // This method will be called each time the player runs into a trigger collider.
    void OnTriggerEnter(Collider collider)
    {
        // Check if we ran into a coin
        if (collider.gameObject.CompareTag("Coin"))
        {
            print("Grabbing coin..");
            
            // Increase score
            GameManager.instance.IncreaseScore(1);
            
            // Play coin collection sound
            coinAudioSource.Play();
 
            // Destroy coin
            Destroy(collider.gameObject);
            
            //refresh the HUD
            hud.Refresh();
        }
        else if(collider.gameObject.CompareTag("Enemy"))
        {
            // Game over
            print("game over");
            
            Invoke(nameof(LaunchGameOverScene), 2.0f);
            
            
        }
        else if (collider.gameObject.CompareTag("Goal"))
        {
            // Next level
            print("next level");
            if (GameManager.instance.currentLevel < GameManager.instance.highestLevel)
            {
                GameManager.instance.IncreaseLevel();
            }
            else
            {
                // Game over
                print("game over");
            
                Invoke(nameof(LaunchGameOverScene), 1.0f);
            }
            
        }
    }

    // This method will be called when Player hits the enemy
    void LaunchGameOverScene()
    {
        // load game over scene
        SceneManager.LoadScene("Game Over");
    }
}
