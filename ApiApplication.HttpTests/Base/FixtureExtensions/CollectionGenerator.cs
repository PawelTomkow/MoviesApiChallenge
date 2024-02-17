using System;
using System.Collections.Generic;
using AutoFixture;

namespace ApiApplication.HttpTests.Base.FixtureExtensions
{
    public static class CollectionGenerator
    {
        public static List<T> GenerateListOf<T>(this Fixture fixture, int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Amount can not be negative and 0");
            }
            
            var tList = new List<T>();
            for (var i = 0; i < amount; i++)
            {
                tList.Add(fixture.Create<T>());
            }

            return tList;
        }
    }
}