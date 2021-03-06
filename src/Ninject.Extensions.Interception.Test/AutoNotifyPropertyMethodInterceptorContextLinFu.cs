#if !NETCOREAPP2_0
namespace Ninject.Extensions.Interception
{
    using FluentAssertions;

    using LinFu.DynamicProxy;
    using Xunit;
    
    public class AutoNotifyPropertyMethodInterceptorContextLinFu : AutoNotifyPropertyMethodInterceptorContext
    {
        protected override InterceptionModule InterceptionModule
        {
            get
            {
                return new LinFuModule();
            }
        }

        [Fact]
        public void WhenAutoNotifyAttributeIsAttachedToAProperty_TheObjectIsProxied()
        {
            typeof(IProxy).IsAssignableFrom(this.ViewModel.GetType()).Should().BeTrue();
        }
    }
} 
#endif
