@echo off
chcp 65001 >nul
echo ========================================
echo   ???? - ???? Android APK
echo ========================================
echo.

set JAVA_HOME=C:\jdk-21.0.4
set ANDROID_HOME=C:\AndroidSDK
set PATH=%JAVA_HOME%\bin;%ANDROID_HOME%\platform-tools;%PATH%

echo [1/3] ????...
call npm install
echo.

echo [2/3] ??Web???Android...
call npx cap sync android
echo.

echo [3/3] ??APK...
cd android
call gradlew.bat assembleDebug
cd ..

if exist "android\app\build\outputs\apk\debug\app-debug.apk" (
  copy "android\app\build\outputs\apk\debug\app-debug.apk" "dist\????.apk"
  echo.
  echo ========================================
  echo   ?????
  echo   APK: dist\????.apk
  echo   EXE: dist\????-win32-x64\????.exe
  echo ========================================
) else (
  echo.
  echo ????????????
)

pause