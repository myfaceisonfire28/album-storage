using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Music
{
    public class MusicGetter
    {
        public static Root? root;

        /// <summary>
        ///  Gets stored music
        /// </summary>
        public static void GetMusic()
        {
            root = new Root();
            XmlSerializer serializer = new XmlSerializer(typeof(Root));
            FileStream stream = File.OpenRead("music.xml");
            
            root = (Root)serializer.Deserialize(stream);
            stream.Dispose();
        }

        /// <summary>
        ///  Stores new music
        /// </summary>
        private static void WriteMusic()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Root));
            FileStream stream = File.OpenWrite("music.xml");
            
            serializer.Serialize(stream, root);
            stream.Dispose();
        }

        /// <summary>
        ///  Adds new music to list
        /// </summary>
        public static void AddAlbum(string albumName, string artistName, string URL)
        {
           var newAlbum = new Album();

           newAlbum.artistName = artistName;
           newAlbum.albumName = albumName; 
           newAlbum.albumURL = URL; 
           newAlbum.dateAdded = DateTime.Now;
           newAlbum.listedTo = false;
           newAlbum.index = root.allMusic.listOfAlbums.Count();

           if(IsAlbumInHere(newAlbum))
           {
                throw new ArgumentException("Album already stored");
           }

           root.allMusic.listOfAlbums.Add(newAlbum);
           WriteMusic();
        }

        /// <summary>
        ///  Checks if an album is in the database yet
        /// </summary>
        public static bool IsAlbumInHere(Album albums)
        {
            foreach(Album alb in root.allMusic.listOfAlbums)
            {
                if(alb.albumName.ToLower() == albums.albumName.ToLower())
                {
                    if(alb.artistName.ToLower() == albums.artistName.ToLower())
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        /// <summary>
        ///  Updates if the album has been listened to
        /// </summary>
        public static void UpdateAlbum(int albumIndex)
        {
            root.allMusic.listOfAlbums[albumIndex].listedTo = true;
            root.allMusic.listOfAlbums[albumIndex].dateListenedTo = DateTime.Now;

            WriteMusic();
        }

        public static void UpdateAlbumRating(int albumIndex, float rating, string thoughts)
        {
            root.allMusic.listOfAlbums[albumIndex].rating = rating;
            


            root.allMusic.listOfAlbums[albumIndex].thoughts = thoughts;
            
            WriteMusic();
        }
    }
}