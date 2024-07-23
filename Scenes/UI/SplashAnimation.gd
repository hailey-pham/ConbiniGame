extends Node2D

@onready
var animPlayer = $AnimationPlayer

@onready
var audio = $AudioStreamPlayer

@onready
var sceneManager = $/root/SceneManager

@onready
var globals = $/root/Globals

var is_game_loaded = false
var is_anim_finished = false

# Called when the node enters the scene tree for the first time.
func _ready():
	animPlayer.play("slide")
	animPlayer.connect("animation_finished", on_anim_finished)
	globals.GameLoaded.connect(on_game_loaded)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	if Input.is_action_pressed("Interact"):
		animPlayer.play("slide")
		animPlayer.advance(8)
		audio.stop()
		on_anim_finished("")
	pass

func on_anim_finished(_animName : String):
	if(is_game_loaded):
		sceneManager.ChangeScene("mainmenu","FadeToBlack")
	is_anim_finished = true

func on_game_loaded():
	is_game_loaded = true
	if(is_anim_finished):
		sceneManager.ChangeScene("mainmenu","FadeToBlack")
