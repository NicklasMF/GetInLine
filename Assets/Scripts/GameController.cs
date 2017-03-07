using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	// 
	public bool gameStarted;

	// People AI //
	[SerializeField] GameObject manPrefab;
	[SerializeField] int peopleToInstantiate;
	int peopleInstantiated = 0;
	[SerializeField] float spawnStart;
	[SerializeField] float spawnDelay;

	Transform spawnpointPos;
	Transform player;

	void Start() {
		spawnpointPos = GameObject.FindGameObjectWithTag("Spawnpoint").transform;
		player = GameObject.FindGameObjectWithTag("Player").transform;
		StartCoroutine(FirstSpawn());
		gameStarted = false;
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			StartGame();
		}

		if ( Input.GetMouseButtonDown(0)){
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit, 100.0f)){
				if (hit.collider.transform.tag == "TouchableLine"){
					int queue = hit.collider.transform.GetComponent<TouchableLine>().queueNo;
					PlayerGoTo(queue);

				}
			}
		}
	}

	IEnumerator FirstSpawn() {
		yield return new WaitForSeconds(spawnStart);

		if (peopleInstantiated < peopleToInstantiate) {
			Instantiate();
			StartCoroutine(SpawnMan());
			peopleInstantiated++;
		}
	}

	IEnumerator SpawnMan() {
		yield return new WaitForSeconds(spawnDelay);

		if (peopleInstantiated < peopleToInstantiate) {
			Instantiate();
			StartCoroutine(SpawnMan());
			peopleInstantiated++;
		}
	}

	void Instantiate() {
		GameObject man = Instantiate(manPrefab, spawnpointPos.transform.position, Quaternion.identity);
		man.name = manPrefab.name;
		man.transform.SetParent(GameObject.Find("World").transform.Find("People").transform);
		print("Ny mand oprettet");
	}

	public Vector3 GetLinePos(int _queue) {
		GameObject[] lines = GameObject.FindGameObjectsWithTag("Line");
		foreach(GameObject line in lines) {
			if (line.GetComponent<LineScript>() != null) {
				if (line.GetComponent<LineScript>().queueNo == _queue) {
					return line.transform.position;
				}
			}
		}
		return Vector3.zero;
	}

	public void PlayerGoTo(int _lineNo) {
		player.GetComponent<PlayerScript>().SetQueue(_lineNo);
		//StartGame();
	}

	public void StartGame() {
		gameStarted = true;
		GameObject[] people = GameObject.FindGameObjectsWithTag("Customer");
		foreach(GameObject person in people) {
			person.GetComponent<People>().AllowServe(true);

		}

	}

}
