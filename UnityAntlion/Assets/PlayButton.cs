using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButton : MonoBehaviour {

	public int index;
	public void onClick() {
		Application.LoadLevel (index);
	}
}
