using Microsoft.WindowsAzure.Storage.Table;

namespace IssueApp.Models.Entity
{
    public class TeamIdEntity : TableEntity
    {
        public TeamIdEntity(string teamId, string teamName, string token)
        {
            PartitionKey = teamId;
            RowKey = teamName;
            Token = token;
        }

        public TeamIdEntity() { }

        public string Token { get; set; }
    }
}