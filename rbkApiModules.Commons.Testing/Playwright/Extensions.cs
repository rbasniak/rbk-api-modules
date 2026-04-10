using Shouldly;

namespace rbkApiModules.Testing.Core;


public static class PlaywrightExtensions
{
    public static async Task ClickButtonAsync(this IPage page, string buttonText, bool force = false)
    {
        var button = page.GetByRole(AriaRole.Button, new() { Name = buttonText });

        button.ShouldNotBeNull($"Button not found with text: {buttonText}");

        await button.ClickAsync(force ? new() { Force = true } : null);
    }

    public static async Task<ILocator> WaitButtonAsync(this IPage page, string buttonText, int timeout = 10000)
    {
        var button = page.GetByRole(AriaRole.Button, new() { Name = buttonText });

        await button.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = timeout });

        return button;
    }

    public static async Task ClickDropdownAsync(this IPage page, string dropdownSelector)
    {
        ValidateSelector(dropdownSelector);

        var dropdown = page.Locator(dropdownSelector);

        dropdown.ShouldNotBeNull($"Dropdown not found with selector: {dropdownSelector}");

        await dropdown.ClickAsync();
    }

    public static async Task SelectDropdownOption(this IPage page, string dropdownSelector, string optionText)
    {
        ValidateSelector(dropdownSelector);

        await page.ClickDropdownAsync(dropdownSelector);

        var option = page.GetByRole(AriaRole.Option, new() { Name = optionText });
        await option.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });

        await option.ClickAsync();
    }

    public static async Task AssertDropDownHasOptionAsync(this IPage page, string dropdownSelector, string optionText)
    {
        ValidateSelector(dropdownSelector);

        await page.ClickDropdownAsync(dropdownSelector);
        var option = page.GetByRole(AriaRole.Option, new() { Name = optionText });
        await option.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });

        var isVisible = await option.IsVisibleAsync();

        isVisible.ShouldBeTrue($"Option '{optionText}' should be visible in dropdown '{dropdownSelector}'");
    }

    public static async Task AssertDropdownShowsSelectedValue(this IPage page, string dropdownSelector, string optionText)
    {
        ValidateSelector(dropdownSelector);

        var dropdown = page.Locator(dropdownSelector);

        dropdown.ShouldNotBeNull($"Dropdown not found with selector: {dropdownSelector}");

        var selectedLabel = dropdown.GetByText(optionText);

        await selectedLabel.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });

        var isVisible = await selectedLabel.IsVisibleAsync();

        isVisible.ShouldBeTrue($"Selected option '{optionText}' should be visible in dropdown '{dropdownSelector}'");
    }

    /// <summary>
    /// Fills an input or textarea by its label text.
    /// </summary>
    public static async Task FillInputByLabelAsync(this IPage page, string labelText, string value)
    {
        var input = page.GetByLabel(labelText);
        await input.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
        await input.FillAsync(value);
    }

    /// <summary>
    /// Fills an input or textarea by selector (e.g. #id or [data-testid="x"]).
    /// </summary>
    public static async Task FillInputAsync(this IPage page, string selector, string value)
    {
        ValidateSelector(selector);

        var input = page.Locator(selector);
        await input.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
        await input.FillAsync(value);
    }

    /// <summary>
    /// Selects an option in a dropdown by its label text.
    /// </summary>
    public static async Task SelectDropdownByLabelAsync(this IPage page, string labelText, string optionText)
    {
        var combobox = page.GetByLabel(labelText);
        await combobox.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
        await combobox.ClickAsync();

        var option = page.GetByRole(AriaRole.Option, new() { Name = optionText });
        await option.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
        await option.ClickAsync();
    }

    private static void ValidateSelector(string selector)
    {
        if (!selector.StartsWith("#"))
        {
            throw new ArgumentException($"Selector must start with '#' for id. Invalid selector: {selector}");
        }
    }
}

public class UiTestException : Exception
{
    public UiTestException() : base("UI element not found or interaction failed.")
    {
    }
    public UiTestException(string message) : base(message)
    {
    }
    public UiTestException(string message, Exception innerException) : base(message, innerException)
    {
    }
}