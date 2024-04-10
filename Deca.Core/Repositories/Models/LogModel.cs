using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desa.Core.Repositories.Models
{
    public class LogModel<T>
    {
        public string ModelCollection { get; set; }
        public string Action { get; set; }
        public DateTime ActionDate { get; set; }
        public T Value { get; set; }

        //talvez adicionar o user que fez a alteração
    }
}
