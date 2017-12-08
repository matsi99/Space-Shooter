using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    public float playerSpeed;
    public GameObject projectilePrefab;
    public GameObject explosionPrefab;
    public GameObject shield;
    public static int score = 0;
    public static int lives = 3;
    public static Text playerStats;
    public static Text comboStats;
    public static int comboPoints = 0;
    private const int maxComboPoints = 12;
    public static int missed = 0;
    public float projectileOffset;
    private float shipInvisibleTime = 1.5f;
    private float shipMoveOnToScreenSpeed = 5f;
    private float blinkRate = .1f;
    private int numberOfTimesToBlink = 10;
    private int blinkCount;
    private int shieldPoints = 3;
    private int shieldRechargeTime = 5;
    private float shieldRechargeTimeCurrent = 0;
    private static bool comboActive = false;

    enum State
    {
        Playing,
        Explosion,
        Invincible
    };
    private State state = State.Playing;

    // Use this for initialization
    void Start()
    {
        playerStats = GameObject.Find("PlayerStats").GetComponent<Text>();
        UpdatePlayerStats();
        comboStats = GameObject.Find("ComboStats").GetComponent<Text>();
        UpdateComboStats();

    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Explosion)
            return;

        float amtToMove = Input.GetAxis("Horizontal") * playerSpeed * Time.deltaTime;
        transform.Translate(Vector3.right * amtToMove, Space.World);

        transform.rotation = Quaternion.Euler(Input.GetAxis("Vertical") * 30,
             -Input.GetAxis("Horizontal") * 30, 0
            );

        amtToMove = Input.GetAxis("Vertical") * playerSpeed * Time.deltaTime;
        transform.Translate(Vector3.up * amtToMove, Space.World);

        if (transform.position.y < -2.7f)
        {
            transform.position = new Vector3(transform.position.x, -2.7f, transform.position.z);
        }
        else if (transform.position.y > 3f)
        {
            transform.position = new Vector3(transform.position.x, 3f, transform.position.z);
        }

        if (transform.position.x < -7.4f)
        {
            transform.position = new Vector3(7.4f, transform.position.y, transform.position.z);
        }
        else if (transform.position.x > 7.4f)
        {
            transform.position = new Vector3(-7.4f, transform.position.y, transform.position.z);
        }

        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown("space") || (comboActive && (Input.GetButton("Fire1") || Input.GetKey("space"))))
        {
            Vector3 position = new Vector3(transform.position.x, transform.position.y + projectileOffset, transform.position.z);
            Instantiate(projectilePrefab, position, Quaternion.identity);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }

        updateShield();
        updateComboActive();

    }

    private void updateShield()
    {
        if (shieldPoints < 3)
        {
            if (shieldRechargeTimeCurrent < shieldRechargeTime)
            {
                shieldRechargeTimeCurrent += Time.deltaTime;
            }
            else
            {
                shieldPoints++;
                shieldRechargeTimeCurrent = 0;
            }
        }

        if (shieldPoints > 0)
        {
            shield.SetActive(true);
            switch (shieldPoints)
            {
                case 3:
                    shield.GetComponent<Renderer>().material.color = new Color(0, 1, 0.5f, 0.24f);
                    break;
                case 2:
                    shield.GetComponent<Renderer>().material.color = new Color(1, 0.92f, 0, 0.24f);
                    break;
                case 1:
                    shield.GetComponent<Renderer>().material.color = new Color(1, 0.5f, 0, 0.24f);
                    break;
            }
        }
        else
        {
            shield.SetActive(false);
        }

    }

    private void updateComboActive()
    {
        if (comboPoints >= maxComboPoints)
        {
            StartCoroutine(comboTimer());
        }
    }

    IEnumerator comboTimer()
    {
        comboActive = true;
        comboStats.fontSize = 32;
        comboStats.color = new Color(0.0f,0.4f,1f);
        comboStats.fontStyle = FontStyle.Bold;
        UpdateComboStats();
        yield return new WaitForSeconds(5);
        comboActive = false;
        comboPoints = 0;
        comboStats.fontSize = 14;
        comboStats.color = new Color(0.95f, 0.95f, 0.95f);
        comboStats.fontStyle = FontStyle.Normal;
        UpdateComboStats();
    }

    public static void UpdatePlayerStats()
    {
        playerStats.text = "Score: " + score.ToString()
            + "\nLives: " + lives.ToString()
            + "\nMissed: " + missed.ToString();

    }

    public static void UpdateComboStats()
    {
        if(comboStats == null)
        {
            return;
        }

        if (comboActive)
        {
            comboStats.text = "Combo Active";
        }
        else
        {
            comboStats.fontSize = 14 + comboPoints;
            comboStats.text = "Combo: " + comboPoints.ToString() + "/" + maxComboPoints.ToString();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && state == State.Playing)
        {
            comboPoints = 0;
            UpdateComboStats();
            if (shieldPoints > 0)
            {
                shieldPoints--;
                Instantiate(explosionPrefab, other.transform.position, Quaternion.identity);
            }
            else
            {
                lives--;
                UpdatePlayerStats();
                StartCoroutine(DestroyShip(other.transform));
            }

            Enemy enemy = (Enemy)other.gameObject.GetComponent("Enemy");
            enemy.SetPositionAndSpeed();
        }
    }

    IEnumerator DestroyShip(Transform t)
    {
        blinkCount = 0;
        state = State.Explosion;
        Instantiate(explosionPrefab, t.position, Quaternion.identity);
        transform.position = new Vector3(0f, -5.5f, transform.position.z);
        transform.rotation = Quaternion.identity;

        yield return new WaitForSeconds(shipInvisibleTime);

        if (lives > 0)
        {
            while (transform.position.y < -2.2)
            {
                float amtToMove = shipMoveOnToScreenSpeed * Time.deltaTime;
                transform.position = new Vector3(0, transform.position.y + amtToMove, transform.position.z);
                yield return 0;
            }
            state = State.Invincible;
            while (blinkCount < numberOfTimesToBlink)
            {
                gameObject.GetComponent<Renderer>().enabled = !gameObject.GetComponent<Renderer>().enabled;
                if (gameObject.GetComponent<Renderer>().enabled)
                {
                    blinkCount++;
                }
                yield return new WaitForSeconds(blinkRate);
            }
            state = State.Playing;
        }
        else
        {
            SceneManager.LoadScene("Lose");
        }
    }
}