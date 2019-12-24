using Microsoft.WindowsAzure.Storage.Table;

namespace IssueApp.AzureTableStorage
{
    public class EntityOperation<TE> where TE : TableEntity
    {
        #region Entity挿入

        /// <summary>
        /// Entity挿入
        /// </summary>
        /// <returns></returns>
        public TableResult InsertEntityResult(TE entity, string tableName)
        {
            //エンティティを用いて挿入用命令を作成
            var insertOperation = TableOperation.Insert(entity);

            //指定テーブルに挿入命令を実行し、結果を返す
            return StorageOperation.GetTableIfNotExistsCreate(tableName).Execute(insertOperation);

        }
        #endregion

        #region Entity検索

        /// <summary>
        /// Entity検索
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <param name="rowKey"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public TE RetrieveEntity(string partitionKey, string rowKey, string tableName)
        {
            //検索操作を行う変数を生成
            var retrieveOperation = TableOperation.Retrieve<TE>(partitionKey, rowKey);

            //テーブルに対して検索操作を行い、結果を返す
            var result = StorageOperation.GetTableIfNotExistsCreate(tableName).Execute(retrieveOperation);

            return result?.Result as TE;
        }
        #endregion

        #region Entity更新
        /// <summary>
        /// Entity更新
        /// </summary>
        /// <returns></returns>
        public TableResult UpdateEntityResult(TE entity, string tableName)
        {
            //アップデート対象のエンティティを取得
            var updateEntity = RetrieveEntity(entity.PartitionKey, entity.RowKey, tableName);

            //アップデート対象のエンティティを指定
            entity.PartitionKey = updateEntity.PartitionKey;
            entity.RowKey = updateEntity.RowKey;
            entity.ETag = updateEntity.ETag;

            //更新操作を行う変数を生成
            var replaceOperation = TableOperation.Replace(entity);

            //テーブルに対して更新操作を行い、結果を返す
            return StorageOperation.GetTableIfNotExistsCreate(tableName).Execute(replaceOperation);
        }
        #endregion

        #region Entity挿入か更新
        /// <summary>
        /// Entity挿入か更新
        /// </summary>
        /// <returns></returns>
        public TableResult InsertOrUpdateEntityResult(TE entity, string tableName)
        {
            //挿入or更新操作を行う変数を生成
            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(entity);

            //テーブルに対して挿入or更新操作を行う
            return StorageOperation.GetTableIfNotExistsCreate(tableName).Execute(insertOrReplaceOperation);
        }
        #endregion
    }
}