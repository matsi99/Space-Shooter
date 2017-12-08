using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour {

    public GameObject enemyPrefab;
    private int lastScore = 0;
    private static int enemyCount = 0;

	// Use this for initialization
	void Start () {
        Instantiate(enemyPrefab);
        enemyCount++;
	}
	
	// Update is called once per frame
	void Update () {
		if(Player.score > (lastScore+ (300*enemyCount*enemyCount)) && enemyCount < 16)
        {
            lastScore = Player.score;
            Instantiate(enemyPrefab);
            enemyCount++;
        }
	}

    public void ResetEnemies()
    {
        var enemies = FindObjectsOfType<Enemy>();
        foreach (var enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
        Instantiate(enemyPrefab);
        enemyCount = 1;
    }
}
