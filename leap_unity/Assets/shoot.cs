using UnityEngine;
using System.Collections;

[System.Serializable]
/*public class Boundary
{
	public float xMin, xMax, zMin, zMax;
}
*/



public class shoot : MonoBehaviour
{

	public GameObject shot;
	public GameObject shot2;
	//public Transform shotSpawn;
	public float fireRate;

	private float nextFire;

	public AudioSource audio;

	public float m_ExplosionForce = 1000f;              // The amount of force added to a tank at the centre of the explosion.
	//public float m_MaxLifeTime = 2f;                    // The time in seconds before the shell is removed.
	public float m_ExplosionRadius = 5f;                // The maximum distance away from the explosion tanks can be and are still affected.
	public ParticleSystem m_ExplosionParticles;         // Reference to the particles that will play on explosion.
	public AudioSource m_ExplosionAudio;                // Reference to the audio that will play on explosion.

	public GameController gameController;
    public float createPosZ;

	void Update ()
	{
		if (Input.GetButton("Fire1") && Time.time > nextFire)
		{
			nextFire = Time.time + fireRate;

			var mousePos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, createPosZ);
			//mousePos.z = 0;
			var screenMousePos = Camera.main.ScreenToWorldPoint (mousePos);
			//print(mousePos+"\t"+screenMousePos);

			Instantiate(shot, screenMousePos, Quaternion.Euler(new Vector3(0, 0, 0)));
			audio.Play ();
		}else if (Input.GetButton("Fire2") && Time.time > nextFire)
		{
			nextFire = Time.time + fireRate;

			var mousePos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 1);
			//mousePos.z = 0;
			var screenMousePos = Camera.main.ScreenToWorldPoint (mousePos);
			//print(mousePos+"\t"+screenMousePos);

			//Instantiate(shot2, screenMousePos, Quaternion.Euler(new Vector3(0, 0, 0)));
			//audio.Play ();

			// Play the particle system.
			Instantiate(m_ExplosionParticles, new Vector3(0,0,3), Quaternion.Euler(new Vector3(0, 0, 0)));

			// Play the explosion sound effect.
			m_ExplosionAudio.Play();

			Bomb ();

		}
	}

	GameObject[] gameObjects;

	void Bomb()
	{
		
		gameObjects = GameObject.FindGameObjectsWithTag ("Enemy");

		for(var i = 0 ; i < gameObjects.Length ; i ++)
		{
            if (gameObjects[i].transform.position.z< 17)
            {
                gameController.AddScore(1);
                Destroy(gameObjects[i]);
            }
		}
	}

}