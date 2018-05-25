using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FakerTest.Tests.Infrastructure
{
    public class ExtendedExpectedExceptionAttribute : ExpectedExceptionBaseAttribute
    {
        private string _parameterName;

        public string ParameterName
        {
            get => _parameterName;
            set
            {
                if (ExceptionType != typeof(ArgumentException) && !ExceptionType.IsSubclassOf(typeof(ArgumentException)))
                {
                    throw new ArgumentException($"Parameter name is allowed only for {nameof(ArgumentException)} and its child classes.");
                }
                _parameterName = value;
            }
        }

        public Type ExceptionType { get; }
        public string ExceptionMessage { get; set; }

        public ExtendedExpectedExceptionAttribute(Type exceptionType)
        {
            ExceptionType = exceptionType ?? throw new ArgumentNullException(nameof(exceptionType));
        }

        protected override void Verify(Exception exception)
        {
            if (exception.GetType() != ExceptionType)
            {
                throw new AssertFailedException($"This exception type was not expected. Expected: '{ExceptionType}'. Actual: '{exception.GetType()}'.", exception);
            }

            if (!string.IsNullOrEmpty(ExceptionMessage) && exception.Message != ExceptionMessage)
            {
                throw new AssertFailedException("This exception message was not expected. " + $"Expected: '{ExceptionMessage}'. Actual: '{exception.Message}'.", exception);
            }

            if (exception is ArgumentException argumentException && !string.IsNullOrEmpty(_parameterName) && _parameterName != argumentException.ParamName)
            {
                throw new AssertFailedException("This exception parameter name was not expected. " + $"Expected: '{ParameterName}'. Actual: '{argumentException.ParamName}'.", exception);
            }
        }
    }

}
