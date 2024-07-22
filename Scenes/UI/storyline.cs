using Godot;
using System;
using System.Threading.Tasks;

public partial class storyline : Control
{
    private RichTextLabel dialogueText;
    private Button nextButton;
    private string[] _dialogueLines;
    private int currentLineIndex = 0;

    private float textDelay = 0.05f;
    private bool isTyping = false;

    private SceneManager sceneManager;

    [Signal]
    public delegate void DialogueFinishedEventHandler();

    public override void _Ready()
    {
        sceneManager = GetNode<SceneManager>("/root/SceneManager");
        dialogueText = GetNode<RichTextLabel>("DialogueText");

        DialogueFinished += OnDialogueFinished;

    }

    private async void OnInteractPressed()
    {
        if (isTyping)
        {
            isTyping = false;
            dialogueText.Text = _dialogueLines[currentLineIndex];
        }
        else
        {
            currentLineIndex++;
            await ShowDialogueLine();
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
       if (@event.IsActionPressed("Interact"))
        {
            OnInteractPressed();
        }
    }

    public void ShowDialogue(string[] dialogueLines)
    {
        GD.Print("ShowDialogue called with lines: ", dialogueLines.Length);

        _dialogueLines = dialogueLines;
        currentLineIndex = 0;
        ShowDialogueLine();
    }

    private async Task ShowDialogueLine()
    {
        if (currentLineIndex < _dialogueLines.Length)
        {
            string line = _dialogueLines[currentLineIndex];
            await TypeText(line);
        }
        else
        {
            EmitSignal(nameof(DialogueFinished));
            Hide();
        }
    }

    public void SetDialogue(string[] dialogueLines)
    {
        _dialogueLines = dialogueLines;
        currentLineIndex = 0;
        ShowDialogueLine();
    }

    private async Task TypeText(string text)
    {
        dialogueText.Text = "";
        isTyping = true;
        foreach (char c in text)
        {
            if (!isTyping)
                break;

            dialogueText.Text += c;
            await ToSignal(GetTree().CreateTimer(textDelay), "timeout");
        }

        isTyping = false;
    }
    private void OnDialogueFinished()
    {
        var sceneManager = GetNode<SceneManager>("/root/SceneManager");
        sceneManager.ChangeScene("seasontitle", "FadeToBlack");
    }
}
