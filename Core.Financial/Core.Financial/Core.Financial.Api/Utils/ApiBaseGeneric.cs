using Common.Resource.ConstantAgileCheck;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Core.Financial.Api.Utils
{
    public class ApiBaseGeneric
    {
        string _urlBase;
        string _token;

        public ApiBaseGeneric(string urlBase, string token = "")
        {
            _urlBase = urlBase;
            _token = token;
        }

        public async Task<Response> GetAsync<T>(string parameter)
        {
            try
            {
                var client = new HttpClient();
                var response = client.GetAsync($"{_urlBase}{_token}{parameter}").Result;
                var result = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result,
                    };
                }

                var list = JsonConvert.DeserializeObject<T>(result);
                return new Response
                {
                    IsSuccess = true,
                    Result = list
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<Response> GetAsync<T>(string parameter, string Token)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", Token);
                var response = client.GetAsync($"{_urlBase}{_token}{parameter}").Result;
                var result = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result,
                    };
                }

                var list = JsonConvert.DeserializeObject<T>(result);
                return new Response
                {
                    IsSuccess = true,
                    Result = list
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<Response> PostAsync<T>(string parameter, T data, string token)
        {
            try
            {
                var client = new HttpClient();
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Add("Authorization", token);
                var response = client.PostAsync($"{_urlBase}{parameter}", content).Result;
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result,
                    };
                }

                return new Response
                {
                    IsSuccess = true,
                    Result = result
                };

            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<Response> PostTokenAsync<T>(string parameter, T data)
        {
            try
            {
                var client = new HttpClient();
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/x-www-form-urlencoded");
                var response = client.PostAsync($"{_urlBase}{parameter}", content).Result;
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result,
                    };
                }

                return new Response
                {
                    IsSuccess = true,
                    Result = result
                };

            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<Response> PostTokenAsync<T>(string parameter)
        {
            try
            {
                var client = new HttpClient();
                var content = new StringContent(parameter, Encoding.UTF8, "application/x-www-form-urlencoded");
                var response = client.PostAsync($"{_urlBase}{ConstantAgileCheck.Token}", content).Result;
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result,
                    };
                }

                return new Response
                {
                    IsSuccess = true,
                    Result = result
                };

            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        public string GetTokenAgileCheck()
        {
            string userAgileCheck = ConfigurationManager.AppSettings["UserAgilCheck"].ToString();
            string passwordAgileCheck = ConfigurationManager.AppSettings["PasswordAgilCheck"].ToString();
            var result = PostTokenAsync<Response>(string.Concat("grant_type=password&username=", userAgileCheck, "&password=", passwordAgileCheck)).Result;
            if (result.IsSuccess)
            {
                JObject jsonObject = JObject.Parse(result.Result.ToString());
                string token = $"Bearer {jsonObject.GetValue("access_token")}";
                return token;
            }
            return string.Empty;
        }
    }
}

public class Response
{
    public bool IsSuccess { get; set; }

    public string Message { get; set; }

    public object Result { get; set; }
}