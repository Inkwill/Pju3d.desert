using UnityEngine;
using UnityEngine.UI;
using CreatorKitCode;
using TMPro;

public class UIHudBase : MonoBehaviour
{
	void Update()
	{
		this.transform.forward = Camera.main.transform.forward;
	}
}
