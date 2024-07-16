shader_type canvas_item;

uniform vec4 blendColor : source_color;

vec4 color_burn(vec4 base, vec4 blend){
	return 1.0 - (1.0 - base) / blend;
}

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.
	COLOR = color_burn(COLOR,blendColor);
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}



