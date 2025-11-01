using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
	[SerializeField] private GameObject toolSlotPrefab; // Single prefab
	[SerializeField] private Transform hotbarContainer; // The Hotbar empty GameObject
	[SerializeField] private int hotbarSize = 4; // How many slots to create

	private Transform[] hotbarSlots;
	private Tool[] hotbarInventory;
	private int currentToolIndex = 0;

	// Tools
	[SerializeField] private GameObject bucketModel;

	void Start()
	{
		CreateHotbarSlots();
		InitializeInventory();
	}

	private void CreateHotbarSlots()
	{
		hotbarSlots = new Transform[hotbarSize];
		hotbarInventory = new Tool[hotbarSize];

		for (int i = 0; i < hotbarSize; i++)
		{
			GameObject slot = Instantiate(toolSlotPrefab, hotbarContainer, false);
			slot.transform.localScale = Vector3.one;
			slot.name = "Slot " + i;
			hotbarSlots[i] = slot.transform;
		}
	}


	private void InitializeInventory()
	{
		// Load the PNG files directly as Texture2D first
		Texture2D axeTexture = Resources.Load<Texture2D>("Sprites/axe");
		Texture2D bucketTexture = Resources.Load<Texture2D>("Sprites/bucket");

		Debug.Log("Axe texture loaded: " + (axeTexture != null ? "YES" : "NO"));
		Debug.Log("Bucket texture loaded: " + (bucketTexture != null ? "YES" : "NO"));

		// If textures loaded, use them
		Sprite axeIcon = null;
		Sprite bucketIcon = null;

		if (axeTexture != null)
			axeIcon = Sprite.Create(axeTexture, new Rect(0, 0, axeTexture.width, axeTexture.height), Vector2.zero);
		if (bucketTexture != null)
			bucketIcon = Sprite.Create(bucketTexture, new Rect(0, 0, bucketTexture.width, bucketTexture.height), Vector2.zero);

		hotbarInventory[0] = null;
		hotbarInventory[1] = new Tool { toolName = "Axe", toolID = 0, toolIcon = axeIcon };
		hotbarInventory[2] = null;
		hotbarInventory[3] = new Tool { toolName = "Bucket", toolID = 1, toolIcon = bucketIcon };

		for (int i = 0; i < hotbarSize; i++)
		{
			UpdateSlotDisplay(i);
		}

		SelectTool(0);
	}


	private void UpdateSlotDisplay(int slotIndex)
	{
		Transform slot = hotbarSlots[slotIndex];
		Image icon = slot.Find("ToolIcon").GetComponent<Image>();
		TextMeshProUGUI toolName = slot.Find("ToolName").GetComponent<TextMeshProUGUI>();

		if (hotbarInventory[slotIndex] != null)
		{
			icon.sprite = hotbarInventory[slotIndex].toolIcon;
			toolName.text = hotbarInventory[slotIndex].toolName;
			icon.enabled = true;
		}
		else
		{
			icon.sprite = null;
			toolName.text = "";
			icon.enabled = false;
		}
	}

	public void AddItemToHotbar(Tool tool)
	{
		for (int i = 0; i < hotbarSize; i++)
		{
			if (hotbarInventory[i] == null)
			{
				hotbarInventory[i] = tool;
				UpdateSlotDisplay(i);
				Debug.Log("Added " + tool.toolName + " to slot " + i);
				return;
			}
		}
		Debug.Log("Hotbar is full!");
	}

	public void SelectTool(int slotIndex)
	{
		if (slotIndex < 0 || slotIndex >= hotbarSize) return;

		// Deselect previous
		hotbarSlots[currentToolIndex].GetComponent<Image>().color = new Color(0f, 0f, 0f, 40f / 255f); // 0,0,0,40 converted to 0-1 range

		// Select new
		currentToolIndex = slotIndex;
		hotbarSlots[currentToolIndex].GetComponent<Image>().color = new Color(0.5f, 0.7f, 1f);

		if (hotbarInventory[slotIndex] != null)
		{
			Debug.Log("Selected: " + hotbarInventory[slotIndex].toolName);
		}
		else
		{
			Debug.Log("Selected: Empty slot");
		}
	}

	public Tool GetCurrentTool()
	{
		// Returns null if empty slot is selected
		return hotbarInventory[currentToolIndex];
	}

	void Update()
	{
		// Number keys 1-4
		for (int i = 0; i < hotbarSize; i++)
		{
			if (Input.GetKeyDown(KeyCode.Alpha1 + i))
			{
				SelectTool(i);
			}
		}

		// Scroll wheel
		float scroll = Input.GetAxis("Mouse ScrollWheel");
		if (scroll > 0f) // Scroll up
		{
			SelectNextTool();
		}
		else if (scroll < 0f) // Scroll down
		{
			SelectPreviousTool();
		}
		UpdateEquippedItem();
	}

	private void SelectNextTool()
	{
		int nextSlot = (currentToolIndex + 1) % hotbarSize;
		SelectTool(nextSlot);
	}

	private void SelectPreviousTool()
	{
		int prevSlot = (currentToolIndex - 1 + hotbarSize) % hotbarSize;
		SelectTool(prevSlot);
	}

	private void UpdateEquippedItem()
	{
		Tool currentTool = GetCurrentTool();

		if (bucketModel != null) bucketModel.SetActive(false);

		if (currentTool != null && currentTool.toolName == "Bucket" && bucketModel != null)
			bucketModel.SetActive(true);
	}

}