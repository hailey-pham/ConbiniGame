using Godot;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic; 

public partial class UpgradeManager : Control
{
	// signals
	[Signal] public delegate void BackButtonPressedEventHandler();
	[Signal] public delegate void UpgradeButtonPressedEventHandler();
	[Signal] public delegate void PurchaseButtonPressedEventHandler();
	[Signal] public delegate void ConfirmButtonPressedEventHandler();
	[Signal] public delegate void DenyButtonPressedEventHandler();
	[Signal] public delegate void InsufficientBackButtonPressedEventHandler();

	// exports
	[Export] public Label currentMoneyLabel;

	[Export] public GridContainer gridContainer;

	[Export] private Label upgradeNameLabel;
	[Export] private Label upgradeDescriptionLabel;
	[Export] private Label upgradeCostLabel;

	// buttons
	private TextureButton backButton;
	private Button purchaseButton;
	private Button confirmButton;
	private Button denyButton;
	private List<Button> upgradeButtons = new List<Button>();
	private Button insufficientBackButton;

	// other variables

	private Upgrade currentUpgrade;
	private globals globals;
	private CanvasLayer popUp;
	private Label upgradeQuestion;
	private CanvasLayer insufficientPopUp;


	public override void _Ready()
	{
		// get nodes
		globals = GetNode<globals>("/root/Globals");
		backButton = GetNode<TextureButton>("BackButton");
		purchaseButton = GetNode<Button>("UpgradeInfo/PurchaseButton");
		confirmButton = GetNode<Button>("PopUp/Theme/Yes");
		denyButton = GetNode<Button>("PopUp/Theme/No");
		popUp = GetNode<CanvasLayer>("PopUp");
		upgradeQuestion = GetNode<Label>("PopUp/Theme/Purchase Question");
		insufficientPopUp = GetNode<CanvasLayer>("InsufficientFundsPopUp");
		insufficientBackButton = GetNode<Button>("InsufficientFundsPopUp/Panel/Back");

		// set default visuals
		currentMoneyLabel.Text = string.Format("Current Funds: ￥"+globals.Money);
		popUp.Visible = false;
		insufficientPopUp.Visible = false;

		// TESTING
		// globals.Money = 10000;

		// declare buttons and functions
		backButton.Pressed += OnBackButtonPressed;
		purchaseButton.Pressed += OnPurchaseButtonPressed;
		confirmButton.Pressed += OnConfirmButtonPressed;
		denyButton.Pressed += OnDenyButtonPressed;
		insufficientBackButton.Pressed += OnInsufficientBackButtonPressed;

		Button temp;
		foreach (KeyValuePair<string, Upgrade> upgrade in globals.Upgrades) {
			if(!upgrade.Value.owned) {
				temp = new Button();
				temp.CustomMinimumSize = new Vector2(95,0);
				temp.ClipText = true;
				temp.TextOverrunBehavior = TextServer.OverrunBehavior.TrimEllipsis;
				// gridContainer.AddChild(temp);
				upgradeButtons.Add(temp);
				upgradeButtons[^1].Text = upgrade.Value.name;
				upgradeButtons[^1].Pressed += () => OnUpgradeButtonPressed(upgrade.Value.name);
				GD.Print(upgradeButtons[^1].Text);

				if(upgrade.Value.name == "Grand Counter" && !globals.Upgrades["More Counters"].owned) {
					upgradeButtons[^1].Visible = false;
				}
			}
		}
		sortButtons(upgradeButtons, 0, upgradeButtons.Count-1);

		foreach (Button btn in upgradeButtons) {
			gridContainer.AddChild(btn);
		}

	}

	private void swap(List<Button> arr, int i, int j)
	{
		Button temp = arr[i];
		arr[i] = arr[j];
		arr[j] = temp;
	}
	private int partition(List<Button> arr, int low, int high) {
		Button pivot = arr[high];

		int i = (low-1);
		for (int j = low; j<= high - 1; j++) {
			if(globals.Upgrades[arr[j].Text].cost < globals.Upgrades[pivot.Text].cost) {
				i++;
				swap(arr, i, j);
			}
		}
		swap(arr, i+1, high);
		return (i+1);
	}
	private void sortButtons(List<Button> arr, int low, int high) {
		if (low < high) {
			int pi = partition(arr, low, high);
			GD.Print(pi);
			sortButtons(arr, low, pi-1);
			sortButtons(arr,pi+1,high);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// update current funds
		currentMoneyLabel.Text = string.Format("Current Funds: ￥"+globals.Money);
	}

	private void OnBackButtonPressed()
    {
        GD.Print("Back button pressed!");
        // todo later: logic for getting upgrade menu up
        EmitSignal(nameof(BackButtonPressed));
        var sceneManager = GetNode<SceneManager>("/root/SceneManager");
        GD.Print(sceneManager);
		sceneManager.ChangeScene("endofdayscene");
    }

	private void OnUpgradeButtonPressed(string name)
	{
		EmitSignal(nameof(UpgradeButtonPressed));

		currentUpgrade = globals.Upgrades[name];
		upgradeNameLabel.Text = currentUpgrade.name;
		upgradeDescriptionLabel.Text = currentUpgrade.description;

		if(!currentUpgrade.owned) {
			upgradeCostLabel.Text = "Cost: ￥"+currentUpgrade.cost;
			purchaseButton.Disabled = false;
		} else {
			upgradeCostLabel.Text = "Cost: OWNED";
			purchaseButton.Disabled = true;
		}
	}

	private void OnPurchaseButtonPressed()
	{
		EmitSignal(nameof(PurchaseButtonPressed));
		if(globals.Money > currentUpgrade.cost) {
			upgradeQuestion.Text = "Do you want to purchase "+currentUpgrade.name+" for ￥"+currentUpgrade.cost+"?";
			popUp.Visible = true;
			
		} else {
			insufficientPopUp.Visible = true;
		}
	}

	private void OnConfirmButtonPressed()
	{
		EmitSignal(nameof(ConfirmButtonPressed));
		
		// if user has enough money to purchase upgrade, subtract money and set owned
		if(globals.Money >= currentUpgrade.cost) {
			globals.Money -= currentUpgrade.cost;
			globals.Upgrades[currentUpgrade.name].owned = true;

			// clear upgrade from current list

			foreach (Button btn in upgradeButtons) {
				if (btn.Text == "Grand Counter" && globals.Upgrades["More Counters"].owned) {
					btn.Visible = true;
				}
			}
			

			upgradeCostLabel.Text = "OWNED";
			purchaseButton.Disabled = true;


			currentMoneyLabel.Text = string.Format("Current Funds: ￥"+globals.Money);

		}
		popUp.Visible = false;
	}

	private void OnDenyButtonPressed()
	{
		EmitSignal(nameof(DenyButtonPressed));
		popUp.Visible = false;
	}

	private void OnInsufficientBackButtonPressed()
	{
		EmitSignal(nameof(InsufficientBackButtonPressed));
		insufficientPopUp.Visible = false;
	}
}
