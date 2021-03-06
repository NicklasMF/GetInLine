﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

	[SerializeField] float speed;

	public bool isBeingServed;
	bool allowMove;
	int queueNo;
	Vector3 startPos, nextPos, endPos;
	Quaternion targetRot;
	bool isOnLine;

	GameObject gameController;

	void Start() {
		SetDefault();
		gameController = GameObject.FindGameObjectWithTag("GameController");
	}

	void Update() {
		if (!allowMove || isBeingServed) {
			return;
		}

		if (transform.rotation != targetRot) {
			Rotate();
		} else if (transform.position == nextPos) {
			nextPos = FindNextTarget();
		}

		if (!isOnLine) {
			if (startPos != nextPos) {
				isOnLine = true;
				gameController.GetComponent<GameController>().StartGame();
			}
		}

		if (!isOnLine) {
			Move();
		} else if (transform.rotation == targetRot) {
			if (NextPosEmpty()){
				Move();
			}
		}


		// Remove //
		if (transform.position == endPos) {
			isBeingServed = true;

		}
	}

	void Move() {
		transform.position = Vector3.MoveTowards(transform.position, nextPos, Time.deltaTime * speed);
	}

	void Rotate() {
		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 100* Time.deltaTime);
	}

	bool CanMoveForward() {
		Collider[] coll = Physics.OverlapBox(transform.position + new Vector3(0,1,0) + transform.forward, new Vector3(0.1f, 0.3f, 0.1f));
		if (coll.Length != 0) {
			return false;
		} else {
			return true;
		}
	}

	bool NextPosEmpty() {
		Collider[] coll = Physics.OverlapBox(nextPos + new Vector3(0,1,0), new Vector3(0.1f, 0.3f, 0.1f));
		if (coll.Length != 0) {
			if (coll.Length == 1) {
				if (coll[0].gameObject == gameObject) {
					return true;
				} else {
					return false;
				}
			}
			return false;
		} else {
			return true;
		}
	}

	Vector3 FindNextTarget() {
		foreach(GameObject tile in GameObject.FindGameObjectsWithTag("Line")) {
			if (tile.transform.position == transform.position) {
				if (tile.transform.name == "LineEnd") {
					return endPos;
				}
				targetRot = tile.transform.rotation;
				Vector3 pos = tile.transform.position + tile.transform.forward;
				if (pos == endPos) {
					isBeingServed = true;
				}
				return pos;
			}
		}
		return transform.position;
	}

	void SetDefault() {
		allowMove = false;

		// Line AI //
		GameObject[] lines = GameObject.FindGameObjectsWithTag("Line");
		foreach(GameObject line in lines) {
			if (transform.position == line.transform.position) {
				isOnLine = true;
				nextPos = FindNextTarget();
			}

			if (line.name == "LineBeginning" && !isOnLine) {
				if (line.GetComponent<LineScript>().queueNo == queueNo) {
					targetRot = line.transform.localRotation;
					startPos = line.transform.position;
					nextPos = startPos;
				}
			} else if (line.name == "LineEnd") {
				endPos = line.transform.position;

				if (line.transform.position == transform.position) {
					isBeingServed = true;
				}
			}
		}

	}

	public void AllowMove(bool _bool) {
		allowMove = _bool;
	}

	public void SetQueue(int _no) {
		queueNo = _no;
		nextPos = gameController.GetComponent<GameController>().GetLinePos(_no);
		AllowMove(true);
	}

}
