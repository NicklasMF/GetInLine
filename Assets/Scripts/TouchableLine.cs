	using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchableLine : MonoBehaviour {

	[HideInInspector] public int queueNo;

	void Start() {
		queueNo = transform.parent.GetComponent<LineScript>().queueNo;
	}
}
