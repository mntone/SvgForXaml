param($settingsFile = "settings.json")
$inkscape = "C:\Program Files\Inkscape\inkscape.com"
$settings = (Get-Content $settingsFile -Encoding UTF8 | ConvertFrom-Json)
foreach ($item in $settings.targets)
{
	foreach ($file in $item.files)
	{
		for ($i = 0; $i -lt $item.size.length; ++$i)
		{
			$option = "-z"
			$width = "-w" + $item.size[$i][1]
			$export = "--export-png=" + $settings.base_dir + ($file.dst -F $item.size[$i][0], $item.size[$i][1])
			& $inkscape $file.src $option $width $export
		}
	}
}