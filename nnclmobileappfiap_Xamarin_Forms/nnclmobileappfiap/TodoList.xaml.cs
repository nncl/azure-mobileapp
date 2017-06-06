﻿using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace nnclmobileappfiap
{
    public partial class TodoList : ContentPage
    {
        TodoItemManager manager;

        public TodoList()
        {
            InitializeComponent();

            manager = TodoItemManager.DefaultManager;

            // OnPlatform<T> doesn't currently support the "Windows" target platform, so we have this check here.
            if (manager.IsOfflineEnabled &&
                (Device.OS == TargetPlatform.Windows || Device.OS == TargetPlatform.WinPhone))
            {
                var syncButton = new Button
                {
                    Text = "Sync items",
                    HeightRequest = 30
                };
                syncButton.Clicked += OnSyncItems;

                buttonsPanel.Children.Add(syncButton);
            }
        }

        #region Authenticator
        bool authenticated = false;
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (authenticated == true)
            {
                await RefreshItems(true, syncItems: true);

                this.loginButton.IsVisible = false; // Hide it when is authenticated
            }
        }

        // Do authenticate
        async void loginButton_Clicked(object sender, EventArgs args)
        {
            if (App.Authenticator != null)
                authenticated = await App.Authenticator.Authenticate();

            if (authenticated == true)
                await RefreshItems(true, syncItems: false);
        }

        #endregion

        // Data methods
        async Task AddItem(Professor item)
        {
            item.Email = "teste@test.com";
            item.Rm = "123";
            item.Aprovado = true;
            // item.Id = "123";
            await manager.SaveTaskAsync(item);
            todoList.ItemsSource = await manager.GetTodoItemsAsync();
        }

        async Task CompleteItem(Professor item)
        {
            item.Aprovado = true;
            await manager.SaveTaskAsync(item);
            todoList.ItemsSource = await manager.GetTodoItemsAsync();
        }

        public async void OnAdd(object sender, EventArgs e)
        {
            var todo = new Professor { Nome = newItemName.Text };
            await AddItem(todo);

            newItemName.Text = string.Empty;
            newItemName.Unfocus();
        }

        // Event handlers
        public async void OnSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var todo = e.SelectedItem as Professor;
            if (Device.OS != TargetPlatform.iOS && todo != null)
            {
                // Not iOS - the swipe-to-delete is discoverable there
                if (Device.OS == TargetPlatform.Android)
                {
                    await DisplayAlert(todo.Nome, "Press-and-hold to complete task " + todo.Nome, "Got it!");
                }
                else
                {
                    // Windows, not all platforms support the Context Actions yet
                    if (await DisplayAlert("Mark completed?", "Do you wish to complete " + todo.Nome + "?", "Complete", "Cancel"))
                    {
                        await CompleteItem(todo);
                    }
                }
            }

            // prevents background getting highlighted
            todoList.SelectedItem = null;
        }

        // http://developer.xamarin.com/guides/cross-platform/xamarin-forms/working-with/listview/#context
        public async void OnComplete(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var todo = mi.CommandParameter as Professor;
            await CompleteItem(todo);
        }

        // http://developer.xamarin.com/guides/cross-platform/xamarin-forms/working-with/listview/#pulltorefresh
        public async void OnRefresh(object sender, EventArgs e)
        {
            var list = (ListView)sender;
            Exception error = null;
            try
            {
                await RefreshItems(false, true);
            }
            catch (Exception ex)
            {
                error = ex;
            }
            finally
            {
                list.EndRefresh();
            }

            if (error != null)
            {
                await DisplayAlert("Refresh Error", "Couldn't refresh data (" + error.Message + ")", "OK");
            }
        }

        public async void OnSyncItems(object sender, EventArgs e)
        {
            await RefreshItems(true, true);
        }

        private async Task RefreshItems(bool showActivityIndicator, bool syncItems)
        {
            using (var scope = new ActivityIndicatorScope(syncIndicator, showActivityIndicator))
            {
                todoList.ItemsSource = await manager.GetTodoItemsAsync(syncItems);
            }
        }

        private class ActivityIndicatorScope : IDisposable
        {
            private bool showIndicator;
            private ActivityIndicator indicator;
            private Task indicatorDelay;

            public ActivityIndicatorScope(ActivityIndicator indicator, bool showIndicator)
            {
                this.indicator = indicator;
                this.showIndicator = showIndicator;

                if (showIndicator)
                {
                    indicatorDelay = Task.Delay(2000);
                    SetIndicatorActivity(true);
                }
                else
                {
                    indicatorDelay = Task.FromResult(0);
                }
            }

            private void SetIndicatorActivity(bool isActive)
            {
                this.indicator.IsVisible = isActive;
                this.indicator.IsRunning = isActive;
            }

            public void Dispose()
            {
                if (showIndicator)
                {
                    indicatorDelay.ContinueWith(t => SetIndicatorActivity(false), TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }
    }
}

