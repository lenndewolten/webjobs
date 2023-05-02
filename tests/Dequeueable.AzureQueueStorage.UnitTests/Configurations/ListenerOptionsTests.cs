﻿using Dequeueable.AzureQueueStorage.Configurations;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;

namespace Dequeueable.AzureQueueStorage.UnitTests.Configurations
{
    public class ListenerOptionsTests
    {
        [Fact]
        public void Given_a_ListenerOptions_when_MinimumPollingIntervalInMilliseconds_is_zero_then_the_validation_result_contains_the_correct_error_message()
        {
            // Arrange
            var sut = new ListenerOptions
            {
                MinimumPollingIntervalInMilliseconds = 0
            };

            // Act
            var result = ValidateModel(sut);

            // Assert
            result.Should().Contain(e => e.ErrorMessage!.Contains("Value for MinimumPollingIntervalInMilliseconds must not be negative."));
        }

        [Fact]
        public void Given_a_ListenerOptions_when_MinimumPollingIntervalInMilliseconds_is_within_range_then_the_validation_result_are_empty()
        {
            // Arrange
            var sut = new ListenerOptions
            {
                MinimumPollingIntervalInMilliseconds = 5
            };

            // Act
            var result = ValidateModel(sut);

            // Assert
            result.Should().NotContain(e => e.MemberNames!.Contains("MinimumPollingIntervalInMilliseconds"));
        }

        [Fact]
        public void Given_a_ListenerOptions_when_MaximumPollingIntervalInMilliseconds_is_zero_then_the_validation_result_contains_the_correct_error_message()
        {
            // Arrange
            var sut = new ListenerOptions
            {
                MaximumPollingIntervalInMilliseconds = 0
            };

            // Act
            var result = ValidateModel(sut);

            // Assert
            result.Should().Contain(e => e.ErrorMessage!.Contains("Value for MaximumPollingIntervalInMilliseconds must not be negative or zero."));
        }

        [Fact]
        public void Given_a_ListenerOptions_when_MaximumPollingIntervalInMilliseconds_is_within_range_then_the_validation_result_are_empty()
        {
            // Arrange
            var sut = new ListenerOptions
            {
                MaximumPollingIntervalInMilliseconds = 5
            };

            // Act
            var result = ValidateModel(sut);

            // Assert
            result.Should().NotContain(e => e.MemberNames!.Contains("MaximumPollingIntervalInMilliseconds"));
        }

        [Fact]
        public void Given_a_ListenerOptions_when_MinimumPollingIntervalInMilliseconds_is_higher_than_MaximumPollingIntervalInMilliseconds_then_ValidatePollingInterval_returns_false()
        {
            // Arrange
            var sut = new ListenerOptions
            {
                MaximumPollingIntervalInMilliseconds = 5,
                MinimumPollingIntervalInMilliseconds = 6
            };

            // Act
            var result = ListenerOptions.ValidatePollingInterval(sut);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Given_a_ListenerOptions_when_MinimumPollingIntervalInMilliseconds_is_lower_than_MaximumPollingIntervalInMilliseconds_then_ValidatePollingInterval_returns_true()
        {
            // Arrange
            var sut = new ListenerOptions
            {
                MaximumPollingIntervalInMilliseconds = 6,
                MinimumPollingIntervalInMilliseconds = 5
            };

            // Act
            var result = ListenerOptions.ValidatePollingInterval(sut);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Given_a_ListenerOptions_when_NewBatchThreshold_is_higher_than_BatchSize_then_ValidateNewBatchThreshold_returns_false()
        {
            // Arrange
            var sut = new ListenerOptions
            {
                NewBatchThreshold = 6,
                BatchSize = 4
            };

            // Act
            var result = ListenerOptions.ValidateNewBatchThreshold(sut);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Given_a_ListenerOptions_when_NewBatchThreshold_is_lower_than_BatchSize_then_ValidateNewBatchThreshold_returns_true()
        {
            // Arrange
            var sut = new ListenerOptions
            {
                NewBatchThreshold = 5,
                BatchSize = 5
            };

            // Act
            var result = ListenerOptions.ValidateNewBatchThreshold(sut);

            // Assert
            result.Should().BeTrue();
        }

        private static IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }
    }
}
