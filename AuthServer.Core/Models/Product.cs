using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Models
{
    public class Product
    {

        // aşağıda [key] yazar isek bu birinci yoldur.
        // ancak burasının şişirilmememsi gerekmektedir.


        // Diğer yol ise Id ya da ProductId yazar isek EntityFreamework bunu id olarak algılar ve primaryKey yapar
        // ancak Product_Id yapar isek entityfreamework bunu anlamaz
        public int Id { get; set; }
        public string Name { get; set; }
        public Decimal Price { get; set; }
        public int Stock { get; set; }
        public int UserId { get; set; }
    }
}
