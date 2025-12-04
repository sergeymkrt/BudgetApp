using BudgetApp.Application.Common.Models;

namespace BudgetApp.Tests.Common;

public class ResultTests
{
    [Fact]
    public void Success_ShouldReturnSucceededTrue()
    {
        var result = Result.Success();

        result.Succeeded.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void Failure_WithSingleError_ShouldReturnSucceededFalse()
    {
        var result = Result.Failure("Something went wrong");

        result.Succeeded.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem();
        result.Errors[0].ShouldBe("Something went wrong");
    }

    [Fact]
    public void Failure_WithMultipleErrors_ShouldReturnAllErrors()
    {
        var errors = new[] { "Error 1", "Error 2", "Error 3" };
        var result = Result.Failure(errors);

        result.Succeeded.ShouldBeFalse();
        result.Errors.Length.ShouldBe(3);
        result.Errors.ShouldBe(errors);
    }

    [Fact]
    public void Match_OnSuccess_ShouldCallOnSuccessFunc()
    {
        var result = Result.Success();

        var output = result.Match(
            () => "success",
            errors => "failure");

        output.ShouldBe("success");
    }

    [Fact]
    public void Match_OnFailure_ShouldCallOnFailureFunc()
    {
        var result = Result.Failure("error");

        var output = result.Match(
            () => "success",
            errors => $"failure: {errors[0]}");

        output.ShouldBe("failure: error");
    }
}

public class ResultOfTTests
{
    [Fact]
    public void Success_ShouldContainData()
    {
        var result = Result<string>.Success("test data");

        result.Succeeded.ShouldBeTrue();
        result.Data.ShouldBe("test data");
        result.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void Failure_ShouldHaveNullData()
    {
        var result = Result<string>.Failure("error");

        result.Succeeded.ShouldBeFalse();
        result.Data.ShouldBeNull();
        result.Errors.ShouldHaveSingleItem();
    }

    [Fact]
    public void Match_OnSuccess_ShouldProvideData()
    {
        var result = Result<int>.Success(42);

        var output = result.Match(
            data => $"value: {data}",
            errors => "failed");

        output.ShouldBe("value: 42");
    }

    [Fact]
    public void Match_OnFailure_ShouldProvideErrors()
    {
        var result = Result<int>.Failure("not found");

        var output = result.Match(
            data => $"value: {data}",
            errors => $"error: {errors[0]}");

        output.ShouldBe("error: not found");
    }
}
