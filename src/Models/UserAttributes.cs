namespace GltfAttributesExporter.Models
{
    public class UserAttribute
    {
        public string key { get; set; }
        public string value { get; set; }

        public static bool ContainsTextFieldFormat(string input)
        {
            return input.Contains("%<") && input.Contains(">%");
        }
    }
}
