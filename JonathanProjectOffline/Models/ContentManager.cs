using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;

namespace JonathanProjectOffline.Models
{
    public static class ContentManager
    {
        public static void AddSongToPlaylist(JonPlaylist playlist, Song song)
        {
            playlist.AddToList(song);
        }

        public static void DeletePlayList(JonPlaylist playlist, ObservableCollection<JonPlaylist> playlists)
        {
            playlists.Remove(playlist);
            /*for(int i = 0; i < playlists.Count; i++)
            {
                if (playlist.Title.Equals(playlists[i].Title))
                {
                    playlists.RemoveAt(i);
                    return;
                }
            }*/
        }

        //MIGRATION
        /*
        public static async Task InitializeLocalFiles(ObservableCollection<Song> Songs)
        {

            //CreatePlaylists();
            var AllSongs = await SetupMusicList();
            await ImportMusicLibrary(Songs, Allsongs);

        }

        private static async Task ImportMusicLibrary(ObservableCollection<Song> Songs, ObservableCollection<StorageFile> Allsongs)
        {
            Songs.Clear();
            await PopulateSongList(AllSongs.ToList());
        }
        
        private static async Task<ObservableCollection<StorageFile>> SetupMusicList()
        {
            // 1. get access to music library

            StorageFolder folder = KnownFolders.MusicLibrary;
            var allSongs = new ObservableCollection<StorageFile>();
            await RetrieveFilesInFolders(allSongs, folder);
            return allSongs;
        }

        private static async Task RetrieveFilesInFolders(ObservableCollection<StorageFile> list, StorageFolder parent)
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

        private static async Task PopulateSongList(List<StorageFile> files, ObservableCollection<Song>Songs)
        {
            int id = 0;
            foreach (var file in files)
            {

                MusicProperties songProperties = await file.Properties.GetMusicPropertiesAsync();

                StorageItemThumbnail currentThumb = await file.GetThumbnailAsync(ThumbnailMode.MusicView, 200, ThumbnailOptions.UseCurrentScale);

                var albumCover = new BitmapImage();
                albumCover.SetSource(currentThumb);
                var song = new Song();

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
        */
        //MIGRATION
    }
}
