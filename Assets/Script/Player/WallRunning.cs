using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce;
    public float maxWallRunTime;
    private float wallRunTimer;

    [Header("Input")]
    private float horizontalInput;
    private float verticalInput;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallhit;
    private RaycastHit rightWallhit;
    private bool wallLeft;
    private bool wallRight;

    [Header("References")]
    public Transform orientation;
    private PlayerMovement pm;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        CheckForWall();
        StateMechine();
    }

    private void FixedUpdate()
    {
        if(pm.wallrunning)
        {
            WallRuningMovement();
        }
    }

    private void CheckForWall() // ���� ��Ҵ°�?
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallhit, wallCheckDistance, whatIsWall); // ������, ����, ��Ʈ ���� ����, �Ÿ�
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallhit, wallCheckDistance, whatIsWall); // ������, ����, ��Ʈ ���� ����, �Ÿ�
    }

    private bool AboveGround() // �� Ż��ŭ ���� �� �ֳ�? 
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void StateMechine()
    {
        // Getting Inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Wallrunning
        if((wallLeft || wallRight) && verticalInput > 0 && AboveGround())
        {
            // �� �޸��� ����
            if (!pm.wallrunning)
            {
                StartWallRun();
            }
            else
            {
                if (pm.wallrunning)
                {
                    StopWallRun();
                }
            }
        }
    }

    private void StartWallRun()
    {
        pm.wallrunning = true;
    }

    private void WallRuningMovement()
    {
        rb.useGravity = false;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        // forward force
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);
    }

    private void StopWallRun()
    {
        pm.wallrunning = false;
    }
}
