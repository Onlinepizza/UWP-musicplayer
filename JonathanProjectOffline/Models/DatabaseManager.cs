using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLitePCL;
using Windows.Storage;
using System.Collections.ObjectModel;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;
using System.IO;

namespace JonathanProjectOffline.Models
{
    public static class DatabaseManager
    {
        public static SQLiteConnection dbConnection = new SQLiteConnection("MusicApp.db");
        //public static SQLiteConnection dbConnection = new SQLiteConnection("MusicAppM.db");
        public static long CurrentUserID;

        public static void CreateDB()
        {
            SQLiteConnection dbConnection = new SQLiteConnection("MusicApp.db");

            CreateSongDB();
            CreateUserDB();
            CreatePlaylistDB();
            //OpenFolder();
            var statement = dbConnection.Prepare(@"PRAGMA foreign_keys = ON");
            statement.Step();
        }

        public async static void CreateMobileDB()
        {
            StorageFile db = null;
            // StorageFolder folder = KnownFolders.RemovableDevices;
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            await RetrieveDbInFolders(db, folder, (parm) =>
            { db = parm; });
            if (db != null)
            {

                string ConnString = Path.Combine(folder.Path, "MusicAppM.db");
                SQLiteConnection dbConnection = new SQLiteConnection(ConnString);
            }
        }

        private static async Task RetrieveDbInFolders(StorageFile db, StorageFolder parent, Action<StorageFile> callback = null)
        {
            foreach (var item in await parent.GetFilesAsync())
            {
                if (item.FileType == ".db")
                    db = item;
                callback(db);
            }
            foreach (var item in await parent.GetFoldersAsync())
            {
                await RetrieveDbInFolders(db, item);
            }
        }
    

    /*
    public static async void CreateMobileDB()
    {

        StorageFile db = null;
        StorageFolder folder = KnownFolders.RemovableDevices;
        await RetrieveDbInFolders(db, folder);
        string ConnString = Path.Combine(db.Path, "MusicAppM.db");
        SQLiteConnection dbConnection = new SQLiteConnection(ConnString);

    }

    private static async Task RetrieveDbInFolders(StorageFile db, StorageFolder parent)
    {
        foreach (var item in await parent.GetFilesAsync())
        {
            if (item.FileType == ".db")
                db = item;
        }
        foreach (var item in await parent.GetFoldersAsync())
        {
            await RetrieveDbInFolders(db, item);
        }
    }*/

    public static bool findUsername(string username)
        {
            using (var statement = dbConnection.Prepare(@"SELECT [ID] FROM User WHERE Username = (?)"))
            {
                statement.Bind(1, username);
                if (statement.Step() == SQLiteResult.ROW)
                {
                    CurrentUserID = (long)statement[0];
                    return true;
                }
                else
                    return false;
            }

        }

        public static bool findUser(string username, string password)
        {
            using(var statement = dbConnection.Prepare(@"SELECT [ID] FROM User WHERE Username = (?) AND Password = (?)"))
            {
                statement.Bind(1, username);
                statement.Bind(2, password);
                if(statement.Step() == SQLiteResult.ROW)
                {
                    CurrentUserID = (long)statement[0];
                    return true;
                }else
                return false;
            }

        }
        

        //TABLE CREATION

        private static async void OpenFolder()
        {
            StorageFolder local_Cachefolder = ApplicationData.Current.LocalCacheFolder;
            await Windows.System.Launcher.LaunchFolderAsync(local_Cachefolder);

        }

        private static void CreateUserDB()
        {
            string sSQL = @"CREATE TABLE IF NOT EXISTS User
                    (ID INTEGER Primary Key AutoIncrement NOT NULL
                    , Username VARCHAR(200) UNIQUE
                    , Password VARCHAR(200));";
            ISQLiteStatement cnStatement = dbConnection.Prepare(sSQL);
            cnStatement.Step();
        }

        private static void CreatePlaylistDB()
        {
            string sSQL = @"CREATE TABLE IF NOT EXISTS Playlist
                    (ID INTEGER Primary Key AutoIncrement NOT NULL
                    , Title VARCHAR(200) 
                    , UserID INT references User(ID));";
            ISQLiteStatement cnStatement = dbConnection.Prepare(sSQL);
            cnStatement.Step();
            CreatePlaylistLineDB();
        }

