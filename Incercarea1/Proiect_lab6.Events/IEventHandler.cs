﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudNative.CloudEvents;
using Proiect_lab6.Events.Models;

namespace Proiect_lab6.Events
{
    public interface IEventHandler
    {
        string[] EventTypes { get; }

        Task<EventProcessingResult> HandleAsync(CloudEvent cloudEvent);
    }
}
