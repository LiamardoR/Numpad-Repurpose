namespace NumpadShortcutsApp
{
    public class NumpadActions
    {
        public string ActionType { get; set; }  // "Open App" or "Play Sound"
        public string Path { get; set; }        // File path to exe or audio
        public string Arguments { get; set; }   // Optional arguments for the app

        // Constructor
        public NumpadActions()
        {
            ActionType = "";
            Path = "";
            Arguments = "";
        }
    }
}
