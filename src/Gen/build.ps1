
$files = Get-ChildItem ..\..\in\*.md

foreach ($file in $files) {
	$outFile = [System.IO.Path]::GetFileNameWithoutExtension($file) + ".md"
	 .\ParseMdTable.exe --source $file --target ..\..\out\$outFile
} 

$files = Get-ChildItem ..\..\out\*.md

foreach ($file in $files) {
	$outFile = [System.IO.Path]::GetFileNameWithoutExtension($file) + ".html"
	Write-Host "Processing" $file "to" $outFile
	pandoc -s $file -o ..\..\docs\$outFile -c kb.css --template kb-template.html --self-contained  --metadata title="Visual Studio 2022 - Keyboard Shortcuts" 
} 