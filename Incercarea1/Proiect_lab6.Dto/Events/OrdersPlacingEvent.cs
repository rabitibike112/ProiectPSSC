using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proiect_lab6.Dto.Models;

namespace Proiect_lab6.Dto.Events
{
    public class OrdersPlacingEvent
    {
        public List<CartOrderDto> Orders { get; init; }
    }
}
