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
    public class DistanceMatrixService
    {
        public DistanceMatrixService()
        {

        }
        public string GetDistanceMatrixURLEvent(Athlete originAthlete, Event destinationEvent)
        {
            return $"https://maps.googleapis.com/maps/api/distancematrix/json?units=imperial&origins={originAthlete.AthleteLatitude},{originAthlete.AthleteLongitude}&destinations={destinationEvent.LocationsLatitude}%2C{destinationEvent.LocationsLongitude}&key=" + APIKeys.GOOGLE_API_KEY;
        }

        public async Task<double> GetDistanceEvent(Athlete originAthlete, Event destinationEvent)
        {
            string apiURL = GetDistanceMatrixURLEvent(originAthlete, destinationEvent);
            double distanceInMiles = 0;
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
                    JToken rows = jsonResults["rows"][0];
                    JToken elements = rows["elements"][0];
                    JToken distance = elements["distance"];

                    var distanceInMeters = (int)distance["value"];

                    distanceInMiles = MeterConverter.ConvertMetersToMiles(distanceInMeters);
                }
            }
            return distanceInMiles;
        }
        public string GetDistanceMatrixURLLocation(Athlete originAthlete, Location destinationLocation)
        {
            return $"https://maps.googleapis.com/maps/api/distancematrix/json?units=imperial&origins={originAthlete.AthleteLatitude},{originAthlete.AthleteLongitude}&destinations={destinationLocation.LocationLatitude}%2C{destinationLocation.LocationLongitude}&key=" + APIKeys.GOOGLE_API_KEY;
        }

        public async Task<double> GetDistanceLocation(Athlete originAthlete, Location destinationLocation)
        {
            string apiURL = GetDistanceMatrixURLLocation(originAthlete, destinationLocation);
            double distanceInMiles = 0;
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
                    JToken rows = jsonResults["rows"][0];
                    JToken elements = rows["elements"][0];
                    JToken distance = elements["distance"];

                    var distanceInMeters = (int)distance["value"];

                    distanceInMiles = MeterConverter.ConvertMetersToMiles(distanceInMeters);
                }
            }
            return distanceInMiles;
        }
    }
}
