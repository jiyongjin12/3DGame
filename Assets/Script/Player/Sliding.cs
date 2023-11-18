using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerObj;
    private Rigidbody rb;
    private PlayerMovement pm;

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    private float slideTimer;

    public float slideYScale;       // 슬라이드 중 사이즈 조정
    private float startYScale;      // 

    [Header("Input")]
    public KeyCode slideKey = KeyCode.LeftControl;
    private float horizontalInput;
    private float verticalInput;

    private bool sliding;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();

        startYScale = playerObj.localScale.y;
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal"); // a d
        verticalInput = Input.GetAxisRaw("Vertical"); // w, s

        if (Input.GetKeyDown(slideKey) && (horizontalInput != 0 || verticalInput != 0)) // 슬리아드 키를 누르고 (a d 또는 w s중 눌려 있다면) 실행
            StartSlide();

        if (Input.GetKeyUp(slideKey) && sliding)
            StopSlide();
    }

    private void FixedUpdate()
    {
        if (sliding)
            SlidingMovement();
    }

    private void StartSlide()
    {
        sliding = true;

        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideTimer = maxSlideTime;
    }

    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // Sliding normal
        if(!pm.OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;
        }
        else // sliding down a slope
        {
            rb.AddForce(pm.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
        } 

        if (slideTimer <= 0)
            StopSlide();
    }

    private void StopSlide()
    {
        sliding = false;

        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
    }

}
