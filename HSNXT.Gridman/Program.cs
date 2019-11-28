using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gameloop.Vdf;
using Gameloop.Vdf.Linq;
using HSNXT.QuickAndDirtyGui;
using ImGuiNET;

namespace HSNXT.Gridman
{
    internal static class Program
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        static Program()
        {
            HttpClient.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36");
        }
        
        private static async IAsyncEnumerable<User> GetUsers(string steamDir)
        {
            if (!Directory.Exists($"{steamDir}/userdata"))
            {
                throw new InvalidOperationException("No active users or invalid steam directory!");
            }

            foreach (var userDirectory in Directory.EnumerateDirectories($"{steamDir}/userdata"))
            {
                var config = $"{userDirectory}/config/localconfig.vdf";
                if (!File.Exists(config))
                {
                    Log("Missing config: " + config);
                    continue;
                }

                var configVdf = VdfConvert.Deserialize(await File.ReadAllTextAsync(config));
                var username = configVdf.Value["friends"]["PersonaName"].Value<string>();
                var steamId32 = uint.Parse(Path.GetFileName(userDirectory));
                yield return new User(steamDir, username, steamId32);
            }
        }

        private static async Task<string> GetProfileGamesHtml(User user)
        {
            using var result = await HttpClient.GetAsync($"http://steamcommunity.com/profiles/{user.SteamId64}/games?tab=all");
            if (!result.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Status code for request for profile {user.Username} was {result.StatusCode}");
            }

            var str = await result.Content.ReadAsStringAsync();
            if (str.Contains("The specified profile could not be found."))
            {
                throw new InvalidOperationException($"Profile {user.Username} does not exist");
            }
            return str;
        }

        private static bool IsValidSteamDirectory(string directory)
        {
            return Directory.Exists(directory) && File.Exists($"{directory}/Steam.exe");
        }

        private static string? GetDefaultSteamInstallPath()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var programFiles86 = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
                if (programFiles86 != null && IsValidSteamDirectory($"{programFiles86}/Steam"))
                    return $"{programFiles86}/Steam";

