//Author = Barış Kıvanç
using UnityEngine;

public class GunMovement : MonoBehaviour
{
    [Space]
    [Space]
    [Header("\t \t \t witnn's Gun Movement Script")]    
    [Space]
    
    public CharacterController character;
    public float minMoveSpeed = 0.1f; // minimum speed for movement
    public float minSprintSpeed = 4; // minimum speed for sprint movement
 
    [Header("Weapon Bob with Delay")]
    public bool weaponBobAndDelayActive;
    public Vector3 restPosition; 
    public float transitionSpeed = 3;
    float bobSpeed;
    public float n_bobSpeed = 6.3f, s_bobSpeed = 9;
    float bobAmount;
    public float n_bobAmount = 0.015f, s_bobAmount = 0.05f;
    float timer = Mathf.PI / 2;     
    public bool invertDelayDirection;
    public float d_Amount = 0.01f;
    public float d_MaxAmount = 0.03f;
    public float d_Smooth = 12;
    private Vector3 locPos;
    
    [Header("Weapon Sprint with Rotate")]
    public bool sprintAndRotActive;
    bool isSprint;
    public float sprintDamp;   
    public Vector3 normalRot;
    public Vector3 sprintRot;
    
    public float r_amount = 25f;
    public float r_maxAmount = 45f;
    public float r_smooth = 60;

    private void Start()
    {
        locPos = transform.localPosition;           
    }
 
    void Update()
    {
        if(isSprint == true)
        {
            bobSpeed = s_bobSpeed;
            bobAmount = s_bobAmount;
        }
        else
        {
            bobSpeed = n_bobSpeed;
            bobAmount = n_bobAmount;
        }

        //Weapon Sprint       
        if (sprintAndRotActive == true)
        {
            var move = character.isGrounded == true && character.velocity.magnitude > minSprintSpeed;
            if (move)
            {               
                Quaternion newRot = Quaternion.Euler(sprintRot.x, sprintRot.y, sprintRot.z);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, newRot, sprintDamp * Time.deltaTime);
                isSprint = true;
            }
            else
            {              
                Quaternion newRot = Quaternion.Euler(normalRot.x, normalRot.y, normalRot.z);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, newRot, sprintDamp * Time.deltaTime);
                isSprint = false;
            }            
            
            float r_factorZ = -(Input.GetAxis("Horizontal")) * r_amount;
            if (r_factorZ > r_maxAmount)
                r_factorZ = r_maxAmount;

            if (r_factorZ < -r_maxAmount)
                r_factorZ = -r_maxAmount;

            if ((Input.GetAxis("Horizontal")) != 0)
            {
                Vector3 rot = transform.localRotation.eulerAngles;
                rot.z = Mathf.Lerp(rot.z, rot.z + r_factorZ, Time.deltaTime);
                transform.localRotation = Quaternion.Euler(rot);
            }         
        }
    }
    
    private void FixedUpdate()
    {                
        //Weapon Bob
        if (weaponBobAndDelayActive == true)
        {
            var move = character.isGrounded == true && character.velocity.magnitude > minMoveSpeed;
            if (move) 
            {
                timer += bobSpeed * Time.deltaTime;
               
                Vector3 newPosition = new Vector3(Mathf.Cos(timer) * bobAmount, restPosition.y + Mathf.Abs((Mathf.Sin(timer) * bobAmount)), restPosition.z);
                transform.localPosition = Vector3.Lerp(transform.localPosition, newPosition, Time.deltaTime * transitionSpeed);
            }
            else
            {
                timer = Mathf.PI / 2; 

                Vector3 newPosition = new Vector3(restPosition.x, restPosition.y, restPosition.z); 
                transform.localPosition = Vector3.Lerp(transform.localPosition, newPosition, Time.deltaTime * transitionSpeed);
            }

            if (timer > Mathf.PI * 2) 
                timer = 0;
        }

        // Weapon Delay System
       
        if (Cursor.lockState == CursorLockMode.None) return;

        float factorX = -Input.GetAxis("Mouse X") * d_Amount;
        float factorY = -Input.GetAxis("Mouse Y") * d_Amount;   

        if (invertDelayDirection == true)
        {
            factorX *= -1;
            factorY *= -1;
        }

        factorX = Mathf.Clamp(factorX, -d_MaxAmount, d_MaxAmount);
        factorY = Mathf.Clamp(factorY, -d_MaxAmount, d_MaxAmount);

        //Vector3 target = new Vector3(locPos.x + factorX, locPos.y + factorY, locPos.z);
        var loc = transform.localPosition;

        loc.x = Mathf.Lerp(loc.x, loc.x + factorX, Time.fixedDeltaTime * d_Smooth);
        loc.y = Mathf.Lerp(loc.y, loc.y + factorY, Time.fixedDeltaTime * d_Smooth);
        transform.localPosition = loc;

    }
}
