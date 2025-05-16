shader_type canvas_item;
uniform vec3 monochrome_color : source_color = vec3(0.145, 0.145, 0.145);
uniform vec3 target_color : source_color = vec3(0, 0, 1);
uniform float color_grace : hint_range(0.001, 2.0) = 0.001;
uniform float saturation : hint_range(0.0, 1.0) = 0.0;
uniform float contrast : hint_range(0.0, 2.0) = 1.0;
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture;

bool CompareVector(vec3 _vec1) {
	if (_vec1.r > target_color.r - color_grace && _vec1.g > target_color.g - color_grace && _vec1.b > target_color.b - color_grace) {
		if (_vec1.r < target_color.r + color_grace && _vec1.g < target_color.g + color_grace && _vec1.b < target_color.b + color_grace) {
			return true;
		}
	}
	return false;
}

void fragment() {
	vec4 tex_color = texture(SCREEN_TEXTURE, SCREEN_UV);

	vec3 result;
	if (CompareVector(tex_color.rgb)) {
		result = tex_color.rgb;
	} else {
		// Mezcla gris y color original
		vec3 gray = vec3(dot(tex_color.rgb, monochrome_color));
		result = mix(gray, tex_color.rgb, saturation);

		// Aplicar contraste (1.0 = sin cambio)
		result = (result - 0.5) * contrast + 0.5;
	}

	COLOR.rgb = clamp(result, 0.0, 1.0);
	COLOR.a = tex_color.a;
}
