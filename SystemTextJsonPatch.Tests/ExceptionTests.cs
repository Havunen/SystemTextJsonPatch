using SystemTextJsonPatch.Exceptions;
using SystemTextJsonPatch.Tests.TestObjectModels;
using Xunit;

namespace SystemTextJsonPatch.Tests
{
	public class ExceptionTests
	{
		[Fact]
		public void OperationShouldBeAvailableInTheException()
		{
			var targetObject = new MyCustomDateOnly(2000, 10, 1);

			var patchDocument = new JsonPatchDocument();
			patchDocument.Replace("NewInt", 1);

			// Act
			var exc = Assert.Throws<JsonPatchException>(() => patchDocument.ApplyTo(targetObject));

			Assert.Equal("The target location specified by path segment 'NewInt' was not found.", exc.Message);
			Assert.NotNull(exc.FailedOperation);
			Assert.Equal("replace", exc.FailedOperation.Op);
			Assert.Equal("/NewInt", exc.FailedOperation.Path);
		}
	}
}
