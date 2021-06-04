using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Kaonavi.Net.Entities;
using Kaonavi.Net.Entities.Api;

namespace Kaonavi.Net.Services
{
    public class KaonaviV2Service
    {
        private const string BaseApiAddress = "https://api.kaonavi.jp/api/v2.0";
        private readonly HttpClient _client;
        private readonly string _consumerKey;
        private readonly string _consumerSecret;

        private const string TokenHeader = "Kaonavi-Token";
        public string? AccessToken
        {
            get => _client.DefaultRequestHeaders.TryGetValues(TokenHeader, out var values) ? values.First() : null;
            set
            {
                _client.DefaultRequestHeaders.Remove(TokenHeader);
                if (!string.IsNullOrWhiteSpace(value))
                    _client.DefaultRequestHeaders.Add(TokenHeader, value);
            }
        }

        private const string DryRunHeader = "Dry-Run";
        /// <summary>
        /// dryrunモードの動作有無を取得または設定します。
        /// <c>true</c>に設定することで、データベースの操作を行わず、リクエストの内容が適切であるかを検証することが出来ます。
        /// https://developer.kaonavi.jp/api/v2.0/index.html#section/dryrun
        /// </summary>
        public bool UseDryRun
        {
            get => _client.DefaultRequestHeaders.TryGetValues(DryRunHeader, out var values) && values.First() == "1";
            set
            {
                _client.DefaultRequestHeaders.Remove(DryRunHeader);
                if (value)
                    _client.DefaultRequestHeaders.Add(DryRunHeader, "1");
            }
        }

        public KaonaviV2Service(HttpClient client, string consumerKey, string consumerSecret)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _consumerKey = consumerKey ?? throw new ArgumentNullException(nameof(consumerKey));
            _consumerSecret = consumerSecret ?? throw new ArgumentNullException(nameof(consumerSecret));

            _client.BaseAddress ??= new(BaseApiAddress);
        }

        public async ValueTask<Token> AuthenticateAsync(CancellationToken cancellationToken = default)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes($"{_consumerKey}:{_consumerSecret}");
            var content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "grant_type", "client_credentials" }
            });
            _client.DefaultRequestHeaders.Authorization = new("Basic", Convert.ToBase64String(byteArray));

            var response = await _client.PostAsync("/token", content, cancellationToken).ConfigureAwait(false);
            await ValidateApiResponseAsync(response).ConfigureAwait(false);

            var token = await response.Content
                .ReadFromJsonAsync<Token>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            _client.DefaultRequestHeaders.Authorization = null;
            return token!;
        }

        public async ValueTask<MemberLayout> FetchMemberLayoutAsync(CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var response = await _client.GetAsync("/member_layouts").ConfigureAwait(false);
            await ValidateApiResponseAsync(response).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<MemberLayout>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!;
        }

        public async ValueTask<IEnumerable<SheetLayout>> FetchSheetLayoutsAsync(CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var response = await _client.GetAsync("/sheet_layouts").ConfigureAwait(false);
            await ValidateApiResponseAsync(response).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<SheetLayoutsResult>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!.Sheets;
        }
        private record SheetLayoutsResult(
            [property: JsonPropertyName("sheets")] IEnumerable<SheetLayout> Sheets
        );

        /// <summary>
        /// 所属情報の一覧を取得します。
        /// https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E6%89%80%E5%B1%9E/paths/~1departments/get
        /// </summary>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        public async ValueTask<IEnumerable<DepartmentInfo>> FetchDepartmentsAsync(CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var response = await _client.GetAsync("/departments").ConfigureAwait(false);
            await ValidateApiResponseAsync(response).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<DepartmentsResult>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!.DepartmentData;
        }
        private record DepartmentsResult(
            [property: JsonPropertyName("department_data")] IEnumerable<DepartmentInfo> DepartmentData
        );

        /// <summary>
        /// <paramref name="taskId"/>と一致するタスクの進捗状況を取得します。
        /// </summary>
        /// <param name="taskId">タスクID</param>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="taskId"/>が0より小さい場合にスローされます。</exception>
        public async ValueTask<TaskProgress> FetchTaskProgressAsync(int taskId, CancellationToken cancellationToken = default)
        {
            if (taskId < 0)
                throw new ArgumentOutOfRangeException(nameof(taskId));

            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var response = await _client.GetAsync($"/tasks/{taskId:D}").ConfigureAwait(false);
            await ValidateApiResponseAsync(response).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<TaskProgress>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!;
        }

        /// <summary>
        /// ロール情報の一覧を取得します。
        /// https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%AD%E3%83%BC%E3%83%AB/paths/~1roles/get
        /// </summary>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        public async ValueTask<IEnumerable<Role>> FetchRolesAsync(CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var response = await _client.GetAsync("/roles").ConfigureAwait(false);
            await ValidateApiResponseAsync(response).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<RolesResult>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!.RoleData;
        }
        private record RolesResult(
            [property: JsonPropertyName("role_data")] IEnumerable<Role> RoleData
        );

        #region Common Method
        private async ValueTask FetchTokenAsync(CancellationToken cancellationToken = default)
            => AccessToken ??= (await AuthenticateAsync(cancellationToken).ConfigureAwait(false)).AccessToken;

        private record ErrorResponse([property: JsonPropertyName("errors")] IEnumerable<string> Errors);
        private async ValueTask ValidateApiResponseAsync(HttpResponseMessage response)
        {
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                string errorMessage = response.Content.Headers.ContentType.MediaType == "application/json"
                    ? string.Join("\n", (await response.Content.ReadFromJsonAsync<ErrorResponse>().ConfigureAwait(false))!.Errors)
                    : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new ApplicationException(errorMessage, ex);
            }
        }
        #endregion
    }
}
