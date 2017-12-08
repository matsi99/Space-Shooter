using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stars : MonoBehaviour {

    public float speed;

    private void Update()
    {
        float amtToMove = speed * Time.deltaTime;
        transform.Translate(Vector3.down * amtToMove, Space.World);

        if(transform.position.y < -10.75f)
        {
            transform.position = new Vector3(transform.position.x, 14f, transform.position.z);
        }
    }
}
