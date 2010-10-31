namespace Ninject.Extensions.Interception
{
#if SILVERLIGHT
#if SILVERLIGHT_MSTEST
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
    using UnitDriven;
#endif
#else
    using Ninject.Extensions.Interception.MSTestAttributes;

#endif
    
    [TestClass]
    public class InterceptionSyntaxContextDynamicProxy2 : InterceptionSyntaxContext<DynamicProxy2Module>
    {
    }
}