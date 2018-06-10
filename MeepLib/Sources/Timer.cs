﻿using System;
using System.Xml.Linq;
using System.Reactive.Linq;

using SmartFormat;

using MeepLib.MeepLang;
using MeepModel.Messages;

namespace MeepLib.Sources
{
    [Macro(Name = "Interval", DefaultProperty = "Interval", Position = MacroPosition.FirstChild)]
    public class Timer : AMessageModule
    {
        public Timer()
        {
            Pipeline = from seq in Observable.Interval(Interval)
                       let message = IssueMessage(seq)
                       where message != null
                       select message;
        }

        /// <summary>
        /// Length of timer interval
        /// </summary>
        /// <value>The interval.</value>
        public TimeSpan Interval { get; set; }

        /// <summary>
        /// Last time the timer elapsed
        /// </summary>
        /// <value>The last elapsed.</value>
        public long LastElapsed { get; private set; }

        /// <summary>
        /// Value in {Smart.Format} to put in each message
        /// </summary>
        /// <value>The payload.</value>
        public string Payload { get; set; }

        public virtual Message CreateMessage(long step)
        {
            var msg = new Step
            {
                Number = step,
                LastIssued = new DateTime(LastElapsed)
            };

            msg.Value = Smart.Format(Payload, msg);

            return msg;
        }

        private Message IssueMessage(long step)
        {
            var msg = CreateMessage(step);
            LastElapsed = msg.CreatedTicks;

            return msg;
        }
    }
}