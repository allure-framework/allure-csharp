﻿using NUnit.Framework;

namespace Allure.Net.Commons.Tests.UserAPITests.AllureFacadeTests;

class BddTests : AllureApiTestFixture
{
    [Test]
    public void EpicCanBeAdded()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.AddEpic("My Epic");

        this.AssertLabels(
            new Label() { name = "epic", value = "My Epic" }
        );
    }

    [Test]
    public void AddEpicAppendsLabel()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });
        AllureApi.AddEpic("My Epic 1");

        AllureApi.AddEpic("My Epic 2");

        this.AssertLabels(
            new Label() { name = "epic", value = "My Epic 1" },
            new Label() { name = "epic", value = "My Epic 2" }
        );
    }

    [Test]
    public void FeatureCanBeAdded()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.AddFeature("My Feature");

        this.AssertLabels(
            new Label() { name = "feature", value = "My Feature" }
        );
    }

    [Test]
    public void AddFeatureAppendsLabel()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });
        AllureApi.AddFeature("My Feature 1");

        AllureApi.AddFeature("My Feature 2");

        this.AssertLabels(
            new Label() { name = "feature", value = "My Feature 1" },
            new Label() { name = "feature", value = "My Feature 2" }
        );
    }

    [Test]
    public void StoryCanBeAdded()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.AddStory("My Story");

        this.AssertLabels(
            new Label() { name = "story", value = "My Story" }
        );
    }

    [Test]
    public void AddStoryAppendsLabel()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });
        AllureApi.AddStory("My Story 1");

        AllureApi.AddStory("My Story 2");

        this.AssertLabels(
            new Label() { name = "story", value = "My Story 1" },
            new Label() { name = "story", value = "My Story 2" }
        );
    }
}
