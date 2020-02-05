using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using System.Text;
using System.Linq;
using RestAPI;

namespace RestAPI
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            string readToken = System.IO.File.ReadLines("token.txt").Last();
            var token = readToken;
            Token Token = new Token();
            var rootApp = new CommandLineApplication()
            {
                Name = "Aplikasi List",
                Description = "Digunakan untuk membuat list yang harus dilakukan",
                ShortVersionGetter = () => "1.0.0",

            };

            rootApp.Command("user", app =>
             {
                 app.Description = "Login or Register User";
                 var login = app.Option("--login", "Login User", CommandOptionType.MultipleValue);
                 var register = app.Option("--register", "Register User", CommandOptionType.MultipleValue);

                 app.OnExecuteAsync(async cancellationToken =>
                 {
                     HttpClientHandler clientHandler = new HttpClientHandler();
                     clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                     {
                         return true;
                     };
                     HttpClient client = new HttpClient(clientHandler);
                     HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:5001/user");

                     if (login.HasValue())
                     {
                         var user = new User()
                         {
                             username = login.Values[0],
                             password = login.Values[1]
                         };

                         var json = JsonConvert.SerializeObject(user);
                         var content = new StringContent(json, Encoding.UTF8, "application/json");
                         var postAsync = await client.PostAsync("https://localhost:5001/user/login", content);
                         var response = await postAsync.Content.ReadAsStringAsync();
                         Token = JsonConvert.DeserializeObject<Token>(response);
                         Token.SaveToken();

                     }
                     else if (register.HasValue())
                     {
                         var user = new User()
                         {
                             username = register.Values[0],
                             password = register.Values[1]
                         };

                         var json = JsonConvert.SerializeObject(user);
                         var content = new StringContent(json, Encoding.UTF8, "application/json");
                         await client.PostAsync("https://localhost:5001/user/register", content);
                     }
                 });
             });

            rootApp.Command("todo", app =>
             {
                 app.Description = "List Kegiatan";
                 var list = app.Option("--list", "show list", CommandOptionType.NoValue);
                 var clear = app.Option("--clear", "clear list", CommandOptionType.NoValue);
                 var add = app.Option("--add", "add list", CommandOptionType.SingleOrNoValue);
                 var update = app.Option("--update", "update list", CommandOptionType.MultipleValue);
                 var delete = app.Option("--delete", "delete list", CommandOptionType.SingleOrNoValue);
                 var done = app.Option("--done", "done list", CommandOptionType.SingleOrNoValue);

                 app.OnExecuteAsync(async cancellationToken =>
                 {
                     HttpClientHandler clientHandler = new HttpClientHandler();
                     clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                     {
                         return true;
                     };
                     HttpClient client = new HttpClient(clientHandler);
                     HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:5001/todo");


                     if (list.HasValue())
                     {
                         if (token != "")
                         {
                             client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                             HttpResponseMessage response = await client.SendAsync(request);
                             var json = await response.Content.ReadAsStringAsync();

                             var obj = JsonConvert.DeserializeObject<List<todo>>(json);

                             Console.WriteLine("List Kegiatan Yang Harus Dilakukan : ");
                             foreach (var list in obj)
                             {
                                 if (list.status == true)
                                 {
                                     Console.WriteLine(list.id + ". " + list.list + " = Done");
                                 }
                                 else
                                 {
                                     Console.WriteLine(list.id + ". " + list.list + " = Undone");
                                 }
                             }
                         }
                     }

                     if (clear.HasValue())
                     {
                         client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",token);
                         HttpResponseMessage response = await client.SendAsync(request);
                         var json = await response.Content.ReadAsStringAsync();

                         var sure = Prompt.GetYesNo("Sure ?", false);
                         if (sure)
                         {
                                 await client.DeleteAsync("https://localhost:5001/todo/clear");
                         }
                     }

                     if (add.HasValue())
                     {
                         client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",token);
                         var todo = new todo()
                         {
                             list = add.Value()
                         };
                         var json = JsonConvert.SerializeObject(todo);
                         var content = new StringContent(json, Encoding.UTF8, "application/json");
                         await client.PostAsync("https://localhost:5001/todo", content);

                     }

                     if (update.HasValue())
                     {
                         client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",token);
                         var todo = "{"
                                     + $"\"list\":\"{update.Values[1]}\""
                                     + "}";
                         var id = Convert.ToInt32(update.Values[0]);
                         var content = new StringContent(todo, Encoding.UTF8, "application/json");
                         await client.PatchAsync("https://localhost:5001/todo/" + id, content);
                     }

                     if (delete.HasValue())
                     {
                         client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",token);
                         var id = delete.Value();
                         await client.DeleteAsync("https://localhost:5001/todo/delete/" + Convert.ToInt32(id));
                     }

                     if (done.HasValue())
                     {
                         client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",token);
                         var todo = "{"
                                  + "\"status\":"
                                  + "true"
                                  + "}";
                         var content = new StringContent(todo, Encoding.UTF8, "application/json");
                         var id = Convert.ToInt32(done.Value());
                         await client.PatchAsync("https://localhost:5001/todo/done/" + id, content);
                     }
                 });
             });

            rootApp.OnExecute(() =>
            {
                rootApp.ShowHelp();
            });
            return rootApp.Execute(args);
        }
    }
    class todo
    {
        [JsonProperty("id")]
        public int id { get; set; }
        [JsonProperty("list")]
        public string list { get; set; }
        [JsonProperty("status")]
        public bool status { get; set; } = false;
        public int user_id { get; set; }
    }
    class User
    {
        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("username")]
        public string username { get; set; }
        [JsonProperty("password")]
        public string password { get; set; }

        public ICollection<todo> todos { get; set; }



    }
}
