:: Copy the test data to the bin/Debug, compile the code and run the produced assembly
SET var=..\bin\Debug
XCOPY "%CD%\test.scs" %var%\test.scs /y /i
CD %var%
@"Console.exe"
@"test.exe"