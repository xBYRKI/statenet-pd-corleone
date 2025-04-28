@echo off
echo Starte Tailwind Watch Build mit tailwindcss.exe...
tailwindcss.exe -i ./wwwroot/css/input.css -o ./wwwroot/css/output.css --watch
pause
