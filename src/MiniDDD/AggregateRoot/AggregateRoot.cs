﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniDDD
{
    public abstract class AggregateRoot<TKey> :
        InlineEventHandler<TKey>,
        IAggregateRoot<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly Queue<IDomainEvent<TKey>> _uncommittedEvents = new Queue<IDomainEvent<TKey>>();
        public IEnumerable<IDomainEvent<TKey>> UncommittedEvents => _uncommittedEvents;

        public virtual TKey Id { get; set; }

        public virtual void Purge()
        {
            if (_uncommittedEvents.Any())
            {
                _uncommittedEvents.Clear();
            }
        }

        public virtual void Replay(IEnumerable<IDomainEvent<TKey>> events)
        {
            //((IPurgeable)this).Purge();
            this.Purge();
            foreach (var e in events)
            {
                ApplyEvent(e);
            }
        }

        protected override void ApplyEvent<TEvent>(TEvent e)
        {
            //var eventHandlerMethods = from m in this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            //                          let parameters = m.GetParameters()
            //                          where m.IsDefined(typeof(InlineEventHandlerAttribute)) &&
            //                          m.ReturnType == typeof(void) &&
            //                          parameters.Length == 1 &&
            //                          parameters[0].ParameterType == e.GetType()
            //                          select m;
            ////e.AggregateRootType = this.GetType().FullName;
            //foreach (var handler in eventHandlerMethods)
            //{
            //    handler.Invoke(this, new object[] { e });
            //}
            base.ApplyEvent(e);
            _uncommittedEvents.Enqueue(e);
        }
    }
}
