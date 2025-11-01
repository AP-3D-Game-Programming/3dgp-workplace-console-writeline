using UnityEngine;
using TMPro;

public class TimeManager : MonoBehaviour
{
	public static TimeManager Instance { get; private set; }

	[SerializeField] private float dayDurationInSeconds = 300f;
	[SerializeField] private TextMeshProUGUI timeDisplay;
	[SerializeField] private Light sun;

	private float currentTimeInDay = 0f;
	private bool dayEnded = false;

	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	void Start()
	{
		if (sun == null)
			sun = Object.FindFirstObjectByType<Light>();

		sun.transform.rotation = Quaternion.Euler(0, 0, 0);
	}

	void Update()
	{
		currentTimeInDay += Time.deltaTime / dayDurationInSeconds;

		if (currentTimeInDay >= 1f)
		{
			currentTimeInDay = 0f;
			dayEnded = true;
			Debug.Log("Midnight! Day ended!");
		}

		// Rotate sun
		float sunRotation = currentTimeInDay * 360f;
		sun.transform.rotation = Quaternion.Euler(sunRotation, 0f, 0f);

		UpdateTimeDisplay();
	}

	private void UpdateTimeDisplay()
	{
		if (timeDisplay != null)
		{
			int hour = Mathf.FloorToInt(currentTimeInDay * 24f);
			int minute = Mathf.FloorToInt((currentTimeInDay * 24f - hour) * 60f);
			timeDisplay.text = hour.ToString("D2") + ":" + minute.ToString("D2");
		}
	}

	public bool HasDayEnded()
	{
		return dayEnded;
	}

	public void ResetDay()
	{
		currentTimeInDay = 0f;
		dayEnded = false;
	}

	public float GetTimeRemainingInSeconds()
	{
		return (1f - currentTimeInDay) * dayDurationInSeconds;
	}
}
