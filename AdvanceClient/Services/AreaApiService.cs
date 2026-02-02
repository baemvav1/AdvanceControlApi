using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using AdvanceClient.Views;

namespace AdvanceClient.Services
{
    /// <summary>
    /// Service interface for Area API operations
    /// </summary>
    public interface IAreaApiService
    {
        Task<bool> CreateAreaAsync(AreaSaveData area);
        Task<bool> UpdateAreaAsync(AreaSaveData area);
        Task<List<AreaData>> GetAreasAsync();
        Task<AreaData?> GetAreaByIdAsync(int id);
        Task<bool> DeleteAreaAsync(int id);
    }

    /// <summary>
    /// Implementation of Area API service
    /// </summary>
    public class AreaApiService : IAreaApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public AreaApiService()
        {
            _httpClient = new HttpClient();
            _baseUrl = "https://localhost:7001/api/Areas"; // Configure as needed
        }

        public AreaApiService(HttpClient httpClient, string baseUrl)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl;
        }

        public async Task<bool> CreateAreaAsync(AreaSaveData area)
        {
            try
            {
                var coordinatesJson = area.Coordenadas != null 
                    ? JsonSerializer.Serialize(area.Coordenadas.ConvertAll(c => new { lat = c.Lat, lng = c.Lng }))
                    : null;

                var queryParams = new List<string>
                {
                    $"nombre={Uri.EscapeDataString(area.Nombre ?? string.Empty)}",
                    $"activo={area.Activo.ToString().ToLower()}"
                };

                if (!string.IsNullOrEmpty(area.Descripcion))
                    queryParams.Add($"descripcion={Uri.EscapeDataString(area.Descripcion)}");
                
                if (!string.IsNullOrEmpty(area.TipoGeometria))
                    queryParams.Add($"tipoGeometria={Uri.EscapeDataString(area.TipoGeometria)}");
                
                if (!string.IsNullOrEmpty(coordinatesJson))
                    queryParams.Add($"coordenadas={Uri.EscapeDataString(coordinatesJson)}");

                var url = $"{_baseUrl}?{string.Join("&", queryParams)}";
                var response = await _httpClient.PostAsync(url, null);
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating area: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateAreaAsync(AreaSaveData area)
        {
            try
            {
                if (!area.IdArea.HasValue)
                    return false;

                var coordinatesJson = area.Coordenadas != null 
                    ? JsonSerializer.Serialize(area.Coordenadas.ConvertAll(c => new { lat = c.Lat, lng = c.Lng }))
                    : null;

                var queryParams = new List<string>();

                if (!string.IsNullOrEmpty(area.Nombre))
                    queryParams.Add($"nombre={Uri.EscapeDataString(area.Nombre)}");
                
                if (!string.IsNullOrEmpty(area.Descripcion))
                    queryParams.Add($"descripcion={Uri.EscapeDataString(area.Descripcion)}");
                
                if (!string.IsNullOrEmpty(area.TipoGeometria))
                    queryParams.Add($"tipoGeometria={Uri.EscapeDataString(area.TipoGeometria)}");
                
                queryParams.Add($"activo={area.Activo.ToString().ToLower()}");
                
                if (!string.IsNullOrEmpty(coordinatesJson))
                    queryParams.Add($"coordenadas={Uri.EscapeDataString(coordinatesJson)}");

                var url = $"{_baseUrl}/{area.IdArea.Value}";
                if (queryParams.Count > 0)
                    url += $"?{string.Join("&", queryParams)}";
                
                var response = await _httpClient.PutAsync(url, null);
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating area: {ex.Message}");
                return false;
            }
        }

        public async Task<List<AreaData>> GetAreasAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseUrl);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<AreaData>>() ?? new List<AreaData>();
                }
                return new List<AreaData>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting areas: {ex.Message}");
                return new List<AreaData>();
            }
        }

        public async Task<AreaData?> GetAreaByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}?idArea={id}");
                if (response.IsSuccessStatusCode)
                {
                    var areas = await response.Content.ReadFromJsonAsync<List<AreaData>>();
                    return areas?.Count > 0 ? areas[0] : null;
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting area by id: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteAreaAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting area: {ex.Message}");
                return false;
            }
        }
    }
}
