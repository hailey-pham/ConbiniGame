extends Node2D

@onready
var animPlayer = $AnimationPlayer

@onready
var audio = $AudioStreamPlayer

@onready
var sceneManager = $/root/SceneManager

# Called when the node enters the scene tree for the first time.
func _ready():
	animPlayer.play("slide")
	animPlayer.connect("animation_finished", on_anim_finished)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	if Input.is_action_pressed("Interact"):
		animPlayer.play("slide")
		animPlayer.advance(8)
		audio.stop()
		on_anim_finished("")
	pass

func on_anim_finished(_animName : String):
	sceneManager.ChangeScene("mainmenu","FadeToBlack")


