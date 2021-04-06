using System;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace DynamicArrays
{
    public class DynamicArrayTests
    {
        [Fact]
        public void Size_Always_ShouldReturnArraySize()
        {
            var target = CreateTarget();
            target.Size().Should().Be(3);
        }

        [Fact]
        public void IsEmpty_ArrayIsNotEmpty_ShouldReturnFalse()
        {
            var target = CreateTarget();
            target.IsEmpty().Should().BeFalse();
        }

        [Fact]
        public void IsEmpty_ArrayIsEmpty_ShouldReturnTrue()
        {
            var target = new DynamicArray<object>();
            target.IsEmpty().Should().BeTrue();
        }

        [Fact]
        public void Get_IndexExists_ShouldReturnItem()
        {
            var target = CreateTarget();
            var node = target.Get(1);

            node.Should().BeEquivalentTo(new { Node = 2 });
        }

        [Fact]
        public void Get_IndexDoesNotExist_ShouldThrowException()
        {
            var target = CreateTarget();

            //this is one way of doing this with pure XUnit
            Assert.Throws<IndexOutOfRangeException>(() => target.Get(5));

            //Here is the another way with FluentAssertions 
            Action act = () => target.Get(5);
            act.Should().Throw<IndexOutOfRangeException>();
        }

        [Fact]
        public void Set_IndexExists_ShouldSetItem()
        {
            var target = CreateTarget();
            target.Set(1, new { Node = 7 });

            target.Get(1).Should().BeEquivalentTo(new { Node = 7 });
        }

        [Fact]
        public void Set_IndexDoesNotExist_ShouldThrowException()
        {
            var target = CreateTarget();

            Action act = () => target.Set(8, new { Node = 7 });
            act.Should().Throw<IndexOutOfRangeException>();
        }

        [Fact]
        public void Clear_Always_ShouldEmptyArray()
        {
            var target = CreateTarget();
            target.Clear();

            target.IsEmpty().Should().BeTrue();
        }

        [Fact]
        public void Add_ResizeIsNotRequired_ShouldAddItem()
        {
            var target = CreateTarget();
            target.Add(new { Node = 13 });

            target.Get(3).Should().BeEquivalentTo(new { Node = 13 });
        }

        [Fact]
        public void Add_ResizeIsRequired_ShouldResizeAndAddItem()
        {
            var target = CreateTarget(3);
            target.Add(new { Node = 13 });

            target.Capacity().Should().Be(6);
            target.Get(2).Should().BeEquivalentTo(new { Node = 13 });
        }

        [Fact]
        public void AddRange_Always_ShouldAddRange()
        {
            var target = CreateTarget();
            object[] newArr = {
                new{Node = 1},
                new{Node = 2},
                new{Node = 3}
            };

            target.AddRange(newArr);

            target.Size().Should().Be(6);
        }

        [Fact]
        public void RemoveAt_IndexDoesNotExist_ShouldThrowException()
        {
            var target = CreateTarget();
            Action act = () => target.RemoveAt(7);

            act.Should().Throw<IndexOutOfRangeException>();
        }

        [Fact]
        public void RemoveAt_IndexExists_ShouldRemoveItemInThatIndex()
        {
            var target = CreateTarget();
            target.RemoveAt(1);

            target.Size().Should().Be(2);
            target.Contains(new { Node = 2 }).Should().BeFalse();
        }

        [Fact]
        public void Remove_ObjectDoesNotExist_ShouldReturnFalse()
        {
            var target = CreateTarget();
            var result = target.Remove(new { Node = 13 });

            result.Should().BeFalse();
        }

        [Fact]
        public void Remove_ObjectExists_ShouldRemoveAndReturnTrue()
        {
            var target = CreateTarget();
            var result = target.Remove(new { Node = 1 });
            //here we don't need to check if object is removed or not
            //because it calls RemoveAt method and it is tested independently
            //we could user any moq framework to check if internal RemoveAt called but for simplicity will not do it
            result.Should().BeTrue();
        }

        //So we can check multiple cases here and also for learning purpose 
        //I will use Theory here and check multiple cases 
        [Theory]
        [InlineData(1, 0)]
        [InlineData(2, 1)]
        [InlineData(4, -1)]
        [InlineData(7, -1)]
        public void IndexOf_DependingOnTheParameter_ShouldReturnIndex(int node, int expectedIndex)
        {
            var target = CreateTarget();
            var result = target.IndexOf(new { Node = node });

            result.Should().Be(expectedIndex);
        }

        //Well, Contains method use indexOf internally  and indexOf is tested
        //we don't need to duplicate code here and test it again 
        //we can just check when contains method called if indexof is also called 
        [Fact]
        public void Contains_Always_ShouldCallIndexOf()
        {
            var target = CreateTarget();
            var node = new { Node = 1 };
            target.Contains(node);

            target.Received(1).IndexOf(node);
        }

        private DynamicArray<object> CreateTarget(int capacity = default)
        {
            if (capacity == default)
            {
                var dArr = Substitute.ForPartsOf<DynamicArray<object>>();
                dArr.AddRange(new object[]
                {
                    new {Node = 1},
                    new {Node = 2},
                    new {Node = 3}
                });

                return dArr;
            }
            else
            {
                var dArr = Substitute.ForPartsOf<DynamicArray<object>>(capacity);
                for (var i = 0; i < capacity - 1; i++)
                {
                    dArr.Add(new { Node = i + 1 });
                }

                return dArr;
            }
        }
    }
}
