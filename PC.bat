set src=doubleracing
set dst=DoubleracingAB

if not exist %dst% ( md %dst%)

mklink/J %cd%\%dst%\Assets %cd%\%src%\Assets
mklink/J %cd%\%dst%\Packages %cd%\%src%\Packages
mklink/J %cd%\%dst%\ProjectSettings %cd%\%src%\ProjectSettings
mklink/J %cd%\%dst%\UserSettings %cd%\%src%\UserSettings

pause
