shader_type canvas_item;
uniform vec3 monochrome_color : source_color = vec3(0.145, 0.145, 0.145);
uniform vec3 target_color : source_color = vec3(0,0,1);
uniform float color_grace : hint_range(0.001, 2.0)  = 0.001;
uniform float saturation : hint_range(0.0, 1.0) = 0;
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture;

bool CompareVector( vec3 _vec1 ) {
	bool fuck = false;
	if (_vec1.rgb.r > target_color.r-color_grace && _vec1.rgb.g > target_color.g-color_grace && _vec1.rgb.b > target_color.b-color_grace) {
		if (_vec1.rgb.r < target_color.r+color_grace && _vec1.rgb.g < target_color.g+color_grace && _vec1.rgb.b < target_color.b+color_grace) {
			fuck = true;
		}
	}
	return fuck;
}

void fragment() {
    vec4 tex_color = texture(SCREEN_TEXTURE, SCREEN_UV);
	if (CompareVector(tex_color.rgb)) {
		COLOR.rgb = tex_color.rgb;
	} else {
    	COLOR.rgb = mix(vec3(dot(tex_color.rgb, monochrome_color)), tex_color.rgb, saturation);
	}
	COLOR.a = tex_color.a;
}

//Thought this was cool, it could probably be made better but it works