namespace Library.Canvas.Models
{
    // Base type for anything that can live inside a module
    public abstract class ModuleItem
    {
        public abstract string Display();
    }

    // A page just wraps string content (the old behavior)
    public class PageItem : ModuleItem
    {
        public string? Content { get; set; }
        public override string Display() => $"Page: {Content}";
    }

    // An assignment embedded directly in the module
    public class AssignmentItem : ModuleItem
    {
        public Assignment Assignment { get; set; } = new Assignment();
        public override string Display() =>
            $"Assignment: {Assignment.Name} ({Assignment.AvailablePoints} pts, due {Assignment.DueDate:MM/dd/yyyy})";
    }

    // A file shows its name and can be opened
    public class FileItem : ModuleItem
    {
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public override string Display() => $"File: {FileName}";

        public string Open()
        {
            if (!string.IsNullOrWhiteSpace(FilePath) && System.IO.File.Exists(FilePath))
                return $"Opening {FileName}:\n{System.IO.File.ReadAllText(FilePath)}";
            return $"File '{FileName}' located at: {FilePath ?? "(no path set)"}";
        }
    }
}