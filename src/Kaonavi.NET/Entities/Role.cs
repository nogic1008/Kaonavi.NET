using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    /// <summary>ロール情報</summary>
    public record Role
    {
        /// <summary>
        /// Roleの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="id"><inheritdoc cref="Id" path="/summary/text()"/></param>
        /// <param name="name"><inheritdoc cref="Name" path="/summary/text()"/></param>
        /// <param name="type"><inheritdoc cref="Type" path="/summary/text()"/></param>
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
