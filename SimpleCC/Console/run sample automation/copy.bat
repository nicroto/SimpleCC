:: Copy the test data to the bin/Debug directory
SET var=..\bin\Debug
XCOPY "%CD%\test.scs" %var%\test.scs /y /i
PAUSE