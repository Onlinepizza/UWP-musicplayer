using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JonathanProjectOffline.Models
{
    public class JonPlaylist
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ObservableCollection<Song> Songs { get; set; }
        public int Count
        {
            get { return Songs.Count; }
        }

        public JonPlaylist(string title)
        {
            Title = title;
            Songs = new ObservableCollection<Song>();

        }

        public void AddToList(Song song)
        {
            Songs.Add(song);
        }
    }
}
