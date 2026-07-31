@echo off
chcp 65001 >nul
echo ========================================
echo   ???? - ????
echo ========================================
echo.

echo [1/4] ????...
call npm install
if errorlevel 1 (echo ?????? & pause & exit /b 1)

echo.
echo [2/4] ??? Android ??...
call npx cap sync android
if errorlevel 1 (echo ???? & pause & exit /b 1)

echo.
echo [3/4] ?? Android APK...
cd android
call gradlew.bat assembleDebug
if errorlevel 1 (echo APK???? & pause & exit /b 1)
cd ..

echo.
echo [4/4] ?? Windows EXE...
call npx electron-packager . "????" --platform=win32 --arch=x64 --out=dist --overwrite --icon=build/icon.ico --asar
if errorlevel 1 (echo EXE???? & pause & exit /b 1)

echo.
echo ========================================
echo   ?????
echo   APK: android\app\build\outputs\apk\debug\app-debug.apk
echo   EXE: dist\????-win32-x64\
echo ========================================
pause
