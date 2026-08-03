using WorkoutApp.Domain.Common;

namespace WorkoutApp.Domain.Tests.Common;

public class EntityTests
{
    private sealed class TestEntity : Entity
    {
        public TestEntity(Guid id) : base(id) { }
    }

    private sealed class OtherTestEntity : Entity
    {
        public OtherTestEntity(Guid id) : base(id) { }
    }

    [Fact]
    public void Equals_SameIdAndType_ReturnsTrue()
    {
        var id = Guid.NewGuid();
        var first = new TestEntity(id);
        var second = new TestEntity(id);

        Assert.True(first.Equals(second));
        Assert.True(first == second);
    }

    [Fact]
    public void Equals_DifferentId_ReturnsFalse()
    {
        var first = new TestEntity(Guid.NewGuid());
        var second = new TestEntity(Guid.NewGuid());

        Assert.False(first.Equals(second));
        Assert.True(first != second);
    }

    [Fact]
    public void Equals_SameIdDifferentType_ReturnsFalse()
    {
        var id = Guid.NewGuid();
        var entity = new TestEntity(id);
        var other = new OtherTestEntity(id);

        Assert.False(entity.Equals(other));
    }

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        var entity = new TestEntity(Guid.NewGuid());

        Assert.False(entity.Equals(null));
        Assert.False(entity == null);
        Assert.True(entity != null);
    }

    [Fact]
    public void Equals_SameReference_ReturnsTrue()
    {
        var entity = new TestEntity(Guid.NewGuid());

        Assert.True(entity.Equals(entity));
    }

    [Fact]
    public void GetHashCode_SameIdAndType_AreEqual()
    {
        var id = Guid.NewGuid();
        var first = new TestEntity(id);
        var second = new TestEntity(id);

        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void EqualityOperator_BothNull_ReturnsTrue()
    {
        Entity? left = null;
        Entity? right = null;

        Assert.True(left == right);
    }
}