 pandoc -s .\default-keyboard-shortcuts-in-visual-studio.md -o docs/kb.html -c kb.css --template kb-template.html --self-contained  --metadata title="Visual Studio 2019 - Keyboard Shortcuts"
 pandoc -s .\curated-shortcut-list.md -o docs/curated-shortcut-list.html -c kb.css --template kb-template.html --self-contained  --metadata title="Visual Studio 2019 - Keyboard Shortcuts"
 