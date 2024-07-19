namespace BlazorAppEvergreenOIDC.Data
{
    public class FileLogger
    {
        public void SaveLogToFile(string dataToSave)
        {
            string path = @".\logs.txt"; // File path

            // Append the new data to the file
            File.AppendAllText(path, Environment.NewLine + DateTime.Now + ": " + dataToSave);
        }
    }
}
