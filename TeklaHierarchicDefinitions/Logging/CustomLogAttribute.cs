using MethodBoundaryAspect.Fody.Attributes;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MethodDecorator.Fody.Interfaces;
using System.Reflection;

namespace TeklaHierarchicDefinitions.Logging
{
    public sealed class CustomLogAttribute : Attribute, IMethodDecorator
    {
        //public override void OnEntry(MethodExecutionArgs args)
        //{
        //    //Logging.Logs.Info($"Init: {args.Method.DeclaringType.FullName}.{args.Method.Name} [{args.Arguments.Length}] params");
        //    //foreach (var item in args.Method.GetParameters())
        //    //{
        //    //    Logging.Logs.Debug($"{item.Name}: {args.Arguments[item.Position]}");
        //    //}
        //}

        //public override void OnExit(MethodExecutionArgs args)
        //{
        //    Logging.Logs.Info($"Exit: [{args.ReturnValue}]");
        //}

        //public override void OnException(MethodExecutionArgs args)
        //{
        //    Logging.Logs.Error(args.Exception.Message + "\r\n" + args.Exception.StackTrace + "\r\n" + args.Exception.TargetSite + "\r\n" + args.Exception.HelpLink); ;

        //}

        private object _instance;
        private MethodBase _method;
        private object[] _args;

        public void Init(object instance, MethodBase method, object[] args)
        {
            _instance = instance;
            _method = method;
            _args = args;
        }

        public void OnEntry()
        {
            //Logging.Logs.Info($"Entering method {_method.Name}");
        }

        public void OnException(Exception exception)
        {
            Logging.Logs.Error($"Exception in method {_method.Name}: {exception.TargetSite}, {exception.Message}, {exception.StackTrace}");
        }

        public void OnExit()
        {
            //Logging.Logs.Info($"Exiting method {_method.Name}");
        }
    }
}
