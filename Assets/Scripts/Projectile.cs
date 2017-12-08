using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Projectile : MonoBehaviour
{

    public float projectileSpeed;
    public GameObject explosionPrefab;

    private EnemyAI enemyAI;

    // Use this for initialization
    void Start()
    {
        enemyAI = FindObjectOfType<EnemyAI>();
    }

    // Update is called once per frame
    void Update()
    {
        float amtToMove = projectileSpeed * Time.deltaTime;
        transform.Translate(Vector3.up * amtToMove);

        if (transform.position.y > 6.4f)
        {
            Destroy(transform.parent.gameObject);
            if (this.transform.parent.childCount == 2)
            {
                enemyAI.ProjectileEvasionScore++;
                Player.comboPoints = 0;
                Player.UpdateComboStats();
            }
        }
    }

    void OnTriggerEnter(Collider otherObject)
    {
        if (otherObject.tag == "Enemy")
        {
            Enemy enemy = (Enemy)otherObject.gameObject.GetComponent("Enemy");
            if (enemy.transform.position.y == 7f)
            {
                Destroy(transform.parent.gameObject);
                return;
            }

            Player.score += 100;
            Player.UpdatePlayerStats();
            Player.comboPoints++;
            Player.UpdateComboStats();
            if (Player.score >= 50000)
            {
                SceneManager.LoadScene("Win");
            }


            Instantiate(explosionPrefab, transform.position, transform.rotation);
            enemy.minSpeed += 0.01f;
            enemy.maxSpeed += 0.015f;
            enemy.SetPositionAndSpeed();
            Destroy(gameObject);
        }
        else if (otherObject.tag == "EnemyAI")
        {
            EnemyAI enemy = (EnemyAI)otherObject.gameObject.GetComponent("EnemyAI");
            enemy.Reset();
            Instantiate(explosionPrefab, transform.position, transform.rotation);
        }

    }

    private void OnDestroy()
    {
        if(this.transform.parent.childCount == 1)
        {
            Destroy(transform.parent.gameObject);
        }
    }


}
