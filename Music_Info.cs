using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Music;

namespace Music
{
    public class Root
    {
        public AllMusic? allMusic;
    }

    public class AllMusic
    {
        public List<Album>? listOfAlbums = new List<Album>();
    }

    public class Album
    {
        public string? albumName;
        public string? albumURL;
        public string? artistName;
        public DateTime? dateAdded;
        public DateTime? dateListenedTo;
        public bool? listedTo;
        public int? index;
        public float? rating = -1;
        public string? thoughts;
    }
}