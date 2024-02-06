using System;

namespace BANCO.Models
{
    public class BancoBase
    {
        public string Id { get; set; }
        public virtual string Nombre { get; set; }

        public BancoBase()
        {
            Id = Guid.NewGuid().ToString();
        }
        public override string ToString()
        {
            return $"{Nombre}, {Id}";
        }
    }

}
