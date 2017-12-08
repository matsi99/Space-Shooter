using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class ShooterAI : MonoBehaviour
    {
        public float speed = 10;
        private float movementX = 0;
        private float shootDelay = 0.3f;
        private float currentShootDelay = 0;
        private EnemyAI enemyAI;

        public GameObject projectilePrefab;
        public float projectileOffset;

        private void Start()
        {
            enemyAI = FindObjectOfType<EnemyAI>();
        }

        private void Update()
        {
            movementX = AI.Neuron.Sigmoid(enemyAI.transform.position.x - transform.position.x);

            float amtToMove = speed * Time.deltaTime;
            transform.Translate(new Vector3(movementX, 0, 0) * amtToMove, Space.World);

            if (transform.position.x > 6.3f)
            {
                //movementX = -movementX;
                transform.position = new Vector3(6.3f, transform.position.y, transform.position.z);
            }
            else if (transform.position.x < -6.3f)
            {
                //movementX = -movementX;
                transform.position = new Vector3(-6.3f, transform.position.y, transform.position.z);
            }

            currentShootDelay += Time.deltaTime;

            if (currentShootDelay >= shootDelay)
            {
                currentShootDelay = 0;
                Vector3 position = new Vector3(transform.position.x, transform.position.y + projectileOffset, transform.position.z);
                Instantiate(projectilePrefab, position, Quaternion.identity);
            }

        }
    }
}
