﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiTenancyFramework.Commands
{
    public sealed class CommandProcessor : ICommandProcessor
    {
        private IServiceProvider _serviceProvider;
        public CommandProcessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string InstitutionCode { get; set; }

        [DebuggerStepThrough]
        public void Process(ICommand command)
        {
            var handlerType =
                typeof(ICommandHandler<>).MakeGenericType(command.GetType());

            dynamic handler = _serviceProvider.GetService(handlerType);
            handler.InstitutionCode = InstitutionCode;

            handler.Handle((dynamic)command);
        }
    }
}