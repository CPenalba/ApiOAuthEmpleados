using ApiOAuthEmpleados.Models;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ApiOAuthEmpleados.Helpers
{
    public class HelperEmpleadoToken
    {
        private HttpContextAccessor contextAccessor;

        public HelperEmpleadoToken(HttpContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor;
        }

        public EmpleadoModel GetEmpleado()
        {
            Claim claim = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "UserData");
            string json = claim.Value;
            string jsonEmpleado = HelperCryptography.DecryptString(json);
            EmpleadoModel model = JsonConvert.DeserializeObject<EmpleadoModel>(json);
            return model;
        }
    }
}
