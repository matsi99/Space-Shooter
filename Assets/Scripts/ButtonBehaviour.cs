using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonBehaviour : MonoBehaviour {

	public void LoadLevelByIndex(int levelIndex)
    {
        ResetStats();
        SceneManager.LoadScene(levelIndex);
    }

    public void LoadLevelByName(string levelName)
    {
        ResetStats();
        SceneManager.LoadScene(levelName);
    }

    public void ResetStats()
    {
        Player.score = 0;
        Player.lives = 3;
        Player.missed = 0;
    }

    private void Update()
    {
        StartCoroutine(checkAnyKey());
    }

    IEnumerator checkAnyKey()
    {
        yield return new WaitForSeconds(0.9f);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        else if (Input.anyKeyDown)
        {
            ResetStats();
            SceneManager.LoadScene("Level1");
        }
    }
}
