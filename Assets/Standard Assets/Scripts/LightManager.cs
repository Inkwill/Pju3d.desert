using System;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
	[SerializeField, Header("Managed Objects")] private Light DirectionalLight = null;
	//[SerializeField] private LightPreset DayNightPreset, LampPreset;
	//private List<Light> SpotLights = new List<Light>();

	[SerializeField, Range(0, 1440), Header("Modifiers"), Tooltip("The game's current time of day")] private float TimeOfDay;
	[SerializeField, Tooltip("Angle to rotate the sun")] private float SunDirection = 170f;
	[SerializeField, Tooltip("How fast time of day will go")] private float DayTimeMultiplier = 2;
	[SerializeField, Tooltip("How fast time of night will go")] private float NightTimeMultiplier = 2;
	//[SerializeField] private bool ControlLights = true;

	private const float inverseDayLength = 1f / 1440f;

	public string TimeInfo { get { TimeSpan timeSpan = TimeSpan.FromMinutes(TimeOfDay); return $"{(int)timeSpan.TotalHours:D2}:{timeSpan.Minutes:D2}"; } }
	/// <summary>
	/// On project start, if controlLights is true, collect all non-directional lights in the current scene and place in a list
	/// </summary>
	private void Start()
	{
		TimeOfDay = 1000;
		// if (ControlLights)
		// {
		// 	Light[] lights = FindObjectsOfType<Light>();
		// 	foreach (Light li in lights)
		// 	{
		// 		switch (li.type)
		// 		{
		// 			case LightType.Disc:
		// 			case LightType.Point:
		// 			case LightType.Rectangle:
		// 			case LightType.Spot:
		// 				SpotLights.Add(li);
		// 				break;
		// 			case LightType.Directional:
		// 			default:
		// 				break;
		// 		}
		// 	}
		// }
	}

	/// <summary>
	/// This method will not run if there is no preset set
	/// On each frame, this will calculate the current time of day factoring game time and the time multiplier (1440 is how many minutes exist in a day 24 x 60)
	/// Then send a time percentage to UpdateLighting, to evaluate according to the set preset, what that time of day should look like
	/// </summary>
	private void Update()
	{
		// if (DayNightPreset == null)
		// 	return;
		float timePercent = TimeOfDay * inverseDayLength;
		float TimeMultiplier = DayTimeMultiplier;
		int intensityMax = 3;
		if (timePercent < 0.35 || timePercent > 0.85) { TimeMultiplier = NightTimeMultiplier; }
		TimeOfDay = TimeOfDay + (Time.deltaTime * TimeMultiplier);
		TimeOfDay = TimeOfDay % 1440;
		DirectionalLight.intensity = (float)(Math.Pow(Math.Sin(TimeOfDay * inverseDayLength * Math.PI), 0.5f) * intensityMax);
	}

	/// <summary>
	/// Based on the time percentage recieved, set the current scene's render settings and light coloring to the preset
	/// In addition, rotate the directional light (the sun) according to the current time
	/// </summary>
	/// <param name="timePercent"></param>
	// private void UpdateLighting(float timePercent)
	// {
	// 	RenderSettings.ambientLight = DayNightPreset.AmbientColour.Evaluate(timePercent);
	// 	RenderSettings.fogColor = DayNightPreset.FogColour.Evaluate(timePercent);

	// 	//Set the directional light (the sun) according to the time percent
	// 	if (DirectionalLight != null)
	// 	{
	// 		if (DirectionalLight.enabled == true)
	// 		{
	// 			DirectionalLight.color = DayNightPreset.DirectionalColour.Evaluate(timePercent);
	// 			DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, SunDirection, 0));
	// 		}
	// 	}

	// 	//Go through each spot light, ensure it is active, and set it's color accordingly
	// 	foreach (Light lamp in SpotLights)
	// 	{
	// 		if (lamp != null)
	// 		{
	// 			if (lamp.isActiveAndEnabled && lamp.shadows != LightShadows.None && LampPreset != null)
	// 			{
	// 				lamp.color = LampPreset.DirectionalColour.Evaluate(timePercent);
	// 			}
	// 		}
	// 	}
	// }
}