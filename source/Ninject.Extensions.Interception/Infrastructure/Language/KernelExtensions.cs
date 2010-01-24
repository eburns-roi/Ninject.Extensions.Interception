#region License

// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2010, Enkari, Ltd.
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 

#endregion

#region Using Directives

using System;
#if !NETCF
using System.Linq.Expressions;
#endif // !NETCF
using System.Reflection;
using Ninject.Extensions.Interception.Advice;
using Ninject.Extensions.Interception.Advice.Builders;
using Ninject.Extensions.Interception.Advice.Syntax;
using Ninject.Extensions.Interception.Registry;
using Ninject.Extensions.Interception.Request;

#endregion

namespace Ninject.Extensions.Interception.Infrastructure.Language
{
    public static class KernelExtensions
    {
        public static IAdviceTargetSyntax Intercept( this IKernel kernel, Predicate<IProxyRequest> predicate )
        {
            return DoIntercept( kernel, predicate );
        }

        /// <summary>
        /// Begins a dynamic interception definition.
        /// </summary>
        /// <param name="condition">The condition to evaluate to determine if a request should be intercepted.</param>
        /// <returns>An advice builder.</returns>
        private static IAdviceTargetSyntax DoIntercept( IKernel kernel, Predicate<IProxyRequest> condition )
        {
            IAdvice advice = kernel.Components.Get<IAdviceFactory>().Create( condition );
            kernel.Components.Get<IAdviceRegistry>().Register( advice );

            return CreateAdviceBuilder( advice );
        }

        /// <summary>
        /// Creates an advice builder.
        /// </summary>
        /// <param name="advice">The advice that will be built.</param>
        /// <returns>The created builder.</returns>
        private static IAdviceTargetSyntax CreateAdviceBuilder( IAdvice advice )
        {
            return new AdviceBuilder( advice );
        }

        public static void AddMethodInterceptor(this IKernel kernel,
                                         MethodInfo method,
                                         Action<IInvocation> action)
        {
            var interceptor = new ActionInterceptor(action);
            kernel.Components.Get<IMethodInterceptorRegistry>().Add(method, interceptor);
        }

        #if !NETCF
        public static void InterceptReplace<T>( this IKernel kernel,
                                                Expression<Action<T>> methodExpr,
                                                Action<IInvocation> action )
        {
            kernel.AddMethodInterceptor( GetMethodFromExpression( methodExpr ), action );
        }

        public static void InterceptAround<T>( this IKernel kernel,
                                               Expression<Action<T>> methodExpr,
                                               Action<IInvocation> beforeAction,
                                               Action<IInvocation> afterAction )
        {
            kernel.InterceptReplace( methodExpr,
                                     i =>
                                     {
                                         beforeAction( i );
                                         i.Proceed();
                                         afterAction( i );
                                     } );
        }

        public static void InterceptBefore<T>( this IKernel kernel,
                                               Expression<Action<T>> methodExpr,
                                               Action<IInvocation> action )
        {
            kernel.InterceptAround( methodExpr, action, i => { } );
        }

        public static void InterceptAfter<T>( this IKernel kernel,
                                              Expression<Action<T>> methodExpr,
                                              Action<IInvocation> action )
        {
            kernel.InterceptAround( methodExpr, i => { }, action );
        }

        public static void InterceptReplaceGet<T>( this IKernel kernel,
                                                   Expression<Func<T, object>> propertyExpr,
                                                   Action<IInvocation> action )
        {
            kernel.AddMethodInterceptor( GetGetterFromExpression( propertyExpr ), action );
        }

        public static void InterceptAroundGet<T>( this IKernel kernel,
                                                  Expression<Func<T, object>> propertyExpr,
                                                  Action<IInvocation> beforeAction,
                                                  Action<IInvocation> afterAction )
        {
            kernel.InterceptReplaceGet( propertyExpr,
                                        i =>
                                        {
                                            beforeAction( i );
                                            i.Proceed();
                                            afterAction( i );
                                        } );
        }

