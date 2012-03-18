namespace SimpleC
{
    public class DefaultDiagnostics: Diagnostics
    {
        public override void Error(int line, int column, string message)
        {
            throw new System.NotImplementedException();
        }

        public override void Warning(int line, int column, string message)
        {
            throw new System.NotImplementedException();
        }

        public override void Note(int line, int column, string message)
        {
            throw new System.NotImplementedException();
        }

        public override int GetErrorCount()
        {
            throw new System.NotImplementedException();
        }

        public override void BeginSourceFile(string sourceFile)
        {
            throw new System.NotImplementedException();
        }

        public override void EndSourceFile()
        {
            throw new System.NotImplementedException();
        }
    }
}
