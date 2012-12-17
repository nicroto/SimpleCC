:: Copy the test data to the bin/Debug directory and run the test.exe
SET var=..\bin\Debug
XCOPY "%CD%\test.scs" %var%\test.scs /y /i
CD %var%
@"Console.exe"
@"test.exe"