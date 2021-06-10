using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    /// <summary>ロール情報</summary>
    public record Role
    {
        /// <summary>
        /// Roleの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="id">ロールID</param>
        /// <param name="name">ロール名</param>
        /// <param name="type">ロールの種別 ("Adm", "一般")</param>
        public Role(int id, string name, string type)
            => (Id, Name, Type) = (id, name, type);

        /// <summary>ロールID</summary>
        [JsonPropertyName("id")]
        public int Id { get; init; }

        /// <summary>ロール名</summary>
        [JsonPropertyName("name")]
        public string Name { get; init; }

        /// <summary>ロールの種別 ("Adm", "一般")</summary>
        [JsonPropertyName("type")]
        public string Type { get; init; }
    }
}
