using JonathanProjectOffline.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace JonathanProjectOffline
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<Song> Songs;
        private ObservableCollection<StorageFile> AllSongs;
        private ObservableCollection<JonPlaylist> Playlists;
        private ObservableCollection<PlaylistLine> CurPlayList;
        private Song ToBeAdded;
        public List<MenuItem> MenuItems;      
        public ObservableCollection<Song> CurrentSong;
        public int CurSong;
        

        public MainPage()
        {
            this.InitializeComponent();
            CurSong = 0;
            CurrentSong = new ObservableCollection<Song>();
            ToBeAdded = null;
            //FadeInWelcomeText.Begin();
            FadeInLoginInput.Begin();
            Songs = new ObservableCollection<Song>();
            CurPlayList = new ObservableCollection<PlaylistLine>();
            Playlists = new ObservableCollection<JonPlaylist>();
            UserManager.PopulateUserlist();
            MenuItems = new List<MenuItem>();
            MenuItems.Add(new MenuItem { Index = "1", DisplayText = "LOCAL SONGS", category = Category.Songs });
            MenuItems.Add(new MenuItem { Index = "2", DisplayText = "PLAYLISTS", category = Category.Playlists });
            DatabaseManager.CreateDB();
            //DatabaseManager.CreateMobileDB();
            
        }
        

        private async Task RetrieveFilesInFolders(ObservableCollection<StorageFile> list, StorageFolder parent)
        {
            foreach (var item in await parent.GetFilesAsync())
            {
                if (item.FileType == ".mp3")
                    list.Add(item);
            }

            foreach (var item in await parent.GetFoldersAsync())
            {
                await RetrieveFilesInFolders(list, item);
            }
        }
        
        private async Task PopulateSongList(List<StorageFile> files)
        {
            int id = 0;
            foreach (var file in files)
            {

                MusicProperties songProperties = await file.Properties.GetMusicPropertiesAsync();

                StorageItemThumbnail currentThumb = await file.GetThumbnailAsync(ThumbnailMode.MusicView, 200, ThumbnailOptions.UseCurrentScale);

                var albumCover = new BitmapImage();
                albumCover.SetSource(currentThumb);
                var song = new LocalSong();

                song.Id = id;
                song.Album = songProperties.Album;
                song.Artist = songProperties.Artist;
                song.Title = songProperties.Title;
                song.AlbumCover = albumCover;
                song.SongFile = file;

                Songs.Add(song);
                id++;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private async Task<ObservableCollection<StorageFile>> SetupMusicList()
        {
            // 1. get access to music library

            StorageFolder folder = KnownFolders.MusicLibrary;
            //StorageFolder folder = KnownFolders.RemovableDevices;
            var allSongs = new ObservableCollection<StorageFile>();
            await RetrieveFilesInFolders(allSongs, folder);
            return allSongs;
        }

        private async Task ImportMusicLibrary()
        {
            Songs.Clear();
            await PopulateSongList(AllSongs.ToList());
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            StartupProgressRing.IsActive = true;
            DatabaseManager.GetPlaylists(Playlists);
            AllSongs = await SetupMusicList();
            await ImportMusicLibrary();
            StartupProgressRing.IsActive = false;
        }

        private async void SongListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ToBeAdded = (Song)e.ClickedItem;
            /* //playsong
            MyMediaElement.SetSource(await song.SongFile.OpenAsync(FileAccessMode.Read),
                song.SongFile.ContentType);*/

            //SongListView.Visibility = Visibility.Collapsed;
            //AddToPlaylistView.Visibility = Visibility.Visible;
            var result = await AddToPlaylistDialog.ShowAsync();
            DatabaseManager.InsertNewSong(ToBeAdded);


        }

        private Point CalcOffsets(UIElement elem)
        {
            // I don't recall the exact specifics on why these
            // calls are needed - but this works.
            var transform = elem.TransformToVisual(this);
            Point point = transform.TransformPoint(new Point(0, 0));

            return point;
        }
        
        private void PlayListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            
            var play = (JonPlaylist)e.ClickedItem;
            CurPlayList.Clear();
            DatabaseManager.getPlaylistLines(play.Id, CurPlayList);
            PlayListView.Visibility = Visibility.Collapsed;
            PlaylistContentView.Visibility = Visibility.Visible;
            BackButton.Visibility = Visibility.Visible;
        }

        private void AddToPlaylistView_ItemClick(object sender, ItemClickEventArgs e)
        {
            DatabaseManager.InsertNewPlaylistLine(ToBeAdded, (JonPlaylist)e.ClickedItem);
            AddToPlaylistDialog.Hide();
        }

        private async void PlaylistContentView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var Line = (PlaylistLine)e.ClickedItem;

            MyMediaElement.SetSource(await Line.song.SongFile.OpenAsync(FileAccessMode.Read),
                Line.song.SongFile.ContentType);
            CurrentSong.Clear();
            CurrentSong.Add(Line.song);
            for(int i = 0; i < CurPlayList.Count; i++)
            {
                if (Line.Title.Equals(CurPlayList[i].Title))
                {
                    CurSong = i;
                    return;
                }
            }
            
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (DatabaseManager.findUser(UserNameTextBox.Text, PassWordTextBox.Text))
            {
                LoginGrid.Visibility = Visibility.Collapsed;
                SplitViewGrid.Visibility = Visibility.Visible;
                UsernameTextBlock.Text = UserNameTextBox.Text;
            }
            /*
            try
            {
                if (UserManager.Login(UserNameTextBox.Text, PassWordTextBox.Text))
                {
                    LoginGrid.Visibility = Visibility.Collapsed;
                    SplitViewGrid.Visibility = Visibility.Visible;
                }
           
            }catch(System.ArgumentException ex)
            {
                UserNameTextBox.Text = ex.Message;
            }
            */
        }

        private void MenuItemsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var choice = (MenuItem)e.ClickedItem;
            switch (choice.category)
            {
                case Category.Playlists:
                    PlayListView.Visibility = Visibility.Visible;
                    SongListView.Visibility = Visibility.Collapsed;
                    break;
                case Category.Songs:
                    SongListView.Visibility = Visibility.Visible;
                    PlayListView.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void BackButton_Click_1(object sender, RoutedEventArgs e)
        {
            PlaylistContentView.Visibility = Visibility.Collapsed;
            PlayListView.Visibility = Visibility.Visible;

            BackButton.Visibility = Visibility.Collapsed;
        }

        private async void MyMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (CurSong < CurPlayList.Count)
            {
                var line = CurPlayList[CurSong + 1];
                CurSong++;
                MyMediaElement.SetSource(await line.song.SongFile.OpenAsync(FileAccessMode.Read),
                    line.song.SongFile.ContentType);
                CurrentSong.Clear();

                CurrentSong.Add(CurPlayList[CurSong].song);
            }
            
            //CurrentSong = CurPlayList[CurSong + 1];
        }

        private void UserNameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            UserNameTextBox.Text = "";
        }

        private void UserNameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (UserNameTextBox.Text.Equals(""))
                UserNameTextBox.Text = "Username";
        }

        private void PassWordTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PassWordTextBox.Text = "";
        }

        private void PassWordTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (PassWordTextBox.Text.Equals(""))
                PassWordTextBox.Text = "Password";
        }

        private void DeletePlayListButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var dplaylist = button.DataContext as JonPlaylist;
            DatabaseManager.DeletePlaylist(dplaylist.Id);
            DatabaseManager.GetPlaylists(Playlists);

        }

        private void PauseButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MyMediaElement.Pause();
            PauseButton.Visibility = Visibility.Collapsed;
            PlayButton.Visibility = Visibility.Visible;
        }

        private void PlayButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MyMediaElement.Play();
            PlayButton.Visibility = Visibility.Collapsed;
            PauseButton.Visibility = Visibility.Visible;
        }

        private void AddPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            DatabaseManager.InsertNewPlaylist(AddPlaylistTextBox.Text);
            DatabaseManager.GetPlaylists(Playlists);
        }

        private async void RegisterNewUserButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await RegisterNewUserDialog.ShowAsync();
        }

        private void NewUserDialogButton_Click(object sender, RoutedEventArgs e)
        {
            if (!DatabaseManager.findUsername(NewUsernameDialogTextBox.Text))
            {
                //new user
                DatabaseManager.InsertNewUser(NewUsernameDialogTextBox.Text, NewPasswordDialogTextBox.Text);
                RegisterNewUserDialog.Hide();
            }else
            {
                UserNameTextBox.Text = "username taken";
            }
        }

        private void DeletePlaylistlineButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var bLine = button.DataContext as PlaylistLine;
            DatabaseManager.DeletePlaylistLine((int)bLine.ID);
            DatabaseManager.getPlaylistLines((int)bLine.PlaylistID, CurPlayList);
            //CurSong--;
        }
    }

}
