using Godot;
using System;

public partial class OrderInformation : Node
{
	public ItemRes item;

	public BoxContainer parent;
	TextureRect texture;
	Label itemLabel;
	Label itemCount;
	public OrderInformation() {

	}

	public OrderInformation(ItemRes _item) {
		GD.Print("Order Information: "+_item.name);
		item = _item;

		parent = new BoxContainer();
		texture = new TextureRect();
		itemLabel = new Label();
		itemCount = new Label();

		texture.Texture = item.spriteTexture;
		itemLabel.Text = item.name;
		itemCount.Text = "x"+item.restockAmount;
		
		parent.AddChild(texture);
		parent.AddChild(itemLabel);
		parent.AddChild(itemCount);
	}

	public void updateInformation() {
		itemCount.Text = "x"+item.restockAmount;
		if(item.currentStock <= 0) {
			parent.Visible = false;
		} else {
			parent.Visible = true;
		}
	}
	
}
