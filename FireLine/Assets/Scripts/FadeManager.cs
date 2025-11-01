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

	// Fade to black and back (for sleeping)
	public IEnumerator FadeToBlackAndBack()
	{
		// Fade to black
		yield return StartCoroutine(FadeIn());

		// Wait while black (this is when day resets)
		yield return new WaitForSeconds(1f);

		// Fade back to normal
		yield return StartCoroutine(FadeOut());
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
