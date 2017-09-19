using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace JonathanProjectOffline.Models
{
    public abstract class Song
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public StorageFile SongFile { get; set; }
        public bool Selected { get; set; }
        public bool Used { get; set; }

        public BitmapImage AlbumCover;

        public abstract void Play();

    }
}
