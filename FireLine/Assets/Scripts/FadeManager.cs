using UnityEngine;
using System.Collections;

public class FadeManager : MonoBehaviour
{
	public static FadeManager Instance { get; private set; }

	[SerializeField] private CanvasGroup fadeCanvasGroup; // Black overlay
	[SerializeField] private float fadeDuration = 1f; // How long fade takes

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
    fadeCanvasGroup.transform.SetAsLastSibling(); // Puts it on top
    fadeCanvasGroup.alpha = 0f;
    Debug.Log("FadeManager initialized. Alpha: " + fadeCanvasGroup.alpha);
}


	public IEnumerator FadeToBlackAndBack(System.Action onFullyBlack = null)
	{
		Debug.Log("Starting fade...");

		// Fade to black
		yield return StartCoroutine(FadeIn());

		// Now fully black - call the callback to update tasks
		if (onFullyBlack != null)
		{
			onFullyBlack.Invoke();
		}


		yield return new WaitForSeconds(0.5f);

		// Fade back
		yield return StartCoroutine(FadeOut());

		Debug.Log("Fade complete");
	}


	private IEnumerator FadeIn()
	{
		float elapsed = 0f;
		while (elapsed < fadeDuration)
		{
			elapsed += Time.deltaTime;
			fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
			yield return null;
		}
		fadeCanvasGroup.alpha = 1f;
	}

	private IEnumerator FadeOut()
	{
		float elapsed = 0f;
		while (elapsed < fadeDuration)
		{
			elapsed += Time.deltaTime;
			fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
			yield return null;
		}
		fadeCanvasGroup.alpha = 0f;
	}
}
