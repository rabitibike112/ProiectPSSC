using Example.Dto.Events;
using Example.Events;
using Example.Events.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Exemple.Domain.Models.ProductsPublishedEvent;

namespace Example.Accomodation.EventProcessor
{
    internal class ProductPublishedEventHandler : AbstractEventHandler<ProductPublishedEvent>
    {
        public override string[] EventTypes => new string[]{typeof(ProductPublishedEvent).Name};

        protected override Task<EventProcessingResult> OnHandleAsync(ProductPublishedEvent eventData)
        {
            Console.WriteLine(eventData.ToString());
            return Task.FromResult(EventProcessingResult.Completed);
        }
    }
}
