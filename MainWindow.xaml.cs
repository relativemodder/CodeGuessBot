using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

namespace DiscordGuessCodeBot
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DiscordSocketClient _client;
        private Dictionary<string, Session> sessions = new Dictionary<string, Session>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private CodeReference GetCodeReference()
        {
            CodeReferences codeReferences = new CodeReferences();

            Random random = new Random();
            return codeReferences[random.Next(codeReferences.Count)];
        }

        public async Task ButtonHandler(SocketMessageComponent component)
        {
            if (!sessions.ContainsKey(component.User.Id.ToString()))
            {
                await component.RespondAsync($"Start new game with /guess!");
                return;
            }

            SocketMessage message = component.Message;

            // Get text extension

            string[] lines = message.Content.Split('\n');

            string extension = lines[0].Replace("```", "");

            if(extension != component.Data.CustomId)
            {
                await component.RespondAsync($"You lose! Score: {sessions[component.User.Id.ToString()].Score}");
                sessions.Remove(component.User.Id.ToString());
                return;
            }

            sessions[component.User.Id.ToString()].Score++;
            await component.RespondAsync($"Guessed! Now your score is: {sessions[component.User.Id.ToString()].Score}");
            await GuessCode(message);
        }

        private async Task GuessCode(SocketMessage message)
        {

            CodeReference codeReference = GetCodeReference();

            WebClient webClient = new WebClient();
            string code = webClient.DownloadString(codeReference.ReferenceURL);

            List<string> extensions = new List<string>()
                {
                    "cs",
                    "php",
                    "py"
                };

            string extension = Path.GetExtension(codeReference.ReferenceURL).Replace(".", "");

            Random random = new Random();

            var builder = new ComponentBuilder();

            extensions.Remove(extension);

            int randomRightAnswer = random.Next(2);

            for (int offset = 0; offset < 2; offset++)
            {
                string ext = extensions[random.Next(random.Next(extensions.Count))];

                if (offset == randomRightAnswer)
                {
                    ext = extension;
                }

                builder = builder.WithButton(ext, ext);
            }


            await message.Channel.SendMessageAsync($"```{extension}\n{code}```", components: builder.Build());
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            if (message.Content == "/start")
                await message.Channel.SendMessageAsync("Hello!");

            if (message.Content == "/guess")
            {
                if (!sessions.ContainsKey(message.Author.Id.ToString()))
                {
                    sessions.Add(message.Author.Id.ToString(), new Session());
                }
                await GuessCode(message);
            }
        }

        private async Task MainAsync()
        {
            _client = new DiscordSocketClient();

            _client.Log += Log;

            string token = Environment.GetEnvironmentVariable("guessCodeToken", EnvironmentVariableTarget.User);

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();


            _client.MessageReceived += MessageReceivedAsync;
            _client.ButtonExecuted += ButtonHandler;


            await Task.Delay(-1);
        }

        private void StartBot(object sender, RoutedEventArgs e)
        {
            Task.Run(() => this.MainAsync().GetAwaiter().GetResult());
        }

        private void SetToken(object sender, RoutedEventArgs e)
        {
            Environment.SetEnvironmentVariable("guessCodeToken", tokenField.Text, EnvironmentVariableTarget.User);
            tokenField.Clear();
        }

        private void OnClosing(object sender, EventArgs e)
        {
            _client.LogoutAsync();
        }
    }
}
