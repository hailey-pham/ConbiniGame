using Godot;
using System;
using System.Diagnostics;

public partial class OrderInformation : Node
{
	public ItemRes item;

	public BoxContainer parent;
	TextureRect texture;
	Label itemLabel;
	Label itemCount;

	private int labelSize = 60;
	public OrderInformation() {

	}

	public OrderInformation(ItemRes _item, int _labelSize = 60) {
		item = _item;

		parent = new BoxContainer();
		texture = new TextureRect();
		itemLabel = new Label();
		itemCount = new Label();

		texture.Texture = item.spriteTexture;
		texture.CustomMinimumSize = new Vector2(16,0);
		texture.StretchMode = TextureRect.StretchModeEnum.KeepCentered;
		itemLabel.Text = item.name;
		itemLabel.CustomMinimumSize = new Vector2(_labelSize,0);
		itemLabel.TextOverrunBehavior = TextServer.OverrunBehavior.TrimEllipsis;
		itemCount.Text = "x"+item.restockAmount;
		
		parent.AddChild(texture);
		parent.AddChild(itemLabel);
		parent.AddChild(itemCount);
	}

	public void updateInformation() {
		itemCount.Text = "x"+item.restockAmount;
		if(item.restockAmount <= 0) {
			parent.Visible = false;
			texture.Visible = false;
			itemLabel.Visible = false;
			itemCount.Visible = false;
		} else {
			parent.Visible = true;
			texture.Visible = true;
			itemLabel.Visible = true;
			itemCount.Visible = true;
		}

	}

	public void sendOffChildren(Control node) {
		node.AddChild(texture);
		node.AddChild(itemLabel);
		node.AddChild(itemCount);
		GD.Print(node.GetChildren());
		GD.Print("Bye bye!");
	}
	
}