        public static void InterceptBeforeGet<T>( this IKernel kernel,
                                                  Expression<Func<T, object>> propertyExpr,
                                                  Action<IInvocation> action )
        {
            kernel.InterceptAroundGet( propertyExpr, action, i => { } );
        }

        public static void InterceptAfterGet<T>( this IKernel kernel,
                                                 Expression<Func<T, object>> propertyExpr,
                                                 Action<IInvocation> action )
        {
            kernel.InterceptAroundGet( propertyExpr, i => { }, action );
        }

        public static void InterceptReplaceSet<T>( this IKernel kernel,
                                                   Expression<Func<T, object>> propertyExpr,
                                                   Action<IInvocation> action )
        {
            kernel.AddMethodInterceptor( GetSetterFromExpression( propertyExpr ), action );
        }

        public static void InterceptAroundSet<T>( this IKernel kernel,
                                                  Expression<Func<T, object>> propertyExpr,
                                                  Action<IInvocation> beforeAction,
                                                  Action<IInvocation> afterAction )
        {
            kernel.InterceptReplaceSet( propertyExpr,
                                        i =>
                                        {
                                            beforeAction( i );
                                            i.Proceed();
                                            afterAction( i );
                                        } );
        }

        public static void InterceptBeforeSet<T>( this IKernel kernel,
                                                  Expression<Func<T, object>> propertyExpr,
                                                  Action<IInvocation> action )
        {
            kernel.InterceptAroundSet( propertyExpr, action, i => { } );
        }

        public static void InterceptAfterSet<T>( this IKernel kernel,
                                                 Expression<Func<T, object>> propertyExpr,
                                                 Action<IInvocation> action )
        {
            kernel.InterceptAroundSet( propertyExpr, i => { }, action );
        }

        private static MethodInfo GetMethodFromExpression<T>( Expression<Action<T>> methodExpr )
        {
            var call = methodExpr.Body as MethodCallExpression;
            if ( call == null )
            {
                throw new InvalidOperationException( "Expression must be a method call" );
            }
            if ( call.Object !=
                 methodExpr.Parameters[0] )
            {
                throw new InvalidOperationException( "Method call must target lambda argument" );
            }
            return call.Method;
        }

        private static MethodInfo GetGetterFromExpression<T>( Expression<Func<T, object>> propertyExpr )
        {
            PropertyInfo propertyInfo = GetPropertyFromExpression( propertyExpr );
            if ( !propertyInfo.CanRead )
            {
                throw new InvalidOperationException( "Property must be readable" );
            }
            return propertyInfo.GetGetMethod();
        }

        private static MethodInfo GetSetterFromExpression<T>( Expression<Func<T, object>> propertyExpr )
        {
            PropertyInfo propertyInfo = GetPropertyFromExpression( propertyExpr );
            if ( !propertyInfo.CanWrite )
            {
                throw new InvalidOperationException( "Property must be writable" );
            }
            return propertyInfo.GetSetMethod();
        }

        private static PropertyInfo GetPropertyFromExpression<T>( Expression<Func<T, object>> propertyExpr )
        {
            Expression body = propertyExpr.Body;
            if ( body is UnaryExpression &&
                 body.NodeType == ExpressionType.Convert )
            {
                body = ( (UnaryExpression) body ).Operand;
            }
            var memberExpr = body as MemberExpression;
            if ( memberExpr == null ||
                 !( memberExpr.Member is PropertyInfo ) )
            {
                throw new InvalidOperationException( "Expression must be a property access" );
            }
            if ( memberExpr.Expression !=
                 propertyExpr.Parameters[0] )
            {
                throw new InvalidOperationException( "Method call must target lambda argument" );
            }
            return (PropertyInfo) memberExpr.Member;
        }
        #endif
    }
}