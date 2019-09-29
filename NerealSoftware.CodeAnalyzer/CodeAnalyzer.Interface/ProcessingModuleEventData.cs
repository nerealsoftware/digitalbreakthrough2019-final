namespace CodeAnalyzer.Interface
{
    public class ProcessingModuleEventData
    {
        public ProcessingModuleEventData(IFileSource currentFile, string message, int currentMainProgress,
            int maxMainProgress, int currentSecondProgress, int maxSecondProgress, IProcessingModule module)
        {
            CurrentFile = currentFile;
            Message = message;
            CurrentMainProgress = currentMainProgress;
            MaxMainProgress = maxMainProgress;
            CurrentSecondProgress = currentSecondProgress;
            MaxSecondProgress = maxSecondProgress;
            Module = module;
        }

        public IProcessingModule Module { get; }
        public IFileSource CurrentFile { get; }
        public string Message { get; }
        public int CurrentMainProgress { get; }
        public int MaxMainProgress { get; }
        public int CurrentSecondProgress { get; }
        public int MaxSecondProgress { get; }
    }
}