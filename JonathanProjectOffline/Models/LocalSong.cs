using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace JonathanProjectOffline.Models
{
    public class LocalSong : Song
    {
        public StorageFile SongFile { get; set; }

        public override void Play()
        {
            throw new NotImplementedException();
        }
    }
}
