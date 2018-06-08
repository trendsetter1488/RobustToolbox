tool
extends EditorPlugin

# Incoming: pain
# GDScript is a fucking turd compared to C#
# How can people actually defend this shit?
# People like me want to get actual work done ffs.
# I literally found a plugin on the VSCode marketplace with better autocomplete and linting than Godot's fucking editor.
# And then Godot's devs wonder why people are put off by it: BECAUSE IT FUCKING SUCKS TO WORK WITH.
# THIS GIVES ME FLASHBACKS TO FUCKING BYOND.
# God I'm glad we didn't fully transition to Godot.

# I still love you Godot I had to vent my anger.
# But seriously this shit is unacceptable. Get your shit together.
# That's my anger vented.

var panel
var has_content_repo = false

func _ready():
	print("Do the thing!")

func _enter_tree():
	check_if_has_content_repo()
	create_model_dirs()
	init_panel()

func _exit_tree():
	if panel != null:
		remove_control_from_bottom_panel(panel)
		panel.free()

func init_panel():
	panel = Button.new()
	panel.text = "Sync SS14 assets"
	panel.connect("pressed", self, "_on_sync_button_pressed")
	add_control_to_bottom_panel(panel, "honk!")

func create_model_dirs():
	var dir = Directory.new()
	dir.make_dir_recursive("res://models/engine")
	if has_content_repo:
		dir.make_dir_recursive("res://models/content")

func _on_sync_button_pressed():
	print("joy")
	var indir = get_ss14_content_dir()
	indir.change_dir("Resources/Models")

	var outdir = Directory.new()
	outdir.open("res://models/content")

	for file in recursively_find_files(indir):
		print(file)
		var inpath = indir.get_current_dir() + "/" + file
		var outpath = outdir.get_current_dir() + "/" + file
		if outdir.file_exists(file):
			# When your language is too cheap to use static.
			var filedummygodwhy = File.new()
			var hash1 = filedummygodwhy.get_md5(inpath)
			var hash2 = filedummygodwhy.get_md5(outpath)
			if hash1 == hash2:
				print("skipping")
				continue

		# God fucking damnit this copy function lied and doesn't take in relative paths correctly.
		outdir.copy(inpath, outpath)


func check_if_has_content_repo():
	var dir = get_ss14_content_dir()
	var files = dir_get_file_list(dir)
	has_content_repo = files.has("Content.Server") and files.has("Content.Client") and files.has("Content.Shared") and files.has("engine")


func get_ss14_content_dir():
	var dir = Directory.new()
	dir.open("../..")
	return dir

func get_ss14_engine_dir():
	var dir = Directory.new()
	dir.open("..")
	return dir

# Iterator returning the list of files in a directory
# Excludes . and ..
func dir_get_file_list(directory):
	# I should not have had to code this.
	# Literally who the fuck thought this was a good API.
	# The thing is I know *why* the API is like this and
	# I can fucking tell you there is no reason to give a shit about it when you're programming in *fucking GDScript*
	# Jesus christ EVEN BYOND HAS A FUNCTION FOR THIS.
	var array = Array()
	directory.list_dir_begin(true, true)
	var path
	while true:
		path = directory.get_next()
		if path == "":
			directory.list_dir_end()
			return array

		array.append(path)

	return null


func recursively_find_files(directory):
	var out = Array()
	_recursively_find_files_internal(directory, out, "")
	return out


func _recursively_find_files_internal(directory, outfiles, currentdirrelative):
	for file in dir_get_file_list(directory):
		var relpath = currentdirrelative + file
		if directory.dir_exists(file):
			var subdir = Directory.new()
			subdir.open(directory.get_current_dir() + "/" + file)
			_recursively_find_files_internal(subdir, outfiles, relpath + "/")

		else:
			outfiles.append(relpath)

# I lied when I said my anger was vented.
