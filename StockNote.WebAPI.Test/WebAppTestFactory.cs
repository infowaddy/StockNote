using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace StockNote.WebAPI.Test
{
    public class WebAppTestFactory : WebApplicationFactory<Program>
    {
        private string environment;
        public WebAppTestFactory(string _environment = "Testing") 
        {
            this.environment = _environment;
        }
        
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
        }

        protected new void Dispose()
        {

        }
    }

}
