:: Copy the test data to the bin/Debug directory
@ECHO OFF
XCOPY "%CD%\test.scs" "..\bin\Debug\test.scs" /y /i
PAUSE