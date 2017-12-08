using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    #region Fields
    public float currentSpeed;
    public float minSpeed;
    public float maxSpeed;
    private float x, y, z;

    private float MinRotateSpeed = 60f;
    private float MaxRotateSpeed = 120f;
    private float MinScale = 0.8f;
    private float MaxScale = 2f;
    private float currentRotationSpeed;
    private float currentScaleX;
    private float currentScaleY;
    private float currentScaleZ;
    private float currentRotationX;
    private float currentRotationY;
    #endregion

    private void Awake()
    {
        SetPositionAndSpeed();
    }

    void Update()
    {
        float rotationSpeed = currentRotationSpeed * Time.deltaTime;
        transform.Rotate(
            new Vector3(currentRotationX, currentRotationY, 0) * rotationSpeed);

        float amtToMove = currentSpeed * Time.deltaTime;
        transform.Translate(Vector3.down * amtToMove, Space.World);

        if (transform.position.y <= -5)
        {
            Player.missed++;
            Player.UpdatePlayerStats();
            SetPositionAndSpeed();
        }
    }

    public void SetPositionAndSpeed()
    {
        currentSpeed = Random.Range(minSpeed, maxSpeed);
        currentRotationSpeed = Random.Range(MinRotateSpeed, MaxRotateSpeed);
        currentScaleX = Random.Range(MinScale, MaxScale);
        currentScaleY = Random.Range(MinScale, MaxScale);
        currentScaleZ = Random.Range(MinScale, MaxScale);
        currentRotationX = Random.Range(0.3f, 1);
        currentRotationX = Random.Range(0, 1) > 0.5 ? -currentRotationX : currentRotationX;
        currentRotationY = Random.Range(0.3f, 1);
        currentRotationY = Random.Range(0, 1) > 0.5 ? -currentRotationY : currentRotationY;

        x = Random.Range(-6f, 6f);
        y = 7f;
        z = 0f;
        transform.position = new Vector3(x, y, z);
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(-30,30));
        transform.localScale = new Vector3(currentScaleX, currentScaleY, currentScaleZ);
    }
}