using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SubtaskUI : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI subtaskText;

	private string baseText; // Store original text
	private int current = 0;
	private int total = 0;

	public void Setup(string text)
	{
		baseText = text;
		subtaskText.text = text;
	}

	// Setup with progress tracking
	public void SetupWithProgress(string text, int current, int total)
	{
		baseText = text;
		this.current = current;
		this.total = total;
		UpdateProgressText();
	}

	// Update progress (call this when task completes)
	public void AddProgress()
	{
		if (current < total)
		{
			current++;
			UpdateProgressText();
		}
	}

	private void UpdateProgressText()
	{
		subtaskText.text = $"{baseText} {current}/{total}";
	}

	public void MarkComplete()
	{
		subtaskText.fontStyle = FontStyles.Strikethrough;
		subtaskText.color = Color.green;
	}
}
