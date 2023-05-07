using Microsoft.EntityFrameworkCore;
using Sales.API.Intefaces;
using Sales.Shared.Entities;
using Sales.Shared.Responses;

namespace Sales.API.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IApiService _apiService;

        public SeedDb(DataContext dataContext, IApiService apiService)
        {
            _context = dataContext;
            _apiService = apiService;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckCountriesAsync();
            await CheckCategoriesAsync();
        }

       


        private async Task CheckCategoriesAsync()
        {
            if (!_context.Categories.Any())
            {
                _context.Categories.Add(new Category() { Name = "Cuidado Personal" });
                _context.Categories.Add(new Category() { Name = "Deportes y Fitness" });
                _context.Categories.Add(new Category() { Name = "Hogar y Muebles" });
                _context.Categories.Add(new Category() { Name = "Inmuebles" });
                _context.Categories.Add(new Category() { Name = "Juegos" });
                _context.Categories.Add(new Category() { Name = "Moda" });
                _context.Categories.Add(new Category() { Name = "Supermercado" });
                _context.Categories.Add(new Category() { Name = "Salud y Equipo Médico" });
                _context.Categories.Add(new Category() { Name = "Servicios" });
                _context.Categories.Add(new Category() { Name = "Tenis" });
                _context.Categories.Add(new Category() { Name = "Tecnología" });
                _context.Categories.Add(new Category() { Name = "Vehiculos" });
            }

            await _context.SaveChangesAsync();
        }

        private async Task CheckCountriesAsync()
        {
            if (!_context.Countries.Any())
            {
                var responseCountries = await _apiService.GetListAsync<CountryResponse>("/v1", "/countries");
                if (responseCountries.IsSuccess)
                {
                    var countries = responseCountries.Result!;
                    foreach (CountryResponse countryResponse in countries)
                    {
                        var country = await _context.Countries!.FirstOrDefaultAsync(c => c.Name == countryResponse.Name!)!;
                        if (country == null)
                        {
                            country = new Country { Name = countryResponse.Name!, States = new List<State>() };
                            var responseStates = await _apiService.GetListAsync<StateResponse>("/v1", $"/countries/{countryResponse.Iso2}/states");
                            var states = responseStates.Result!;
                            if (responseStates.IsSuccess)
                            {
                                foreach (StateResponse stateResponse in states!)
                                {
                                    var state = country.States!.FirstOrDefault(s => s.Name == stateResponse.Name!)!;
                                    if (state == null)
                                    {
                                        state = new State { Name = stateResponse.Name!, Cities = new List<City>() };
                                        var responseCities = await _apiService.GetListAsync<CityResponse>("/v1", $"/countries/{countryResponse.Iso2}/states/{stateResponse.Iso2}/cities");
                                        if (responseCities.IsSuccess)
                                        {
                                            var cities = responseCities.Result!;

                                            foreach (CityResponse cityResponse in cities)
                                            {
                                                if (cityResponse.Name == "Mosfellsbær" || cityResponse.Name == "Șăulița")
                                                {
                                                    continue;
                                                }
                                                var city = state.Cities!.FirstOrDefault(c => c.Name == cityResponse.Name!)!;
                                                if (city == null)
                                                {
                                                    state.Cities.Add(new City() { Name = cityResponse.Name! });
                                                }
                                            }


                                        }
                                        if (state.CitiesCount > 0)
                                        {
                                            country.States.Add(state);
                                        }
                                    }

                                }

                            }

                            if (country.StatesCount > 0)
                            {
                                _context.Countries.Add(country);
                                await _context.SaveChangesAsync();
                            }

                        }
                    }
                }

                await _context.SaveChangesAsync();
            }

        }
    }
}
