using System.Threading.Tasks;
using SpotifyAPI.Web;

namespace Music.Spotify_Help
{
    class Spotify_Helper
    {
        public static async Task PlayAlbum(string Url)
        {
            if(!IsAlbumValid(Url)){throw new ArgumentException("Album is not a valid spotify album.");}
            
            if(!await IsSpotifyUsed()) {Console.WriteLine("Spotify not in use"); return;}

            var SpotAlbum = await TokenManager.SpotClient.Albums.Get(GetURI(Url));
            Console.Write("Adding album to queue");
            foreach(var Item in SpotAlbum.Tracks.Items)
            {
                await TokenManager.SpotClient.Player.AddToQueue(new PlayerAddToQueueRequest(Item.Uri));
                Console.Write(".");
                Thread.Sleep(1000);
            }
            Console.Write("Finished!!! \n");
        }

        private static string GetURI(string Url)
        {
            string Uri = Url.Split("album/")[1];
            if(Uri.Contains("?"))
            {
                Uri = Uri.Split("?")[0];
            }
            return Uri;
        }

        private static bool IsAlbumValid(string AlbumUrl)
        {
            return AlbumUrl.Contains("spotify");
        }

        private static async Task<bool> IsSpotifyUsed()
        {
            var temp = await TokenManager.SpotClient.Player.GetCurrentlyPlaying(new PlayerCurrentlyPlayingRequest());
            return temp.IsPlaying;
        }



    }
}