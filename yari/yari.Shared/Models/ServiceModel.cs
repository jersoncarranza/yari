//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Web.Http;


namespace yari.Models
{
    public class ServiceModel
    {
        public event EventHandler<UsuarioEventArgs> GetUsuariosCompleted;

        public event EventHandler<UsuarioLoginEventArgs> GetLoginCompleted;


        internal async Task<string> GetLogin(ObservableCollection<Usuario> MyUsersList)
        {
            var httpClient = new HttpClient();
            var request = await httpClient.GetAsync(new Uri("http://yari.azurewebsites.net/api/usuario/login/?name=" + MyUsersList[0].NombreUsuario + "&pass=" + MyUsersList[0].ClaveUsuario, UriKind.RelativeOrAbsolute));
            var txtJson = await request.Content.ReadAsStringAsync();

            //JsonValue jsonList = JsonValue.Parse(txtJson);
            //var valor1 = jsonList.GetArray().GetStringAt(0);

            //if (GetLoginCompleted != null)
                //GetLoginCompleted(this, new UsuarioLoginEventArgs(Convert.ToBoolean(txtJson)));
            return txtJson;
        }

        internal async void GetUsers()// Task<HttpResponseMessage> GetUsers()
        {
            var httpClient = new HttpClient();
            var request = await httpClient.GetAsync(new Uri("http://yari.azurewebsites.net/api/usuario/all", UriKind.RelativeOrAbsolute));
            var txtJson = await request.Content.ReadAsStringAsync();


            //JsonValue jsonList = JsonValue.Parse(txtJson);
            //JsonObject jsonObject = JsonObject.Parse(txtJson);
            //dynamic data = JObject.Parse(results.ToString());

            //var results = jsonList.GetArray();
            //Product myProduct = JsonConvert.DeserializeObject<Product>(myJsonString);

            //Usuario myProduct = JsonConvert.DeserializeObject<Usuario>(request.Content.ToString());

            List<Usuario> listUser = new List<Usuario>();
            //foreach (var item in jsonList.GetArray())
            //{
            //    Usuario obju = new Usuario();
            //    obju.IdUsuario = int.Parse(item.GetArray().GetStringAt(0));
            //    obju.IdTipoUsuario = int.Parse(item.GetArray().GetStringAt(1));
            //    obju.NombreUsuario = item.GetArray().GetStringAt(2);
            //    obju.ClaveUsuario = item.GetArray().GetStringAt(3);
            //    obju.UsuarioCreador = int.Parse(item.GetArray().GetStringAt(4));

            //    listUser.Add(obju);
            //}

            if (GetUsuariosCompleted != null)
                GetUsuariosCompleted(this, new UsuarioEventArgs(listUser));

        }


    }
}
