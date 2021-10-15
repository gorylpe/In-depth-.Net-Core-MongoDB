using System;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver.Core.Events;

namespace Mongo
{
	public class EventSubscriber : IEventSubscriber
	{
		private readonly ILogger<EventSubscriber> _logger;

		public EventSubscriber(ILogger<EventSubscriber> logger)
		{
			_logger = logger;
		}

		public bool TryGetEventHandler<TEvent>(out Action<TEvent> handler)
		{
			handler = null;
			if (typeof(TEvent) == typeof(CommandStartedEvent))
			{
				handler = e => Handle((CommandStartedEvent) (object) e);
				return true;
			}

			return false;
		}

		private void Handle(CommandStartedEvent e)
		{
			_logger.LogDebug("Executing command {CommandName} \n {Command}", e.CommandName, e.Command.ToJson());
		}
	}
}