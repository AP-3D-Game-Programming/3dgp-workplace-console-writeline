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
			// Parse the current task text to extract the total
			string text = subtaskText.text;

			// Look for pattern like "0/5" or "1/5"
			if (text.Contains("/"))
			{
				int slashIndex = text.LastIndexOf('/');
				int spaceBeforeSlash = text.LastIndexOf(' ', slashIndex);

				if (spaceBeforeSlash >= 0 && slashIndex > spaceBeforeSlash)
				{
					// Extract total value after the slash
					string afterSlash = text.Substring(slashIndex + 1).Trim();
					int total;

					// Parse just the number (ignore any text after it)
					string[] parts = afterSlash.Split(' ');
					if (int.TryParse(parts[0], out total))
					{
						// Update the progress
						current = currentValue;
						this.total = total;

						// Rebuild the text with updated progress
						string beforeProgress = text.Substring(0, spaceBeforeSlash + 1);
						subtaskText.text = $"{beforeProgress}{current}/{total}";

						// Auto-complete when progress reaches total
						if (current >= total)
						{
							MarkComplete();
						}
					}
				}
			}
		}
	}


	public string GetTaskText()
	{
		return subtaskText != null ? subtaskText.text : "";
	}

}
