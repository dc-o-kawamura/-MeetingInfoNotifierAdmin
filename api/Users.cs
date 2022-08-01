using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MeetingInfoNotifierAdmin.Api.Entities;
using User = MeetingInfoNotifierAdmin.Api.Entities.User;
using MeetingInfoNotifierAdmin.Api.Queries;

namespace MeetingInfoNotifierAdmin.Api
{
    public static class Users
    {
        #region ユーザー情報一覧を取得
        /// <summary>
        /// ユーザー情報一覧を取得する。
        /// </summary>
        /// <param name="req">HTTPリクエスト</param>
        /// <param name="client">CosmosDBのドキュメントクライアント</param>
        /// <param name="log">ロガー</param>
        /// <returns>ユーザー情報一覧</returns>
        [FunctionName("GetUsers")]
        public static async Task<IActionResult> GetUsers(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Users")] HttpRequest req,
            [CosmosDB(
                databaseName: "meeting-info-db",
                collectionName: "Users",
                ConnectionStringSetting = "CosmosDbConnectionString")
                ]DocumentClient client,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string message = string.Empty;

            try
            {
                log.LogInformation("GET Users");

                // クエリパラメータから検索条件パラメータを設定
                UsersQueryParameter queryParameter = new UsersQueryParameter()
                {
                    Ids = req.Query["ids"],
                    Name = req.Query["name"],
                    EmailAddress = req.Query["emailAddress"],
                    UserPrincipal = req.Query["userPrincipal"]
                };

                // Slackチャンネル情報を取得
                message = JsonConvert.SerializeObject(await GetUsers(client, queryParameter, log));
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }

            return new OkObjectResult(message);
        }

        /// <summary>
        /// ユーザー情報一覧を取得する。
        /// </summary>
        /// <param name="client">CosmosDBのドキュメントクライアント</param>
        /// <param name="queryParameter">抽出条件パラメータ</param>
        /// <param name="log">ロガー</param>
        /// <returns>ユーザー情報一覧</returns>
        internal static async Task<IEnumerable<User>> GetUsers(
                   DocumentClient client,
                   UsersQueryParameter queryParameter,
                   ILogger log
                   )
        {
            // Get a JSON document from the container.
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("meeting-info-db", "Users");
            IDocumentQuery<User> query = client.CreateDocumentQuery<User>(collectionUri, new FeedOptions { EnableCrossPartitionQuery = true, PopulateQueryMetrics = true })
                .Where(queryParameter.GetWhereExpression())
                .AsDocumentQuery();
            log.LogInformation(query.ToString());

            var documentItems = new List<User>();
            while (query.HasMoreResults)
            {
                foreach (var documentItem in await query.ExecuteNextAsync<User>())
                {
                    documentItems.Add(documentItem);
                }
            }
            return documentItems;
        }
        #endregion
    }
}
