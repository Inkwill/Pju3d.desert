using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : MonoBehaviour
{
	public string name;
	public GameObject talkCam;


	void Start()
	{
		if (talkCam) talkCam.SetActive(false);
	}

	// Update is called once per frame
	public void StartTalk()
	{
		if (talkCam) talkCam.SetActive(true);
	}

	public void EndTalk()
	{

		if (talkCam) talkCam.SetActive(false);
	}
}
