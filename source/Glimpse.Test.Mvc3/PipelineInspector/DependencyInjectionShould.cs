﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Glimpse.Core.Extensibility;
using Glimpse.Mvc.PipelineInspector;
using Glimpse.Test.Common;
using Moq;
using Xunit;
using Xunit.Extensions;

namespace Glimpse.Test.Mvc3.PipelineInspector
{
    public class DependencyInjectionShould : IDisposable
    {
        private readonly IDependencyResolver originalResolver;

        public DependencyInjectionShould()
        {
            originalResolver = DependencyResolver.Current;
        }

        [Theory, AutoMock]
        public void ProxyDependencyResolver(DependencyInjectionInspector sut, IPipelineInspectorContext context, IDependencyResolver dependencyResolver)
        {
            DependencyResolver.SetResolver(dependencyResolver);

            context.ProxyFactory.Setup(f => f.IsWrapInterfaceEligible(It.IsAny<Type>())).Returns(true);
            context.ProxyFactory.Setup(f => f.WrapInterface(It.IsAny<IDependencyResolver>(), It.IsAny<IEnumerable<IAlternateImplementation<IDependencyResolver>>>(), Enumerable.Empty<object>())).Returns(dependencyResolver);

            sut.Setup(context);

            Assert.Equal(dependencyResolver, DependencyResolver.Current);
            context.Logger.Verify(l => l.Debug(It.Is<string>(s => s.Contains("IDependencyResolver")), It.IsAny<object[]>()));
        }

        [Theory, AutoMock]
        public void ContinueIfUnableToProxyDependencyResolver(DependencyInjectionInspector sut, IPipelineInspectorContext context, IDependencyResolver dependencyResolver)
        {
            DependencyResolver.SetResolver(dependencyResolver);

            context.ProxyFactory.Setup(f => f.IsWrapInterfaceEligible(It.IsAny<Type>())).Returns(false);

            sut.Setup(context);

            Assert.Equal(dependencyResolver, DependencyResolver.Current);
        }

        public void Dispose()
        {
            DependencyResolver.SetResolver(originalResolver);
        }
    }
}