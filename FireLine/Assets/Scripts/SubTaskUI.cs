using UnityEngine;
using TMPro;

public class SubtaskUI : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI subtaskText;

	private string baseText;
	private int current = 0;
	private int total = 0;
	private bool isComplete = false;

	public void Setup(string text)
	{
		baseText = text;
		subtaskText.text = text;
		isComplete = false;
	}

	public void SetupWithProgress(string text, int current, int total)
	{
		baseText = text;
		this.current = current;
		this.total = total;
		isComplete = false;
		UpdateProgressText();
	}

	public void AddProgress()
	{
		if (current < total)
		{
			current++;
			UpdateProgressText();

			// Auto-complete when progress reaches total
			if (current >= total)
			{
				MarkComplete();
			}
		}
	}

	private void UpdateProgressText()
	{
		subtaskText.text = $"{baseText} {current}/{total}";
	}

	public void MarkComplete()
	{
		isComplete = true;
		subtaskText.fontStyle = FontStyles.Strikethrough;
		subtaskText.color = Color.green;
	}

	public bool IsComplete()
	{
		return isComplete;
	}

	public void UpdateProgress(int currentValue)
	{
		if (subtaskText != null)
		{
			// Extract the total from the text
			string[] parts = subtaskText.text.Split('/');
			if (parts.Length > 1)
			{
				int total = int.Parse(parts[1]);
				subtaskText.text = $"Chop {currentValue} wood  {currentValue}/{total}";

				if (currentValue >= total)
				{
					MarkComplete();
				}
			}
		}
	}

	public string GetTaskText()
	{
		return subtaskText != null ? subtaskText.text : "";
	}

}
