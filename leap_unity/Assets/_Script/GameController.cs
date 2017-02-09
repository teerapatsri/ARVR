using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
	public GameObject[] hazards;
	public Vector3 spawnValues;
	public int hazardCount;
	public float spawnWait;
	public float startWait;
	public float waveWait; 

	public Text scoreText;
	public Text restartText;
	public Text gameOverText;

	private bool gameOver;
	private bool restart;
	private int score;
    private int hp=3;

    void Start ()
	{
		gameOver = false;
		restart = false;
		restartText.text = "HP : 3";
		gameOverText.text = "";
		score = 0;
		UpdateScore ();
		StartCoroutine (SpawnWaves ());
	}

	void Update ()
	{

        if (score >= 20)
        {
            gameOverText.text = "You Win !!!";
            restart = true;
            gameOver = true;
        }

        if (hp<=0)
        {
            gameOverText.text = "You Lose T-T";
            restart = true;
            gameOver = true;
        }

        if (restart)
		{
			if (Input.GetKeyDown (KeyCode.R))
			{
				Application.LoadLevel (Application.loadedLevel);
			}
		}
	}

	IEnumerator SpawnWaves ()
	{
		yield return new WaitForSeconds (startWait);
		while (true)
		{
            
            for (int i = 0; i < hazardCount; i++)
			{
				GameObject hazard = hazards[Random.Range (0, hazards.Length)];
				Vector3 spawnPosition = new Vector3 (Random.Range (-spawnValues.x, spawnValues.x), Random.Range(-spawnValues.y, spawnValues.y), spawnValues.z);
				Quaternion spawnRotation = Quaternion.identity;
				Instantiate (hazard, spawnPosition, spawnRotation);
				yield return new WaitForSeconds (spawnWait);
			}
			yield return new WaitForSeconds (waveWait);



			if (gameOver)
			{
				restartText.text = "Press 'R' for Restart";
				restart = true;
				break;
			}
		}
	}

	public void AddScore (int newScoreValue)
	{
		score += newScoreValue;
		UpdateScore ();
	}

	void UpdateScore ()
	{
		scoreText.text = "Score: " + score;
	}

    public void hpDown()
    {
        hp -= 1;
        UpdateHp();
    }

    void UpdateHp()
    {
        restartText.text = "HP: " + hp;
    }

    public void GameOver ()
	{
		gameOverText.text = "Game Over!";
		gameOver = true;
	}
}