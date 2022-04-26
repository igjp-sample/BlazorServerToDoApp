using Bunit;
using NUnit.Framework;
using BlazorServerToDoApp.Data;
using Microsoft.Extensions.DependencyInjection;
using BlazorServerToDoApp.Internals;
using System.Threading.Tasks;
using System;
using BlazorServerToDoApp.Models;

namespace BlazorServerToDoApp;

public class AppPageTests
{
    private static Bunit.TestContext CreateTestContext(params ToDoItem[] toDoItems)
    {
        var ctx = new Bunit.TestContext();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        ctx.Services.AddSingleton<IToDoItemRepository>(_ => new MockToDoItemRepository(toDoItems));
        ctx.Services.AddIgniteUIBlazor();
        return ctx;
    }

    [Test]
    public void AddItem_Test()
    {
        // Given
        using var ctx = CreateTestContext();

        var cut = ctx.RenderComponent<App>();
        cut.Find(".todo-items-count").TextContent.MarkupMatches("アイテム数: 0");
        Assert.AreEqual(0, cut.FindAll(".description").Count);

        // When
        var newItemText = $"こんにちは, {Guid.NewGuid()}";
        cut.Instance._NewItem = newItemText;
        cut.Find(".add-button").Click();
        cut.Render();

        // Then
        cut.Find(".todo-items-count").TextContent.MarkupMatches("アイテム数: 1");
        Assert.AreEqual(1, cut.FindAll(".description").Count);
        cut.Find(".description").TextContent.MarkupMatches(newItemText);
    }

    [Test]
    public void RemoveItem_Test()
    {
        // Given
        using var ctx = CreateTestContext(
            new ToDoItem { Description = "牛乳を忘れないで" },
            new ToDoItem { Description = "卵も忘れないで" });

        var cut = ctx.RenderComponent<App>();
        cut.Find(".todo-items-count").TextContent.MarkupMatches("アイテム数: 2");
        var descriptions = cut.FindAll(".description");
        Assert.AreEqual(2, descriptions.Count);
        descriptions[0].TextContent.MarkupMatches("牛乳を忘れないで");
        descriptions[1].TextContent.MarkupMatches("卵も忘れないで");

        // When
        var deleteButtons = cut.FindAll(".delete-button");
        deleteButtons[0].Click();
        cut.Render();

        // Then
        cut.Find(".todo-items-count").TextContent.MarkupMatches("アイテム数: 1");
        descriptions = cut.FindAll(".description");
        Assert.AreEqual(1, descriptions.Count);
        descriptions[0].TextContent.MarkupMatches("卵も忘れないで");
    }
}