﻿using ShapeCrawler.Models;
using Xunit;

// ReSharper disable TooManyChainedReferences

namespace ShapeCrawler.UnitTests
{
    public class TestFile_017
    {
        [Fact]
        public void SlidesCollection_Test()
        {
            // Arrange
            var pre = new Presentation(Properties.Resources._017);

            // Act-Assert
            var slides = pre.Slides; // should not throws exception
        }
    }
}
