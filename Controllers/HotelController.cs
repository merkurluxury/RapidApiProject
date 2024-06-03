using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RapidApiProject.Models;
using static System.Net.WebRequestMethods;

namespace RapidApiProject.Controllers
{
    public class HotelController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SearchHotel(SearchViewModel search)
        {

            string destid = await GetDestId(search.City);
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://booking-com15.p.rapidapi.com/api/v1/hotels/searchHotels?dest_id={destid}&search_type=CITY&arrival_date={search.CheckIn.ToString("yyyy-MM-dd")}&departure_date={search.CheckOut.ToString("yyyy-MM-dd")}&adults={search.AdultCount}&room_qty={search.RoomCount}&page_number=1&languagecode=en-us&currency_code=EUR"),
                Headers =
    {




        { "X-RapidAPI-Key", "4268250e9amsh944d049f1a046b0p1d44b5jsn236e496eb6e7" },
        { "X-RapidAPI-Host", "booking-com15.p.rapidapi.com" },
    },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<HotelListViewModel>(body);
                TempData["Photo"] = values.data.hotels[0].property.photoUrls[0].Replace("square60", "square480");
                return View(values.data.hotels.ToList());
            }

        }

        public async Task<string> GetDestId(string city)

        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://booking-com15.p.rapidapi.com/api/v1/hotels/searchDestination?query={city}"),
                Headers =
                    {
                      { "X-RapidAPI-Key", "4268250e9amsh944d049f1a046b0p1d44b5jsn236e496eb6e7" },
                        { "X-RapidAPI-Host", "booking-com15.p.rapidapi.com" },
                    },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<CityIdViewsModel>(body);
                return values.data[0].dest_id.ToString();
            }
        }



        [HttpPost]
        public async Task<IActionResult> HotelDetails(string hotelID, string arrivalDate, string departureDate)
        {

            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://booking-com15.p.rapidapi.com/api/v1/hotels/getHotelDetails?hotel_id={hotelID}&arrival_date={arrivalDate}&departure_date={departureDate}&languagecode=en-us&currency_code=EUR"),
                Headers = {
                  { "X-RapidAPI-Key", "4268250e9amsh944d049f1a046b0p1d44b5jsn236e496eb6e7" },
                  {
                     "X-RapidAPI-Host",
                     "booking-com15.p.rapidapi.com"
                  },
               },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<HotelDetailsViewModel>(body);
                return View(value);
            }
        }


    }
}



