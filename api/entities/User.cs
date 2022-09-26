using System;
using Newtonsoft.Json;

namespace MeetingInfoNotifierAdmin.Api.Entities
{
    /// <summary>
    /// ユーザーを表す
    /// </summary>
    public class User
    {
        /// <summary>
        /// 既定のコンストラクタ
        /// </summary>
        public User()
        {
            Id = Guid.NewGuid().ToString();
        }

        #region プロパティ
        /// <summary>
        /// 一意とするID
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
        /// <summary>
        /// ユーザー名
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// メールアドレス
        /// </summary>
        [JsonProperty("emailAddress")]
        public string EmailAddress { get; set; }

        #endregion
    }
}
