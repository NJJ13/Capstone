using FreshAir.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FreshAir.Services
{
    public class GeocodeServiceLocation
    {
        public GeocodeServiceLocation()
        {
                
        }
        public string GetGeocodingURL(Location place)
        {
            return $"https://maps.googleapis.com/maps/api/geocode/json?address={place.Address}+{place.City}+{place.State}+{place.ZipCode}+&key=" + APIKeys.GOOGLE_API_KEY;
        }

        public async Task<Location> GetGeocoding(Location place)
        {
            string apiURL = GetGeocodingURL(place);

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(apiURL);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    JObject jsonResults = JsonConvert.DeserializeObject<JObject>(data);
                    JToken results = jsonResults["results"][0];
                    JToken location = results["geometry"]["location"];

                    place.LocationLatitude = (double)location["lat"];
                    place.LocationLongitude = (double)location["lng"];
                }
            }
            return place;
        }
    }
}
