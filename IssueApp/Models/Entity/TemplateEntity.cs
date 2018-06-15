using Microsoft.WindowsAzure.Storage.Table;

namespace IssueApp.Models.Entity
{
    /// <summary>
    /// テンプレート用Entity
    /// Tempalte : Label1つにつきissueのテンプレート1つ
    /// </summary>
    public class TemplateEntity : TableEntity
    {
        public TemplateEntity(string label, string temp)
        {
            PartitionKey = label;
            RowKey = "label";
            Template = temp;
        }

        public TemplateEntity() { }

        public string Template { get; set; }
    }
}