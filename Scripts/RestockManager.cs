using Godot;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;


public partial class RestockManager : Node
{	
	 [Signal] public delegate void BackButtonPressedEventHandler();

	 [Signal] public delegate void ItemButtonPressedEventHandler();

	 [Signal] public delegate void AddButtonPressedEventHandler();

	 [Signal] public delegate void SubtractButtonPressedEventHandler();

	 [Signal] public delegate void InsufficientBackButtonPressedEventHandler();

	 [Signal] public delegate void PurchaseButtonPressedEventHandler();

	private TextureButton backButton;

	private Button insufficientBackButton;

	private CanvasLayer insufficientPopUp;

	private Button purchaseButton;
	

	[Export] Label totalCostLabel;
	Label currentMoneyLabel;

	Label currentItemLabel;
	TextureRect currentItemSprite;
	Label currentItemSellLabel;
	Label currentItemRestockLabel;
	Label currentItemCurrentStock;
	Label currentItemPurchaseAmount;
	private globals globals;

	ItemRes currentItem;

	private HBoxContainer itemContainer;

	private GridContainer purchasedItemContainer;

	private List<TextureButton> itemButtons = new List<TextureButton>();

	private Button addButton;
	private Button subtractButton;

	private List<OrderInformation> purchaseList = new List<OrderInformation>();
	public override void _Ready()
	{
		// declare nodes
		globals = GetNode<globals>("/root/Globals");
		backButton = GetNode<TextureButton>("Visuals/BackButton");
		itemContainer = GetNode<HBoxContainer>("VBoxContainer/ItemScroll/HBoxContainer");
		currentMoneyLabel = GetNode<Label>("Visuals/CurrentMoney");
		purchasedItemContainer = GetNode<GridContainer>("FullOrder/Control/ScrollContainer/GridContainer");
		insufficientPopUp = GetNode<CanvasLayer>("InsufficientFundsPopUp");
		insufficientBackButton = GetNode<Button>("InsufficientFundsPopUp/Panel/Back");
		purchaseButton = GetNode<Button>("FullOrder/Purchase");

		currentItemLabel = GetNode<Label>("ItemInformation/ItemName");
		currentItemSprite = GetNode<TextureRect>("ItemInformation/ItemSprite");
		currentItemSellLabel = GetNode<Label>("ItemInformation/Sell Cost");
		currentItemRestockLabel = GetNode<Label>("ItemInformation/RestockCost");
		currentItemCurrentStock = GetNode<Label>("ItemInformation/CurrentStock");
		currentItemPurchaseAmount = GetNode<Label>("ItemInformation/AmountToOrder/PurchaseAmount");

		addButton = GetNode<Button>("ItemInformation/AmountToOrder/More");
		subtractButton = GetNode<Button>("ItemInformation/AmountToOrder/Less");
		insufficientPopUp.Visible = false;

		currentItem = globals.Stock["Apple"];

		backButton.Pressed += OnBackButtonPressed;
		addButton.Pressed += OnAddButtonPressed;
		subtractButton.Pressed += OnSubtractButtonPressed;
		insufficientBackButton.Pressed += OnInsufficientBackButtonPressed;
		purchaseButton.Pressed += OnPurchaseButtonPressed;

		// link scroll bars
		GetNode<ScrollContainer>("VBoxContainer/ItemScroll").GetHScrollBar().Share(GetNode<HScrollBar>("VBoxContainer/HScrollBar"));

		TextureButton temp;
		TextureRect tempTexture;
		foreach (KeyValuePair<string, ItemRes> item in globals.Stock) {
				

				// add button 
				temp = new TextureButton();
				temp.TextureNormal = GD.Load<Texture2D>("res://Assets/ButtonBG.png");
				temp.TexturePressed = GD.Load<Texture2D>("res://Assets/ButtonBGPressed.png");
				temp.TextureFocused = GD.Load<Texture2D>("res://Assets/ButtonBGPressed.png");
				temp.SetAnchorsPreset(Control.LayoutPreset.Center);
				itemButtons.Add(temp);
				itemButtons[^1].Pressed += () => OnItemButtonPressed(item.Value.name);
				itemContainer.AddChild(temp);

				// add texture on button
				tempTexture = new TextureRect();
				tempTexture.Texture = item.Value.spriteTexture;
				tempTexture.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.Center);
				temp.AddChild(tempTexture);

				// add to purchase list
				purchaseList.Add(new OrderInformation(item.Value));
				purchasedItemContainer.AddChild(purchaseList[^1].parent);
				// purchaseList[^1].sendOffChildren(purchasedItemContainer);
				
		}

		

