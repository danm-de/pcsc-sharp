using System;
using System.Diagnostics;
using NUnit.Framework;

namespace PCSC.Tests
{
    /// <summary>
    /// Abstract helper class to make nunit tests more readable.
    /// </summary>
    [DebuggerStepThrough, DebuggerNonUserCode]
    public class Spec
    {
        [DebuggerStepThrough]
        [OneTimeSetUp]
        public void SetUp() {
            EstablishContext();
            BecauseOf();
        }

        [DebuggerStepThrough]
        [OneTimeTearDown]
        public void TearDown() {
            Cleanup();
        }

        /// <summary>
        /// Test setup. Place your initialization code here.
        /// </summary>
        [DebuggerStepThrough]
        protected virtual void EstablishContext() { }

        /// <summary>
        /// Test run. Place the tested method / action here.
        /// </summary>
        [DebuggerStepThrough]
        protected virtual void BecauseOf() { }

        /// <summary>
        /// Test clean. Close/delete files, close database connections ..
        /// </summary>
        [DebuggerStepThrough]
        protected virtual void Cleanup() { }

        /// <summary>
        /// Creates an Action delegate.
        /// </summary>
        /// <param name="func">Method the shall be created as delegate.</param>
        /// <returns>A delegate of type <see cref="Action"/></returns>
        protected Action Invoking(Action func) {
            return func;
        }
    }
}