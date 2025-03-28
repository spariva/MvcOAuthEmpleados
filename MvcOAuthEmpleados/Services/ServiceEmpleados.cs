using System.Net.Http.Headers;
using System.Text;
using MvcOAuthEmpleados.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MvcOAuthEmpleados.Services
{
    public class ServiceEmpleados
    {
        private string UrlApi;
        private MediaTypeWithQualityHeaderValue Header;
        private IHttpContextAccessor httpContextAccessor;

        public ServiceEmpleados(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.UrlApi = configuration.GetValue<string>("ApiUrls:ApiEmpleados");
            this.Header = new MediaTypeWithQualityHeaderValue("application/json");
            this.httpContextAccessor = httpContextAccessor;
        }


        public async Task<string> GetTokenAsync(string user, string pass)
        {
            LoginModel model = new LoginModel
            {
                UserName = user,
                Password = pass
            };

            using (HttpClient client = new HttpClient())
            {
                string request = "api/auth/login";
                client.BaseAddress = new Uri(UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string json = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);

                if (response.IsSuccessStatusCode)
                {
                    string data = await
                    response.Content.ReadAsStringAsync();
                    JObject keys = JObject.Parse(data);
                    string token = keys.GetValue("response").ToString();
                    return token;
                }
                else
                {
                    return "Petición incorrecta: " + response.StatusCode;
                }

            }
        }


        private async Task<T> CallApiAsync<T>(string request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                HttpResponseMessage response = await
                client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }



        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                HttpResponseMessage response = await
                client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        public async Task<List<Empleado>> GetEmpleadosAsync()
        {
            string request = "api/empleados";
            return await this.CallApiAsync<List<Empleado>>(request);
        }

        public async Task<Empleado> FindEmpleadoAsync(int id)
        {
            string request = "api/empleados/" + id;
            string token =
            this.httpContextAccessor.HttpContext.User
            .FindFirst(z => z.Type == "Token").Value;

            return await this.CallApiAsync<Empleado>(request, token);
        }
    }
}
