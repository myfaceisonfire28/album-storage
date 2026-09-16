using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Collections;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Swan.Parsers;
using Music.Spotify_Help;
using SpotifyAPI.Web;
using System.Threading.Tasks;


namespace Music
{
    class Program
    {
        static bool PlayMusic = true;
        public static void Main()
        {
            TokenManager.GetToken();
            MusicGetter.GetMusic();
            Album RandomAlbum = null;
            
            string input = "";
            while(input != "quit")
            {
                Console.WriteLine("Commands: Add, New, Old, Any, Last, Update, Total, Best, Play, Backup, Quit");
                input = Console.ReadLine().ToLower();

                switch (input)
                {
                    case "backup":
                        File.Delete("../Documents/backup_music.xml");
                        File.Copy("music.xml", "../Documents/backup_music.xml");
                    break;

                    case "play":
                        PlayMusic = !PlayMusic;
                        Console.WriteLine($"Will ask to add to queue: {PlayMusic}");
                        PressAnyKey();
                    break;
                    
                    case "add":
                        Console.WriteLine("Give album name");
                        var Album = Console.ReadLine();
                        
                        Console.WriteLine("Give artist name");
                        var Artist = Console.ReadLine();
                        
                        Console.WriteLine("Give album URL");
                        var URL = Console.ReadLine();
                        try
                        {
                            MusicGetter.AddAlbum(Album, Artist, URL);
                        }
                        catch(ArgumentException e)
                        {
                            Console.WriteLine(e.Message);
                            PressAnyKey();
                        }
                    break;

                    case "total":
                        Console.Clear();
                        Console.WriteLine($"Total albums: {MusicGetter.root.allMusic.listOfAlbums.Count}");
                        Console.WriteLine($"Total listened: {MusicSearcher.GetAlbumsListenedTo().Count}");
                        Console.WriteLine($"Total yet to listen: {MusicSearcher.GetAlbumsNotListenedTo().Count}");
                        PressAnyKey();
                    break;

                    case "new":
                        Console.Clear();
                        RandomAlbum = MusicSearcher.GetRandomNewAlbum();
                        PrintAndPlayQuestion(RandomAlbum);
                    break;

                    case "old":
                        try
                        {
                            Console.Clear();
                            RandomAlbum = MusicSearcher.GetRandomOldAlbum();
                            PrintAndPlayQuestion(RandomAlbum);

                        }
                        catch(ArgumentException e)
                        {
                            Console.Clear();
                            Console.WriteLine(e.Message);
                            PressAnyKey();
                        }
                    break;
                    
                    case "last":
                        if(RandomAlbum == null)
                        {
                            Console.WriteLine("You do not have a last album silly.");
                            PressAnyKey();
                            break;
                        }
                        Console.Clear();
                        PrintAlbum(RandomAlbum);
                        PressAnyKey();
                    break;

                    case "any":
                        Console.Clear();
                        RandomAlbum = MusicSearcher.GetRandomAlbum();
                        PrintAndPlayQuestion(RandomAlbum);
                    break;

                    case "update":
                        Console.WriteLine("Give album index");
                        var index = int.Parse(Console.ReadLine());
                        if((bool)MusicGetter.root.allMusic.listOfAlbums[index].listedTo)
                        {
                            Console.WriteLine("Already reviewed it");
                            PressAnyKey();
                            break;
                        }
                        

                        if(MusicGetter.root.allMusic.listOfAlbums[index].rating == -1)
                        {
                            Console.WriteLine("Give a rating out of 10");
                            var rating = float.Parse(Console.ReadLine());
                            Console.WriteLine("Give some initial thoughts");
                            var thoughts = Console.ReadLine();
                            MusicGetter.UpdateAlbumRating(index, rating, thoughts);
                        }
                        MusicGetter.UpdateAlbum(index);
                    break;

                    case "best":
                        GetTop();
                    break;

                }
                Console.Clear();

            }
        }

        public static void PressAnyKey()
        {
            Console.WriteLine("press any key to continue...");
            Console.ReadKey();
        }

        private static void PrintAndPlayQuestion(Album album)
        {
            PrintAlbum(album);
            if(PlayMusic)
            {
                DoYouWantToPlay(album.albumURL);
            }
            PressAnyKey();
        }


        public static void PrintAlbum(Album album)
        {
            Console.WriteLine($"{album.albumName} by {album.artistName}");
            Console.WriteLine($"    Added: {((DateTime)album.dateAdded)}");
            Console.WriteLine($"    URL: {album.albumURL}");
            Console.WriteLine($"    Index: {album.index} (Important for marking as listened to)");
            Console.WriteLine($"    Listened to: {album.listedTo}");
            if((bool)album.listedTo)
            {
            Console.WriteLine($"    Listened on: {((DateTime)album.dateListenedTo)}");
            Console.WriteLine($"    Rating: {album.rating}");
            Console.WriteLine($"    Initial thoughts: \n {album.thoughts}");
            }
        }

        public static async Task DoYouWantToPlay(string Url)
        {
            Console.WriteLine("Do you want to add album to queue? Y/N");
            var key = Console.ReadKey().KeyChar;
            if(key == 'y')
            {
                Console.Clear();
                try
                {
                    await Spotify_Helper.PlayAlbum(Url);
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                }

                return;
            }
            Console.Clear();
            Console.WriteLine("Your loss!");            
        }


        public static void GetTop()
        {
            int currentIndex = 0;
            string input = "";
            while(input != "quit")
            {
                List<List<Album>> AllAlbums = MusicSearcher.GetHighestRatedAlbums();

                int maxIndex = AllAlbums.Count-1;

                PrintAlbums(AllAlbums[currentIndex]);
                Console.WriteLine("Next, Back, Page, Quit");
                Console.WriteLine($"Page {currentIndex+1} out of {maxIndex+1}");
                input = Console.ReadLine();
                switch(input.ToLower())
                {
                    case "next":
                        Console.Clear();
                        if(currentIndex != maxIndex)
                        {
                            currentIndex++;
                        }
                    break;

                    case "back":
                        Console.Clear();
                        if(currentIndex != 0)
                        {
                            currentIndex--;
                        }
                    break;

                    case "page":
                        Console.WriteLine("Give page to skip to");
                        int Page = int.Parse(Console.ReadLine())-1;
                        
                        if(Page >= 0 && Page <= maxIndex)
                        {
                            currentIndex=Page;
                        }
                    break;
                }
            }
        }


        public static void PrintAlbums(List<Album> albums)
        {
            Console.Clear();
            Console.WriteLine("================================");
            foreach(var album in albums)
            {
                PrintAlbum(album);
                Console.WriteLine("================================");
            }
        }
    }
    
}