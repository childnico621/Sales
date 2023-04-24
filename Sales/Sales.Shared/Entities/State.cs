using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sales.Shared.Entities
{
    public class State
    {
        public int Id { get; set; }

        [Display(Name = "Departamento/Estado")]
        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Name { get; set; } = null!; 

        public Country? Country { get; set; }
        public int CountryId { get; set; }

        [Display(Name = "Ciudades")]
        public ICollection<City>? Cities { get; set; } = null!;
        public int CitiesCount => Cities == null ? 0 : Cities.Count;
    }
}
