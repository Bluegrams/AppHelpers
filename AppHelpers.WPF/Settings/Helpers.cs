using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Bluegrams.Application
{
    static class Helpers
    {
        public static Delegate CreateHandler(EventInfo eventInfo, Expression<Action> actionExpr)
        {
            var parameters = eventInfo.EventHandlerType
                .GetMethod("Invoke").GetParameters()
                .Select(p => Expression.Parameter(p.ParameterType));
            var invokeExpr = Expression.Invoke(actionExpr);
            return Expression.Lambda(eventInfo.EventHandlerType, invokeExpr, parameters).Compile();
        }
    }
}
