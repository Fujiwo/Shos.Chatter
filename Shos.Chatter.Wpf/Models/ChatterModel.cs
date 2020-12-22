using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Shos.Chatter.Wpf.Models
{
    public class ChatterModel : BindableBase
    {
        static HttpClient httpClient = new HttpClient();

        public Server Server { get; set; } = new Server();
        
        int userId = 1;

        public int UserId {
            get => userId;
            set => SetProperty(ref userId, value);
        }

        IEnumerable<Shos.Chatter.Server.Models.User>? users = null;
        IEnumerable<Shos.Chatter.Server.Models.Chat>? chats = null;

        public IEnumerable<Shos.Chatter.Server.Models.User> Users {
            get {
                if (users is null)
                    users = GetUsers().Result;
                return users;
            }
        }

        public IEnumerable<Shos.Chatter.Server.Models.Chat> Chats {
            get {
                if (chats is null)
                    chats = GetChats().Result;
                return chats;
            }
        }

        public async Task Start()
        {
            Server.UpdateUsers += async () => await UpdateUsers(false);
            Server.UpdateChats += async () => await UpdateChats(false);
            await Server.Start();
        }

        public async Task<bool> Add(Shos.Chatter.Server.Models.User user)
        {
            try {
                var uri  = $"{Server.Url}api/Users";
                var json = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");
                using var response = await httpClient.PostAsync(uri, json).ConfigureAwait(false);
                if (response.IsSuccessStatusCode) {
                    await UpdateUsers(true);
                    return true;
                }
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }
            return false;
        }

        public async Task<bool> Delete(Shos.Chatter.Server.Models.User user)
        {
            try {
                var uri = $"{Server.Url}api/Users/{user.Id}";
                using var response = await httpClient.DeleteAsync(uri).ConfigureAwait(false);
                if (response.IsSuccessStatusCode) {
                    await UpdateUsers(true);
                    return true;
                }
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }
            return false;
        }

        public async Task<bool> Update(Shos.Chatter.Server.Models.User user)
        {
            try {
                var uri = $"{Server.Url}api/Users/{user.Id}";
                var json = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");
                using var response = await httpClient.PutAsync(uri, json).ConfigureAwait(false);
                if (response.IsSuccessStatusCode) {
                    await UpdateUsers(true);
                    return true;
                }
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }
            return false;
        }

        public async Task<bool> Add(Shos.Chatter.Server.Models.Chat chat)
        {
            try {
                chat.UserId = UserId;
                var uri  = $"{Server.Url}api/Chats";
                var json = new StringContent(JsonSerializer.Serialize(chat), Encoding.UTF8, "application/json");
                using var response = await httpClient.PostAsync(uri, json).ConfigureAwait(false);
                if (response.IsSuccessStatusCode) {
                    await UpdateChats(true);
                    return true;
                }
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }
            return false;
        }

        public async Task<bool> Delete(Shos.Chatter.Server.Models.Chat chat)
        {
            try {
                var uri = $"{Server.Url}api/Chats/{chat.Id}";
                using var response = await httpClient.DeleteAsync(uri).ConfigureAwait(false);
                if (response.IsSuccessStatusCode) {
                    await UpdateChats(true);
                    return true;
                }
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }
            return false;
        }

        public async Task<bool> Update(Shos.Chatter.Server.Models.Chat chat)
        {
            try {
                var uri = $"{Server.Url}api/Chats/{chat.Id}";
                var json = new StringContent(JsonSerializer.Serialize(chat), Encoding.UTF8, "application/json");
                using var response = await httpClient.PutAsync(uri, json).ConfigureAwait(false);
                if (response.IsSuccessStatusCode) {
                    await UpdateChats(true);
                    return true;
                }
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }
            return false;
        }

        async Task<IEnumerable<Shos.Chatter.Server.Models.User>> GetUsers()
        {
            try {
                var uri = new Uri($"{Server.Url}api/Users");
                var request = new HttpRequestMessage(HttpMethod.Get, uri);
                request.Headers.Add("ContentType", "application/json");
                var response = await httpClient.SendAsync(request).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadAsAsync<IEnumerable<Shos.Chatter.Server.Models.User>>();
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }
            return new Shos.Chatter.Server.Models.User[] { };
        }

        async Task<IEnumerable<Shos.Chatter.Server.Models.Chat>> GetChats()
        {
            try {
                var uri = new Uri($"{Server.Url}api/Chats");
                var request = new HttpRequestMessage(HttpMethod.Get, uri);
                request.Headers.Add("ContentType", "application/json");
                var response = await httpClient.SendAsync(request).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadAsAsync<IEnumerable<Shos.Chatter.Server.Models.Chat>>();
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }
            return new Shos.Chatter.Server.Models.Chat[] { };
        }

        async Task UpdateUsers(bool notifyEnabled = false)
        {
            var newUsers = GetUsers().Result;
            if (users is null || !newUsers.SequenceEqual(users)) {
                users = newUsers;
                RaisePropertyChanged(nameof(Users));
                if (notifyEnabled)
                    await Server.NotifyUpdateUsers();
            }
        }

        async Task UpdateChats(bool notifyEnabled = false)
        {
            var newChats = GetChats().Result;
            if (chats is null || !newChats.SequenceEqual(chats)) {
                chats = newChats;
                RaisePropertyChanged(nameof(Chats));
                if (notifyEnabled)
                    await Server.NotifyUpdateChats();
            }
        }
    }

    public class Server : BindableBase
    {
        public event Action? UpdateUsers;
        public event Action? UpdateChats;

        HubConnection? hubConnection = null;
        public string Url => App.Settings.ServerUrl;

        public async Task NotifyUpdateUsers() => await hubConnection.InvokeAsync("UpdateUsers");
        public async Task NotifyUpdateChats() => await hubConnection.InvokeAsync("UpdateChats");

        public async Task Start()
        {
            hubConnection = new HubConnectionBuilder().WithUrl($"{Url}chatterhub").WithAutomaticReconnect().Build();

            hubConnection.On(nameof(UpdateUsers), () => UpdateUsers?.Invoke());
            hubConnection.On(nameof(UpdateChats), () => UpdateChats?.Invoke());
            try {
                await hubConnection.StartAsync();
                Debug.WriteLine("hubConnection.State: {hubConnection.State}");
            } catch (Exception ex) {
                Debug.WriteLine(ex);
                throw;
            }
        }
    }
}
