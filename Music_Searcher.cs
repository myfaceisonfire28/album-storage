using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using Music;
using System.Security.Cryptography;

namespace Music
{
    public class MusicSearcher
    {
        public static List<Album> GetAlbumsNotListenedTo()
        {
            return MusicGetter.root.allMusic.listOfAlbums.FindAll(alb => alb.listedTo is false);
        }

        public static List<Album> GetAlbumsListenedTo()
        {
            return MusicGetter.root.allMusic.listOfAlbums.FindAll(alb => alb.listedTo is true);
        }
        public static Album GetRandomAlbum()
        {
            int amount = MusicGetter.root.allMusic.listOfAlbums.Count;
            return MusicGetter.root.allMusic.listOfAlbums[NewRandomNumber(amount)]; 
        }

        public static Album GetRandomOldAlbum()
        {
            var newAlbums = GetAlbumsListenedTo();
            int amount = newAlbums.Count;

            if(amount < 1) {throw new ArgumentException("No listened to albums");}
            return newAlbums[NewRandomNumber(amount)];
        }

        public static Album GetRandomNewAlbum()
        {
            var newAlbums = GetAlbumsNotListenedTo();
            int amount = newAlbums.Count;

            return newAlbums[NewRandomNumber(amount)];
        }

        static int lastRandomNumb = 0;
        public static int NewRandomNumber(int max)
        {
            int temp = -1;
            while(temp == lastRandomNumb || temp == -1)
            {
                temp = new Random().Next(0, max);
                Thread.Sleep(200);
            }
            if(max > 1)
            {
                lastRandomNumb = temp;
            }
            return temp;
        } 


        public static List<List<Album>> GetHighestRatedAlbums()
        {
            List<Album> albums = GetAlbumsListenedTo();
            int Length = albums.Count;
            if(Length == 0) {throw new ArgumentException("Have not listened to anything");}

            bool Swapped = true;
            while(Swapped)
            {
                bool temp1 = false;
                for(int i = 0; i < Length-1; i++)
                {
                    if(albums[i].rating < albums[i+1].rating)
                    {
                        var temp = albums[i];
                        albums[i] = albums[i+1];
                        albums[i+1] = temp;
                        temp1 = true;
                    }
                }
                Swapped = temp1;
            }
            
            int Times = Length/5;
            int Remainder = Length%5;

            List<List<Album>> test = new List<List<Album>>();

            for(int i = 0; i < Times; i++)
            {
                test.Add(new List<Album>());
                for(int y = 0; y < 5; y++)
                {
                    test[i].Add(albums[i*5+y]);
                }
            }
            if(Remainder != 0)
            {
                test.Add(new List<Album>());
            }
            for(int i = 0; i < Remainder; i++)
            {
                test.Last().Add(albums[Times*5+i]);
            }


            return test;



        }
    }
}