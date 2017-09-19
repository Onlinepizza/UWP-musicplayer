using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JonathanProjectOffline.Models
{
    public class PlaylistLine
    {
        public long ID { get; set; }
        public long SongID
        {
            get { return song.Id; }
        }
        public string Title
        {
            get { return song.Title; }
        }
        public string Artist
        {
            get { return song.Artist; }
        }
        public string Album
        {
            get { return song.Album; }
        }
        public long PlaylistID { get; set; }

        public Song song { get; set; }
    }
}