                var programFiles = Environment.GetEnvironmentVariable("ProgramFiles");
                if (programFiles != null && IsValidSteamDirectory($"{programFiles}/Steam"))
                    return $"{programFiles}/Steam";
            }
            else
            {
                var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                if (IsValidSteamDirectory($"{userProfile}/.local/share/Steam"))
                    return $"{userProfile}/.local/share/Steam";

                if (IsValidSteamDirectory($"{userProfile}/.steam/steam"))
                    return $"{userProfile}/.steam/steam";
                
                if (IsValidSteamDirectory($"{userProfile}/Library/Application Support/Steam"))
                    return $"{userProfile}/Library/Application Support/Steam";
            }

            return null;
        }
        
        private static readonly Regex ProfileGamesRegex = new Regex(
            @"var rgGames = \[(?<json>.*)\];\r?\n\s*var rgChangingGames = \[\];", RegexOptions.Compiled | RegexOptions.Singleline
        );

        private static async IAsyncEnumerable<Game> FindProfileGames(User user)
        {
            var html = await GetProfileGamesHtml(user);
            using var document = JsonDocument.Parse($"[{ProfileGamesRegex.Match(html).Groups["json"].Value}]");
            foreach (var game in document.RootElement.EnumerateArray())
            {
                yield return new Game(game.GetProperty("appid").GetUInt32(), game.GetProperty("name").GetString());
            }
        }

        private static IAsyncEnumerable<Game> FindGames(User user)
        {
            return FindProfileGames(user); //TODO addUnknownGames, addNonSteamGames
        }

        private static async Task<bool> HasOfficialLibraryArt(Game game)
        {
            while (true)
            {
                using var result
                    = await HttpClient.GetAsync($"http://cdn.akamai.steamstatic.com/steam/apps/{game.AppId}/library_600x900.jpg");
                if (result.IsSuccessStatusCode)
                {
                    return true;
                }

                if (result.StatusCode == HttpStatusCode.NotFound)
                {
                    return false;
                }
                
                Log($"Received unknown status code {result.StatusCode} for {game}, retrying");
                await Task.Delay(1500);
            }
        }

        private static bool HasUnofficialLibraryArt(User user, Game game)
        {
            return File.Exists($"{user.UserdataRoot}/config/grid/{game.AppId}p.png");
        }

        private static Task SetUnofficialLibraryArt(User user, Game game, byte[] bytes)
        {
            return File.WriteAllBytesAsync($"{user.UserdataRoot}/config/grid/{game.AppId}p.png", bytes);
        }

        private static async IAsyncEnumerable<(Grid grid, int index)> GetGrids(string steamGridDbToken, IReadOnlyList<Game> games, string[] styles, string[] types)
        {
            // Retrieve grids by platform ID.
            const string platform = "steam";
            using var result
                = await HttpClient.SendAsync(new HttpRequestMessage
                {
                    RequestUri = new Uri($"https://www.steamgriddb.com/api/v2/grids/{platform}/{string.Join(',', games.Select(e => e.AppId))}" +
                                         $"?styles={string.Join(',', styles)}&dimensions=600x900&types={string.Join(',', types)}"),
                    Method = HttpMethod.Get,
                    Headers =
                    {
                        { "Authorization", $"Bearer {steamGridDbToken}"} 
                    }
                });
            if (!result.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Bad status code: {result.StatusCode}");
            }

            var responses = JsonSerializer.Deserialize<GridsResponse>(await result.Content.ReadAsStringAsync());
            if (!responses.Success)
            {
                throw new InvalidOperationException("Response was not a success");
            }

            var i = -1;
            foreach (var response in responses.Data)
            {
                i++;
                if (!response.Success)
                {
                    Log($"GetGrids response for game {games[i]} was not a success: {response.Status}");
                    continue;
                }

                if (response.Data.Length == 0)
                {
                    Log($"There are no covers for {games[i]}");
                    continue;
                }
                
                yield return (response.Data[0], i);
            }
        }

        private static readonly CircularBuffer<string> LocalConsole = new CircularBuffer<string>(80);
        private static void Log(string text)
        {
            Console.WriteLine(text);
            LocalConsole.PushBack(text);
        }

        private static void Main()
        {
            var steamPath = GetDefaultSteamInstallPath() ?? @"E:\Steam";
            var steamGridDbToken = "put your token herr";
            Task workTask = null;
            var autoScrollConsole = true;

            var steamGridDbAlternate = true;
            var steamGridDbBlurred = true;
            var steamGridDbWhiteLogo = false;
            var steamGridDbMaterial = false;
            var steamGridDbNoLogo = false;
            
            var steamGridDbType = 0; // 0 = static, 1 = animated

            GuiWindowBuilder.Create(gui =>
            {
                gui.InputText("Steam installation path", ref steamPath, 256);

                gui.InputText("SteamGridDB token", ref steamGridDbToken, 256);

                gui.Checkbox("alternate", ref steamGridDbAlternate); gui.SameLine();
                gui.Checkbox("blurred", ref steamGridDbBlurred); gui.SameLine();
                gui.Checkbox("white_logo", ref steamGridDbWhiteLogo); gui.SameLine();
                gui.Checkbox("material", ref steamGridDbMaterial); gui.SameLine();
                gui.Checkbox("no_logo", ref steamGridDbNoLogo); gui.SameLine();
                gui.Text("SteamGridDB art styles");
                
                gui.Combo("SteamGridDB type", ref steamGridDbType, "static", "animated");

                if (workTask?.IsCompleted == true)
                {
                    workTask = null;
                }
                
                if (gui.Button("HIT IT", workTask != null))
                {
                    var steamGridDbTypeString = new[] { steamGridDbType == 0 ? "static" : "animated" };
                    var steamGridDbStyles = new List<string>();
                    if (steamGridDbAlternate) steamGridDbStyles.Add("alternate");
                    if (steamGridDbBlurred) steamGridDbStyles.Add("blurred");
                    if (steamGridDbWhiteLogo) steamGridDbStyles.Add("white_logo");
                    if (steamGridDbMaterial) steamGridDbStyles.Add("material");
                    if (steamGridDbNoLogo) steamGridDbStyles.Add("no_logo");
                    var steamGridDbStylesArray = steamGridDbStyles.ToArray();
                    
                    workTask = Task.Run(async () =>
                    {
                        try
                        {
                            Log("Started");
                            await foreach (var user in GetUsers(steamPath))
                            {
                                Log($"User: {user.Username}");
                                var candidateGames = new List<Game>();
                                await foreach (var game in FindGames(user))
                                {
                                    if (await HasOfficialLibraryArt(game))
                                    {
                                        Log($"Game: {game} has Steam art");
                                    }
                                    else if (HasUnofficialLibraryArt(user, game))
                                    {
                                        Log($"Game: {game} has custom art");
                                    }
                                    else
                                    {
                                        Log($"Game: {game} has no art");
                                        candidateGames.Add(game);
                                    }
                                }

                                foreach (var gameList in ChunkBy(candidateGames, 15))
                                {
                                    Log($"Downloading games: {string.Join(',', gameList)}");
                                    await foreach (var (grid, i) in GetGrids(steamGridDbToken, gameList, steamGridDbStylesArray, steamGridDbTypeString))
                                    {
                                        var bytes = await HttpClient.GetByteArrayAsync(grid.Url);
                                        await SetUnofficialLibraryArt(user, gameList[i], bytes);
                                        Log($"Written game {gameList[i]}");
                                    }
                                }
                            }

                            Log($"Done");
                        }
                        catch (Exception e)
                        {
                            Debugger.Break();
                            Log(e.ToString());
                        }
                    });
                }
                
                gui.Separator();
                
                var footerHeightToReserve = gui.Style.ItemSpacing.Y + gui.GetFrameHeightWithSpacing(); // 1 separator, 1 input text
                gui.BeginChild("ScrollingRegion", new Vector2(0, -footerHeightToReserve), false, ImGuiWindowFlags.HorizontalScrollbar); // Leave room for 1 separator + 1 InputText
                {
                    if (gui.BeginPopupContextWindow())
                    {
                        if (gui.Selectable("Clear")) LocalConsole.Clear();
                        gui.EndPopup();
                    }

                    var copyToClipboard = gui.Button("Copy");
                    gui.SameLine();
                    gui.Checkbox("Auto-scroll", ref autoScrollConsole);

                    if (copyToClipboard) gui.LogToClipboard();
                    foreach (var line in LocalConsole) gui.TextUnformatted(line);
                    if (copyToClipboard) gui.LogFinish();

                    if (autoScrollConsole && gui.GetScrollY() >= gui.GetScrollMaxY())
                    {
                        gui.SetScrollHereY(1.0F);
                    }
                }
                gui.EndChild();
            });
        }

        public static T[][] ChunkBy<T>(IEnumerable<T> source, int chunkSize) 
        {
            return source
                .Select((x, i) => (Index: i, Value: x))
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToArray())
                .ToArray();
        }
    }

    internal readonly struct User
    {
        private const ulong IdConversionConstant = 0x110000100000000UL;

        public string Username { get; }
        public string UserdataRoot { get; }
        public uint SteamId32 { get; }
        public ulong SteamId64 => SteamId32 + IdConversionConstant;
        
        public User(string steamDir, string username, uint steamId32)
        {
            UserdataRoot = $"{steamDir}/userdata/{steamId32}";
            Username = username;
            SteamId32 = steamId32;
        }
    }
    
    internal readonly struct Game
    {
        public uint AppId { get; }
        public string Name { get; }

        public Game(uint appId, string name)
        {
            AppId = appId;
            Name = name;
        }

        public override string ToString() => $"{AppId}:{Name}";
    }
    
    public class GridsResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("data")]
        public GridResponse[] Data { get; set; }
    }

    public class GridResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("status")]
        public long Status { get; set; }

        [JsonPropertyName("data")]
        public Grid[] Data { get; set; }
    }

    public class Grid
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("score")]
        public long Score { get; set; }

        [JsonPropertyName("style")]
        public string Style { get; set; }

        [JsonPropertyName("url")]
        public Uri Url { get; set; }

        [JsonPropertyName("thumb")]
        public Uri Thumb { get; set; }

        [JsonPropertyName("author")]
        public GridAuthor Author { get; set; }
    }

    public class GridAuthor
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("steam64")]
        public string Steam64 { get; set; }

        [JsonPropertyName("avatar")]
        public Uri Avatar { get; set; }
    }
}