		updateItemInfo();
		updateTotalItemList();
		currentMoneyLabel.Text = string.Format("Current Funds: ￥"+globals.Money);

	}

	private void updateItemInfo() {
		currentItemLabel.Text = "Item: "+currentItem.name;
		currentItemSprite.Texture = currentItem.spriteTexture;
		currentItemSellLabel.Text = "￥"+currentItem.price;
		currentItemRestockLabel.Text = "￥"+currentItem.restockPrice;
		currentItemCurrentStock.Text = ""+currentItem.currentStock; 
		currentItemPurchaseAmount.Text = ""+currentItem.restockAmount;
	}

	private void updateTotalItemList() {
		foreach(OrderInformation oi in purchaseList) {
			oi.updateInformation();
		}
	}

	private void GiveMoney() {
		globals.Money = 30;
		globals.Stock["Water"].currentStock = 2;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		totalCostLabel.Text = string.Format("￥"+globals._purchaseCost);
	}

	public void completePurchase() {
		// check if user has enough funds to restock
		// add restock amounts to global inventory
		if(globals.Money >= globals._purchaseCost) {
			globals.Money -= globals._purchaseCost;
			globals._purchaseCost = 0;
			foreach (var item in globals.Stock) {
				item.Value.currentStock += item.Value.restockAmount;
				item.Value.restockAmount = 0;
			}
			updateItemInfo();
			updateTotalItemList();
			currentMoneyLabel.Text = string.Format("Current Funds: ￥"+globals.Money);
			
		} else {
			GD.Print("not enough money for purchase");
			insufficientPopUp.Visible = true;
		}	
	}
	
	
	private void OnBackButtonPressed()
	{
		EmitSignal(nameof(BackButtonPressed));
		var sceneManager = GetNode<SceneManager>("/root/SceneManager");
		sceneManager.ChangeScene(sceneManager.PrevScene);
	}

	private void OnItemButtonPressed(string itemName)
	{
		EmitSignal(nameof(ItemButtonPressed));
		currentItem = globals.Stock[itemName];
		updateItemInfo();
	}

	private void OnAddButtonPressed() 
	{
		if(globals.Stock[currentItem.name].restockAmount < 99) {
			globals.Stock[currentItem.name].restockAmount += 1;
			globals._purchaseCost += globals.Stock[currentItem.name].restockPrice;
			updateTotalItemList();
			currentItemPurchaseAmount.Text = ""+currentItem.restockAmount;
		}
		
	}

	private void OnSubtractButtonPressed()
	{
		if(globals.Stock[currentItem.name].restockAmount > 0) {
			globals.Stock[currentItem.name].restockAmount -= 1;
			globals._purchaseCost -= globals.Stock[currentItem.name].restockPrice;
			updateTotalItemList();
			currentItemPurchaseAmount.Text = ""+currentItem.restockAmount;
		}
	}

	private void OnInsufficientBackButtonPressed()
	{
		EmitSignal(nameof(InsufficientBackButtonPressed));
		insufficientPopUp.Visible = false;
	}

	private void OnPurchaseButtonPressed()
	{
		EmitSignal(nameof(PurchaseButtonPressed));
		completePurchase();
	}
}
