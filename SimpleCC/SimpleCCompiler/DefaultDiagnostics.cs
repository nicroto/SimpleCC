﻿using System;
namespace SimpleC
{
    public class DefaultDiagnostics: Diagnostics
    {
        private int errorCount = 0;
        public int ErrorCount
        {
            get { return errorCount; }
        }

        private int warningCount = 0;
        public int WarningCount
        {
            get { return warningCount; }
        }

        private int noteCount = 0;
        public int NoteCount
        {
            get { return noteCount; }
        }

        public override void Error(int line, int column, String message)
        {
            Console.WriteLine(string.Format("Error on line {0}, col {1}: {2}", line, column, message));
            errorCount++;
        }

        public override void Warning(int line, int column, String message)
        {
            Console.WriteLine("Warning on line {0}, col {1}: {2}", line, column, message);
            warningCount++;
        }

        public override void Note(int line, int column, String message)
        {
            Console.WriteLine("Not on line {0}, col {1}: {2}", line, column, message);
            noteCount++;
        }
        public override int GetErrorCount()
        {
            return errorCount;
        }
        public override void BeginSourceFile(string sourceFile)
        {
            // no op
        }

        public override void EndSourceFile()
        {
            // no op
        }
    }
}
