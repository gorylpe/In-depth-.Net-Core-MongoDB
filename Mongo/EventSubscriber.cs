using System;
using System.Reflection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver.Core.Events;

namespace Mongo
{
	public class EventSubscriber : IEventSubscriber
	{
		private readonly ILogger<EventSubscriber>  _logger;
		private readonly ReflectionEventSubscriber _subscriber;

		public EventSubscriber(ILogger<EventSubscriber> logger)
		{
			_logger = logger;
			_subscriber = new ReflectionEventSubscriber(this, bindingFlags: BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
		}

		public bool TryGetEventHandler<TEvent>(out Action<TEvent> handler)
		{
			return _subscriber.TryGetEventHandler(out handler);
		}

		private void Handle(CommandStartedEvent e)
		{
			_logger.LogDebug("Executing command {CommandName} \n {Command}", e.CommandName, e.Command.ToJson());
		}
	}
}