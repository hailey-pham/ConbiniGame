using Godot;
using Godot.Collections;
using System;
using System.Threading.Tasks;

public partial class SceneManager : Node
{
	[Export]
	public Dictionary scenes;

	[Export]
	public Dictionary transitions;

	private Node sceneParent;
	private bool transitioning = false;

	private string prevScene;
	private string currScene;

	[Signal]
	public delegate void SceneChangedEventHandler(string SceneName);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//load references to children components
		sceneParent = GetNode<Node>("SceneParent");

		//load the main menu when the game starts
		currScene = "splashscreen";
		LoadNewScene("splashscreen");

	}

	public async void ChangeScene(string sceneName, string transitionName = null)
	{
		if (!transitioning)
		{
			if (transitionName != null)
			{

				PackedScene transitionScene = LoadNewTransition(transitionName);
				SceneTransition transition = (SceneTransition)transitionScene.Instantiate();
				AddChild(transition);

				transitioning = true;
				await transition.PlayOutAsync();
				RemoveChildScenes();
				LoadNewScene(sceneName);
				await transition.PlayInAsync();
				transitioning = false;

				RemoveChild(transition);
			}
			else
			{
				RemoveChildScenes();
				LoadNewScene(sceneName);
			}
		}
	}

	private void RemoveChildScenes()
	{
		foreach (var child in sceneParent.GetChildren())
		{
			sceneParent.RemoveChild(child);
		}
	}

	private PackedScene LoadNewTransition(string transitionName)
	{
		try
		{
			PackedScene transition = (PackedScene)GD.Load((string)transitions[transitionName]);
			return transition;
		}
		catch (Exception)
		{
			GD.PrintErr("Transition not found!");
			return null;
		}
	}
	private void LoadNewScene(string sceneName)
	{
		try
		{
			PackedScene scene = (PackedScene)GD.Load((string)scenes[sceneName]);
			sceneParent.AddChild(scene.Instantiate());
			//announce what scene we've changed to
			//calendar uses this
			EmitSignal(nameof(SceneChanged), sceneName);
			prevScene = currScene;
			currScene = sceneName;
		}
		catch (Exception)
		{

			GD.PrintErr("Scene not found!");
		}
	}

	public string PrevScene { get => prevScene; set => prevScene = value; }
}