        private static void CreatePlaylistLineDB()
        {
            string sSQL = @"CREATE TABLE IF NOT EXISTS Playlistline
                    (ID INTEGER Primary Key AutoIncrement NOT NULL
                    , PlaylistID INT references Playlist(ID) 
                    , SongID INT references Music(ID));";
            ISQLiteStatement cnStatement = dbConnection.Prepare(sSQL);
            cnStatement.Step();
        }

        private static void CreateSongDB()
        {
            string sSQL = @"CREATE TABLE IF NOT EXISTS Song
                    (ID INTEGER Primary Key AutoIncrement NOT NULL
                    , Artist VARCHAR(200)
                    , Title VARCHAR(200)
                    , Album VARCHAR(200)
                    , Path VARCHAR(255)
                    , Type VARCHAR(50));";
            ISQLiteStatement cnStatement = dbConnection.Prepare(sSQL);
            cnStatement.Step();
        }

        //TABLE CREATION END

        //INSERT

        public static void InsertNewUser(string username, string password)
        {
            using (var statement = dbConnection.Prepare(@"INSERT INTO User(Username, Password) VALUES (?,?)"))
            {
                statement.Bind(1, username);
                statement.Bind(2, password);
                statement.Step();
            }
        }

        public static void InsertNewPlaylist(string title)
        {
            using (var statement = dbConnection.Prepare(@"INSERT INTO Playlist(Title, UserID) VALUES (?,?)"))
            {
                statement.Bind(1, title);
                statement.Bind(2, CurrentUserID);
                statement.Step();
            }
        }

        public static void InsertNewPlaylistLine(Song song, JonPlaylist playlist)
        {
            InsertNewSong(song);
            using (var statement = dbConnection.Prepare(@"INSERT INTO Playlistline(PlaylistID, SongID) VALUES(?,?)"))
            {
                statement.Bind(1, playlist.Id);
                statement.Bind(2, getSong(song));
                statement.Step();

            }
        }

        public static void InsertNewSong(Song song)
        {
            if (getSong(song) == -1)
            {
                using (var statement = dbConnection.Prepare(@"INSERT INTO Song(Artist, Title, Album, Path) VALUES (?,?,?,?)"))
                {
                    statement.Bind(1, song.Artist);
                    statement.Bind(2, song.Title);
                    statement.Bind(3, song.Album);
                    string path = song.SongFile.Path;
                    statement.Bind(4, path);
                    statement.Step();

                }
            }
        }

        //INSERT END

        //GET

        public static void GetPlaylists(ObservableCollection<JonPlaylist> list)
        {
            list.Clear();
            using(var statement = dbConnection.Prepare(@"Select [ID],[Title] FROM Playlist WHERE UserID = ? "))
            {
                statement.Bind(1, CurrentUserID);
                while(statement.Step() == SQLiteResult.ROW)
                {
                    var sid = (long)statement[0];
                    string sTitle = (string)statement[1];
                    JonPlaylist plist = new JonPlaylist(sTitle);
                    plist.Id = (int)sid;
                    list.Add(plist);
                }
            }
        }

        public static async void getPlaylistLines(int playlistID, ObservableCollection<PlaylistLine> playlistlines)
        {
            playlistlines.Clear();
            using (var statement = dbConnection.Prepare(@"SELECT [SongID],[ID] FROM Playlistline WHERE PlaylistID = ?"))
            {
                statement.Bind(1, playlistID);
                while (statement.Step() == SQLiteResult.ROW)
                {
                    var sID = (long)statement[0];
                    Song newsong = await getSong((int)sID);
                    PlaylistLine newLine = new PlaylistLine { ID = (long)statement[1],PlaylistID = playlistID, song = newsong };
                    playlistlines.Add(newLine);
                }
            }
        }

