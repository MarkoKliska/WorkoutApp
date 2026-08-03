using WorkoutApp.Domain.Common;

namespace WorkoutApp.Domain.Tests.Common;

public class ValueObjectTests
{
    private sealed class TestValueObject : ValueObject
    {
        private readonly string _value;
        private readonly int _number;

        public TestValueObject(string value, int number)
        {
            _value = value;
            _number = number;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return _value;
            yield return _number;
        }
    }

    private sealed class OtherValueObject : ValueObject
    {
        private readonly string _value;

        public OtherValueObject(string value)
        {
            _value = value;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return _value;
        }
    }

    [Fact]
    public void Equals_SameComponents_ReturnsTrue()
    {
        var first = new TestValueObject("abc", 1);
        var second = new TestValueObject("abc", 1);

        Assert.True(first.Equals(second));
        Assert.True(first == second);
    }

    [Fact]
    public void Equals_DifferentComponents_ReturnsFalse()
    {
        var first = new TestValueObject("abc", 1);
        var second = new TestValueObject("abc", 2);

        Assert.False(first.Equals(second));
        Assert.True(first != second);
    }

    [Fact]
    public void Equals_DifferentType_ReturnsFalse()
    {
        var first = new TestValueObject("abc", 1);
        var other = new OtherValueObject("abc");

        Assert.False(first.Equals(other));
    }

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        var value = new TestValueObject("abc", 1);

        Assert.False(value.Equals(null));
        Assert.True(value != null);
    }

    [Fact]
    public void GetHashCode_SameComponents_AreEqual()
    {
        var first = new TestValueObject("abc", 1);
        var second = new TestValueObject("abc", 1);

        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }
}