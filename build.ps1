Expand-Archive -Force -LiteralPath src\gen\pandoc.zip -DestinationPath src\gen\
Write-Host "Unpacked pandoc"

$files = Get-ChildItem content\*.md

foreach ($file in $files) {
	$outFile = [System.IO.Path]::GetFileNameWithoutExtension($file) + ".md"
	Write-Host "Processing content markdown files" $file "to" $outFile
	 src\gen\ParseMdTable.exe --source $file --target intermediate\$outFile
} 

$files = Get-ChildItem intermediate\*.md

foreach ($file in $files) {
	$outFile = [System.IO.Path]::GetFileNameWithoutExtension($file) + ".html"
	Write-Host "Processing intermediate markdown files" $file "to" $outFile
	src\gen\pandoc.exe -s $file -o docs\$outFile -c kb.css --template kb-template.html --self-contained  --metadata title="Visual Studio 2022 - Keyboard Shortcuts" 
} 