        /// <summary>
        /// This method fetches a song in the database by ID.
        /// </summary>
        /// <param name="sID"></param>
        /// <returns></returns>
        private static async Task<Song> getSong(int sID)
        {
            //TODO IMPLIMENT TYPE CHECK
            Song newsong = null;
            using (var statement = dbConnection.Prepare(@"SELECT [ID],[Path] FROM Song WHERE ID = ?"))
            {
                statement.Bind(1, sID);
                //SQLiteResult.DONE == statement.Step()
                var result = statement.Step();
                if(SQLiteResult.ROW == result)
                {
                    string path = (string)statement[1];
                    StorageFolder folder = KnownFolders.MusicLibrary;
                    StorageFile songfile = await StorageFile.GetFileFromPathAsync(path);
                    MusicProperties prop = await songfile.Properties.GetMusicPropertiesAsync();
                    StorageItemThumbnail currentThumb = await songfile.GetThumbnailAsync(ThumbnailMode.MusicView, 200, ThumbnailOptions.UseCurrentScale);
                    var albumCover = new BitmapImage();
                    albumCover.SetSource(currentThumb);
                    var songID = (long)statement[0];
                    newsong = new LocalSong()
                    {
                        Id = (int)songID,
                        SongFile = songfile,
                        Artist = prop.Artist,
                        Album = prop.Album,
                        Title = prop.Title,
                        AlbumCover = albumCover
                        
                    };
                }
            }
            return newsong;
        }

        /// <summary>
        /// This is an overload of the method getsong which searches for the song in the datastructure and retrieves the ID.
        /// </summary>
        /// <param name="song"></param>
        /// <returns></returns>
        private static long getSong(Song song)
        {
            //TODO IMPLIMENT TYPE CHECK
            long returnvalue = -1;
            using (var statement = dbConnection.Prepare(@"SELECT [ID] FROM Song WHERE Title = ? AND Album = ?"))
            {
                statement.Bind(1, song.Title);
                statement.Bind(2, song.Album);
                
                //SQLiteResult.DONE == statement.Step()
                var result = statement.Step();
                if (SQLiteResult.ROW == result)
                {
                    returnvalue = (long)statement[0];
                }
            }
            return returnvalue;
        }

        //GET END

        public static void DeletePlaylist(int pId)
        {
            using (var statement = dbConnection.Prepare(@"DELETE FROM Playlistline Where PlaylistID = ?"))
            {
                statement.Bind(1, pId);
                statement.Step();
                using (var nStatement = dbConnection.Prepare(@"DELETE FROM Playlist Where ID = ?"))
                {
                    nStatement.Bind(1, pId);
                    nStatement.Step();
                }
            }
        }

        public static void DeletePlaylistLine(int pId)
        {
            using (var statement = dbConnection.Prepare(@"DELETE FROM Playlistline Where ID = ?"))
            {
                statement.Bind(1, pId);
                statement.Step();
            }
        }

       /* public static void DeletePlaylistLineBySong(int sID)
        {
            using (var statement = dbConnection.Prepare(@"SELECT [ID] FROM Playlistline WHERE SongID = ? limit 1"))
            {
                if(statement.Step() == SQLiteResult.ROW)
                {
                    var id = (long)statement[0];
                    var lineID = (Int64)id; 
                    DeletePlaylistLine((int)lineID);
                }
            }
        }*/
       
        public static async Task FillSongTable()
        {
            // 1. get access to music library

            StorageFolder folder = KnownFolders.MusicLibrary;
            var allSongs = new List<StorageFile>();
            await RetrieveFilesInFolders(allSongs, folder);
        }

        private static async Task RetrieveFilesInFolders(List<StorageFile> list, StorageFolder parent)
        {
            foreach (var item in await parent.GetFilesAsync())
            {
                if (item.FileType == ".mp3")
                {
                    list.Add(item);
                    MusicProperties songProperties = await item.Properties.GetMusicPropertiesAsync();
                    string sArtist = songProperties.Artist;
                    string sTitle = songProperties.Title;
                    string sAlbum = songProperties.Album;
                    string sPath = item.Path;
                    sArtist = sArtist.Replace("'", "''");
                    sTitle = sTitle.Replace("'", "''");
                    sAlbum = sAlbum.Replace("'", "''");
                    sPath = sPath.Replace("'", "''");


                    string sSQL = @"INSERT INTO [Music]
                            ([Artist],[Title],[Album],[Path])
                                VALUES 
                            ('" + sArtist + "','" + sTitle + "','" + sAlbum + "','" + sPath + "');";


                    dbConnection.Prepare(sSQL).Step();
                }

            }

            foreach (var item in await parent.GetFoldersAsync())
            {
                await RetrieveFilesInFolders(list, item);
            }
        }

    }
}
