﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Text.RegularExpressions;

using MeepLib.Config;
using MeepLib.Messages;

namespace MeepLib
{
    /// <summary>
    /// Context of message and its environment for easy addressing in {Smart.Format} templates
    /// </summary>
    /// <remarks>EG: "{mdl.Name}: {msg.Value}" is a valid SmartFormat template.</remarks>
    public class MessageContext
    {
        public MessageContext(Message message, AMessageModule module)
        {
            msg = message;
            mdl = module;

            _cfgMutex.WaitOne();
            if (cfg == null)
                cfg = ANamable.InventoryByBase<ANamable>().ToDictionary(x => x.Name);
            _cfgMutex.ReleaseMutex();
        }

        public MessageContext(Message message, AMessageModule module, Match match)
            :this(message, module)
        {
            rgx = match;
        }

        /// <summary>
        /// The message being processed
        /// </summary>
        /// <value>The message.</value>
        public Message msg { get; set; }

        /// <summary>
        /// Module currently processing the message
        /// </summary>
        /// <value>The mdl.</value>
        public AMessageModule mdl { get; set; }

        /// <summary>
        /// Regex match
        /// </summary>
        /// <value>The Regex match.</value>
        /// <remarks>For modules that use a regex on all or part of a message,
        /// the result of the Match can be made available here.</remarks>
        public Match rgx { get; set; }

        /// <summary>
        /// Lookup on named modules
        /// </summary>
        /// <value></value>
        /// <remarks>If a config element (AppKey, Header, Login, etc.) has been
        /// given a Name then it will be accessible here, no matter where in the
        /// pipeline its defined.
        /// 
        /// <para>E.G.: You can define the secret key, username and password of
        /// a web service in a file that you XInclude from the main pipeline, 
        /// then fetch its values from anywhere else, thus keeping them out of
        /// your repository.</para></remarks>
        public static Dictionary<string,ANamable> cfg { get; set; }
        private static Mutex _cfgMutex = new Mutex(false);

        public static void InvalidateCache()
        {
            cfg = null;
        }

        /// <summary>
        /// Return the ToString value of the message
        /// </summary>
        /// <value>The contents.</value>
        /// <remarks>For simple Smart.Format templates: {Contents} is better
        /// than {msg.ToString()}</remarks>
        public string Contents
        {
            get
            {
                return msg.ToString();
            }
        }
    }
}
