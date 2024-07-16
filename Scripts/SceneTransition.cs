using Godot;
using System;
using System.Threading.Tasks;

public abstract partial class SceneTransition : Node
{
    public abstract Task PlayOutAsync();
    public abstract Task PlayInAsync();
}
