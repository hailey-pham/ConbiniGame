using Godot;
using System;

public partial class Item : Node2D
{
    public ItemRes itemRes;
    private Sprite2D sprite;
    public override void _Ready()
    {
        sprite = GetNode<Sprite2D>("Sprite2D");
        if(itemRes != null )
        {
            sprite.Texture = itemRes.spriteTexture;
        }
    }
   
}
