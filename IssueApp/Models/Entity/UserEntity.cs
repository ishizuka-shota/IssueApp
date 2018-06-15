using Microsoft.WindowsAzure.Storage.Table;

namespace IssueApp.Models.Entity
{
    /// <summary>
    /// ユーザEntity
    /// slackユーザ1人につき、GitHubアクセストークン1つ
    /// </summary>
    public class UserEntity : TableEntity
    {
        public UserEntity(string userId, string userName, string accessToken)
        {
            PartitionKey = userId;
            RowKey = userName;
            AccessToken = accessToken;
        }

        public UserEntity() { }

        public string AccessToken { get; set; }
    }
}